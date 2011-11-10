using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    [XmlType("Hero")]
    public class XHero : XElement
    {
		private Vector2 offset;
		
		[XmlAttribute]
		public float Speed;
		
		[XmlIgnore]
		public Vector2 Direction;

        [XmlAttribute("Direction")]
        public string zDirection
		{
			get { return this.Direction.ToStr(); }
			set { this.Direction = value.ToVector2(); }
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			p.LevelEngine.hero = this;
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			offset = this.Direction * (Speed * (float)p.GameTime.ElapsedGameTime.TotalSeconds);
			
			this.Bounds.Offset((int)Math.Round(offset.X), (int)Math.Round(offset.Y));
		}
		
		public void AlignToBoundaryOf(XElement e, Vector2 direction)
        {
        }
    }
}
