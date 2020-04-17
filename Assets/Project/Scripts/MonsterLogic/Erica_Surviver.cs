﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monsters
{
    public class Erica_Surviver : Monster
    {
        protected float doubleshot_animation_time = 1.5f;

        public virtual void DoubleShot(Monster subjectMonster, int atk)
        {
            gameObject.transform.LookAt(subjectMonster.transform.position);
            GetComponent<Animator>().SetTrigger("DoubleShot");
            AnimationFinishedTime = Time.time + doubleshot_animation_time;
            ActionParam.AttackDamage = atk*2;
            ActionParam.AttackSubject = subjectMonster;
            WaitForAnimation = MonsterAction.Attack;
        }
    }
}

