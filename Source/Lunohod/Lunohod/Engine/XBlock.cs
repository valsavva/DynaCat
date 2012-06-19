using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunohod.Xge;

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

        public void SetEdges(XEdgeType edgeType)
        {
            this.Edges = edgeType;
        }

        public override bool ProcessCollision(LevelEngine level, System.Drawing.RectangleF intersect)
        {
			switch (this.Edges)
			{
				case XEdgeType.None : break;
				case XEdgeType.Bounce : {

					var sb = this.GetScreenBounds();

					if (level.Hero.DirectionX > 0)
						level.Hero.Bounds.X = sb.Left - level.Hero.Bounds.Width - (level.Hero.Bounds.Right - sb.Left);
					else if (level.Hero.DirectionX < 0)
						level.Hero.Bounds.X = sb.Right + sb.Right - level.Hero.Bounds.Left;
					else if (level.Hero.DirectionY > 0)
						level.Hero.Bounds.Y = sb.Top - level.Hero.Bounds.Height - (level.Hero.Bounds.Bottom - sb.Top);
					else if (level.Hero.DirectionY < 0)
						level.Hero.Bounds.Y = sb.Bottom + sb.Bottom - level.Hero.Bounds.Top;

					// change hero's direction
					level.Hero.Direction = level.Hero.Direction.Reverse();
					
					this.EnqueueEvent(level.Hero.Id + ":bounce");
				}; break;
				case XEdgeType.Stick : {
					var sb = this.GetScreenBounds();

					if (level.Hero.DirectionX > 0)
						level.Hero.Bounds.X = sb.Left - level.Hero.Bounds.Width;
					else if (level.Hero.DirectionX < 0)
						level.Hero.Bounds.X = sb.Right;
					else if (level.Hero.DirectionY > 0)
						level.Hero.Bounds.Y = sb.Top - level.Hero.Bounds.Height;
					else if (level.Hero.DirectionY < 0)
						level.Hero.Bounds.Y = sb.Bottom;

					level.Hero.Direction = Direction.VectorStop;

					this.EnqueueEvent(level.Hero.Id + ":stick");
				}; break;
				case XEdgeType.Teleport : break;
			}
			
			return true;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsEnum<XEdgeType>("Edges", ref this.Edges);

            base.ReadXml(reader);
        }

		public override void GetMethod(string methodName, out Action<List<Expression>> method)
		{
            switch (methodName)
            {
				case "SetEdges": method = (ps) => SetEdges((XEdgeType)Enum.Parse(typeof(XEdgeType), ps[0].GetStrValue())); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
	}
}
