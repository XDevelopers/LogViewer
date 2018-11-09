using System.Xml.Serialization;

namespace Sitatex.Logging.Models
{
    [XmlRoot(ElementName = "appendToFile")]
    public class AppendToFile
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

}
