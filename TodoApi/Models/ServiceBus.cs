namespace TodoApi.Models
{
    /// <summary>
    /// This class contains the configuration for the Service Bus
    /// </summary>
    public class ServiceBus
    {
        /// <summary>
        /// Gets or sets the value of the connection string of the Service Bus namespace.
        /// </summary>
        public string ConnectionString {get; set;}

        /// <summary>
        /// Gets or sets the name of the queue used for notifications.
        /// </summary>
        public string QueueName { get; set; }
    }
}
