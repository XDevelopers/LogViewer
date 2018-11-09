using System.Xml.Serialization;

namespace Sitatex.Logging.Models
{
    [XmlRoot(ElementName = "rollingStyle")]
    public class RollingStyle
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

}
