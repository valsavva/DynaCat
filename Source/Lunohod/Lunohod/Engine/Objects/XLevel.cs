using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
    [XmlType("Level")]
    public class XLevel : XObject
    {
        [XmlAttribute]
        public string Name;
        [XmlElement("Layer")]
        public XLayer[] Layers;
        [XmlAttribute]
        public double Width;
        [XmlAttribute]
        public double Height;
		
		public override void Initialize(InitializeParameters p)
		{
			foreach(var layer in this.Layers)
				layer.Initialize(p);
		}
		
		public override void Update(UpdateParameters p)
		{
			foreach(var layer in this.Layers)
				layer.Update(p);
		}

		public void Draw(DrawParameters p)
		{
			foreach(var layer in this.Layers)
				layer.Draw(p);
		}
    }
}
