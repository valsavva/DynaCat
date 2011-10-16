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
	public class XAnimationBase : XObject
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
		
		[XmlAttribute]
		public string Target
		{
			get {
				if (TargetId == null || TargetProperty == null)
					return null;
				return TargetId + "." + TargetProperty;
			}
			set {
				var parts = value.Split('.');
				this.TargetId = parts[0];
				this.TargetProperty = parts[1];
			}
		}

		[XmlAttribute("Duration")]
		public string zDuration
		{
			get { return this.Duration.ToString(); }
			set { this.Duration = TimeSpan.Parse(value, CultureInfo.InvariantCulture); }
		}
	}
}

