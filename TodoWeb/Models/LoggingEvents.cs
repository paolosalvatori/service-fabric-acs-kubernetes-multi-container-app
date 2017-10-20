namespace TodoWeb.Models
{
    #region snippet_LoggingEvents
    /// <summary>
    /// This class contains the event id of logging events.
    /// </summary>
    public class LoggingEvents
    {
        /// <summary>
        /// ListItems
        /// </summary>
        public const int ListItems = 1001;

        /// <summary>
        /// GetItem
        /// </summary>
        public const int GetItem = 1002;

        /// <summary>
        /// InsertItem
        /// </summary>
        public const int InsertItem = 1003;

        /// <summary>
        /// UpdateItem
        /// </summary>
        public const int UpdateItem = 1004;

        /// <summary>
        /// DeleteItem
        /// </summary>
        public const int DeleteItem = 1005;

        /// <summary>
        /// GetItemNotFound
        /// </summary>
        public const int GetItemNotFound = 4000;

        /// <summary>
        /// UpdateItemNotFound
        /// </summary>
        public const int UpdateItemNotFound = 4001;

        /// <summary>
        /// MethodCallDuration
        /// </summary>
        public const int MethodCallDuration = 5000;

        /// <summary>
        /// Configuration
        /// </summary>
        public const int Configuration = 5001;
    }
    #endregion
}
