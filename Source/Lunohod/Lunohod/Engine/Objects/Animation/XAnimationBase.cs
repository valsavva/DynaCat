using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;

namespace Lunohod.Objects
{
	public class XAnimationBase : XComponent
	{
		public XAnimationBase()
		{
		}
		
		[XmlIgnore]
		public TimeSpan Duration;
		
		[XmlAttribute]
		public bool Autoreverse;
		
		[XmlAttribute]
		public string TargetId;
		
		[XmlAttribute]
		public string TargetProperty;

		[XmlAttribute("Duration")]
		public string zDuration
		{
			get { return this.Duration.ToString(); }
			set { this.Duration = TimeSpan.Parse(value, CultureInfo.InvariantCulture); }
		}
	}
}

