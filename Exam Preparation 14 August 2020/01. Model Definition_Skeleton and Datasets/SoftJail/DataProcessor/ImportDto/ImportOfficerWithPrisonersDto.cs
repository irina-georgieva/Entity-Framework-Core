using SoftJail.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficerWithPrisonersDto
    {
        [Required]
        [MinLength(ValidationConstants.OfficerFullNameMinLength)]
        [MaxLength(ValidationConstants.OfficerFullNameMaxLength)]
        [XmlElement("Name")]
        public string FullName { get; set; }

        [Range(typeof(decimal), ValidationConstants.PrisonerBailMinValue, ValidationConstants.PrisonerBailMaxValue)]
        [XmlElement("Money")]
        public decimal Salary { get; set; }

        [Required]
        [XmlElement("Position")]
        public string Position { get; set; }

        [Required]
        [XmlElement("Weapon")]
        public string Weapon { get; set; }

        [XmlElement("DepartmentId")]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public ImportOfficerPrisonderDto[] Prisoners { get; set; }
    }
}
