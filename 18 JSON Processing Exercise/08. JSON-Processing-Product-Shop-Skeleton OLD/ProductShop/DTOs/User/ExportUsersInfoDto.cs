using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductShop.DTOs.User
{
    [JsonObject]
    public class ExportUsersInfoDto
    {
        [JsonProperty("usersCount")]
        public int UsersCount
            => this.Users.Any() ? this.Users.Length : 0;

        [JsonProperty("users")]
        public ExportUsersWithFullProductInfoDto[] Users { get; set; }
    }
}
