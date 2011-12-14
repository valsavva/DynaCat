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
		
		private Vector2 location;
		
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
			
			location = new Vector2(this.Bounds.X, this.Bounds.Y);
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			offset = this.Direction * (this.Speed * (float)p.GameTime.ElapsedGameTime.TotalSeconds);
			
			location += offset;
			
			this.Bounds.X = (int)location.X;
			this.Bounds.Y = (int)location.Y;
			
			//this.Bounds.Offset((int)Math.Round(offset.X), (int)Math.Round(offset.Y));
		}
		
		public void AlignToBoundaryOf(XElement e, Vector2 direction)
        {
        }
    }
}
