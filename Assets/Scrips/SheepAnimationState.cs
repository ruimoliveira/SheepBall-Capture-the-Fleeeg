using System.Collections;
using System.Collections.Generic;
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

    public abstract class IAnimState
    {
        protected Animator m_animator;

        public IAnimState(ref Animator m_animator)
        {
            this.m_animator = m_animator;
        }

        public virtual IAnimState next(int state)
        {
            return this;
        }
    }

    class Flying : IAnimState
    {

        public Flying(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetTrigger("FirstOption");
        }

        public override IAnimState next(int state)
        {
            Debug.Log("Flying State: " + state);
            switch (state)
            {
                case (int)State.Available:
                    return new Iddle(ref this.m_animator);
            }
            return this;
        }
    }

    class Ball : IAnimState
    {
        public Ball(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetTrigger("FirstOption");
        }

        public override IAnimState next(int state)
        {
            Debug.Log("Ball State: " + state);
            switch (state)
            {
                case (int)State.Flying:
                    return new Flying(ref this.m_animator);

                case (int)State.Available:
                    return new Iddle(ref this.m_animator);
            }
            return this;
        }
    }
    class Walking : IAnimState
    {
        public Walking(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetTrigger("FirstOption");
        }

        public override IAnimState next(int state)
        {
            Debug.Log("Walking State: " + state);
            switch (state)
            {
                case (int)State.Unavailable:
                    return new Ball(ref this.m_animator);

                case (int)State.Waiting:
                    return new Iddle(ref this.m_animator);

                default:

                    return new Iddle(ref this.m_animator); //TODO: change
            }
        }
    }

    class Scared : IAnimState
    {
        public Scared(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetTrigger("SecondOption");
        }

        public override IAnimState next(int state)
        {
            Debug.Log("Scared State: " + state);
            switch (state)
            {
                case (int)State.Unavailable:
                    return new Ball(ref this.m_animator);
                default:
                    if(state != (int)State.Unavailable) 
                        return new Iddle(ref this.m_animator);
                    else
                        return this;
            }
        }
    }

    class Iddle : IAnimState
    {
        public Iddle(ref Animator m_animator) : base(ref m_animator)
        {
            this.m_animator.SetTrigger("SecondOption");
        }

        public override IAnimState next(int state)
        {
            Debug.Log("Iddle State: " + state);
            switch (state)
            {
                case (int)State.Moving:
                    return new Walking(ref this.m_animator);

                case (int)State.Scared:
                    return new Scared(ref this.m_animator);
            }
            return this;
        }
    }
}
