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
    public class XLevel : XComponent
    {
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public double Width;
        [XmlAttribute]
        public double Height;
		
        [XmlElement(ElementName = "Resources", Type = typeof(XResourceBundle))]
        [XmlElement(ElementName = "Layer", Type = typeof(XLayer))]
        public override XComponent[] Subcomponents { get; set; }
		
		[XmlIgnore]
		public XLayer[] Layers;
		
		[XmlIgnore]
		public XResourceBundle Resources { get; private set; }
		
		public override void Initialize(InitializeParameters p)
		{
			this.Resources = this.GetComponent<XResourceBundle>();
			this.Layers = this.GetComponents<XLayer>().ToArray();
			
			base.Initialize(p);
		}
		
		public override void Dispose()
		{
			base.Dispose();

			Layers.ForEach(l => l.Dispose());
		}
    }
}
