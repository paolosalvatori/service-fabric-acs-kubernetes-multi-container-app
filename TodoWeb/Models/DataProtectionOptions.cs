namespace TodoWeb.Models
{
    /// <summary>
    /// This class contains the configuration for the Data Protection
    /// </summary>
    public class DataProtectionOptions
    {
        /// <summary>
        /// Gets or sets the configuration for the blob storage 
        /// </summary>
        public BlobStorage BlobStorage { get; set; }
    }
}
