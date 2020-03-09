using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using GameLogic;

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

        private float steptime;

        /// <summary>
        /// A link to the gameManager
        /// </summary>
        [Tooltip("A link to the gameManager.")]
        public GameManager gameManager;

        private WorldStates currentState = WorldStates.Idle;
        private SortedSet<Monster> actingMonsterList;
        private Monster currentActMonster;

        

        #region Public Fucntion
        /// <summary>
        /// Upload the information of the monster to the world(Monsters)
        /// </summary>
        /// <param name="monster">A link to the monster</param>
        /// <returns>monster uid if success, -1 is error</returns>
        public int uploadMonsterInWorld(Monster monster)
        {
            int uid = monster.GetUId();
            if (monsters.ContainsKey(uid) == true)
            {
                Debug.LogErrorFormat("Error! Monster with uid {0} already exists", uid);
                return -1;
            }
            monsters[uid] = monster;
            monster.MonsterTurnEnd += MonsterMoveEnd;
            return uid;
        }

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
        #endregion

        #region Unity Function
        /// <summary>
        /// Invoke when World is enable
        /// </summary>
        void Start()
        { 
            Debug.Log("Creating a new world");
            monsters = new Dictionary<int, Monster>();
            actingMonsterList = new SortedSet<Monster>(new MonsterComparator()) ;
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
        
        /// <summary>
        /// FSM for each tick
        /// </summary>
        void Update()
        {
            float nowTime = Time.time;
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
                        currentState = WorldStates.Wait_For_Monster;
                        currentActMonster.MonsterStartTurn.Invoke();
                        //currentActMonster.MonsterTurnEnd += MonsterMoveEnd;
                    }
                    break;
                case WorldStates.Summary:
                    if (nowTime - steptime < 1) break;
                    WinningCheck();
                    break;
            }
        }
        #endregion

        #region Private Function
        /// <summary>
        /// For all Monster reset at the begin of each turn.
        /// </summary>
        private void TurnInit()
        {
            Debug.Log("World: Reseted All Monster.");
            foreach (KeyValuePair<int, Monster> monsterPair in monsters)
            {
                Monster thisMonster = monsterPair.Value;
                if (thisMonster.isExiled) continue; //A Exiled Monster cannot be reset.
                thisMonster.MonsterReset();
            }
            Debug.Log("World: Finished init");
            World_ResetFinished();
        }

        /// <summary>
        /// Reset the ActingMonsterList Sorted Array before battle
        /// </summary>
        private void ResetForOneBattle()
        {
            worldAttrib.currentTurn = 0;
            ResetForOneTurn();
        }
        /// <summary>
        /// Reset the ActingMonsterList Sorted Array.
        /// </summary>
        private void ResetForOneTurn()
        {
            actingMonsterList.Clear();
            foreach (KeyValuePair<int, Monster> monsterPair in monsters)
            {
                Monster monster = monsterPair.Value;
                if (monster.isExiled == false && monster.isAlive == true) 
                    actingMonsterList.Add(monster);
            }
            Debug.LogFormat("Clearing for one turn ended. Detected {0} living animals.", actingMonsterList.Count);
            steptime = Time.time;
        }

        /// <summary>
        /// Winning Check, Invoke when this turn finished.
        /// </summary>
        private void WinningCheck()
        {
            ResetForOneTurn();
            int Player0Count = 0;
            int Player1Count = 0;
            foreach(Monster monster in actingMonsterList)
            {
                if(monster.monsterOwner==gameManager.Player0)
                {
                    Player0Count++;
                }
                else if(monster.monsterOwner==gameManager.Player1)
                {
                    Player1Count++;
                }
            }
            if(worldAttrib.currentTurn<20 && Player0Count>0 && Player1Count>0)
            {
                worldAttrib.currentTurn++;
                currentState = WorldStates.Battle;
                return;
            }

            if(Player0Count==Player1Count)
            {//Draw
                Debug.Log("This turn Draw");
                BattleEnd(-1);
                currentState = WorldStates.Idle;
            }
            else if(Player0Count<Player1Count)
            {//Player1Win
                Debug.Log("Player 1 wins.");
                BattleEnd(1);
                currentState = WorldStates.Idle;
            }
            else
            {//Player0Win
                Debug.Log("Player 0 wins.");
                BattleEnd(0);
                currentState = WorldStates.Idle;
            }
            return;
        }
        #endregion

        #region Event Handler
        /// <summary>
        /// Event_ResetStart Handler
        /// </summary>
        private void ResetBegin()
        {
            TurnInit(); //Reset all monster at the begin of the turn.
            return;
        }

        /// <summary>
        /// Delegate MonsterMove End Handler
        /// </summary>
        private void MonsterMoveEnd()
        {
            Debug.LogFormat("Monster {0} move ended.", currentActMonster.GetUId());
            currentState = WorldStates.Battle;
        }
        /// <summary>
        /// Invoke when Battle is begin by GameManager
        /// </summary>
        private void BattleBegin()
        {
            Debug.Log("World: Battle begin.");
            ResetForOneBattle();
            currentState = WorldStates.Battle;
        }
        #endregion
    }

}

