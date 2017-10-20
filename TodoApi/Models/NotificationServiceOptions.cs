namespace TodoApi.Models
{
    /// <summary>
    /// This class contains the configuration for the Notification helper
    /// </summary>
    public class NotificationServiceOptions
    {
        /// <summary>
        /// Gets or sets the configuration for the ServiceBusNotificationService
        /// </summary>
        public ServiceBus ServiceBus { get; set; }
    }
}
