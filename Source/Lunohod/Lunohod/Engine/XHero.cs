using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    /// <summary>
    /// Represents the main character in the game.
    /// </summary>
    [XmlType("Hero")]
    public class XHero : XElement
    {
		private Vector2 offset;
		private Vector2 towerCenter;
		private Vector2 heroCenter;
		private double distanceToTower;


        /// <summary>
        /// Specifies hero's default speed.
        /// </summary>
        [XmlAttribute]
        public float DefaultSpeed { get; set; }
        /// <summary>
        /// Specifies hero's current speed.
        /// </summary>
        [XmlAttribute]
        public float Speed { get; set; }
        /// <summary>
        /// Specifies hero's deceleration.
        /// </summary>
        [XmlAttribute]
        public float Deceleration { get; set; }
        /// <summary>
		/// Specifies hero's direction.
		/// </summary>
		[XmlIgnore]
		public Vector2 Direction;
        /// <exclude />
        [XmlAttribute("Direction")]
        public string zDirection
		{
			get { return this.Direction.ToStr(); }
			set { this.Direction = value.ToVector2(); }
		}
		
		/// <summary>
		/// Gets hero's current health level.
		/// </summary>
		[XmlIgnore]
		public float Health { get; private set; }
		/// <summary>
		/// Gets a value indicating whether hero is dead.
		/// </summary>
		/// <value>
		/// <c>true</c> if hero is dead; otherwise, <c>false</c>.
		/// </value>
		[XmlIgnore]
		public bool IsDead { get { return this.Health <= 0; } }
		/// <summary>
		/// Gets a value indicating whether this <see cref="Lunohod.Objects.XHero"/> is in transaction.
		/// </summary>
		/// <value>
		/// <c>true</c> if in transaction; otherwise, <c>false</c>.
		/// </value>
        [XmlIgnore]
		public bool InTransaction { get; private set; }

        internal double DistanceToTower
        {
            get { return distanceToTower; }
        }
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			p.LevelEngine.Hero = this;
			this.Health = p.LevelEngine.LevelObject.DefaultSettings.HeroHealth;

            if (this.DefaultSpeed == 0)
                this.DefaultSpeed = this.Speed;
			
			distanceToTower = double.MaxValue;
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			// calculate the new location of the hero
			if (!this.InTransaction)
			{
                if (this.Direction != Lunohod.Direction.VectorStop)
                    this.Speed = this.Speed * (1.0f - this.Deceleration * (float)p.GameTime.ElapsedGameTime.TotalSeconds);

				offset = this.Direction * (this.Speed * (float)p.GameTime.ElapsedGameTime.TotalSeconds);
	
	            this.Bounds.Offset(offset.X, offset.Y);
			}
			
			// calculate distance to the tower
            p.LevelEngine.Tower.Bounds.Center(ref towerCenter);
			this.Bounds.Center(ref heroCenter);
			distanceToTower = (towerCenter - heroCenter).Length();
		}
        /// <summary>
        /// Puts hero into the "transactional" state, when he does not respond to the player commands.
        /// </summary>
		public void StartTransaction()
		{
			this.InTransaction = true;
		}
        /// <summary>
        /// Ends transactional state.
        /// </summary>
		public void EndTransaction()
		{
			this.InTransaction = false;
		}
		/// <summary>
		/// Sets hero's direction.
		/// </summary>
		/// <param name="sx"></param>
		/// <param name="sy"></param>
		public void SetDirection(string sx, string sy)
		{
			int x = int.Parse(sx);
			int y = int.Parse(sy);
			
			this.Direction = new Vector2(x, y);
		}
        /// <summary>
        /// Causes hero to lose the <c>damage</c> amount of health.
        /// </summary>
        /// <param name="damage">Amount of healt hero should lose.</param>
		public void InflictDamage(float damage)
		{
			this.Health -= damage;
		}
    }
}
