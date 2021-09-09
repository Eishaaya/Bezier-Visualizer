using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bezier_Visualizer
{
    class ScalableSprite : Sprite
    {
        public Vector2 Scale2D;
        public override Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)(Location.X + Origin.X * Scale2D.X), (int)(Location.Y + Origin.Y * Scale2D.Y), (int)(Image.Width * Scale2D.X), (int)(Image.Height * Scale2D.Y));
            }
        }
        public ScalableSprite (Texture2D image, Vector2 location, Color color, float rotation, SpriteEffects effects, Vector2 origin, Vector2 Scale, float depth = 1, float plebScale = 1)
            :base (image, location, color, rotation, effects, origin, plebScale, depth)
        {
            Scale2D = Scale;
        }

        #region clone

        public new ScalableSprite Clone()
        {
            var copy = new ScalableSprite(Image, Location, Color, rotation, effect, Origin, Scale2D, Depth, Scale);
            CloneLogic(copy);

            return copy;
        }
        protected new void CloneLogic<T>(T copy) where T : ScalableSprite
        {
            base.CloneLogic(copy);
            copy.Scale2D = Scale2D;
        }

        #endregion


        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(Image, Location, null, Color, rotation, Origin, Scale2D, effect, Depth);
        }

    }
}
