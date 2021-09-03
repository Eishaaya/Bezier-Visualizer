using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace Bezier_Visualizer
{
    struct DualUlong
    {        
        public ulong X { get; set; }
        public ulong Y { get; set; }

        public DualUlong(ulong x, ulong y)
        {
            X = x;
            Y = y;
        }
    }

    class Bezier2D
    {
        public enum PointType
        {
            Linear,
            Curve,
            ZigZag,
            EaseInOut,
        }

        Vector2 multiplyer;
        public float Rotation { get; private set; }
        public Vector2 Location { get; private set; }
        Bezier xCurve;
        Bezier yCurve;

        public static Bezier2D BuildBezier2D(double timeMulti, Vector2 start, Vector2 end, PointType command, int degree = 0)
        {
            //int xFlipper = 1;
            //if (start.X > end.X)
            //{
            //    xFlipper = -1;
            //}
            //int yFlipper = 1;
            //if (start.Y > end.Y)
            //{
            //    yFlipper = -1;
            //}
            switch (command)
            {
                case PointType.Linear:
                    return new Bezier2D(new Bezier(timeMulti, new double[] { 0, 1 }, new double[] { start.X, end.X }, start.X),
                                        new Bezier(timeMulti, new double[] { 0, 1 }, new double[] { start.Y, end.Y }, start.Y),
                                        new Vector2(1, 1));
                default:
                    return null;
            }
        }

        public Bezier2D(Bezier xCurve, Bezier yCurve, Vector2 multiplyer)
        {
            this.xCurve = xCurve;
            this.yCurve = yCurve;
            this.multiplyer = multiplyer;
        }

        public void Update(GameTime gameTime)
        {
            if (yCurve.Update(gameTime) && xCurve.Update(gameTime))
            {
                UpdateProperties();
            }
        }
        void UpdateProperties()
        {
            var oldLocation = Location;
            Location = new Vector2((float)xCurve.Location * multiplyer.X, (float)yCurve.Location * multiplyer.Y);
            Rotation = oldLocation.PointAt(Location);
        }
    }

    class Bezier : HalfBezier
    {
        HalfBezier timeBezier;

        public Bezier(double timeMulti, double[] timepoints, double[] points, double location = 0)
            : base(1, points, location)
        {
            timeBezier = new HalfBezier(timeMulti, timepoints);
        }

        public override bool Update(GameTime gameTime)
        {
            timeBezier.Update(gameTime);
            return Update(timeBezier.Location);
        }
    }
    class HalfBezier
    {
        protected double timeMultiplier;
        protected double time;
        protected double[] points;
        public double Location { get; set; }

        public HalfBezier(double timeMulti, double[] points, double location = 0)
        {
            Location = location;
            this.time = 0;
            timeMultiplier = timeMulti;
            this.points = points;
        }

        public virtual bool Update(GameTime gameTime)
        {
            if (time >= 1 || time < 0)
            {
                return false;
            }
            time += (gameTime.ElapsedGameTime.TotalMilliseconds) / timeMultiplier / 1000;
            Location = BezierFuncs.Get().BezierCalc(points, time);
            return true;
        }
         
        public bool Update(double thyme)
        {
            time = thyme;
            if (thyme >= 1)
            {
                time = 1;
                return false;
            }
            else if (thyme <= 0)
            {
                time = 0;
                return false;
            }
            Location = BezierFuncs.Get().BezierCalc(points, time);
            return true;
        }
    }


    class BezierFuncs
    {
        static BezierFuncs Instance;
        Dictionary<DualUlong, ulong> cents;
        public static BezierFuncs Get()
        {
            if (Instance == null)
            {
                Instance = new BezierFuncs();
            }
            return Instance;
        }
        BezierFuncs()
        {
            cents = new Dictionary<DualUlong, ulong>();
        }

        public double BezierCalc(double[] points, double time)
        {
            double result = 0;
            for (uint i = 0; i < points.Length; i++)
            {
                result += PascalIndex(i, (ulong)points.Length - 1) * Math.Pow(1 - time, points.Length - 1 - i) * Math.Pow(time, i) * points[i];
            }
            return result;
        }

        public ulong PascalIndex(ulong column, ulong row)
        {
            var key = new DualUlong(column, row);
            if (column > row)
            {
                return 0;
            }
            if (cents.ContainsKey(key))
            {
                return cents[key];
            }
            var rowFact = row.Factorial();
            var colFact = column.Factorial();
            var bothFact = (row - column).Factorial();

            var answer = rowFact / (colFact * bothFact);
            cents.Add(key, answer);
            return answer;
        }
    }
}
