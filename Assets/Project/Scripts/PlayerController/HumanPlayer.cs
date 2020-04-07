using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CardInfo;
using UnityEngine.XR.MagicLeap;
using InputController;
using UI;

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
        private PlayerStates myState=PlayerStates.Init;

        /// <summary>
        /// Only Init when game start
        /// </summary>
        private bool _bInit=false;

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
        #endregion

        #region Public Variable
        /// <summary>
        /// NetworkEvent Sender
        /// </summary>
        public NetworkPlayer networkPlayer = null;

        /// <summary>
        /// CardInfo Reader
        /// </summary>
        public GetCardInstruction myCardDataBase;

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
        /// GameObject of the CardUIManager
        /// </summary>
        [Tooltip("GameObject of the CardUIManager")]
        public CardUIManager CardUIManager;
        #endregion

        // Start is called before the first frame update
        public virtual void Start()
        {
            if(_bInit)
            {
                myState = PlayerStates.Init;
                startTime = Time.time;
                _bInit = true;
            }
            //Event Init
            if(Event_PlayerTurnStart == null)
            {
                Event_PlayerTurnStart = new UnityEvent();
            }
            Event_PlayerTurnStart.AddListener(PlayerTurnStartInvoke);
            if(Event_ScanFinished==null)
            {
                Event_ScanFinished = new UnityEvent();
            }
            Event_ScanFinished.AddListener(ScanFinishedInvoke);

            PlayedCard += PlayedCardInvoked;

            ControllerManager.ClickOnCard += ClickOnCardInvoked;
            ControllerManager.ClickOnHex += ClickOnHexInvoked;

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
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    Debug.Log("PlayerStates=WaitForStart");
                    break;
                case (PlayerStates.ImageTrackingStart):
                    myState = PlayerStates.Main_Phase;
                    break;
                case (PlayerStates.Main_Phase): // Wait For Hand Event From Hand Tracker
                    Debug.Log("PlayerStates=Main_Phase");
                    Debug.Log("Now is your turn, Main-Phase.Please choose a card.");
                    if (myCardDataBase)
                    {
                        CardUIManager.ShowCardUI();
                    }
                    break;
                case (PlayerStates.Confirm_Phase): // Wait For Hand (OpenHand or Fist) From Hand Tracker
                    //Debug.Log("PlayerStates=Confirm_Phase");
                    break;
                case (PlayerStates.Spawn_Phase):
                    //Debug.Log("PlayerStates=Spawn_Phase");
                    //Spawn Actor Here
                    //Debug.Log("Now is your turn, Spawn-Phase.\n Your monster is spawning into battlefield.");
                    Debug.Log("Spawn A Charcter Name:" + PlayedCardName);
                    if (myCardDataBase)
                    {
                        Cards myCard = myCardDataBase.GetCard(PlayedCardName);
                        PlayedCard(PlayerId, myCard.id, targetHexId);
                    }
                    myState = PlayerStates.End;
                    break;
                case (PlayerStates.ImageTrackingStop):
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
             if(myState==PlayerStates.WaitForStart)
            {
                myState = PlayerStates.ImageTrackingStart;
            }
        }

        /// <summary>
        /// Call this function when Event_ScanFished invoke
        /// Change User state from Init to WaitForStart
        /// </summary>
        void ScanFinishedInvoke()
        {
            if(myState==PlayerStates.Init)
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
            if(myState==PlayerStates.Main_Phase)
            {
                PlayedCardName=myCardDataBase.GetRandomCard().CardName;
                CardUIManager.HideCardUI();
                Debug.Log("Now Player want to use " + PlayedCardName);
                myState = PlayerStates.Confirm_Phase;
                //Debug.LogFormat("Now Player want to use {0}, ATK: {1}, HP: {2}, SPEED: {3}, SPECIAL EFFECT: {4}", PlayedCardName.CardName, PlayedCardName.Attack, PlayedCardName.HP, PlayedCardName.Speed, PlayedCardName.SpecialEffect);
            }
            return;
        }

        /// <summary>
        /// This function will call when click on a Hex
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnHexInvoked(int HexTileID)
        {
            if(myState == PlayerStates.Confirm_Phase)
            {
                targetHexId = HexTileID;
                myState = PlayerStates.Spawn_Phase;
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
            return;
        }
        #endregion
    }
}


