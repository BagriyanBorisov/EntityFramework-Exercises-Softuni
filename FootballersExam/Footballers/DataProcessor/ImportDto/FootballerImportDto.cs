using Footballers.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    
    public class FootballerImportDto
    {
        [XmlElement("Name")]
        public string? Name { get; set; }

        [XmlElement("ContractStartDate")]
        public string? ContractStartDate { get; set; }

        [XmlElement("ContractEndDate")]
        public string? ContractEndDate { get; set; }

        [XmlElement("PositionType")]
        public int PositionType { get; set; }

        [XmlElement("BestSkillType")]
        public int BestSkillType { get; set; }

    }
}
