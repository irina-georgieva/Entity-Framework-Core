using Newtonsoft.Json;
using SoftJail.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonerMailsDto
    {
        [Required]
        [JsonProperty(nameof(Description))]
        public string Description { get; set; }

        [JsonProperty(nameof(Sender))]
        public string Sender { get; set; }

        [Required]
        [JsonProperty(nameof(Address))]
        [RegularExpression(ValidationConstants.MailAddressRegex)]
        public string Address { get; set; }
    }
}
