using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bezier_Visualizer
{
    class Enemy : AnimatingSprite
    {
        public enum EnemyState
        {
            Idle,
            Moving,
            Attacking,
            Dying
        }
        public enum MoveType
        {
            Swoop,
            Zigzag,
            Charge,
            Teleport
        }
        public enum AttackType
        {
            OneHit,
            Brawl,
            Kamikaze
        }

        protected bool isDead;
        public MoveType moveType { get; }
        public AttackType attackType { get; }
        public EnemyState CurrentState { get; set; }
        public float Attack { get; }
        private float health = default;
        public float AttackRange { get; }
        public float Health
        {
            get => health;

            set
            {
                float newValue = value;
                if (newValue <= 0)
                {
                    isDead = true;
                }
            }
        }

        protected Dictionary<EnemyState, FrameObject> totalFrames;

        public Enemy(Texture2D image, Vector2 location, Color color, float rotation, SpriteEffects effects, float attack, float attackrange, float health, MoveType movetype, AttackType attacktype, Rectangle hitbox, Vector2 origin, float scale, float depth, Dictionary<EnemyState, FrameObject> totalframes, int time, Vector2[][][] Origins = null)
          : base(image, location, color, rotation, effects, hitbox, origin, scale, depth, ((AnimationFrame[][])totalframes[EnemyState.Idle])[0], time, Origins == null ? null : Origins[(int)EnemyState.Idle][0])
        {
            moveType = movetype;
            attackType = attacktype;
            CurrentState = EnemyState.Idle;
            isDead = false;
            Attack = attack;
            Health = health;
            AttackRange = attackrange;
            totalFrames = totalframes;
        }

        public void Update(GameTime time)
        {
            if (CurrentState == EnemyState.Idle)
            {
                if (LastFrame)
                 {
                    var idle = totalFrames[EnemyState.Idle];
                    AnimationFrame[][] idleArray = idle;
                    var newFrames = random.Next(idleArray.Length);
                    Frames = idleArray[newFrames];
                    FrameTime = idle.frameSpeed[newFrames];
                }
            }
            Animate(time);
        }
    }
}
