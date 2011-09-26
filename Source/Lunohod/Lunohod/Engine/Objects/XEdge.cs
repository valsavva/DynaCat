using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace Lunohod.Objects
{
    public enum XEdgeType
    {
        Bounce,
        Stick,
        Teleport
    }

    public enum XAlignType
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 3
    }

    [XmlType("Edge")]
    public class XEdge
    {
        [XmlAttribute]
        public XAlignType Align;
        [XmlAttribute]
        public XEdgeType Type = XEdgeType.Stick;

        public virtual void ProcessCollision(LevelEngine engine, XBlock block, RectangleF newBounds)
        {
            var hero = engine.hero;

            if (this.Type == XEdgeType.Stick)
            {
                hero.AlignToBoundaryOf(block, hero.Move);
                hero.Move = XHeroMoveType.None;
            }
            else if (this.Type == XEdgeType.Bounce)
            {
                hero.AlignToBoundaryOf(block, hero.Move);
                hero.Move = hero.Move.ReverseMove();
            }
        }

        public Color GetBrush()
        {
            switch (this.Type)
            {
                case XEdgeType.Bounce: return Color.Black;
                case XEdgeType.Stick: return Color.Green;
                case XEdgeType.Teleport: return Color.Orange;
            }
            return Color.Black;
        }
    }
}
