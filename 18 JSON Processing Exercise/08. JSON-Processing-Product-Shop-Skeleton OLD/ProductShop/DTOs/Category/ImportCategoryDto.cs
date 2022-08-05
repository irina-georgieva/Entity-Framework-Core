using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.DTOs.Category
{
    [JsonObject]
    public class ImportCategoryDto
    {
        [JsonProperty("name")]
        [Required]
        public string Name { get; set; }
    }
}
