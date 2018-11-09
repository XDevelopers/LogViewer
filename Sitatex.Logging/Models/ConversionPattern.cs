using System.Xml.Serialization;

namespace Sitatex.Logging.Models
{
    [XmlRoot(ElementName = "conversionPattern")]
    public class ConversionPattern
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

}
