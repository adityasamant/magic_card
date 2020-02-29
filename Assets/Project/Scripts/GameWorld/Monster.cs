using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public delegate void MonsterTurnEnd(MonsterMovement movement);

    public class Monster : MonoBehaviour
    {
        /// <summary>
        /// the total number of all monster in the game
        /// </summary>
        static private int monsterCount = 0;

        /// <summary>
        /// the unique id of this monster 
        /// </summary>
        private int uid;
        private GameObject monsterObj;

        /// <summary>
        /// All attributes a monster have
        /// </summary>
        public int hp;
        public int atk;
        public int def;
        public int spd;
        public string monsterName;
        public bool alive;
        public World world;

        public MonsterTurnEnd MonsterTurnEnd;

        public UnityEvent MonsterStartTurn ;
        public UnityEvent MonsterStateUpdate ;

        /// <summary>
        /// Initialization of a monster.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hp"></param>
        /// <param name="atk"></param>
        /// <param name="def"></param>
        /// <param name="spd"></param>

        /// <summary>
        /// get the monster's uid
        /// </summary>
        public int GetUId() { return uid; }
        static public bool operator <(Monster a, Monster b) { return a.uid < b.uid; }
        static public bool operator >(Monster a, Monster b) { return a.uid > b.uid; }


        void Start()
        {
            Debug.Log("A new monster with uid={0} is created." + uid);
            uid = Monster.monsterCount;
            Monster.monsterCount++;
            if (MonsterStartTurn == null)
                MonsterStartTurn = new UnityEvent();
            if (MonsterStateUpdate == null)
                MonsterStateUpdate = new UnityEvent();
            MonsterStartTurn.AddListener(MoveMent);
            MonsterStateUpdate.AddListener(StateUpdate);
        }

        void OnDestroy()
        {
            Debug.Log("A new monster with uid={0} is destroyed." + uid);
        }

        /// <summary>
        /// Monster's movement in one turn.
        /// </summary>
        public void MoveMent()
        {
            if (this.alive == false)
                Debug.LogFormat("Error! Monster {0} is dead", uid);
            else
                Debug.Log("I'm a monster and I don't want to do anything!");
            MonsterMovement movement = new MonsterMovement();
            movement.action = MonsterMovement.MonsterAction.Nothing;
            movement.uid = uid;
            MonsterTurnEnd(movement);
            // need inplemented
        }

        /// <summary>
        /// this function will transmit all states of the monster to its respective GameObject (with visualization)
        /// </summary>
        public void StateUpdate()
        {

        }
    } 
}
