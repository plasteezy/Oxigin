using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Web.Admin.Models
{
    [XmlRoot(ElementName = "params")]
    public class QueryParameterCollection
    {
        [XmlElement("param")]
        public List<QueryParameter> QueryParameters { get; set; }
    }

    [XmlRoot(ElementName = "param")]
    public class QueryParameter
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string RawType { get; set; }

        [XmlAttribute("default")]
        public string Default { get; set; }

        [XmlElement("ui")]
        public UI UserInterfaceElement { get; set; }

        [XmlIgnore]
        public QueryParameterType Type { get; set; }

        [XmlIgnore]
        public string Value { get; set; }

        [XmlRoot(ElementName = "ui")]
        public class UI
        {
            [XmlAttribute("type")]
            public string RawType { get; set; }

            [XmlIgnore]
            public UserInterfaceElementType Type { get; set; }

            [XmlAnyAttribute]
            public XmlAttribute[] CustomAttributes { get; set; }
        }
    }
}