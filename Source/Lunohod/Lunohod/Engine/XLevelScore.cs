using System;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;

namespace Lunohod.Objects
{
    [XmlType("LevelScore")]
	public class XLevelScore : XObject
	{
		[XmlAttribute]
		public double AvaliablePoints;
		[XmlAttribute]
		public double Score;
		[XmlAttribute]
		public double Time;
		[XmlAttribute]
		public double Health;
		
		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);

			reader.ReadAttrAsFloat("AvailablePoints", ref this.AvaliablePoints);
			reader.ReadAttrAsFloat("Score", ref this.Score);
			reader.ReadAttrAsFloat("Time", ref this.Time);
			reader.ReadAttrAsFloat("Health", ref this.Health);
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("LevelScore");

			base.WriteXml(writer);
			writer.WriteAttributeString("AvailablePoints", this.AvaliablePoints.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("Score", this.Score.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("Time", this.Time.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("Health", this.Health.ToString(CultureInfo.InvariantCulture));
			
			writer.WriteEndElement();
		}
		
		public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			switch (propertyName)
			{
                case "AvaliablePoints": getter = () => AvaliablePoints; setter = (v) => AvaliablePoints = v; break;
                case "Score": getter = () => Score; setter = (v) => Score = v; break;
                case "Time": getter = () => Time; setter = (v) => Time = v; break;
                case "Health": getter = () => Health; setter = (v) => Health = v; break;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
	}
}

