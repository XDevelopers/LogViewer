using System.Xml.Serialization;

namespace Sitatex.Logging.Models
{
    [XmlRoot(ElementName = "maximumFileSize")]
    public class MaximumFileSize
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

}
