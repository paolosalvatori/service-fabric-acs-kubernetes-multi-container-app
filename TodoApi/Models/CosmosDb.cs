namespace TodoApi.Models
{
    /// <summary>
    /// This class contains the configuration for the Cosmos Db repository used by the TodoContext class.
    /// </summary>
    public class CosmosDb
    {
        /// <summary>
        /// Gets or sets the value of the endpoint uri.
        /// </summary>
        public string EndpointUri { get; set; }

        /// <summary>
        /// Gets or sets the value of the primary key.
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the value of the database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the value of the collection name.
        /// </summary>
        public string CollectionName { get; set; }
    }
}
