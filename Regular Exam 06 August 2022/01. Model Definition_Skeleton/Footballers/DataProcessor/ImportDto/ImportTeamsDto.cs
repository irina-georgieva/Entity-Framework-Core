using Footballers.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamsDto
    {
        [Required]
        [MinLength(ValidationConstants.TeamNameMinLength)]
        [MaxLength(ValidationConstants.TeamNameMaxLength)]
        [RegularExpression(ValidationConstants.TeamNameRegex)]
        [JsonProperty(nameof(Name))]
        public string Name { get; set; }

        [Required]
        [MinLength(ValidationConstants.TeamNationalityMinLength)]
        [MaxLength(ValidationConstants.TeamNationalityMaxLength)]
        [JsonProperty(nameof(Nationality))]
        public string Nationality { get; set; }

        [Required]
        [JsonProperty(nameof(Trophies))]
        public int Trophies { get; set; }

        [JsonProperty(nameof(Footballers))]
        public HashSet<int> Footballers { get; set; }
    }
}
