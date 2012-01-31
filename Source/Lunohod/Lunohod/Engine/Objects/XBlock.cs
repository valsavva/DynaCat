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
    public class XBlock : XElement
    {
		/// <summary>
		/// The default edge.
		/// </summary>
        [XmlAttribute]
        public XEdgeType Edges;
		
		public override void Initialize (InitializeParameters p)
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
					level.hero.Direction = level.hero.Direction.Reverse();
					
					level.hero.Bounds.Offset(
						(int)(intersect.Width * level.hero.Direction.X),
						(int)(intersect.Height * level.hero.Direction.Y)
					);
				}; break;
				case XEdgeType.Stick : {
					// don't change hero's direction, just keep him in place
					var direction = level.hero.Direction.Reverse();
					
					level.hero.Bounds.Offset(
						(int)(intersect.Width * direction.X),
						(int)(intersect.Height * direction.Y)
					);
				
					level.hero.Direction = Direction.VectorStop;
				}; break;
				case XEdgeType.Teleport : break;
			}
			
			return true;
        }
    }
}
