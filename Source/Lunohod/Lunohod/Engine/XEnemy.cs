using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    /// <summary>
    /// Represents a hero's "enemy". Enemy is an entity that can damage hero by the <see cref="Damage"/> amount
    /// when it attacks him.
    /// </summary>
    [XmlType("Enemy")]
    public class XEnemy : XElement, IExploding
    {
		/// <summary>
		/// The damage the enemy will cause when attacking the hero. Default value is 1.
		/// </summary>
		[XmlAttribute]
		public float Damage = 1;
        /// <inheritdoc />
        [XmlAttribute]
        public bool IsExploding { get; set; }
        /// <summary>
        /// Makes the enemy attack hero. This will depleat hero's health by the amount specified in the <see cref="Damage"/> attribute.
        /// </summary>
		public void Attack()
		{
			var hero = GameEngine.Instance.LevelEngine.Hero;
			hero.InflictDamage(this.Damage);
		}
    }
}

