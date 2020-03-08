﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameLogic;

namespace GameWorld
{
    public class MonsterMovement
    {
        public enum MonsterAction
        {
            Move,
            Attack,
            Spell,
            Nothing
        }
        /// <summary>
        /// type of this movement
        /// </summary>
        public MonsterAction action = MonsterAction.Nothing;
        /// <summary>
        /// the source of this movement.
        /// </summary>
        public int uid;
        /// <summary>
        /// params of this movement
        /// </summary>
        public int param0, param1, param2;
    }

    public delegate void MonsterTurnEnd();

    [System.Serializable]
    public class StateUpdateEvent : UnityEvent<string, int> { };

    public class Monster : MonoBehaviour
    {
        #region static Value
        /// <summary>
        /// the total number of all monster in the game
        /// </summary>
        static private int monsterCount = 0;
        #endregion

        #region Private Attributes
        /// <summary>
        /// the unique id of this monster 
        /// </summary>
        private int uid;
        /// <summary>
        /// The link to the GameObject
        /// </summary>
        private GameObject monsterObj;
        /// <summary>
        /// Monster's original HP property, should not be change during game, @HP should be reset to this value when turn begin.
        /// </summary>
        private int origin_HP;
        /// <summary>
        /// Monster's original ATK property, should not be change during game, @ATK should be reset to this value when turn begin.
        /// </summary>
        private int origin_ATK;
        /// <summary>
        /// Monster's original SPD property, should not be change during game, @SPD should be reset to this value when turn begin.
        /// </summary>
        private int origin_SPD;
        /// <summary>
        /// Monster's original Owner, should not be change during game. All monster should return to its owner's control when turn begin.
        /// </summary>
        private Player origin_monsterOwner;
        /// <summary>
        /// The hex this monster is casted on. Should not be change during game. Monster will return to this hex when turn begin.
        /// </summary>
        private int origin_HexIndex;
        #endregion

        #region Public Attributes
        /// <summary>
        /// Monster Current HP, will increase or decrease during battle. Will reset to @origin_HP when turn begin.
        /// </summary>
        public int HP;
        /// <summary>
        /// Monster Current ATK, will increase or decrease during battle. Will reset to @origin_ATK when turn begin.
        /// </summary>
        public int ATK;
        /// <summary>
        /// Monster Current SPD, will increase or decrease during battle. Will reset to @origin_SPD when turn begin.
        /// </summary>
        public int SPD;
        /// <summary>
        /// Monster Current Owner, will be change because of the card effect.All monster should return to its owner's control when turn begin.
        /// </summary>
        public Player monsterOwner;
        /// <summary>
        /// Monster Name, will never change after init.
        /// </summary>
        public string monsterName;
        /// <summary>
        /// The Current Hex this monster is steped on. Will reset to the @origin_HexIndex when turn begin.
        /// </summary>
        public int HexIndex;
        /// <summary>
        /// A bool value to store wether the monster is alived or not. Will return to true when turn begin.
        /// A dead monster doesn't attack and have no effect.
        /// </summary>
        public bool isAlive;
        /// <summary>
        /// A bool value to store wether the monster is exiled or not. Will never return to true when turn begin.
        /// A exiled monster will have no move and no effect. And will do nothing when turn begin.
        /// </summary>
        public bool isExiled;
        #endregion

        #region Public Variable
        /// <summary>
        /// The link to the game world
        /// </summary>
        [Tooltip("A link to the game world.")]
        public World world;
        #endregion

        #region Delegate and Event Handler
        /// <summary>
        /// A delegate instant, invoke when monster finish their turn.
        /// </summary>
        public MonsterTurnEnd MonsterTurnEnd;
        /// <summary>
        /// A Event invoked by the World, that tell the monster to start their turn.
        /// </summary>
        public UnityEvent MonsterStartTurn;
        /// <summary>
        /// A Event invoked by the world and other monster, that tell the monster to update their state.
        /// </summary>
        public StateUpdateEvent MonsterStateUpdate;
        #endregion

        #region Public Function
        /// <summary>
        /// Initialization of a monster.
        /// Set all value to its original value, the original value should not be change during game in common case.
        /// </summary>
        /// <param name="name">Monster Origin Name</param>
        /// <param name="hp">Monster Origin HP</param>
        /// <param name="atk">Monster Origin Atk</param>
        /// <param name="spd">Monster Origin Spd</param>
        /// <param name="monsterOwner">Monster Origin Owner</param>
        /// <param name="HexIndex"></param>
        public void MonsterInit(string name,int hp,int atk,int spd,Player monsterOwner,int HexIndex)
        {
            monsterName = name;
            origin_HP = hp;
            origin_ATK = atk;
            origin_SPD = spd;
            origin_monsterOwner = monsterOwner;
            origin_HexIndex = HexIndex;
            MonsterReset();
        }

        /// <summary>
        /// Reset a monster, all stats to its original stats.
        /// Invoke when turn start.
        /// </summary>
        public void MonsterReset()
        {
            if(isExiled)
            {
                Debug.LogFormat("Monster {0} Error: An exiled monster should be reset.", gameObject.transform.name);
                return;
            }
            HP = origin_HP;
            ATK = origin_ATK;
            SPD = origin_SPD;
            monsterOwner = origin_monsterOwner;
            HexIndex = origin_HexIndex;
            isAlive = true;
        }
        /// <summary>
        /// get the monster's uid
        /// </summary>
        public int GetUId() { return uid; }
        #endregion

        #region Override Operator
        /// <summary>
        /// Sorted Monster by SPD and UID
        /// </summary>
        static public bool operator <(Monster a, Monster b) {
            if (a.SPD != b.SPD)
                return a.SPD < b.SPD;
            else
                return a.uid < b.uid;
        }
        /// <summary>
        /// Sorted Monster by SPD and UID
        /// </summary>
        static public bool operator >(Monster a, Monster b) {
            if (a.SPD != b.SPD)
                return a.SPD > b.SPD;
            else
                return a.uid > b.uid;
        }
        #endregion

        #region Unity Function
        /// <summary>
        /// This Function will invoke when game start and monster is created
        /// </summary>
        void Start()
        {
            Debug.Log("A new monster with uid={0} is created." + uid);
            uid = Monster.monsterCount;
            Monster.monsterCount++;
            if (MonsterStartTurn == null)
                MonsterStartTurn = new UnityEvent();
            if (MonsterStateUpdate == null)
                MonsterStateUpdate = new StateUpdateEvent();
            MonsterStartTurn.AddListener(MoveMent);
            MonsterStateUpdate.AddListener(StateUpdate);
            isExiled = false;
        }

        /// <summary>
        /// This Function will invoke when the monster is destoryed.
        /// </summary>
        void OnDestroy()
        {
            Debug.Log("A new monster with uid={0} is destroyed." + uid);
        }
        #endregion

        #region Event Function
        /// <summary>
        /// Monster's movement in one turn.
        /// </summary>
        public void MoveMent()
        {
            if (this.isAlive == false)
                Debug.LogFormat("Error! Monster {0} is dead", uid);
            else
                Debug.Log("I'm a monster and I don't want to do anything!");
            MonsterTurnEnd();
            // need inplemented
        }
        

        /// <summary>
        /// this function will transmit all states of the monster to its respective GameObject (with visualization)
        /// </summary>
        /// <param name="StateField">
        /// "Damage", HP-=newState
        /// </param>
        public void StateUpdate(string StateField,int newState)
        {
            if(StateField=="Damage")
            {
                HP -= newState;
                if (HP < 0)
                    isAlive = false;
            }
                
        }
        #endregion
    }
}
