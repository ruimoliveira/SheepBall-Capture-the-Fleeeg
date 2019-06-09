using UnityEngine;

namespace SheepAnimationState
{
    public enum State
    {
        Available,
        Rotating,
        Moving,
        Waiting,
        Unavailable,
        Scared,
        Flying
    };


    public enum AnimStates
    {
        Iddle,
        Scared,
        Walking,
        Ball,
        Shot
    };

    public abstract class IAnimState
    {
        protected Animator m_animator;

        public IAnimState(ref Animator m_animator)
        {
            this.m_animator = m_animator;
            this.m_animator.SetTrigger("Next");
        }

        public IAnimState next(int state)
        {

            if ((state == (int)State.Available || state == (int)State.Waiting) && !GetType().Equals(typeof(Iddle)))
            {
                return new Iddle(ref m_animator);
            }

            if (state == (int)State.Unavailable && !GetType().Equals(typeof(Ball)))
            {
                return new Ball(ref m_animator);
            }

            if ((state == (int)State.Moving || state == (int)State.Rotating) && !GetType().Equals(typeof(Walking)))
            {
                return new Walking(ref m_animator);
            }

            if (state == (int)State.Scared && !GetType().Equals(typeof(Scared)))
            {
                return new Scared(ref m_animator);
            }

            if (state == (int)State.Flying && !GetType().Equals(typeof(Shot)))
            {
                return new Shot(ref m_animator);
            }

            return this;
        }
    }

    class Shot : IAnimState
    {

        public Shot(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetInteger("Index", (int)AnimStates.Shot);
        }
    }

    class Ball : IAnimState
    {
        public Ball(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetInteger("Index", (int)AnimStates.Ball);
        }
    }
    class Walking : IAnimState
    {
        public Walking(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetInteger("Index", (int)AnimStates.Walking);
        }
    }

    class Scared : IAnimState
    {
        public Scared(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetInteger("Index", (int)AnimStates.Scared);
        }
    }

    class Iddle : IAnimState
    {
        public Iddle(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetInteger("Index", (int)AnimStates.Iddle);
        }
    }
}
