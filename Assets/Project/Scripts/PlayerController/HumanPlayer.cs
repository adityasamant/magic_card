using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CardInfo;
using UnityEngine.XR.MagicLeap;
using InputController;
using UI;
using GameWorld;
using Monsters;

namespace GameLogic
{
    /// <summary>
    /// Using a Finited State Machine to control the game loop in Player Turn
    /// </summary>
    public class HumanPlayer : Player
    {
        #region Private Variable
        /// <summary>
        /// myState record the states of the player
        /// </summary>
        private PlayerStates myState = PlayerStates.Init;

        /// <summary>
        /// Only Init when game start
        /// </summary>
        private bool _bInit = false;

        /// <summary>
        /// Remember the startTime, wait 5 seconds for MagicLeap to StartUp
        /// </summary>
        private float startTime;

        /// <summary>
        /// The Card Player played
        /// </summary>
        private string PlayedCardName;

        /// <summary>
        /// This is a pointer to the Monster in UI
        /// </summary>
        private GameObject UIMonsterpreview;

        ///<summary>
        ///This is a string recording the monster name
        ///</summary>
        private string UIMonsterName;

        ///<summary>
        ///This is Raycast hit point position
        ///</summary>
        private Vector3 RayHitPosition;

        ///<summary>
        /// The ID of one hex to place new monster
        ///</summary>
        private int targetHexId;

        ///<summary>
        /// The num of monster card unchosen
        ///</summary>
        private int MCardUnchosen;

        ///<summary>
        /// The num of terrian card unchosen
        ///</summary>
        private int TCardUnchosen;

        /// <summary>
        /// For Audio Tutorial, When first time player choose hex, play the audio
        /// </summary>
        private bool _isFirstChosenHex = true;
        /// <summary>
        /// For Audio Tutorial, When first time player play card, play the audio
        /// </summary>
        private bool _isFirstPlayCard = true;
        #endregion

        #region Public Variable
        /// <summary>
        /// NetworkEvent Sender
        /// </summary>
        public NetworkPlayer networkPlayer = null;

        /// <summary>
        /// Num of available monster card (default 3)
        /// </summary>
        public int numOfMonsterCouldUse = 3;

        /// <summary>
        /// Num of available terrain card (default 2)
        /// </summary>
        public int numOfTerrainCouldUse = 2;

        /// <summary>
        /// CardInfo Reader
        /// Get all card from database
        /// </summary>
        public NewCardDB myCardDataBase;

        /// <summary>
        /// The Array of HexMap
        /// </summary>
        public Transform HexMap;

        /// <summary>
        /// GameObject of MainCamera
        /// </summary>
        public GameObject MainCamera;

        /// <summary>
        /// GameObject of Controller Manager
        /// </summary>
        public ControllerManager ControllerManager;

        /// <summary>
        /// GameObject of the ContentUIManager
        /// </summary>
        [Tooltip("GameObject of the ContentUIManager")]
        public ContentUIManager ContentUIManager;
        /// <summary>
        /// The Text box in UI show Instructions.
        /// </summary>
        [Tooltip("The Text box in UI show Instructions.")]
        public Text InstructionUI;
        #endregion

        // Start is called before the first frame update
        public virtual void Start()
        {
            if (_bInit)
            {
                myState = PlayerStates.Init;
                startTime = Time.time;
                _bInit = true;
            }
            //Event Init
            if (Event_PlayerTurnStart == null)
            {
                Event_PlayerTurnStart = new UnityEvent();
            }
            Event_PlayerTurnStart.AddListener(PlayerTurnStartInvoke);
            if (Event_ScanFinished == null)
            {
                Event_ScanFinished = new UnityEvent();
            }
            Event_ScanFinished.AddListener(ScanFinishedInvoke);

            PlayedCard += PlayedCardInvoked;
            PlayerEnd += PlayerEndInvoked;

            ControllerManager.ClickOnCard += ClickOnCardInvoked;
            ControllerManager.ClickOnHex += ClickOnHexInvoked;
            ControllerManager.ClickOnMonster += ClickOnMonsterInvoked;

            MCardUnchosen = numOfMonsterCouldUse;
            TCardUnchosen = numOfTerrainCouldUse;
        }

        ///<summary>
        ///Update is called once per frame
        ///</summary> 
        public virtual void Update()
        {
            //Raycast hit point position
            RayHitPosition = ControllerManager.hitPoint;

            //StateMachine Related
            switch (myState)
            {
                case (PlayerStates.Init):
                    Debug.Log("PlayerStates=Init");
                    ContentUIManager.ShowUICanvas();
                    InstructionUI.text = "Place BattleFiled";
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    InstructionUI.text = "Wait For Start";
                    break;
                case (PlayerStates.Action_Phase):
                    InstructionUI.text = "Action";
                    break;
                case (PlayerStates.Main_Phase):
                    if (MCardUnchosen == 0)
                    {
                        myState = PlayerStates.Action_Phase;
                        break;
                    }
                    InstructionUI.text = "Choose A Card";
                    if (myCardDataBase)
                    {
                        ContentUIManager.DisplayCard();
                    }
                    break;
                case (PlayerStates.Confirm_Phase): // Wait For Click on Hex
                    //Debug.Log("PlayerStates=Confirm_Phase");
                    break;
                case (PlayerStates.Spawn_Phase):
                    //Debug.Log("PlayerStates=Spawn_Phase");
                    //Debug.Log("Now is your turn, Spawn-Phase.\n Your monster is spawning into battlefield.");
                    InstructionUI.text = PlayedCardName + ":" + targetHexId.ToString();
                    Debug.Log("Spawn A Charcter Name:" + PlayedCardName);
                    if (myCardDataBase)
                    {
                        NewCard myCard = myCardDataBase.GetCard(PlayedCardName);
                        PlayedCard(PlayerId, myCard.id, targetHexId);
                        MCardUnchosen--;
                    }
                    myState = PlayerStates.End;
                    break;
                case (PlayerStates.End):
                    //Return the game control loop to main logic
                    //Debug.Log("PlayerStates=End");
                    PlayerEnd(PlayerId);
                    myState = PlayerStates.WaitForStart;
                    break;
                default:
                    break;
            }
        }

        #region Event Handler
        /// <summary>
        /// Call this function when Event_PlayerTurnStart invoke
        /// Change User state from WaitForStart to Main_Phase
        /// </summary>
        void PlayerTurnStartInvoke()
        {
            if (myState == PlayerStates.WaitForStart)
            {
                myState = PlayerStates.Main_Phase;
            }
        }

        /// <summary>
        /// Call this function when Event_ScanFished invoke
        /// Change User state from Init to WaitForStart
        /// </summary>
        void ScanFinishedInvoke()
        {
            if (myState == PlayerStates.Init)
            {
                myState = PlayerStates.WaitForStart;
            }
        }

        /// <summary>
        /// This function will call when click on a Card
        /// </summary>
        /// <param name="CardName">The Chosen Card Name</param>
        private void ClickOnCardInvoked(string CardName)
        {
            if (myState == PlayerStates.Main_Phase)
            {
                PlayedCardName = CardName;
                ContentUIManager.ClearContentUI();
                InstructionUI.text = "Place it!";
                Debug.Log("Now Player want to use " + PlayedCardName);
                if(networkPlayer)
                {
                    networkPlayer.Send_ClickOnCard(CardName);
                }
                myState = PlayerStates.Confirm_Phase;
            }
            return;
        }

        /// <summary>
        /// This function will call when click on a Hex
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnHexInvoked(int HexTileID)
        {
            if (myState == PlayerStates.Confirm_Phase)
            {
                targetHexId = HexTileID;
                if(_isFirstChosenHex)
                {
                    AudioManager._instance.Play("ChooseHex");
                    _isFirstChosenHex = false;
                }
                
                if(networkPlayer)
                {
                    networkPlayer.Send_ClickOnHex(HexTileID);
                }
                myState = PlayerStates.Spawn_Phase;
            }
            // if (myState == PlayerStates.Action_Phase)
            // {
            //     foreach (KeyValuePair<int, Monster> monsterPair in world.monsters)
            //     {
            //         Monster thisMonster = monsterPair.Value;
            //         if (thisMonster.isAlive && thisMonster.HexIndex == HexTileID){
            //             if(thisMonster.monsterOwner.GetPlayerId() == PlayerId){
            //                 ContentUIManager.ShowActionBtn();
            //             }else{
            //                 ContentUIManager.HideActionBtn();
            //             }
            //         }
            //     }
            // }
            return;
        }

        /// <summary>
        /// This function will call when click on a monster
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnMonsterInvoked(Monster currMonster)
        {
            if (myState == PlayerStates.Action_Phase)
            {
                if (currMonster.isAlive && currMonster.monsterOwner.GetPlayerId() == PlayerId)
                {
                    ContentUIManager.ShowActionBtn();
                }
                else
                {
                    ContentUIManager.HideActionBtn();
                }
            }
            return;
        }
        /// <summary>
        /// Invoke when Player turn End
        /// </summary>
        /// <param name="PlayerId">The Player Index</param>
        private void PlayerEndInvoked(int PlayerId)
        {
            if(networkPlayer)
            {
                networkPlayer.Send_PlayerTurnEnd(PlayerId);
            }
            return;
        }

        #endregion

        #region Delegate Handler
        /// <summary>
        /// Invoked when the player is playing card
        /// </summary>
        /// <param name="PlayerId">The Player Id of the played card.</param>
        /// <param name="CardIndex">The played card index</param>
        /// <param name="HexIndex">The hex that card has been place on</param>
        private void PlayedCardInvoked(int PlayerId, int CardIndex, int HexIndex)
        {
            Debug.LogFormat("Player {0} playing card {1} in hex {2}", PlayerId, CardIndex, HexIndex);
            if(_isFirstPlayCard)
            {
                AudioManager._instance.Play("FirstPlayCard");
                _isFirstPlayCard = false;
            }
            
            InstructionUI.text = "PlayedCard";
            if(networkPlayer)
            {
                networkPlayer.Send_PlayCard(PlayerId, CardIndex, HexIndex);
            }
            return;
        }
        #endregion
    }
}


