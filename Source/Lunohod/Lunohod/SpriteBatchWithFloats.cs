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
        private Rectangle tmpRect1;
        private Rectangle tmpRect2;

        public SpriteBatchWithFloats(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        internal void Draw(Texture2D texture2D, System.Drawing.RectangleF screenBounds, System.Drawing.RectangleF? sourceRectangle, Color color, double rotation, Vector2 origin, SpriteEffects spriteEffects, float layerDepth)
        {
            screenBounds.ToRectangle(ref tmpRect1);
			
			if (sourceRectangle.HasValue)
			{
            	sourceRectangle.Value.ToRectangle(ref tmpRect2);
	            base.Draw(texture2D, tmpRect1, tmpRect2, color, (float)rotation, origin, spriteEffects, layerDepth);
			}
			else
			{
	            base.Draw(texture2D, tmpRect1, null, color, (float)rotation, origin, spriteEffects, layerDepth);
			}

        }

        internal void Draw(Texture2D texture2D, Vector2 location, System.Drawing.RectangleF? sourceRectangle, Color color, double rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layerDepth)
        {
			if (sourceRectangle.HasValue)
			{
            	sourceRectangle.Value.ToRectangle(ref tmpRect2);
	            base.Draw(texture2D, location, tmpRect2, color, (float)rotation, origin, scale, spriteEffects, layerDepth);
			}
			else
			{
	            base.Draw(texture2D, location, null, color, (float)rotation, origin, scale, spriteEffects, layerDepth);
			}

        }

        internal void Draw(Texture2D texture2D, System.Drawing.RectangleF screenBounds, System.Drawing.RectangleF? sourceRectangle, Color actualBackColor)
        {
            screenBounds.ToRectangle(ref tmpRect1);
			
			if (sourceRectangle.HasValue)
			{
            	sourceRectangle.Value.ToRectangle(ref tmpRect2);
            	base.Draw(texture2D, tmpRect1, tmpRect2, actualBackColor);
			}
			else
			{
            	base.Draw(texture2D, tmpRect1, null, actualBackColor);
			}
        }

        internal void Draw(Texture2D texture2D, System.Drawing.RectangleF screenBounds, Color c)
        {
            screenBounds.ToRectangle(ref tmpRect1);

            base.Draw(texture2D, tmpRect1, c);
        }
    }
}
