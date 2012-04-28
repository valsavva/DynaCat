using System;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Lunohod.Objects
{
    [XmlRoot("SettingsFile")]
	public class XSettingsFile : XObject
	{
		[XmlAttribute]
		public bool MuteSound;
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("MuteSound", MuteSound.ToString(CultureInfo.InvariantCulture));
			
			base.WriteXml(writer);
		}
		
		public override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadAttrAsBoolean("MuteSound", ref this.MuteSound);
			
			base.ReadXml(reader);
		}
	}
}

