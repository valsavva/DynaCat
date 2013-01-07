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
        [XmlAttribute]
        public bool AskForReiew = true;
        [XmlAttribute]
        public int LaunchNumber = 0;
		
		public override void WriteXml(XmlWriter writer)
		{
            writer.WriteAttributeString("MuteSound", MuteSound.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("AskForReview", AskForReiew.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("LaunchNumber", LaunchNumber.ToString(CultureInfo.InvariantCulture));
			
			base.WriteXml(writer);
		}
		
		public override void ReadXml(System.Xml.XmlReader reader)
		{
            reader.ReadAttrAsBoolean("MuteSound", ref this.MuteSound);
            reader.ReadAttrAsBoolean("AskForReview", ref this.AskForReiew);
            reader.ReadAttrAsInt("LaunchNumber", ref this.LaunchNumber);
			
			base.ReadXml(reader);
		}
	}
}

