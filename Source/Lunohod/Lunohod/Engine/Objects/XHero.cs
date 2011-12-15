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
		private Vector2 towerCenter;
		private Vector2 heroCenter;
		private double distanceToTower;
		private Point oldLocation;
		
		
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
		
		[XmlIgnore]
		public double DistanceToTower
		{
			get { return distanceToTower; }
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			p.LevelEngine.hero = this;
			
			distanceToTower = double.MaxValue;
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			// calculate the new location of the hero
			offset = this.Direction * (this.Speed * (float)p.GameTime.ElapsedGameTime.TotalSeconds);

			if (oldLocation.X != this.Bounds.X || oldLocation.Y != this.Bounds.Y)
			{
				this.Bounds.ToVector2(ref location);

				oldLocation = this.Bounds.Location();
			}
			
			location += offset;
			
			this.Bounds.X = (int)location.X;
			this.Bounds.Y = (int)location.Y;
			
			// calculate distance to the tower
			p.LevelEngine.tower.Bounds.Center.ToVector2(ref towerCenter);
			this.Bounds.Center.ToVector2(ref heroCenter);
			distanceToTower = (towerCenter - heroCenter).Length();
		}
		
		public void AlignToBoundaryOf(XElement e, Vector2 direction)
        {
        }
    }
}
