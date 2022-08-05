using Newtonsoft.Json;

using System.Linq;

namespace ProductShop.DTOs.Product
{
    [JsonObject]
    public class ExportSoldProductsFullInfoDto
    {
        [JsonProperty("count")]
        public int ProductsCount
            => SoldProducts.Any() ? SoldProducts.Length : 0;

        [JsonProperty("products")]
        public ExportSoldProductShortInfoDto[] SoldProducts { get; set; }
    
        
    }
}
