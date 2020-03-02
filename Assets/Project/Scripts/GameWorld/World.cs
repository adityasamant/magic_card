using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace GameWorld
{

    public delegate void World_ResetFinished();

    public enum WorldStates
    {
        Idle,
        Battle,
        Wait_For_Monster,
        Summary
    }


    public class WorldGlobalAttrib
    {
        public int currentTurn = 0;
    }

    public delegate void BattleEnd(int winner);

    public class World : MonoBehaviour
    {
        /// <summary>
        /// data structure of all monsters
        /// </summary>
        public Dictionary<int, Monster> monsters;
        /// <summary>
        /// the game map
        /// </summary>
        public HexTileMap tileMap;
        /// <summary>
        /// The global informations of the world
        /// </summary>
        public WorldGlobalAttrib worldAttrib;

        public BattleEnd BattleEnd;
        public UnityEvent Event_BattleBegin;

        public World_ResetFinished World_ResetFinished;

        public UnityEvent Event_ResetStart;
        
        public bool created = false;

        private WorldStates currentState = WorldStates.Idle;
        private SortedSet<Monster> actingMonsterList;
        private Monster currentActMonster;

        /// <summary>
        /// get monster instance by its uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// return the monster if it exists, otherwise return null 
        public Monster getMonsterByID(int uid)
        {
            if (monsters.ContainsKey(uid) == false)
            {
                Debug.LogErrorFormat("Error! No monster has uid {0}", uid);
                return null;
            }
            else
                return monsters[uid];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public int uploadMonsterInWorld(Monster monster)
        {
            int uid = monster.GetUId();
            if (monsters.ContainsKey(uid) == true)
            {
                Debug.LogErrorFormat("Error! Monster with uid {0} already exists", uid);
                return -1;
            }
            monsters[uid] = monster;
            return uid;
        }

        public void init()
        {
            Debug.Log("World: Finished init");
            monsters.Clear();
            World_ResetFinished();
        }
        public void ResetForOneBattle()
        {
            worldAttrib.currentTurn = 0;
            ResetForOneTurn();
        }
        public void ResetForOneTurn()
        {
            actingMonsterList.Clear();
            foreach (KeyValuePair<int, Monster> monsterPair in monsters)
            {
                Monster monster = monsterPair.Value;
                if (monster.alive == true)
                    actingMonsterList.Add(monster);
            }
            Debug.LogFormat("Clearing for one turn ended. Detected {0} living animals.", actingMonsterList.Count);
        }

        void Start()
        { 
            Debug.Log("Creating a new world");
            monsters = new Dictionary<int, Monster>();
            actingMonsterList = new SortedSet<Monster>() ;
            if(tileMap == null)
                tileMap = new HexTileMap();
            worldAttrib = new WorldGlobalAttrib();
            if (Event_BattleBegin == null)
                Event_BattleBegin = new UnityEvent();
            Event_BattleBegin.AddListener(BattleBegin);
            if (Event_ResetStart == null)
                Event_ResetStart = new UnityEvent();
            Event_ResetStart.AddListener(ResetBegin);
        }
        
        void Update()
        {
            switch (currentState)
            {
                case WorldStates.Idle:
                case WorldStates.Wait_For_Monster:
                    // do nothing
                    break;
                case WorldStates.Battle:
                    if (actingMonsterList.Count == 0)
                        currentState = WorldStates.Summary;
                    else
                    {
                        // choose the next monster and 
                        currentActMonster = actingMonsterList.Min;
                        actingMonsterList.Remove(currentActMonster);
                        Debug.LogFormat("Monster {0} is moving...", currentActMonster.GetUId());
                        currentActMonster.MonsterStartTurn.Invoke();
                        currentActMonster.MonsterTurnEnd += MonsterMoveEnd;
                        currentState = WorldStates.Wait_For_Monster;
                    }
                    break;
                case WorldStates.Summary:
                    ResetForOneTurn();
                    worldAttrib.currentTurn++;
                    Debug.LogFormat("World: turn {0} is over.", worldAttrib.currentTurn);
                    if(worldAttrib.currentTurn == 20)
                    {
                        Debug.Log("Player 0 wins.");
                        BattleEnd(0);
                        currentState = WorldStates.Idle;
                    }
                    break;
            }
        }

        void BattleBegin()
        {
            Debug.Log("World: Battle begin.");
            ResetForOneBattle();
            currentState = WorldStates.Battle;
        }

        #region Event Handler
        /// <summary>
        /// Event_ResetStart Handler
        /// </summary>
        private void ResetBegin()
        {
            init();
            return;
        }
        #endregion

        void MonsterMoveEnd(MonsterMovement movement)
        {
            Debug.LogFormat("Monster {0} move ended.", currentActMonster.GetUId());
            switch (movement.action)
            {
                case MonsterMovement.MonsterAction.Nothing:
                    break;
                default:
                    Debug.Log("A movement that this world doesn't understand.");
                    break;
            }
            currentState = WorldStates.Battle;
        }
    }

}

