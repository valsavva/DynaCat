using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Xml;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    /// <summary>
    /// Represents a hero's "enemy". Enemy is an entity that can damage hero by the <see cref="Damage"/> amount
    /// when it attacks him.
    /// </summary>
    [XmlType("Enemy")]
    public class XEnemy : XElement, IExploding, IHasPoints
    {
		[XmlAttribute]
		public double Points { get; set; }
		/// <summary>
		/// The damage the enemy will cause when attacking the hero. Default value is 1.
		/// </summary>
		[XmlAttribute]
		public double Damage = 1;
        /// <inheritdoc />
        [XmlAttribute]
        public bool IsExploding { get; set; }
		
		internal bool DamageSpecified;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
		}
		
        /// <summary>
        /// Makes the enemy attack hero. This will depleat hero's health by the amount specified in the <see cref="Damage"/> attribute.
        /// </summary>
		public void Attack()
		{
			var hero = ((LevelEngine)this.GetScreen().ScreenEngine).Hero;
			hero.InflictDamage(this.Damage);
		}

        public override void ReadXml(XmlReader reader)
        {
			this.Points = reader.ReadAttrAsFloat("Points", 0);
            this.DamageSpecified = reader.ReadAttrAsFloat("Damage", ref this.Damage);

            base.ReadXml(reader);
        }

		public override void GetMethod(string methodName, out Action<List<Expression>> method)
		{
            switch (methodName)
            {
				case "Attack": method = (ps) => Attack(); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
        public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
        {
            switch (propertyName)
            {
                case "Damage": getter = () => Damage; setter = (v) => Damage = v; break;
                default:
                    base.GetProperty(propertyName, out getter, out setter); break;
            }
        }
    }
}

