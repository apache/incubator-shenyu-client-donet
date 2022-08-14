using System;
using System.Text;

namespace Apache.ShenYu.Client.Utils
{
    public class ZkOptions
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public ZkOptions()
        {
            ConnectionSpanTimeout = TimeSpan.FromMilliseconds(60000);
            SessionSpanTimeout = TimeSpan.FromMilliseconds(15000);
            OperatingSpanTimeout = TimeSpan.FromMilliseconds(60000);
            ReadOnly = false;
            SessionId = 0;
            SessionPasswd = null;
            EnableEphemeralNodeRestore = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionTimeout"></param>
        /// <param name="operatingTimeout"></param>
        /// <param name="sessionTimeout"></param>
        protected ZkOptions(int connectionTimeout, int operatingTimeout, int sessionTimeout)
        {
            ConnectionSpanTimeout = TimeSpan.FromMilliseconds(connectionTimeout);
            SessionSpanTimeout = TimeSpan.FromMilliseconds(sessionTimeout);
            OperatingSpanTimeout = TimeSpan.FromMilliseconds(operatingTimeout);
            ReadOnly = false;
            SessionId = 0;
            SessionPasswd = null;
            EnableEphemeralNodeRestore = true;
        }

        /// <summary>
        /// create ZooKeeper client
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ZkOptions(string connectionString) : this()
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            ConnectionString = connectionString;
        }

        /// <summary>
        /// create ZooKeeper client
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="connectionTimeout"></param>
        /// <param name="operatingTimeout"></param>
        /// <param name="sessionTimeout"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ZkOptions(string connectionString
            , int connectionTimeout
            , int operatingTimeout
            , int sessionTimeout) : this(connectionTimeout, operatingTimeout, sessionTimeout)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            ConnectionString = connectionString;
        }

        /// <summary>
        /// create ZooKeeper client
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="connectionTimeout"></param>
        /// <param name="operatingTimeout"></param>
        /// <param name="retryCount"></param>
        /// <param name="sessionTimeout"></param>
        /// <param name="readOnly"></param>
        /// <param name="sessionId"></param>
        /// <param name="sessionPasswd"></param>
        /// <param name="baseRoutePath"></param>
        /// <param name="enableEphemeralNodeRestore"></param>
        /// <param name="logToFile"></param>
        public ZkOptions(string connectionString
            , int connectionTimeout
            , int operatingTimeout
            , int retryCount
            , int sessionTimeout
            , bool readOnly
            , long sessionId
            , string sessionPasswd
            , string baseRoutePath
            , bool enableEphemeralNodeRestore
            , bool logToFile) : this(connectionString, connectionTimeout, operatingTimeout, sessionTimeout)
        {
            ConnectionTimeout = connectionTimeout;
            OperatingTimeout = operatingTimeout;
            RetryCount = retryCount;
            SessionTimeout = sessionTimeout;
            ReadOnly = readOnly;
            SessionId = sessionId;
            SessionPasswd = sessionPasswd;
            BaseRoutePath = baseRoutePath;
            EnableEphemeralNodeRestore = enableEphemeralNodeRestore;
            LogToFile = logToFile;
        }

        public ZkOptions SetConnectionTimeout(int connectionTimeout)
        {
            this.ConnectionTimeout = connectionTimeout;
            this.ConnectionSpanTimeout = TimeSpan.FromMilliseconds(connectionTimeout);
            return this;
        }

        public ZkOptions SetSessionTimeout(int sessionTimeout)
        {
            this.SessionTimeout = sessionTimeout;
            this.SessionSpanTimeout = TimeSpan.FromMilliseconds(sessionTimeout);
            return this;
        }

        public ZkOptions SetOperatingTimeout(int operatingTimeout)
        {
            this.OperatingTimeout = operatingTimeout;
            this.OperatingSpanTimeout = TimeSpan.FromMilliseconds(operatingTimeout);
            return this;
        }

        public ZkOptions SetMaxRetry(int maxRetry)
        {
            this.RetryCount = maxRetry;
            return this;
        }

        public ZkOptions SetSessionPassword(string sessionPassword)
        {
            this.SessionPasswd = sessionPassword;
            return this;
        }



        /// <summary>
        /// connect string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// wait zooKeeper connect time
        /// </summary>
        public int ConnectionTimeout { get; set; } = 60 * 1000;

        /// <summary>
        /// execute zooKeeper handler retry waittime
        /// </summary>
        public int OperatingTimeout { get; set; } = 60 * 1000;

        /// <summary>
        /// retry count
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// zookeeper session timeout
        /// </summary>
        public int SessionTimeout { get; set; } = 15 * 1000;

        /// <summary>
        /// readonly
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// session Id。
        /// </summary>
        public long SessionId { get; set; }

        /// <summary>
        /// session password
        /// </summary>
        public string SessionPasswd { get; set; }

        public byte[] SessionPasswdBytes
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SessionPasswd))
                {
                    return Encoding.UTF8.GetBytes(SessionPasswd);
                }
                return null;
            }
        }

        /// <summary>
        /// log to file options
        /// </summary>
        public bool LogToFile { get; set; } = false;

        /// <summary>
        /// base root path
        /// </summary>
        public string BaseRoutePath { get; set; }

        /// <summary>
        /// enable effect shortnode recover
        /// </summary>
        public bool EnableEphemeralNodeRestore { get; set; }

        #region Internal

        /// <summary>
        /// wait zooKeeper connect span time
        /// </summary>
        internal TimeSpan ConnectionSpanTimeout { get; set; }

        /// <summary>
        /// execute zooKeeper handler retry span waittime
        /// </summary>
        internal TimeSpan OperatingSpanTimeout { get; set; }

        /// <summary>
        /// zookeeper session timeout
        /// </summary>
        internal TimeSpan SessionSpanTimeout { get; set; }

        #endregion Internal
    }
}
