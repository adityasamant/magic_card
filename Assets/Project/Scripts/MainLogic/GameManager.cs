using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameWorld;
using DicePackage;
using TerrainScanning;

namespace GameLogic
{
    
    public enum GameStates
    {
        Init,
        Wait_For_Mode_Selection,
        Wait_For_Map_Scan,
        Online_Only_Wait_For_Connection,
        Game_Begin,
        ChooseFirstPlayer,
        UpKeepStep,
        Turn_Begin,
        Player_Turn_Begin,
        Wait_For_Player_Turn,
        Battle_Begin,
        Wait_For_Battle, 
        Battle_End,
        Turn_End,
        Game_End,
        Error
    };

    public enum GameMode
    {
        Offline_Mode,
        Online_Mode_Server,
        Online_Mode_Client
    }

    public class GameGlobalState
    {
        public int matchCnt;
        public int[] playerWinCnt = new int[2];
        public int lastWinner;
        public void Reset()
        {
            matchCnt = 0;
            playerWinCnt[0] = playerWinCnt[1] = 0;
        }
    }

    public delegate void GameBegin();
    public delegate void DrawCardBegin();
    public delegate void Player0Begin();
    public delegate void Player1Begin();
    public delegate void BattleBegin();

    public class GameManager : MonoBehaviour
    {
        #region Public Variable
        /// <summary>
        /// Store a Pointer to ScanMesh to capture the ScanFinished event
        /// </summary>
        public meshCode ScanMesh;
        public GameObject ScanMeshObject;

        /// <summary>
        /// Store a Pointer to Player0 to send playerstart event and scanfinished event
        /// </summary>
        public Player Player0;
        public Player Player1;

        /// <summary>
        /// The Game World
        /// </summary>
        public World world;

        /// <summary>
        /// The global states(attributes) of a single game, including howmany turns it has passed, howmany times each player wins, e.t.c.
        /// </summary>
        public GameGlobalState gameGlobalState;

        /// <summary>
        /// Limit Player time per turn.
        /// </summary>
        [Tooltip("Limit Player time per turn.")]
        public float PlayerTurnTimeLimited = 30;

        /// <summary>
        /// A link to set the Dice Prefab
        /// </summary>
        public GameObject DicePrefab;

        #endregion

        #region Private Variable
        /// <summary>
        /// Store the time when this state began
        /// </summary>
        private float stateBeginTime;

        private List<Player> playerTurnList;
        private int currentPlayerId;

        private GameStates currentState = GameStates.Init;

        private GameMode currentMode;

        /// <summary>
        /// Store a instant of Dice
        /// </summary>
        private Dice DiceInstant = null;

        /// <summary>
        /// Store a gameobject that is Dice
        /// </summary>
        private GameObject DiceObject = null;

        /// <summary>
        /// Boolean for waiting for dice stop
        /// </summary>
        private bool bWaitForDice=false;

        /// <summary>
        /// Boolean true for Player0 go first, false for Player1 go first
        /// </summary>
        private bool bPlayer0First = true;
        #endregion

        private void ChangeState(GameStates dstStates)
        {
            Debug.LogFormat("Game Manager: Now Change to "+dstStates);
            currentState = dstStates;
            stateBeginTime = Time.time;
        } 

        // Start is called before the first frame update
        void Start()
        {
            gameGlobalState = new GameGlobalState();
            playerTurnList = new List<Player>() ;
            ScanMesh.ScanFinished += ScanFinshedUpdate;
            Player0.PlayerEnd += PlayerEndUpdate;
            Player0.PlayerId = 0;
            Player1.PlayerEnd += PlayerEndUpdate;
            Player1.PlayerId = 1;
            world.BattleEnd += BattleEndUpdate;
            world.World_ResetFinished += World_ResetFinishedUpdate;
            DiceObject = Instantiate(DicePrefab,transform);
            DiceObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            DiceInstant = DiceObject.GetComponent<Dice>();
            
            DiceObject.SetActive(false);
            ChangeState(GameStates.Init);
        }

        // Update is called once per frame
        void Update()
        {
            float nowTime = Time.time;
            int winner = -1;
            switch (currentState)
            {
                case GameStates.Init:
                    ChangeState(GameStates.Wait_For_Mode_Selection);
                    break;
                case GameStates.Wait_For_Mode_Selection: // TODO: let user select mode.
                    currentMode = GameMode.Offline_Mode;
                    ScanMeshObject.SetActive(true);
                    ScanMesh.ScanFinished += ScanFinshedUpdate;
                    ChangeState(GameStates.Wait_For_Map_Scan);
                    Debug.Log("Offline Mode.");
                    break;
                case GameStates.Online_Only_Wait_For_Connection: // TODO: add online wait for connection
                    Debug.LogError("Error! Online mode hasn't implemented yet.");
                    ChangeState(GameStates.Error);
                    break;
                case GameStates.Game_Begin:
                    if (nowTime - stateBeginTime > 3)
                    {
                        gameGlobalState.Reset();
                        //ChangeState(GameStates.UpKeepStep);
                        bWaitForDice = false;
                        ChangeState(GameStates.ChooseFirstPlayer);
                    }
                    break;
                case GameStates.ChooseFirstPlayer:
                    if(!bWaitForDice)
                    {
                        bWaitForDice = true;
                        DiceObject.SetActive(true);
                        DiceInstant.ResetPosition(DiceObject.transform.position + new Vector3(0, 1, 0));
                        DiceInstant.Dice_stop += DiceStopHandle;
                    }
                    break;
                case GameStates.UpKeepStep:
                    if(nowTime-stateBeginTime>3)
                    {
                        world.Event_ResetStart.Invoke();
                    }
                    break;
                case GameStates.Turn_Begin:
                    if (nowTime - stateBeginTime > 2)
                    {
                        if(bPlayer0First==true)
                        {
                            Debug.Log("Player0 Go First.");
                            playerTurnList.Add(Player0);
                            playerTurnList.Add(Player1);
                            bPlayer0First = false;  //Switch Player go first next turn
                        }
                        else
                        {
                            Debug.Log("Player1 Go First.");
                            playerTurnList.Add(Player1);
                            playerTurnList.Add(Player0);
                            bPlayer0First = true;   //Switch Player go first next turn
                        }
                        ChangeState(GameStates.Player_Turn_Begin);
                    }
                    break;
                case GameStates.Player_Turn_Begin:
                    if (playerTurnList.Count == 0) // all players' turn is over. 
                        ChangeState(GameStates.Battle_Begin);
                    if (nowTime - stateBeginTime > 1)
                    {
                        Player currentPlayer = playerTurnList[0];
                        playerTurnList.Remove(currentPlayer);
                        currentPlayerId = currentPlayer.PlayerId;
                        currentPlayer.Event_PlayerTurnStart.Invoke();
                        ChangeState(GameStates.Wait_For_Player_Turn);
                    }
                    break ;
                case GameStates.Wait_For_Player_Turn:
                    if (nowTime - stateBeginTime > PlayerTurnTimeLimited) // Setting a time limit for each player. TODO: when this is triggered, a delegent should be sent to the Player.
                    {
                        Debug.LogWarningFormat("Player {0}'s time out. The system will consider it hasn't done anything.", currentPlayerId);
                        ChangeState(GameStates.Player_Turn_Begin);
                    }
                    break;
                case GameStates.Battle_Begin:
                    if (nowTime - stateBeginTime > 2)
                    {
                        world.Event_BattleBegin.Invoke();
                        ChangeState(GameStates.Wait_For_Battle);
                    }
                    break;
                case GameStates.Battle_End:
                    gameGlobalState.matchCnt++;
                    winner = gameGlobalState.lastWinner;
                    if (winner == -1)
                        Debug.LogFormat("Round {0}: Draw! Current Score: {1}:{2}", gameGlobalState.matchCnt, gameGlobalState.playerWinCnt[0], gameGlobalState.playerWinCnt[1]);
                    else
                    {
                        gameGlobalState.playerWinCnt[winner]++;
                        Debug.LogFormat("Round {0}: Player {1} wins! Current Score: {2}:{3}", gameGlobalState.matchCnt, winner, gameGlobalState.playerWinCnt[0], gameGlobalState.playerWinCnt[1]);
                    }
                    if ((winner >= 0 && gameGlobalState.playerWinCnt[winner] > 2) || gameGlobalState.matchCnt == 5)
                        ChangeState(GameStates.Game_End);
                    else
                        ChangeState(GameStates.UpKeepStep);
                    break;
                case GameStates.Game_End:
                    if (gameGlobalState.playerWinCnt[0] != gameGlobalState.playerWinCnt[1])
                        winner = (gameGlobalState.playerWinCnt[0] < gameGlobalState.playerWinCnt[1] ? 1 : 0);
                    else winner = -1;
                    if (winner == -1)
                        Debug.Log("Game over. Draw.");
                    else
                        Debug.LogFormat("Game over. Player {0} wins. Congrat to him!!!", winner);
                    ChangeState(GameStates.Game_Begin);// Todo: player can choose to restart a game or 
                    break;
                case GameStates.Error:
                    // TODO: some operations to get back to init.
                    if(nowTime - stateBeginTime > 10)
                    {
                        ChangeState(GameStates.Init);
                    }
                    break;
            }
        }

        void ScanFinshedUpdate()
        {
            Debug.Log("ScanFinished");
            List<int> path = new List<int>();
            //Test pathfinding
            path = world.tileMap.getShortestPath(105,46);

            Player0.Event_ScanFinished.Invoke();
            if(currentState == GameStates.Wait_For_Map_Scan)
            {
                if (currentMode == GameMode.Offline_Mode)
                {
                    Player1.Event_ScanFinished.Invoke();
                    ChangeState(GameStates.Game_Begin);
                }
                else
                    ChangeState(GameStates.Online_Only_Wait_For_Connection);
            }
            return;
        }

        #region Delegate Handler
        /// <summary>
        /// Invoke when Player Turn Ended.
        /// </summary>
        /// <param name="PlayerId">
        /// Input the player id.
        /// </param>
        void PlayerEndUpdate(int PlayerId)
        {
            if (currentState != GameStates.Wait_For_Player_Turn || currentPlayerId != PlayerId)
            {
                Debug.LogWarningFormat("Sorry player {0}, this is not your turn yet.", PlayerId);
                return;
            }
            else
            {
                Debug.LogFormat("Player {0} has finished it's turn.", PlayerId);
                ChangeState(GameStates.Player_Turn_Begin);
            }
        }
        /// <summary>
        /// post-process of a battle.
        /// </summary>
        /// <param name="winner">
        /// the winner of this battle. winner = -1 if there's a draw. 
        /// </param>
        void BattleEndUpdate(int winner)
        {
            gameGlobalState.lastWinner = winner;
            ChangeState(GameStates.Battle_End);
        }
        /// <summary>
        /// Update game states after world is finished
        /// </summary>
        private void World_ResetFinishedUpdate()
        {
            Debug.Log("WorldResetFinished");
            ChangeState(GameStates.Turn_Begin);
        }
        ///<summary>
        /// Dice Handle
        ///</summary>
        void DiceStopHandle()
        {
            if(currentState==GameStates.ChooseFirstPlayer)
            {
                if(DiceInstant.GetDiceCount()<=3)
                {
                    bPlayer0First = true;
                }
                else
                {
                    bPlayer0First = false;
                }
                ChangeState(GameStates.UpKeepStep);
            }
            return;
        }
        #endregion
    }

}

