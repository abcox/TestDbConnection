using System;
using System.Collections.Generic;
using System.Text;

namespace TestDbConnector.models
{
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("root")]

    public class ListenerResponse_TpPriceBookRecord
    {
        //<root>
        //  <inserted>
        //    <row>
        //      <Name>test3</Name>
        //      <Price>3</Price>
        //    </row>
        //  </inserted>

        [System.Xml.Serialization.XmlElement("inserted")]
        public TpPriceBookRecord_Insert inserted { get; set; }

        [System.Xml.Serialization.XmlElement("updated")]
        public TpPriceBookRecord_Update updated { get; set; }
    }

    public class TpPriceBookRecord_Insert
    {
        [System.Xml.Serialization.XmlElement("row")]
        public TpPriceBookRecord row { get; set; }
    }

    public class TpPriceBookRecord_Update
    {
        [System.Xml.Serialization.XmlElement("row")]
        public TpPriceBookRecord row { get; set; }
    }

    public class TpPriceBookRecord
    {
        [System.Xml.Serialization.XmlElement("Id")]
        public int Id { get; set; }

        [System.Xml.Serialization.XmlElement("Name")]
        public string Name { get; set; }

        [System.Xml.Serialization.XmlElement("Price")]
        public decimal Price { get; set; }

        [System.Xml.Serialization.XmlElement("RefId")]
        public int? RefId { get; set; }

    }
}
