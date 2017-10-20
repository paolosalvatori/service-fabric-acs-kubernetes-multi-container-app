namespace TodoApi.Models
{
    /// <summary>
    /// This class contains configuration data for the data repository used by the TodoContext class.
    /// </summary>
    public class RepositoryServiceOptions
    {
        /// <summary>
        /// Gets or sets the configuration for the CosmosDbNotificationService
        /// </summary>
        public CosmosDb CosmosDb { get; set; }
    }
}
