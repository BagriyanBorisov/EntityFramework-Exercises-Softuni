using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellerDto
    {
        [JsonProperty("Name")]
        [Required]
        [MaxLength(20)]
        [MinLength(5)]
        public string? Name { get; set; }

        [JsonProperty("Address")]
        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        public string? Address { get; set; }


        [JsonProperty("Country")]
        [Required]
        public string? Country { get; set; }

        [JsonProperty("Website")]
        [Required]
        [RegularExpression(@"(www\.[A-Za-z0-9\-]+\.com)")]
        public string? Website { get; set; }

        [JsonProperty("Boardgames")]
        public int[] Boardgames { get; set; }
    }
}
