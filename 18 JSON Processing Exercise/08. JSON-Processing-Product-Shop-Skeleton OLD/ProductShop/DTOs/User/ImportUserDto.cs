using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ProductShop.DTOs.User
{
    [JsonObject]
    public class ImportUserDto
    {
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Age")]
        public int? Age { get; set; }
    }

}
