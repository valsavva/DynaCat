using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunohod;

namespace Lunohod.Objects
{
    [XmlType("Level")]
    public class XLevel : XScreen
    {
		[XmlIgnore]
		public XLevelSettings DefaultSettings { get; private set; }
		
		[XmlIgnore]
		public List<XElement> Exploding { get; private set; }
		
		public override void Initialize(InitializeParameters p)
		{
			this.DefaultSettings = this.GetComponents<XLevelSettings>().FirstOrNew();
			this.Exploding = new List<XElement>();

			base.Initialize(p);
		}
    }
}
