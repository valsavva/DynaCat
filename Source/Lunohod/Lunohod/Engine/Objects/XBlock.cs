using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace Lunohod.Objects
{
    [XmlType("Block")]
    public class XBlock : XElement
    {
        [XmlAttribute]
        public XEdgeType DefaultEdge;

        [XmlElement(ElementName = "Edge", Type = typeof(XEdge))]
        [XmlElement(ElementName = "Teleport", Type = typeof(XTeleport))]
        public XEdge[] Edges;
		
		public override void Initialize (InitializeParameters p)
		{
			base.Initialize(p);

			InitializeEdges();
        }

        private void InitializeEdges()
        {
            var tmp = this.Edges ?? new XEdge[0];
            this.Edges = new XEdge[4];
            for (int i = 0; i < 4; i++)
            {
                this.Edges[i] = tmp.FirstOrDefault(e => (int)e.Align == i) ?? new XEdge() { Align = (XAlignType)i, Type = this.DefaultEdge };
            }
        }

        public override void ProcessCollision(LevelEngine engine, RectangleF newBounds)
        {
            var hero = engine.hero;

            XEdge edge = this.Edges[(int)hero.Move.ReverseEdge()];

            edge.ProcessCollision(engine, this, newBounds);
        }
    }
}
