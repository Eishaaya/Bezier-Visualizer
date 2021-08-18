using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
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
        Sprite drawnLine;
        Vector2 offSet;
        Sprite settingsBox;

        Texture2D draggedTexture;
        Button draggedPoint;
        Button selectedPoint;
        ButtonLabel xLabel;
        ButtonLabel yLabel;
        ButtonLabel arrangementLabel;

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

            offSet = new Vector2(0, bounds.Y / 4);

            draggedTexture = Content.Load<Texture2D>("Point");

            

            graphBackGround = new ScalableSprite(Content.Load<Texture2D>("Pixel"), Vector2.Zero, Color.LightGray, 0, SpriteEffects.None, Vector2.Zero, new Vector2(540, 1080), 1);
            midBeam = new ScalableSprite(Content.Load<Texture2D>("Pixel"), new Vector2(0, offSet.Y), Color.Black, 0, SpriteEffects.None, new Vector2(0, .5f), new Vector2(540, 10), 1);
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
            settingsBox = new Sprite(Content.Load<Texture2D>("Box"), new Vector2(bounds.Y / 2, 47), Color.White, 0, SpriteEffects.None, Vector2.Zero, 1, 1);
            drawnLine = new Sprite(Content.Load<Texture2D>("Line"), Vector2.Zero, Color.Gold, 0, SpriteEffects.None, new Vector2(6), 1, 1);
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
            if (bezier != null)
            {
                bezier.Update(gameTime);
                drawnLine.Location = bezier.Location + offSet;
                drawnLine.rotation = (bezier.Rotation) * -1 + MathHelper.Pi;
            }
            else
            {
                drawnLine.Location = new Vector2(-1000);
            }

            if (selectedPoint != null)
            {

            }

            if (pointMaker.check(mousePos, mouseDown) && !prevDown && bezier == null)
            {
                if (draggedPoint == null)
                {
                    SetDraggedPoint(mousePos);
                }
                selectedPoint = null;
                grabbedIndex = -1;
            }
            else if (delete.check(mousePos, mouseDown))
            {
                if (bezier != null)
                {
                    bezier = null;
                }                
                DeletePoint();
            }
            else if (clear.check(mousePos, mouseDown))
            {
                points.Clear();
            }
            else if (run.check(mousePos, mouseDown))
            {
                if (points.Count > 1)
                {
                    double[] pointsX = new double[points.Count];
                    double[] pointsY = new double[points.Count];

                    for (int i = 0; i < points.Count; i++)
                    {
                        var coords = points[i].Location.ConvertPos(new Vector2(bounds.Y / 2), offSet);
                        pointsX[i] = coords.X;
                        pointsY[i] = coords.Y;
                    }
                    bezier = new Bezier2D(new Bezier(5, pointsX, new double[] { 0, 1 }),
                                          new Bezier(5, new double[] { 0, 1 }, pointsY), new Vector2(bounds.Y / 2));
                }
            }
            else if (draggedPoint != null)
            {
                DragLogic(mousePos, mouseDown);
            }
            else if (bezier == null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    if (i != grabbedIndex)
                    {
                        if (point.check(mousePos, mouseDown) && !prevDown)
                        {
                            selectedPoint = null;
                            SetDraggedPoint(mousePos, i);
                        }
                        else if (point.check(mousePos, ms.MiddleButton == ButtonState.Pressed))
                        {
                            selectedPoint = points[i];
                            grabbedIndex = i;
                        }
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
            else if (selectedPoint != null)
            {
                points.RemoveAt(grabbedIndex);
                selectedPoint = null;
                grabbedIndex = -1;              
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
                var degree = (float)Math.Pow(i / (points.Count != 1 ? (float)points.Count - 1 : 1), 1.75);
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
            spriteBatch.Begin();
            if (bezier == null)
            {
                GraphicsDevice.Clear(Color.SlateGray);
                graphBackGround.Draw(spriteBatch);

                foreach (ScalableSprite line in gridLines)
                {
                    line.Draw(spriteBatch);
                }


            }
            else
            {
                drawnLine.Draw(spriteBatch);
            }

            foreach (Sprite point in points)
            {
                point.Draw(spriteBatch);
            }

            if (draggedPoint != null)
            {
                draggedPoint.Draw(spriteBatch);
            }

            settingsBox.Draw(spriteBatch);
            pointMaker.Draw(spriteBatch);
            delete.Draw(spriteBatch);
            clear.Draw(spriteBatch);
            run.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
