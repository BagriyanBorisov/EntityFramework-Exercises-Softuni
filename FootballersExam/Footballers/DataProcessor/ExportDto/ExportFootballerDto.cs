using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Footballers.Data.Models.Enums;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType("Footballer")]
    public class ExportFootballerDto
    {
        [XmlElement("Name")] 
        public string Name { get; set; } = null!;

        [XmlElement("Position")]
        public PositionType PositionType { get; set; } 
    }
}
