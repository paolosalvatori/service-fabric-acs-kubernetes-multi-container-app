namespace TodoApi.Models
{
    /// <summary>
    /// This class contains the configuration for the Blob Storage
    /// </summary>
    public class BlobStorage
    {
        /// <summary>
        /// Gets or sets the value of the connection string of the Storage Account.
        /// </summary>
        public string ConnectionString {get; set;}

        /// <summary>
        /// Gets or sets the name of the container used to store the data protection key.
        /// </summary>
        public string ContainerName { get; set; }
    }
}
