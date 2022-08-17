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
using System.Collections.Generic;
using System.Threading.Tasks;
using org.apache.zookeeper;
using org.apache.zookeeper.data;

namespace Apache.ShenYu.Client.Utils
{
    /// <summary>
    /// Abstract ZooKeeperClient
    /// </summary>
    public interface IZookeeperClient : IDisposable
    {
        /// <summary>
        /// wait zk connect to give states
        /// </summary>
        /// <param name="states"></param>
        /// <param name="timeout"></param>
        /// <returns>success:true,fail:false</returns>
        bool WaitForKeeperState(Watcher.Event.KeeperState states, TimeSpan timeout);

        /// <summary>
        /// retry util zk connected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callable">execute zk action</param>
        /// <returns></returns>
        Task<T> RetryUntilConnected<T>(Func<Task<T>> callable);

        /// <summary>
        /// get give node data
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IEnumerable<byte>> GetDataAsync(string path);

        /// <summary>
        /// node exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns>if have return true，then return false。</returns>
        Task<bool> ExistsAsync(string path);

        /// <summary>
        /// create node
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="acls"></param>
        /// <param name="createMode"></param>
        /// <returns></returns>
        /// <remarks>
        ///
        /// </remarks>
        Task<string> CreateAsync(string path, byte[] data, List<ACL> acls, CreateMode createMode);

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="acls"></param>
        /// <param name="createMode"></param>
        /// <returns></returns>
        Task<bool> CreateWithParentAsync(string path, byte[] data, List<ACL> acls, CreateMode createMode);

        /// <summary>
        /// set node data
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="version"></param>
        /// <returns>node stat</returns>
        Task<Stat> SetDataAsync(string path, byte[] data, int version = -1);

        /// <summary>
        /// delete node
        /// </summary>
        /// <param name="path"></param>
        /// <param name="version"></param>
        Task DeleteAsync(string path, int version = -1);

        /// <summary>
        /// subscribe connect stat change
        /// </summary>
        /// <param name="listener"></param>
        void SubscribeStatusChange(ConnectionStateChangeHandler listener);

        /// <summary>
        /// unsubscribe connect stat change
        /// </summary>
        /// <param name="listener"></param>
        void UnSubscribeStatusChange(ConnectionStateChangeHandler listener);
    }
}
