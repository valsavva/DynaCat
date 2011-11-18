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
        public XEdgeType DefaultEdge;
		
		
        [XmlElement(ElementName = "Edge", Type = typeof(XEdge))]
        [XmlElement(ElementName = "Teleport", Type = typeof(XTeleport))]
		public XEdge[] Edges;
		
		public override void Initialize (InitializeParameters p)
		{
			InitializeEdges();

			base.Initialize(p);
			
			p.LevelEngine.obstacles.Add(this);
        }

        private void InitializeEdges()
        {
            var tmp = this.Edges ?? new XEdge[0];

			this.Edges = new XEdge[4];
            for (int i = 0; i < 4; i++)
            {
                this.Edges[i] = tmp.FirstOrDefault(e => (int)e.Align == i)
					?? new XEdge() {  Align = (XAlignType)i, Type = this.DefaultEdge };
				
				this.Edges[i].Parent = this;
            }

			this.Subcomponents.AddRange(this.Edges);
        }

        public override bool ProcessCollision(LevelEngine level, Rectangle intersect)
        {
			// find the edge that is the opposite of the hero's directio
			var edgeAlign = level.hero.Direction.ToAlign().Reverse();
			var edge = this.Edges[(int)edgeAlign];
			
			return edge.ProcessCollision(level, intersect);
        }
    }
}
