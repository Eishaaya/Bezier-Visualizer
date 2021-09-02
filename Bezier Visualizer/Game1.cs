using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace Bezier_Visualizer
{
    public class Game1 : Game
    {
        int placements = 0;
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
        float gridWidth;
        Sprite settingsBox;

        Texture2D draggedTexture;
        Button draggedPoint;
        Button selectedPoint;
        ButtonLabel xLabel;
        ButtonLabel yLabel;
        ButtonLabel arrangementLabel;
        ButtonLabel timeLabel;

        double time = 3;

        Label indexLabel;

        bool anyKeyPressed;

        List<Sprite> drawnPoints;

        float newNumber;

        Dictionary<ButtonLabel, Func<bool>> mapping;
        Dictionary<Keys, string> keyStrings;

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
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ms = new MouseState();
            ks = new KeyboardState();
            points = new List<Button>();
            gridLines = new List<ScalableSprite>();
            drawnPoints = new List<Sprite>();
            draggedPoint = null;
            prevDown = false;
            anyKeyPressed = false;
            grabbedIndex = -1;

            gridWidth = bounds.Y / 2;
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

            settingsBox = new Sprite(Content.Load<Texture2D>("Box"), new Vector2(gridWidth, 47), Color.White, 0, SpriteEffects.None, Vector2.Zero, 1, 1);            
            drawnLine = new Sprite(Content.Load<Texture2D>("Line"), Vector2.Zero, Color.Gold, 0, SpriteEffects.None, new Vector2(6), 1, 1);
            run = new Button(Content.Load<Texture2D>("RunButton"), new Vector2(bounds.X - 400, 0), Color.White, 0, SpriteEffects.None, new Vector2(0, 0), 1, 1, Color.Gray, Color.DarkGray);
            pointMaker = new Button(Content.Load<Texture2D>("MakeButton"), new Vector2(bounds.X - 300, 0), Color.White, 0, SpriteEffects.None, new Vector2(0, 0), 1, 1, Color.Gray, Color.DarkGray);
            delete = new Button(Content.Load<Texture2D>("DeleteButton"), new Vector2(bounds.X - 200, 0), Color.White, 0, SpriteEffects.None, new Vector2(0, 0), 1, 1, Color.Gray, Color.DarkGray);
            clear = new Button(Content.Load<Texture2D>("ClearButton"), new Vector2(bounds.X - 100, 0), Color.White, 0, SpriteEffects.None, new Vector2(0, 0), 1, 1, Color.Gray, Color.DarkGray);


            arrangementLabel = new ButtonLabel(new Button(Content.Load<Texture2D>("Short Highlight"), new Vector2(gridWidth + 259, run.Image.Height + 300),
                                                          Color.Transparent, 0, SpriteEffects.None, Vector2.Zero, 1, 1, Color.White, Color.LightGray),
                                               new Label(Content.Load<SpriteFont>("RightFont"), Color.White, new Vector2(gridWidth + 269, run.Image.Height + 324 - 18), ""));

            xLabel = new ButtonLabel(new Button(Content.Load<Texture2D>("Highlight"), new Vector2(gridWidth + 50, run.Image.Height + 50),
                                                Color.Transparent, 0, SpriteEffects.None, Vector2.Zero, 1, 1, Color.White, Color.LightGray),
                                     new Label(Content.Load<SpriteFont>("RightFont"), Color.White, new Vector2(gridWidth + 50, run.Image.Height + 74 - 18), ""));

            yLabel = new ButtonLabel(new Button(Content.Load<Texture2D>("Highlight"), new Vector2(gridWidth + 50, run.Image.Height + 150),
                                                Color.Transparent, 0, SpriteEffects.None, Vector2.Zero, 1, 1, Color.White, Color.LightGray),
                                    new Label(Content.Load<SpriteFont>("RightFont"), Color.White, new Vector2(gridWidth + 50, run.Image.Height + 174 - 18), ""));

            timeLabel = new ButtonLabel(new Button(Content.Load<Texture2D>("Short Highlight"), new Vector2(gridWidth + 259, run.Image.Height + 600),
                                                          Color.Transparent, 0, SpriteEffects.None, Vector2.Zero, 1, 1, Color.White, Color.LightGray),
                                               new Label(Content.Load<SpriteFont>("RightFont"), Color.White, new Vector2(gridWidth + 269, run.Image.Height + 624 - 18), "3"));

            indexLabel = new Label(Content.Load<SpriteFont>("RightFont"), Color.White, new Vector2(gridWidth + 272, settingsBox.Location.Y + 474 - 18), "NaN");


            mapping = new Dictionary<ButtonLabel, Func<bool>>()
            {
                [arrangementLabel] = SwapArrangment,
                [xLabel] = SetX,
                [yLabel] = SetY,
                [timeLabel] = SetTime
            };

            keyStrings = new Dictionary<Keys, string>()
            {
                [Keys.D1] = "1",
                [Keys.D2] = "2",
                [Keys.D3] = "3",
                [Keys.D4] = "4",
                [Keys.D5] = "5",
                [Keys.D6] = "6",
                [Keys.D7] = "7",
                [Keys.D8] = "8",
                [Keys.D9] = "9",
                [Keys.D0] = "0",

                [Keys.OemMinus] = "-",
                [Keys.Decimal] = ".",
                [Keys.OemPeriod] = "."
            };

            //Func<int, int, int> sumFunction = mapping[0];
            //sumFunction(5, 5);
        }

        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            #region updateSetup

            base.Update(gameTime);
            ms = Mouse.GetState();
            ks = Keyboard.GetState();           

            var mousePos = new Vector2(ms.Position.X, ms.Position.Y);
            var mouseDown = ms.LeftButton == ButtonState.Pressed;
            var midDown = ms.MiddleButton == ButtonState.Pressed;

            #endregion

            #region drawing
            if (bezier != null)
            {
                var oldLocation = drawnLine.Location;
                bezier.Update(gameTime);
                drawnLine.Location = bezier.Location + offSet;
                drawnLine.rotation = (bezier.Rotation) * -1 + MathHelper.Pi;
                if (drawnLine.Location != oldLocation)
                {
                    drawnPoints.Add(drawnLine.Clone());
                    drawnPoints.ColorPoints(Color.DarkOrange);
                }
            }
            else
            {
                drawnLine.Location = new Vector2(-1000);
                drawnPoints = new List<Sprite>();
            }
            #endregion

            #region selectionSettings

            if (timeLabel.Clicked || timeLabel.Check(mousePos, mouseDown))
            {
                InputLogic(timeLabel, xLabel, yLabel, arrangementLabel);
            }

            if (selectedPoint != null)
            {

                if (arrangementLabel.Clicked || arrangementLabel.Check(mousePos, mouseDown))
                {
                    InputLogic(arrangementLabel, xLabel, yLabel, timeLabel);
                }
                else if (xLabel.Clicked || xLabel.Check(mousePos, mouseDown))
                {
                    InputLogic(xLabel, arrangementLabel, yLabel, timeLabel);
                }
                else if (yLabel.Clicked || yLabel.Check(mousePos, mouseDown))
                {
                    InputLogic(yLabel, arrangementLabel, xLabel, timeLabel);
                }
                else if (mouseDown && mousePos.X < gridWidth)
                {
                    CancelSelection();
                }

                else
                {
                    var coords = selectedPoint.Location.ConvertPos(new Vector2(gridWidth), offSet);
                    if (!arrangementLabel.Clicked)
                    {
                        arrangementLabel.Label.SetText(grabbedIndex, 0);
                    }
                    if (!xLabel.Clicked)
                    {
                        xLabel.Label.SetText(coords.X, 5);
                    }
                    if (!yLabel.Clicked)
                    {
                        yLabel.Label.SetText(coords.Y, 5);
                    }
                }
            }

            else
            {
                xLabel.Label.Clear();
                yLabel.Label.Clear();
                arrangementLabel.Label.Clear();
            }

            #endregion

            #region topButtons

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
                if (bezier != null)
                {
                    bezier = null;
                }
                points.Clear();
                indexLabel.SetText("NaN");
            }
            else if (run.check(mousePos, mouseDown))
            {
                CancelSelection();
                DeletePoint();
                CheckPoints(mousePos, mouseDown, midDown);
                if (points.Count > 1)
                {
                    double[] pointsX = new double[points.Count];
                    double[] pointsY = new double[points.Count];

                    for (int i = 0; i < points.Count; i++)
                    {
                        var coords = points[i].Location.ConvertPos(new Vector2(gridWidth), offSet);
                        pointsX[i] = coords.X;
                        pointsY[i] = coords.Y;
                    }
                    bezier = new Bezier2D(new Bezier(time, pointsX, new double[] { 0, 1 }),
                                          new Bezier(time, new double[] { 0, 1 }, pointsY), new Vector2(gridWidth));
                }
            }
            #endregion

            #region pointManagment

            else
            {
                if (draggedPoint != null)
                {
                    DragLogic(mousePos, mouseDown);
                }
                if (bezier == null)
                {
                    CheckPoints(mousePos, mouseDown, midDown);
                }
            }

            prevDown = mouseDown;
        }
        #endregion

        #region PointPlacingFunctions

        void CancelSelection()
        {
            selectedPoint = null;
            grabbedIndex = -1;
            xLabel.Clicked = false;
            yLabel.Clicked = false;
            arrangementLabel.Clicked = false;
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
            if (draggedPoint.Location.X > gridWidth || draggedPoint.Location.X < 0 || draggedPoint.Location.Y > bounds.Y || draggedPoint.Location.Y < 0)
            {
                return;
            }
            draggedPoint.Color = Color.White;
            if (ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl))
            {
                draggedPoint.Location = new Vector2((float)Math.Round(draggedPoint.Location.X / gridWidth, 1) * gridWidth, (float)Math.Round(draggedPoint.Location.Y / gridWidth, 1) * gridWidth);
            }
            if (grabbedIndex < 0)
            {
                points.Add(draggedPoint);
            }
            else
            {
                points[grabbedIndex] = draggedPoint;
            }
            if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift))
            {
                draggedPoint = draggedPoint.Clone();
            }
            else
            {
                draggedPoint = null;
            }
            ColorPoints();
            placements++;
        }

        void ColorPoints()
        {
            for (int i = 0; i < points.Count; i++)
            {
                var degree = (float)Math.Pow(i / (points.Count != 1 ? (float)points.Count - 1 : 1), 1.75);
                points[i].NormalColor = Color.Lerp(Color.CornflowerBlue, Color.Red, degree);
            }
            if (points.Count > 0)
            {
                indexLabel.SetText(points.Count - 1);
            }
            else
            {
                indexLabel.SetText("NaN");
            }
        }

        void SetDraggedPoint(Vector2 mousePos, int index = -1)
        {
            draggedPoint = new Button(draggedTexture, mousePos, Color.White, 0, SpriteEffects.None, new Vector2(10, 10), 1, 1, Color.DarkGray, Color.Gray);
            grabbedIndex = index;
        }

        #endregion

        #region seperatedBlocks
        void DragLogic(Vector2 mousePos, bool mouseDown)
        {
            draggedPoint.Location = mousePos;
            if (ms.RightButton == ButtonState.Pressed && !prevDown)
            {
                DeletePoint();
            }
            else if (mouseDown && !prevDown)
            {
                prevDown = true;
                PlacePoint();
            }
        }

        void InputLogic (ButtonLabel label, params ButtonLabel[] others)
        {
            if (ks.GetPressedKeys().Length > 0)
            {
                if (!anyKeyPressed)
                {
                    anyKeyPressed = true;
                    var input = ks.GetPressedKeys()[0];

                    var numberInputed = keyStrings.ContainsKey(input);

                    if (!numberInputed)
                    {
                        if (ks.IsKeyDown(Keys.Back))
                        {
                            label.Clicked = false;
                        }
                        if (ks.IsKeyDown(Keys.Enter))
                        {
                            var parseable = float.TryParse(label.Label.Text, out newNumber);

                            if (!parseable || !mapping[label]())
                            {
                                label.Clicked = false;
                                return;
                            }
                        }
                    }
                    else
                    {
                        label.Label.Add(keyStrings[input][0]);
                    }
                }
            }
            else
            {
                anyKeyPressed = false;
            }
            foreach (var button in others)
            {
                button.Clicked = false;
            }
        }

        bool SwapArrangment ()
        {
            var intNumber = (int)newNumber;
            if (intNumber >= 0 && intNumber < points.Count)
            {
                for (int i = grabbedIndex; i < points.Count - 1; i++)
                {
                    points[i] = points[i + 1];
                }

                for (int i = points.Count - 1; i > intNumber; i--)
                {
                    points[i] = points[i - 1];
                }

                points[intNumber] = selectedPoint;

                grabbedIndex = intNumber;
                selectedPoint = points[intNumber];
                arrangementLabel.Clicked = false;

                ColorPoints();
                return true;
            }
            return false;
        }

        bool SetX ()
        {
            if (newNumber >= 0 && newNumber <= 1)
            {
                selectedPoint.Location = new Vector2(newNumber * gridWidth, selectedPoint.Location.Y);
                xLabel.Clicked = false;                
            }
            return false;
        }
        bool SetY()
        {
            if (newNumber >= -.5 && newNumber <= 1.5)
            {
                selectedPoint.Location = new Vector2(selectedPoint.Location.X, newNumber * gridWidth + offSet.Y);
                yLabel.Clicked = false;
            }
            return false;
        }

        bool SetTime()
        {
            if (newNumber >= 1 && newNumber <= 999)
            {
                time = newNumber;
                timeLabel.Clicked = true;                
            }
            return false;
        }

        void CheckPoints(Vector2 mousePos, bool mouseDown, bool midDown)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (i != grabbedIndex || selectedPoint == null)
                {
                    if (point.check(mousePos, mouseDown) && !prevDown)
                    {
                        selectedPoint = null;
                        SetDraggedPoint(mousePos, i);
                    }
                    else if (point.check(mousePos, midDown))
                    {
                        selectedPoint = points[i];
                        grabbedIndex = i;

                        xLabel.Clicked = false;
                        yLabel.Clicked = false;
                        arrangementLabel.Clicked = false;
                    }
                }
            }
        }
        #endregion

        #region drawing
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (bezier == null)
            {
                GraphicsDevice.Clear(Color.SlateGray);
                graphBackGround.Draw(spriteBatch);
                indexLabel.Print(spriteBatch);
                timeLabel.Draw(spriteBatch);

                foreach (ScalableSprite line in gridLines)
                {
                    line.Draw(spriteBatch);
                }
            }
            else
            {
                foreach (var point in drawnPoints)
                {
                    point.Draw(spriteBatch);
                }
            }

            foreach (Sprite point in points)
            {
                point.Draw(spriteBatch);
            }

            if (draggedPoint != null)
            {
                draggedPoint.Draw(spriteBatch);
            }

            pointMaker.Draw(spriteBatch);
            delete.Draw(spriteBatch);
            clear.Draw(spriteBatch);
            run.Draw(spriteBatch);

            xLabel.Draw(spriteBatch);
            yLabel.Draw(spriteBatch);
            arrangementLabel.Draw(spriteBatch);

            settingsBox.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}
