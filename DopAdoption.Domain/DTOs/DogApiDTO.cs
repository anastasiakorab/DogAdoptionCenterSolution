using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DopAdoption.Domain.DTOs
{
    public class DogApiDTO
    {
        [JsonPropertyName("imageUrl")]
        public string PhotoLink { get; set; } = string.Empty;

        [JsonPropertyName("breed")]
        public string TypeOfDog { get; set; } = string.Empty;

        [JsonPropertyName("subBreed")]
        public string DogVariety { get; set; } = string.Empty;

        [JsonPropertyName("provider")]
        public string Source { get; set; } = "dog.ceo";
    }
}


