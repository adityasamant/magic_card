using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monsters
{
    public class Jolleen_Knight : Monster
    {
        protected float spell_animation_time = 2.5f;
        
        public virtual void Spell(Monster subjectMonster)
        {
            GetComponent<Animator>().SetTrigger("Spell");
            AnimationFinishedTime = Time.time + spell_animation_time;
            //WaitForAnimation = MonsterAction.Spell;
            ActionParam.AttackDamage = 1;
            ActionParam.AttackSubject = subjectMonster;
            WaitForAnimation = MonsterAction.Attack;
        }

        void Start()
        {
            base.Start();
            attack_animation_time = 2.0f; 
        }
    }
}

