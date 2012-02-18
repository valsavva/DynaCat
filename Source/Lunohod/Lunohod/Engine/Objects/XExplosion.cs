using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace Lunohod.Objects
{
	
    [XmlType("Explosion")]
	public class XExplosion : XElement
	{
		private List<XElement> eplodingElements = new List<XElement>();
		private List<string> rangeNames = new List<string>();
		private List<float> rangeValuesSquared = new List<float>();
		
		[XmlAttribute]
		public string Ranges;
		
		
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.Ranges.Split(',').ForEach(s => {
				var parts = s.Split('=');
				rangeNames.Add(parts[0]);
				var rangeValue = float.Parse(parts[1], CultureInfo.InvariantCulture);
				rangeValuesSquared.Add(rangeValue * rangeValue);
			});
		}
		
		public void Explode()
		{
			this.eplodingElements.Clear();
			
			this.GetScreenBounds();
			
			this.PropState.ScreenBounds.Value.Center(ref this.tmpVector1);
			
			this.GetRoot().TraveseTree(o => {
				XElement e = o as XElement;
				
				if (e == null || !e.IsExploding)
					return;
				
				if (!e.PropState.ScreenBounds.HasValue)
					return;
				
				e.PropState.ScreenBounds.Value.Center(ref this.tmpVector2);
				
				var distanceSquared = this.tmpVector1.SquaredDistanceTo(this.tmpVector2);
				
				for(int i = 0; i < rangeNames.Count; i++)
				{
					if (distanceSquared <= rangeValuesSquared[i])
						e.GetSignalContainer("events").Signal(rangeNames[i]);
				}
			});
		}
	}
}

