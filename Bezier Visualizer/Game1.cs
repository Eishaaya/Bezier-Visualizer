using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace Bezier_Visualizer
{
    public class Game1 : Game
    {
        public static Vector2 bounds = new Vector2(940, 1080);
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Bezier2D bezier;
        List<Button> points;
        Button pointMaker;
        Button clear;
        Button delete;
        Button run;
        MouseState ms;
        KeyboardState ks;
        bool prevDown;
        int grabbedIndex;
        ScalableSprite graphBackGround;
        ScalableSprite midBeam;
        List<ScalableSprite> gridLines;

        Texture2D draggedTexture;
        Button draggedPoint;

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
            //tell chris to remember the number

            ms = new MouseState();
            ks = new KeyboardState();
            points = new List<Button>();
            gridLines = new List<ScalableSprite>();
            draggedPoint = null;
            prevDown = false;
            grabbedIndex = -1;

            draggedTexture = Content.Load<Texture2D>("Point");



            graphBackGround = new ScalableSprite(Content.Load<Texture2D>("Pixel"), Vector2.Zero, Color.LightGray, 0, SpriteEffects.None, Vector2.Zero, new Vector2(540, 1080), 1);
            midBeam = new ScalableSprite(Content.Load<Texture2D>("Pixel"), new Vector2(0, bounds.Y / 4), Color.Black, 0, SpriteEffects.None, new Vector2(0, .5f), new Vector2(540, 10), 1);
            gridLines.Add(midBeam);
            midBeam = new ScalableSprite(Content.Load<Texture2D>("Pixel"), new Vector2(0, bounds.Y * 3 / 4), Color.Black, 0, SpriteEffects.None, new Vector2(0, .5f), new Vector2(540, 10), 1);
            gridLines.Add(midBeam);
            midBeam = new ScalableSprite(Content.Load<Texture2D>("Pixel"), new Vector2(5, 0), Color.Black, 0, SpriteEffects.None, new Vector2(.5f, 0), new Vector2(10, 1080), 1);
            gridLines.Add(midBeam);
            midBeam = new ScalableSprite(Content.Load<Texture2D>("Pixel"), new Vector2(535, 0), Color.Black, 0, SpriteEffects.None, new Vector2(.5f, 0), new Vector2(10, 1080), 1);
            gridLines.Add(midBeam);

            int lineAmount = 20;
            
            for (int i = 0; i <= lineAmount; i++)
            {
                gridLines.Add(new ScalableSprite(Content.Load<Texture2D>("Pixel"), new Vector2(0, i * bounds.Y / lineAmount), Color.Black, 0, SpriteEffects.None, new Vector2(0, .5f), new Vector2(540, 2), 1));
                if (i < lineAmount / 2)
                {
                    gridLines.Add(new ScalableSprite(Content.Load<Texture2D>("Pixel"), new Vector2(i * bounds.Y / lineAmount, 0), Color.Black, 0, SpriteEffects.None, new Vector2(.5f, 0), new Vector2(2, 1080), 1));
                }
            }

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



            if (pointMaker.check(mousePos, mouseDown) && !prevDown)
            {
                if (draggedPoint == null)
                {
                    SetDraggedPoint(mousePos);
                }
            }
            else if (delete.check(mousePos, mouseDown))
            {
                DeletePoint();
            }
            else if (clear.check(mousePos, mouseDown))
            {
                points.Clear();
            }
            else if (run.check(mousePos, mouseDown))
            {
                ;
            }
            else if (draggedPoint != null)
            {
                DragLogic(mousePos, mouseDown);
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    if (point.check(mousePos, mouseDown) && !prevDown)
                    {
                        SetDraggedPoint(mousePos, i);
                    }
                }
            }

            prevDown = mouseDown;
        }

        void DeletePoint()
        {
            if (draggedPoint != null)
            {
                if (grabbedIndex >= 0)
                {
                    points.RemoveAt(grabbedIndex);
                }
                draggedPoint = null;

                ColorPoints();
            }
        }

        void PlacePoint()
        {
            if (draggedPoint.Location.X > bounds.Y / 2 || draggedPoint.Location.X < 0 || draggedPoint.Location.Y > bounds.Y || draggedPoint.Location.Y < 0)
            {
                return;
            }
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
            ColorPoints();
        }

        void ColorPoints()
        {
            for (int i = 0; i < points.Count; i++)
            {
                var degree = (float)i / (points.Count != 1? (float)points.Count - 1 : 1);
                points[i].NormalColor = Color.Lerp(Color.CornflowerBlue, Color.Red, degree);
                points[i].Color = points[i].originalColor;
            }
        }

        void SetDraggedPoint(Vector2 mousePos, int index = -1)
        {
            draggedPoint = new Button(draggedTexture, mousePos, Color.White, 0, SpriteEffects.None, new Vector2(10, 10), 1, 1, Color.DarkGray, Color.Gray);
            grabbedIndex = index;
        }

        void DragLogic(Vector2 mousePos, bool mouseDown)
        {
            draggedPoint.Location = mousePos;
            if (ms.RightButton == ButtonState.Pressed && !prevDown)
            {
                DeletePoint();
            }
            else if (mouseDown && !prevDown)
            {
                PlacePoint();
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SlateGray);
            spriteBatch.Begin();
            graphBackGround.Draw(spriteBatch);
            pointMaker.Draw(spriteBatch);
            delete.Draw(spriteBatch);
            clear.Draw(spriteBatch);
            run.Draw(spriteBatch);

            foreach (ScalableSprite line in gridLines)
            {
                line.Draw(spriteBatch);
            }

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
