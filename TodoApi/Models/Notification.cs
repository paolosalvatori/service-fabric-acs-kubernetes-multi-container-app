using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    /// <summary>
    /// Notification class
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Gets or sets the value of the Id property.
        /// </summary>
        [Required]
        [JsonProperty(PropertyName = "id", Order = 1)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the value of the Operation property.
        /// </summary>
        [Required]
        [JsonProperty(PropertyName = "op", Order = 2)]
        public string Operation { get; set; }


        /// <summary>
        /// Gets or sets the value of the Operation property.
        /// </summary>
        [Required]
        [JsonProperty(PropertyName = "item", Order = 3)]
        public TodoItem Item { get; set; }
    }
}
