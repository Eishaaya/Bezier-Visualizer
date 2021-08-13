using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bezier_Visualizer
{
    public class Timer
    {
        TimeSpan Wait;
        TimeSpan until;

        public static implicit operator TimeSpan (Timer timer) => timer.until;
        public static implicit operator Timer (TimeSpan span) => new Timer(span);
        public static implicit operator Timer (int time) => new Timer(time);

        public Timer(TimeSpan until)
        {
            this.until = until;
            this.Wait = TimeSpan.Zero;
        }


        public Timer(int length)
            : this(TimeSpan.FromMilliseconds(length)) { }

        public void Tick(GameTime time)
        {
            Wait += time.ElapsedGameTime;
        }
        
        public int GetMillies()
        {
            return (int)until.TotalMilliseconds;
        }

        public bool Ready(bool reset = true)
        {
            if(Wait >= until)
            {
                if (reset)
                {
                    Wait = TimeSpan.Zero;
                }
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Wait = TimeSpan.Zero;
        }
    }
}
