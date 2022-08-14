/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Apache.ShenYu.Client.Models.DTO;
using Apache.ShenYu.Client.Options;
using Apache.ShenYu.Client.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using org.apache.zookeeper;

namespace Apache.ShenYu.Client.Registers
{
    public class ShenyuZookeeperRegister : ShenyuAbstractRegister
    {
        private readonly ILogger<ShenyuZookeeperRegister> _logger;
        private ShenyuOptions _shenyuOptions;
        private ZookeeperClient _zkClient;
        private Dictionary<string, string> _nodeDataMap = new Dictionary<string, string>();
        //private HashSet<string> _metadataSet = new HashSet<string>();
        private ConcurrentDictionary<string, RegistryCenterHealthCheckModel> m_healthCheck = new ConcurrentDictionary<string, RegistryCenterHealthCheckModel>();

        public ShenyuZookeeperRegister(ILogger<ShenyuZookeeperRegister> logger)
        {
            _logger = logger;
        }

        public override Task Init(ShenyuOptions shenyuOptions)
        {
            var serverList = shenyuOptions.Register.ServerList;
            this._shenyuOptions = shenyuOptions;
            //props
            var props = shenyuOptions.Register.Props;
            int sessionTimeout = Convert.ToInt32(props.GetValueOrDefault(Constants.RegisterConstants.SessionTimeout, "3000"));
            int connectionTimeout = Convert.ToInt32(props.GetValueOrDefault(Constants.RegisterConstants.ConnectionTimeout, "3000"));
            int baseSleepTime = Convert.ToInt32(props.GetValueOrDefault(Constants.RegisterConstants.BaseSleepTime, "1000"));
            int maxRetries = Convert.ToInt32(props.GetValueOrDefault(Constants.RegisterConstants.MaxRetry, "3"));
            int maxSleepTime = Convert.ToInt32(props.GetValueOrDefault(Constants.RegisterConstants.MaxSleepTime, int.MaxValue.ToString()));
            ZkOptions zkConfig = new ZkOptions(serverList);
            zkConfig.SetMaxRetry(maxRetries)
                    .SetOperatingTimeout(baseSleepTime)
                    .SetSessionTimeout(sessionTimeout)
                    .SetConnectionTimeout(connectionTimeout);

            props.TryGetValue(Constants.RegisterConstants.Password, out string password);
            if (!string.IsNullOrEmpty(password))
            {
                zkConfig.SetSessionPassword(password);
            }

            this._zkClient = new ZookeeperClient(zkConfig);
            this._zkClient.SubscribeStatusChange(async (client, connectionStateChangeArgs) =>
            {
                var healthCheckModel = m_healthCheck.GetOrAdd(zkConfig.ConnectionString, new RegistryCenterHealthCheckModel(true, 0));
                switch (connectionStateChangeArgs.State)
                {
                    case Watcher.Event.KeeperState.Disconnected:
                    case Watcher.Event.KeeperState.Expired:
                        if (client.WaitForKeeperState(Watcher.Event.KeeperState.SyncConnected,
                            zkConfig.OperatingSpanTimeout))
                        {
                            if (healthCheckModel.HealthType == HealthTypeEnum.Disconnected)
                            {
                                _logger.LogError("zookeeper server disconnected");
                            }
                            healthCheckModel.SetHealth();
                        }
                        else
                        {
                            healthCheckModel.SetUnHealth(HealthTypeEnum.Disconnected,
                                "Connection session disconnected");
                        }

                        break;

                    case Watcher.Event.KeeperState.AuthFailed:
                        healthCheckModel.SetUnHealth(HealthTypeEnum.AuthFailed, "AuthFailed");
                        _logger.LogError("zookeeper server AuthFailed");
                        break;

                    case Watcher.Event.KeeperState.SyncConnected:
                    case Watcher.Event.KeeperState.ConnectedReadOnly:
                        healthCheckModel.SetHealth();
                        foreach (var node in _nodeDataMap)
                        {
                            var existStat = await _zkClient.ExistsAsync(node.Key);
                            if (existStat)
                            {
                                await _zkClient.CreateWithParentAsync(node.Key,
                                    Encoding.UTF8.GetBytes(node.Value),
                                    ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL);
                                _logger.LogInformation("zookeeper client register success: {}", node.Value);
                            }
                        }
                        break;
                }
                await Task.CompletedTask;
            });
            return Task.CompletedTask;
        }

        public override async Task PersistInterface(MetaDataRegisterDTO metadata)
        {
            string contextPath = ContextPathUtils.BuildRealNode(metadata.contextPath, metadata.appName);
            await RegisterMetadataAsync(contextPath, metadata);
        }

        public override async Task PersistURI(URIRegisterDTO registerDTO)
        {
            string contextPath = ContextPathUtils.BuildRealNode(registerDTO.contextPath, registerDTO.appName);
            await RegisterURIAsync(contextPath, registerDTO);
        }

        private async Task RegisterURIAsync(string contextPath, URIRegisterDTO registerDTO)
        {
            string uriNodeName = BuildURINodeName(registerDTO);
            string uriPath = RegisterPathConstants.BuildURIParentPath(registerDTO.rpcType, contextPath);
            string realNode = RegisterPathConstants.BuildRealNode(uriPath, uriNodeName);
            await _zkClient.CreateWithParentAsync(uriPath, null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            string nodeData = JsonConvert.SerializeObject(registerDTO, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            _nodeDataMap[realNode] = nodeData;
            await _zkClient.CreateOrUpdateAsync(realNode, Encoding.UTF8.GetBytes(nodeData), ZooDefs.Ids.OPEN_ACL_UNSAFE,
                      CreateMode.EPHEMERAL);
        }

        private string BuildURINodeName(URIRegisterDTO registerDTO)
        {
            string host = registerDTO.host;
            int port = registerDTO.port;
            return String.Join(Constants.COLONS, host, port.ToString());
        }

        private async Task RegisterMetadataAsync(string contextPath, MetaDataRegisterDTO metadata)
        {
            string metadataNodeName = BuildMetadataNodeName(metadata);
            string metaDataPath = RegisterPathConstants.BuildMetaDataParentPath(metadata.rpcType, contextPath);
            string realNode = RegisterPathConstants.BuildRealNode(metaDataPath, metadataNodeName);
            //create parent node
            await _zkClient.CreateWithParentAsync(metaDataPath, null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            //// avoid dup registration for metadata
            //lock (this._metadataSet)
            //{
            //    this._metadataSet.Add(realNode);
            //}
            var metadataStr = JsonConvert.SerializeObject(metadata, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            await _zkClient.CreateOrUpdateAsync(realNode, Encoding.UTF8.GetBytes(metadataStr), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            _logger.LogInformation("{} zookeeper client register metadata success: {}", metadata.rpcType, metadata);
        }

        public override async Task Close()
        {
           this._zkClient.Dispose();
           await Task.CompletedTask;
        }
    }
}
