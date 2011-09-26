using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace Lunohod.Objects
{
    public enum XHeroMoveType
    {
        None = -1,
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    [XmlType("Hero")]
    public class XHero : XElement
    {
        [XmlAttribute]
        public double Speed;
        [XmlAttribute]
        public XHeroMoveType Move;

        public void AlignToBoundaryOf(XElement e, XHeroMoveType moveType)
        {
            RectangleF newBounds = this.Bounds;
            RectangleF elementBounds = e.Bounds;

            if (moveType == XHeroMoveType.Left)
                newBounds.X = elementBounds.Right + 1;
            else if (moveType == XHeroMoveType.Right)
                newBounds.X = elementBounds.Left - newBounds.Width - 1;
            else if (moveType == XHeroMoveType.Up)
                newBounds.Y = elementBounds.Bottom + 1;
            else if (moveType == XHeroMoveType.Down)
                newBounds.Y = elementBounds.Top - newBounds.Height - 1;
			
			this.Bounds = newBounds;
        }
    }
}
