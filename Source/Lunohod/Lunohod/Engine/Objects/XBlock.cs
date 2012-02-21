using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
	
    [XmlType("Block")]
    public class XBlock : XElement, IExploding
    {
		/// <summary>
		/// The default edge.
		/// </summary>
        [XmlAttribute]
        public XEdgeType Edges;
        /// <inheritdoc />
        [XmlAttribute]
        public bool IsExploding { get; set; }
        
        public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			p.LevelEngine.obstacles.Add(this);
        }

        public override bool ProcessCollision(LevelEngine level, System.Drawing.RectangleF intersect)
        {
			switch (this.Edges)
			{
				case XEdgeType.None : break;
				case XEdgeType.Bounce : {
					// change hero's direction
					level.Hero.Direction = level.Hero.Direction.Reverse();
					
					level.Hero.Bounds.Offset(
						intersect.Width * level.Hero.Direction.X,
						intersect.Height * level.Hero.Direction.Y
					);
				}; break;
				case XEdgeType.Stick : {
					// don't change hero's direction, just keep him in place
					var direction = level.Hero.Direction.Reverse();
					
					level.Hero.Bounds.Offset(
						intersect.Width * direction.X,
						intersect.Height * direction.Y
					);
				
					level.Hero.Direction = Direction.VectorStop;
				}; break;
				case XEdgeType.Teleport : break;
			}
			
			return true;
        }
    }
}
