#region Using Directives
using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TodoApi.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Diagnostics; /**/
#endregion

namespace TodoApi.Services
{
    /// <summary>
    /// This class is used to send notifications to a Service Bus queue.
    /// </summary>
    public class ServiceBusNotificationService : NotificationService
    {
        #region Private Instance Fields
        private readonly NotificationServiceOptions _options;
        private readonly QueueClient _queueClient;
        private readonly ILogger<NotificationService> _logger;
        #endregion

        #region Public Constructor
        /// <summary>
        /// Creates a new instance of the ServiceBusNotificationService class
        /// </summary>
        public ServiceBusNotificationService(IOptions<NotificationServiceOptions> options,
                                             ILogger<ServiceBusNotificationService> logger)
        {
            if (options?.Value == null)
            {
                throw new ArgumentNullException(nameof(options), "No configuration is defined for the notification service in the appsettings.json.");
            }

            if (options.Value.ServiceBus == null)
            {
                throw new ArgumentNullException(nameof(options), "No ServiceBus element is defined in the configuration for the notification service in the appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.ServiceBus.ConnectionString))
            {
                throw new ArgumentNullException(nameof(options), "No connection string is defined in the configuration of the Service Bus notification service in the appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.ServiceBus.QueueName))
            {
                throw new ArgumentNullException(nameof(options), "No queue name is defined in the configuration of the Service Bus notification service in the appsettings.json.");
            }

            _options = options.Value;
            _logger = logger;
            _queueClient = new QueueClient(_options.ServiceBus.ConnectionString,
                                           _options.ServiceBus.QueueName);
            _logger.LogInformation(LoggingEvents.Configuration, "ConnectionString = {connectionstring}", _options.ServiceBus.ConnectionString);
            _logger.LogInformation(LoggingEvents.Configuration, "QueueName = {queuename}", _options.ServiceBus.QueueName);

        }
        #endregion

        #region Public Overridden Methods
        /// <summary>
        /// Send to a notification to a given queue
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override async Task SendNotificationAsync(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();


            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(notification)))
            {
                MessageId = notification.Id
            };
            message.UserProperties.Add("source", "TodoApi");
            await _queueClient.SendAsync(message);

            stopwatch.Stop();
            _logger.LogInformation($"Notification sent to {_options.ServiceBus.QueueName} queue in {stopwatch.ElapsedMilliseconds} ms.");
        } 
        #endregion
    }
}
