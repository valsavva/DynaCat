using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace Lunohod.Objects
{
    [XmlType("Teleport")]
    public class XTeleport : XEdge
    {
        public XTeleport()
        {
            this.Type = XEdgeType.Teleport;
        }

        [XmlAttribute]
        public string Id;

        public override void ProcessCollision(LevelEngine engine, XBlock block, RectangleF newBounds)
        {
            XEdge otherEdge = null;
            XBlock otherBlock = engine.obstacles
                .Where(b => b is XBlock)
                .Cast<XBlock>()
                .Where<XBlock>(b =>
                {
                    otherEdge = b.Edges
                        .Where(e => this != e && e is XTeleport)
                        .Cast<XTeleport>()
                        .Where(t => t.Id == this.Id)
                        .FirstOrDefault();

                    return otherEdge != null;
                })
                .FirstOrDefault();

            if (otherEdge == null)
                throw new Exception("Other teleport was not found");

            var hero = engine.hero;

            RectangleF blockBounds = block.Bounds;
            RectangleF otherBounds = otherBlock.Bounds;

            float delta = 0;

            if (this.Align == XAlignType.Left || this.Align == XAlignType.Right)
                delta = (newBounds.Y - blockBounds.Y);
            else
                delta = (newBounds.X - blockBounds.X);

            hero.AlignToBoundaryOf(otherBlock, otherEdge.Align.ReverseMove());

            newBounds = hero.Bounds;

            if (otherEdge.Align == XAlignType.Left || otherEdge.Align == XAlignType.Right)
                newBounds.Y = otherBounds.Y + delta;
            else
                newBounds.X = otherBounds.X + delta;

            hero.Bounds = newBounds;

            hero.Move = (XHeroMoveType)otherEdge.Align;

            //engine.DequeuePastActions();
        }
    }
}
