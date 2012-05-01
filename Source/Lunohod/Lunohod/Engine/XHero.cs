using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Lunohod.Xge;

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
        public double DefaultSpeed;
        /// <summary>
        /// Specifies hero's current speed.
        /// </summary>
        [XmlAttribute]
        public double Speed;
        /// <summary>
        /// Specifies hero's deceleration.
        /// </summary>
        [XmlAttribute]
        public double Deceleration;
        /// <summary>
		/// Specifies hero's direction.
		/// </summary>
		[XmlIgnore]
		public Vector2 Direction;
        /// <summary>
        /// Gets or sets the X component of Direction
        /// </summary>
        [XmlIgnore]
        public float DirectionX {get { return this.Direction.X; } set { this.Direction.X = value; } }
        /// <summary>
        /// Gets or sets the Y component of Direction
        /// </summary>
        [XmlIgnore]
        public float DirectionY {get { return this.Direction.Y; } set { this.Direction.Y = value; } }
		/// <summary>
		/// Gets the default health.
		/// </summary>
		/// <value>
		/// The default health.
		/// </value>
		[XmlIgnore]
		public double DefaultHealth { get; private set; }
		/// <summary>
		/// Gets the default bomb count.
		/// </summary>
		/// <value>
		/// The default bomb count.
		/// </value>
		[XmlIgnore]
		public double DefaultBombCount { get; private set; }
		/// <summary>
		/// Gets a value indicating whether hero is dead.
		/// </summary>
		/// <value>
		/// <c>true</c> if hero is dead; otherwise, <c>false</c>.
		/// </value>
		[XmlIgnore]
		public bool IsDead { get { return this.Health <= 0; } }
		/// <summary>
		/// Gets or sets the current bomb count.
		/// </summary>
		/// <value>
		/// The bomb count.
		/// </value>
		[XmlIgnore]
		public double BombCount;
		/// <summary>
		/// Gets hero's current health level.
		/// </summary>
		[XmlIgnore]
		public double Health;
		[XmlIgnore]
		public double Score;
		[XmlIgnore]
		public double Time;
		
		[XmlAttribute]
		public bool CanMove;
		[XmlAttribute]
		public bool CanCollide;
		[XmlAttribute]
		public bool CanReceiveSignals;
		
		
        internal double DistanceToTower
        {
            get { return distanceToTower; }
        }
		
		public XHero()
		{
			this.CanMove = true;
			this.CanCollide = true;
			this.CanReceiveSignals = true;
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			p.LevelEngine.Hero = this;
			
			this.DefaultHealth = p.LevelEngine.LevelInfo.HeroHealth;
			this.Health = this.DefaultHealth;
			
			this.DefaultBombCount = p.LevelEngine.LevelInfo.BombCount;
			this.BombCount = this.DefaultBombCount;

            if (this.DefaultSpeed == 0)
                this.DefaultSpeed = this.Speed;
			
			distanceToTower = double.MaxValue;
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			this.Time += p.GameTime.ElapsedGameTime.TotalSeconds;
			
			// calculate the new location of the hero
			if (this.CanMove)
			{
                if (this.Direction != Lunohod.Direction.VectorStop)
                    this.Speed = this.Speed * (1.0f - this.Deceleration * p.GameTime.ElapsedGameTime.TotalSeconds);

                offset = this.Direction * (float)(this.Speed * p.GameTime.ElapsedGameTime.TotalSeconds);
	
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
		public void SetTransaction(bool canMove = false, bool canCollide = true, bool canReceiveSignals = true)
		{
			this.CanMove = canMove;
			this.CanCollide = canCollide;
			this.CanReceiveSignals = canReceiveSignals;
		}
        /// <summary>
        /// Ends transactional state.
        /// </summary>
		public void EndTransaction()
		{
			this.CanMove = true;
			this.CanCollide = true;
			this.CanReceiveSignals = true;
		}
		/// <summary>
		/// Sets hero's direction.
		/// </summary>
		/// <param name="sx"></param>
		/// <param name="sy"></param>
		public void SetDirection(double x, double y)
		{
			this.Direction = new Vector2((float)x, (float)y);
		}
        /// <summary>
        /// Causes hero to lose the <c>damage</c> amount of health.
        /// </summary>
        /// <param name="damage">Amount of healt hero should lose.</param>
		public void InflictDamage(double damage)
		{
			this.Health -= damage;
		}
		public void AddScore(double points)
		{
			this.Score += points;
		}
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsFloat("DefaultSpeed", ref this.DefaultSpeed);
            reader.ReadAttrAsFloat("Speed", ref this.Speed);
            reader.ReadAttrAsFloat("Deceleration", ref this.Deceleration);
            reader.ReadAttrAsVector2("Direction", ref this.Direction);

            base.ReadXml(reader);
        }
		
		public override void GetMethod(string methodName, out Action<List<Expression>> method)
		{
            switch (methodName)
            {
                case "SetTransaction": method = (ps) => {
					if (ps == null)
						SetTransaction();
					else
						SetTransaction(ps.Count > 0 ? ps[0].GetBoolValue() : false, ps.Count > 1 ? ps[1].GetBoolValue() : true, ps.Count > 2 ? ps[2].GetBoolValue() : true); 
				}; break;
                case "EndTransaction": method = (ps) => EndTransaction(); break;
                case "SetDirection": method = (ps) => SetDirection(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
				case "AddScore": method = (ps) => AddScore(ps[0].GetNumValue()); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
		
		public override void GetProperty(string propertyName, out Func<bool> getter, out Action<bool> setter)
		{
			switch (propertyName)
			{
                case ("IsDead"): getter = () => IsDead; setter = null; break;
				case ("Enabled") : getter = () => Enabled; setter = (v) => Enabled = v; break;
				case ("CanMove") : getter = () => CanMove; setter = (v) => CanMove = v; break;
				case ("CanCollide") : getter = () => CanCollide; setter = (v) => CanCollide = v; break;
				case ("CanReceiveSignals") : getter = () => CanReceiveSignals; setter = (v) => CanReceiveSignals = v; break;
				default:
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}

		public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			switch (propertyName)
			{
                case "Health": getter = () => Health; setter = (v) => Health = v; break;
                case "Score": getter = () => Score; setter = (v) => Score = v; break;
                case "Time": getter = () => Time; setter = (v) => Time = v; break;
                case "DefaultHealth": getter = () => DefaultHealth; setter = (v) => DefaultHealth = v; break;
                case "Speed": getter = () => Speed; setter = (v) => Speed = v; break;
                case "DefaultSpeed": getter = () => DefaultSpeed; setter = (v) => DefaultSpeed = v; break;
                case "BombCount": getter = () => BombCount; setter = (v) => BombCount = v; break;
                case "DefaultBombCount": getter = () => DefaultBombCount; setter = (v) => DefaultBombCount = v; break;
                case "Deceleration": getter = () => Deceleration; setter = (v) => Deceleration = v; break;
                case "DirectionX": getter = () => DirectionX; setter = (v) => DirectionX = (float)v; break;
                case "DirectionY": getter = () => DirectionY; setter = (v) => DirectionY = (float)v; break;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
	}
}
