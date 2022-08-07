using System.ComponentModel.DataAnnotations;

using SoftJail.Common;

using Newtonsoft.Json;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentWithSellsDto
    {
        [Required]
        [MinLength(ValidationConstants.DepartmenrNameMinLength)]
        [MaxLength(ValidationConstants.DepartmenrNameMaxLength)]
        [JsonProperty(nameof(Name))]
        public string Name { get; set; }

        [JsonProperty(nameof(Cells))]
        public ImportDepartmentCellDto[] Cells { get; set; }

    }
}
