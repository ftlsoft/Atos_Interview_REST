using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace Interview_Atos.Models
{
    public class Customer
    {
        /// <summary>
        /// Simple integer, not GUID
        /// </summary>
        [JsonPropertyName("Id")]
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("Firstname")]
        public string Firstname { get; set; }

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("Lastname")]
        public string Lastname { get; set; }
    }
}
