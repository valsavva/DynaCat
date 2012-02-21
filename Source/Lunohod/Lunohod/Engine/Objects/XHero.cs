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
		private Vector2 towerCenter;
		private Vector2 heroCenter;
		private double distanceToTower;
		
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
		
        [XmlIgnore]
		public bool InTransaction { get; private set; }
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			p.LevelEngine.Hero = this;
			
			distanceToTower = double.MaxValue;
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			// calculate the new location of the hero
			if (!this.InTransaction)
			{
				offset = this.Direction * (this.Speed * (float)p.GameTime.ElapsedGameTime.TotalSeconds);
	
	            this.Bounds.Offset(offset.X, offset.Y);
			}
			
			// calculate distance to the tower
            p.LevelEngine.Tower.Bounds.Center(ref towerCenter);
			this.Bounds.Center(ref heroCenter);
			distanceToTower = (towerCenter - heroCenter).Length();
		}
		
		public void StartTransaction()
		{
			this.InTransaction = true;
		}

		public void EndTransaction()
		{
			this.InTransaction = false;
		}
		
		public void SetDirection(string sx, string sy)
		{
			int x = int.Parse(sx);
			int y = int.Parse(sy);
			
			this.Direction = new Vector2(x, y);
		}
    }
}
