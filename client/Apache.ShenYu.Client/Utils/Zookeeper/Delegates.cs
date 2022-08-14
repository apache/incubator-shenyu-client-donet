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

using System.Collections.Generic;
using System.Threading.Tasks;
using org.apache.zookeeper;

namespace Apache.ShenYu.Client.Utils
{
    /// <summary>
    /// connect state changehander params
    /// </summary>
    public class ConnectionStateChangeArgs
    {
        /// <summary>
        /// connect state
        /// </summary>
        public Watcher.Event.KeeperState State { get; set; }
    }

    /// <summary>
    /// NodeChange params
    /// </summary>
    public abstract class NodeChangeArgs
    {
        /// <summary>
        /// create a new node with params
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        protected NodeChangeArgs(string path, Watcher.Event.EventType type)
        {
            Path = path;
            Type = type;
        }

        /// <summary>
        /// changeType
        /// </summary>
        public Watcher.Event.EventType Type { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; private set; }
    }

    /// <summary>
    /// node data change params
    /// </summary>
    public sealed class NodeDataChangeArgs : NodeChangeArgs
    {
        /// <summary>
        /// create  new nodedata with params
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="currentData"></param>
        public NodeDataChangeArgs(string path, Watcher.Event.EventType type, IEnumerable<byte> currentData) : base(path,
            type)
        {
            CurrentData = currentData;
        }

        /// <summary>
        /// current nodedata last data
        /// </summary>
        public IEnumerable<byte> CurrentData { get; private set; }
    }

    /// <summary>
    ///  childnode change args
    /// </summary>
    public sealed class NodeChildrenChangeArgs : NodeChangeArgs
    {
        /// <summary>
        /// create childnodes change agrs
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type">event tyoe </param>
        /// <param name="currentChildrens"></param>
        public NodeChildrenChangeArgs(string path, Watcher.Event.EventType type, IEnumerable<string> currentChildrens) :
            base(path, type)
        {
            CurrentChildrens = currentChildrens;
        }

        /// <summary>
        /// currentnode all childnodes
        /// </summary>
        public IEnumerable<string> CurrentChildrens { get; private set; }
    }

    /// <summary>
    /// node datachange delegate
    /// </summary>
    /// <param name="client"></param>
    /// <param name="args"></param>
    public delegate Task NodeDataChangeHandler(IZookeeperClient client, NodeDataChangeArgs args);

    /// <summary>
    /// childnode datachange delegate
    /// </summary>
    /// <param name="client"></param>
    /// <param name="args"></param>
    public delegate Task NodeChildrenChangeHandler(IZookeeperClient client, NodeChildrenChangeArgs args);

    /// <summary>
    /// connectstat change delegate
    /// </summary>
    /// <param name="client"></param>
    /// <param name="args"></param>
    public delegate Task ConnectionStateChangeHandler(IZookeeperClient client, ConnectionStateChangeArgs args);
}
