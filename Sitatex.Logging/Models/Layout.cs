using System.Xml.Serialization;

namespace Sitatex.Logging.Models
{
    [XmlRoot(ElementName = "layout")]
    public class Layout
    {
        [XmlElement(ElementName = "conversionPattern")]
        public ConversionPattern ConversionPattern { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

}
