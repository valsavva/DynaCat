using System;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
    [XmlType("LevelInfo")]
	public class XLevelInfo : XObject
	{
        [XmlAttribute]
        public string Name;
		
		[XmlAttribute]
		public string File;

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Name = reader["Name"];
            this.File = reader["File"];

            base.ReadXml(reader);
			
			if (string.IsNullOrEmpty(this.Name))
				this.Name = this.Id;
        }
	}
}

