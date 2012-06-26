using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using Lunohod.Xge;

namespace Lunohod.Objects
{
	
    [XmlType("Explosion")]
	public class XExplosion : XElement
	{
		private List<XElement> eplodingElements;
		private List<string> rangeNames;
		private List<double> rangeValuesSquared;
		
		[XmlAttribute]
		public string Ranges;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			var pairs = this.Ranges.Split(',');
			
			eplodingElements = new List<XElement>(pairs.Length);
			rangeNames = new List<string>(pairs.Length);
			rangeValuesSquared = new List<double>(pairs.Length);
	
			pairs.ForEach(s => {
				var parts = s.Split('=');
				rangeNames.Add(parts[0]);
				var rangeValue = double.Parse(parts[1], CultureInfo.InvariantCulture);
				rangeValuesSquared.Add(rangeValue * rangeValue);
			});
		}
		
		public void Explode()
		{
			this.eplodingElements.Clear();
			
			this.GetScreenBounds();
			
			this.PropState.ScreenBounds.Value.Center(ref this.tmpVector1);
			
			XLevel level = this.GetScreen() as XLevel;
			
			for(int i = 0; i < level.Exploding.Count; i++)
			{
				XElement e = level.Exploding[i];
				
				if (!e.Enabled || !((IExploding)e).IsExploding)
					continue;
				
				e.GetScreenBounds();
				
				e.PropState.ScreenBounds.Value.Center(ref this.tmpVector2);
				
				var distanceSquared = this.tmpVector1.SquaredDistanceTo(this.tmpVector2);
				
				for(int j = 0; j < rangeNames.Count; j++)
				{
					if (distanceSquared <= rangeValuesSquared[j])
					{
						this.EnqueueEvent(e.Id + ":" + rangeNames[j]);
					}
				}
			}
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Ranges = reader["Ranges"];

            base.ReadXml(reader);
        }

		public override void GetMethod(string methodName, out Action<List<Expression>> method)
		{
            switch (methodName)
            {
				case "Explode": method = (ps) => Explode(); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
	}
}

