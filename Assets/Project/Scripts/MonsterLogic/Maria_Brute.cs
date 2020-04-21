using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monsters
{
    public class Maria_Brute : Monster
    {
        public bool is_defensed = false;
        protected float defense_animation_time = 0.5f;
        protected float undefense_animation_time = 0.5f;

        override public void Move(int destination)
        {
            is_defensed = false;
            base.Move(destination);
        }

        public virtual void Defense()
        {
            if (is_defensed) return;
            is_defensed = true;
            GetComponent<Animator>().SetTrigger("Defense");
            AnimationFinishedTime = Time.time + defense_animation_time;
            WaitForAnimation = MonsterAction.Defense;
        }
        public virtual void UnDefense()
        {
            if (!is_defensed) return;
            is_defensed = false;
            GetComponent<Animator>().SetTrigger("Reset");
            AnimationFinishedTime = Time.time + undefense_animation_time;
            WaitForAnimation = MonsterAction.Defense;
        }
    }
}

