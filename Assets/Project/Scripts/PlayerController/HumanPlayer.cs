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
        #endregion

        #region Public Variable
        /// <summary>
        /// Num of available monster card (default 3)
        /// </summary>
        public int numOfMonsterCouldUse;

        /// <summary>
        /// Num of available terrain card (default 2)
        /// </summary>
        public int numOfTerrainCouldUse;

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
        /// GameObject of the ContentUIManager
        /// </summary>
        [Tooltip("GameObject of the ContentUIManager")]
        public ContentUIManager ContentUIManager;
        /// <summary>
        /// The Text box in UI show Instructions.
        /// </summary>
        [Tooltip("The Text box in UI show Instructions.")]
        public Text InstructionUI;

        public Monster currMonster;
        public Monster targetMonster;
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
            AttackDelegate += AttackInvoked;

            ControllerManager.ClickOnCard += ClickOnCardInvoked;
            ControllerManager.ClickOnHex += ClickOnHexInvoked;
            ControllerManager.ClickOnMonster += ClickOnMonsterInvoked;
            ControllerManager.ClickOnBtn += ClickOnBtnInvoked;

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
                    //Debug.Log("PlayerStates=Init");
                    ContentUIManager.ShowUICanvas();
                    InstructionUI.text = "Place BattleFiled";
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    //Debug.Log("PlayerStates=WaitForStart");
                    // InstructionUI.text = "Wait For Your Turn";
                    break;
                case (PlayerStates.Main_Phase):
                    //(MCardUnchosen <= 0) => Card selection finished
                    if (MCardUnchosen <= 0)
                    {
                        myState = PlayerStates.Action_Phase;
                        InstructionUI.text = "Player " + PlayerId + " Turn";
                        break;
                    }
                    else
                    {
                        InstructionUI.text = "Choose A Card";
                        if (myCardDataBase)
                        {
                            ContentUIManager.DisplayCard();
                        }
                    }
                    break;
                case (PlayerStates.Action_Phase):
                    // InstructionUI.text = "Your Turn";
                    break;
                case (PlayerStates.Attack_Phase):
                    myState = PlayerStates.ChooseTarget_Phase;
                    break;
                case (PlayerStates.ConfirmMonsterPosition_Phase): // Wait For Click on Hex
                    //Debug.Log("PlayerStates=ConfirmMonsterPosition_Phase");
                    InstructionUI.text = "Choose Spawn Position";
                    break;
                case (PlayerStates.Spawn_Phase):
                    //Debug.Log("PlayerStates=Spawn_Phase");
                    //Debug.Log("Spawn A Charcter Name:" + PlayedCardName);
                    if (myCardDataBase)
                    {
                        Cards myCard = myCardDataBase.GetCard(PlayedCardName);
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
                    InstructionUI.text = "Wait For Your Turn";
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
                InstructionUI.text = "Wait For Your Turn";
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
                myState = PlayerStates.ConfirmMonsterPosition_Phase;
            }
            return;
        }

        /// <summary>
        /// This function will call when click on a Hex
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnHexInvoked(int HexTileID)
        {
            if (myState == PlayerStates.ConfirmMonsterPosition_Phase)
            {
                targetHexId = HexTileID;
                myState = PlayerStates.Spawn_Phase;
            }
            if (myState == PlayerStates.Move_Phase)
            {

            }
            return;
        }

        /// <summary>
        /// This function will call when click on a monster
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnMonsterInvoked(Monster clickedMonster)
        {
            //choose an action
            if (myState == PlayerStates.Action_Phase)
            {
                currMonster = clickedMonster;
                if (currMonster.isAlive && currMonster.monsterOwner.GetPlayerId() == PlayerId)
                {
                    ContentUIManager.ShowActionBtn();
                    InstructionUI.text = currMonster.monsterName;
                }
                else
                {
                    ContentUIManager.HideActionBtn();
                }
            }

            //confirm your attack target
            if (myState == PlayerStates.ChooseTarget_Phase)
            {
                targetMonster = clickedMonster;
                //TODO check attack range
                if (targetMonster.isAlive && targetMonster.monsterOwner.GetPlayerId() != PlayerId)
                {
                    AttackDelegate(PlayerId, currMonster, targetMonster);
                }
                else
                {
                }
                //Should go to next monster
                myState = PlayerStates.End;
            }
            return;
        }

        /// <summary>
        /// This function will call when click on a button
        /// </summary>
        /// <param name="btnName"></param>
        private void ClickOnBtnInvoked(string btnName)
        {
            if (myState == PlayerStates.Action_Phase)
            {
                if (currMonster.isAlive && currMonster.monsterOwner.GetPlayerId() == PlayerId)
                {
                    switch (btnName)
                    {
                        case ("AttackBtn"):
                            InstructionUI.text = "Choose Attack Target";
                            ContentUIManager.HideActionBtn();
                            myState = PlayerStates.Attack_Phase;
                            break;
                        case ("SkillBtn"):
                            InstructionUI.text = "Choose Skill Target";
                            ContentUIManager.HideActionBtn();
                            break;
                        case ("IdleBtn"):
                            InstructionUI.text = "Idle";
                            ContentUIManager.HideActionBtn();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Debug.Log("Current Monster doesn't match UI!");
                }
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

        /// <summary>
        /// Invoked when the player is attacking
        /// </summary>
        /// <param name="PlayerId">The Player Id of the played card.</param>
        /// <param name="currMonster">Controlled Monster</param>
        /// <param name="targetMonster">Target Monster</param>
        private void AttackInvoked(int PlayerId, Monster currMonster, Monster targetMonster)
        {
            Debug.LogFormat("Player {0} use {1} attack {2}", PlayerId, currMonster, targetMonster);
            currMonster.Attack(targetMonster, currMonster.ATK);
            return;
        }
        #endregion
    }
}


