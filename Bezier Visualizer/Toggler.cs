using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bezier_Visualizer
{
    class Toggler : Button
    {
        public Sprite Ball;
        public Sprite bottomColor;
        public ScalableSprite MovingColor;
        public Label laby;
        public bool On { get; private set; }
        public bool Done { get; private set; }
        public bool hold { get; set; }

        Vector2 setOff;

        public Toggler(Texture2D image, Vector2 location, Vector2 origin, Sprite ball, Sprite bottom, ScalableSprite movingColor, SpriteFont font = null, string text = "")
            : this(image, location, Color.White, Color.DarkGray, Color.Gray, origin, ball, bottom, movingColor, font, text) { }
        public Toggler(Texture2D image, Vector2 location, Color color, Color hoverColor, Color clickedColor, Vector2 origin, Sprite ball, Sprite bottom, ScalableSprite movingColor, SpriteFont font = null, string text = "")
            : this(image, location, color, 0, SpriteEffects.None, origin, 1, 1, hoverColor, clickedColor, ball, bottom, movingColor, font, text) { }
        public Toggler(Texture2D image, Vector2 location, Color color, float rotation, SpriteEffects effect, Vector2 origin, float superscale, float depth, Color hovercolor, Color clickedcolor, Sprite Ball, Sprite Bottom, ScalableSprite Moving, SpriteFont font = null, string text = "", float stringH = 50, float offx = 0, float offy = 0, bool On = false)
            : base(image, location, color, rotation, effect, origin, superscale, depth, hovercolor, clickedcolor)
        {
            Done = true;

            if (font != null)
            {
                laby = new Label(font, Color, new Vector2(location.X + image.Width / 2 - (int)font.MeasureString(text).X / 2, location.Y + stringH), text, TimeSpan.Zero);
            }
            this.Ball = Ball;
            bottomColor = Bottom;
            MovingColor = Moving;
            this.On = On;
            if (this.On)
            {
                this.Ball.Location = Location + new Vector2(Image.Width - this.Ball.Image.Width, 0) - setOff;
            }
            else
            {
                this.Ball.Location = Location + setOff;
            }
            setOff = new Vector2(offx, offy);
            this.Ball.Location += setOff;
            MovingColor.Scale2D = new Vector2((this.Ball.Location.X - Location.X) / (Image.Width - this.Ball.Image.Width), MovingColor.Scale2D.Y);
        }
      
        public override bool check(Vector2 cursor, bool isclicked)
        {
            Move();
            var tempState = base.check(cursor, isclicked);
            if (!hold)
            {
                if (Done)
                {
                    Done = !tempState;
                    return !Done;
                }
                if (tempState)
                {
                    On = !On;
                    return tempState;
                }
            }
            return false;
        }
        public void Move()
        {
            if (!Done)
            {
                if (!On)
                {
                    Ball.Location = Vector2.Lerp(Ball.Location, Location + new Vector2(Image.Width - Ball.Image.Width, 0) - setOff, .1f);
                    MovingColor.Scale2D = new Vector2((Ball.Location.X - Location.X) / (Image.Width - Ball.Image.Width), MovingColor.Scale2D.Y);
                    if (Vector2.Distance(Ball.Location, Location + new Vector2(Image.Width - Ball.Image.Width, 0) - setOff) <= .1f)
                    {
                        Ball.Location = Location + new Vector2(Image.Width - Ball.Image.Width, 0) - setOff;
                        On = !On;
                        Done = true;
                    }
                }
                else
                {
                    Ball.Location = Vector2.Lerp(Ball.Location, Location + setOff, .1f);
                    MovingColor.Scale2D = new Vector2((Ball.Location.X - Location.X) / (Image.Width - Ball.Image.Width), MovingColor.Scale2D.Y);
                    if (Vector2.Distance(Ball.Location, Location + setOff) <= .1f)
                    {
                        Ball.Location = Location + setOff;
                        On = !On;
                        Done = true;
                    }
                }
            }
        }

        public void Set (bool on)
        {
            On = on;
            Done = true;
            if (On)
            {
                Ball.Location = Location + new Vector2(Image.Width - Ball.Image.Width, 0) - setOff;
                return;
            }
            else
            {
                Ball.Location = Location + setOff;
                return;
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            bottomColor.Draw(batch);
            MovingColor.Draw(batch);
            base.Draw(batch);
            Ball.Draw(batch);
            if (laby != null)
            {
                laby.Print(batch);
            }
        }
    }
}
