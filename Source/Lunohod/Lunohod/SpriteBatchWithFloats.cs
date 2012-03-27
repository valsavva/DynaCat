using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod
{
    public class SpriteBatchWithFloats : SpriteBatch
    {
        private Rectangle tmpRect;

        public SpriteBatchWithFloats(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        internal void Draw(Texture2D texture2D, System.Drawing.RectangleF screenBounds, Rectangle? sourceRectangle, Color color, double rotation, Vector2 origin, SpriteEffects spriteEffects, float layerDepth)
        {
            screenBounds.ToRectangle(ref tmpRect);

            base.Draw(texture2D, tmpRect, sourceRectangle, color, (float)rotation, origin, spriteEffects, layerDepth);
        }

        internal void Draw(Texture2D texture2D, System.Drawing.RectangleF screenBounds, Rectangle? sourceRectangle, Color actualBackColor)
        {
            screenBounds.ToRectangle(ref tmpRect);

            base.Draw(texture2D, tmpRect, sourceRectangle, actualBackColor);
        }

        internal void Draw(Texture2D texture2D, System.Drawing.RectangleF screenBounds, Color c)
        {
            screenBounds.ToRectangle(ref tmpRect);

            base.Draw(texture2D, tmpRect, c);
        }
    }
}
