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

namespace Apache.ShenYu.Client.Utils
{
    public class Constants
    {
        public const string LoginPath = "/platform/login";
        public const string XAccessToken = "X-Access-Token";

        public const string UriPath = "/shenyu-client/register-uri";
        public const string MetaPath = "/shenyu-client/register-metadata";

        public const string MetaType = "metadata";
        public const string UriType = "uri";

        public const string SelectorJoinRule = "-";
        public const string PathSeparator = "/";
        public const string DotSeparator = ".";

        public class RegisterType
        {
            public const string Http = "http";
            public const string Zookeeper = "zookeeper";
            public const string Consul = "consul";
            public const string Nacos = "nacos";
            public const string Etcd = "etcd";
        }

        public class RegisterRpcType
        {
            public const string Http = "http";
            public const string SpringCloud = "springCloud";
        }

        public class RegisterConstants
        {
            public const string UserName = "UserName";
            public const string Password = "Password";

            public const string SessionTimeout = "SessionTimeout";
            public const string ConnectionTimeout = "SessionTimeout";
        }
    }
}
