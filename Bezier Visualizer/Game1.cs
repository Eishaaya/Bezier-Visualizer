using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Bezier_Visualizer
{
    public class Game1 : Game
    {
        public static Vector2 bounds = new Vector2(1920, 1080);
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Bezier2D bezier;
        List<Sprite> points;
        Button pointMaker;
        Button clear;
        Button delete;
        Button run;
        MouseState ms;
        KeyboardState ks;
        bool prevDown;
        int grabbedIndex;

        Texture2D draggedTexture;
        Sprite draggedPoint;

        public Game1()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)bounds.X;
            graphics.PreferredBackBufferHeight = (int)bounds.Y;
        }
        protected override void Initialize()
        {
            ms = new MouseState();
            ks = new KeyboardState();
            points = new List<Sprite>();
            draggedPoint = null;
            prevDown = false;
            grabbedIndex = -1;

            draggedTexture = Content.Load<Texture2D>("Point");
            run = new Button(Content.Load<Texture2D>("RunButton"), new Vector2(bounds.X - 400, 0), Color.White, 0, SpriteEffects.None, new Vector2(0, 0), 1, 1, Color.Gray, Color.DarkGray);
            pointMaker = new Button(Content.Load<Texture2D>("MakeButton"), new Vector2(bounds.X - 300, 0), Color.White, 0, SpriteEffects.None, new Vector2(0, 0), 1, 1, Color.Gray, Color.DarkGray);
            delete = new Button(Content.Load<Texture2D>("DeleteButton"), new Vector2(bounds.X - 200, 0), Color.White, 0, SpriteEffects.None, new Vector2(0, 0), 1, 1, Color.Gray, Color.DarkGray);
            clear = new Button(Content.Load<Texture2D>("ClearButton"), new Vector2(bounds.X - 100, 0), Color.White, 0, SpriteEffects.None, new Vector2(0, 0), 1, 1, Color.Gray, Color.DarkGray);
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ms = Mouse.GetState();
            ks = Keyboard.GetState();
            var mousePos = new Vector2(ms.Position.X, ms.Position.Y);
            var mouseDown = ms.LeftButton == ButtonState.Pressed;

            if (draggedPoint != null)
            {
                draggedPoint.Location = mousePos;
                if (ms.RightButton == ButtonState.Pressed && !prevDown)
                {
                    if (grabbedIndex >= 0)
                    {
                        points.RemoveAt(grabbedIndex);
                    }
                    draggedPoint = null;
                }
                else if (mouseDown && !prevDown)
                {
                    draggedPoint.Color = Color.White;
                    if (grabbedIndex < 0)
                    {
                        points.Add(draggedPoint);
                    }
                    else
                    {
                        points[grabbedIndex] = draggedPoint;
                    }
                    draggedPoint = null;
                }
            }
            else
            {
                if (pointMaker.check(mousePos, mouseDown))
                {
                    draggedPoint = new Sprite(draggedTexture, mousePos, Color.White, 0, SpriteEffects.None, new Vector2(10, 10), 1, 1);
                }
                else if (delete.check(mousePos, mouseDown))
                {
                    ;
                }
                else if (clear.check(mousePos, mouseDown))
                {
                    ;
                }
                else if (run.check(mousePos, mouseDown))
                {
                    ;
                }
            }
            prevDown = mouseDown;
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            pointMaker.Draw(spriteBatch);
            delete.Draw(spriteBatch);
            clear.Draw(spriteBatch);
            run.Draw(spriteBatch);

            foreach (Sprite point in points)
            {
                point.Draw(spriteBatch);
            }

            if (draggedPoint != null)
            {
                draggedPoint.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
