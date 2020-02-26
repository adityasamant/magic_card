using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace GameLogic
{
    /// <summary>
    /// When this player is end its turn, it will pop a delegate to Main Logic
    /// </summary>
    /// <param name="id">
    /// The playerid, should be unique for each Player
    /// </param>
    public delegate void PlayerEnd(int id);

    /// <summary>
    /// PlayerStates are states for the Finited State Machine
    /// </summary>
    public enum PlayerStates
    {
        Init,
        WaitForStart,
        Main_Phase,
        Confirm_Phase,
        Spawn_Phase,
        End,
        Error
    };

    /// <summary>
    /// Using a Finited State Machine to control the game loop in Player Turn
    /// </summary>
    public class PlayerFSM : MonoBehaviour
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

        #endregion

        #region Event Define
        UnityEvent Event_PlayerTurnStart;
        #endregion

        #region Public Variable
        /// <summary>
        /// Start Hand Tracker when game Launch
        /// </summary>
        public GameObject myHandTrackerObject;
        ///<summary>
        /// A link to HandTracker
        ///</summary>
        public HandTracker myHandTracker;
        /// <summary>
        /// Start Card Tracker when game Launch
        /// </summary>
        public GameObject myCardeTrackerObject;
        /// <summary>
        /// A link to CardTracker
        /// </summary>
        public CardTracker myCardTracker;
        /// <summary>
        /// Player Identity, should be unique for each player
        /// </summary>
        public int PlayerId;

        /// <summary>
        /// Define the object for delegate
        /// </summary>
        public PlayerEnd PlayerEnd;

        /// <summary>
        /// Monster Prefab List
        /// </summary>
        public GameObject[] MonsterPrefabLists;

        /// <summary>
        /// The Text box in UI show Instructions based on states.
        /// </summary>
        [Tooltip("The Text box in UI show Instructions based on states.")]
        public Text Instructions;
        #endregion

        // Start is called before the first frame update
        void Start()
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
        }

        // Update is called once per frame
        void Update()
        {
            switch(myState)
            {
                case (PlayerStates.Init):
                    Debug.Log("PlayerStates=Init");
                    Instructions.text = "Now is Init States. Wait 5 seconds.";
                    float nowTime = Time.time;
                    if(nowTime-startTime>5.0f)
                    {
                        if(myHandTrackerObject)
                            myHandTrackerObject.SetActive(true);
                        if(myCardeTrackerObject)
                            myCardeTrackerObject.SetActive(true);
                        if(myHandTracker)
                        {
                            myHandTracker.FindPose += HandGesture;
                        }
                        myState = PlayerStates.WaitForStart;
                    }
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    Debug.Log("PlayerStates=WaitForStart");
                    Instructions.text = "Now is not your turn. Please wait for the game going.";
                    nowTime = Time.time; //Test Only
                    if (nowTime - startTime > 10.0f)
                    {
                        myState = PlayerStates.Main_Phase;
                        myHandTracker.Event_enableOKtracked.Invoke();
                    }
                    break;
                case (PlayerStates.Main_Phase): // Wait For Hand Event From Hand Tracker
                    Debug.Log("PlayerStates=Main_Phase");
                    Instructions.text = "Now is your turn, Main-Phase.\n Please choose a card. \n And shape your right hand to OK pose to confirm.";
                    break;
                case (PlayerStates.Confirm_Phase): // Wait For Hand (OpenHand or Fist) From Hand Tracker
                    Debug.Log("PlayerStates=Confirm_Phase");
                    Instructions.text = "Now is your turn, Confirm-Phase.\n Please use Open-Hand pose to comfirm. \n Or use Fist pose to go back.";
                    break;
                case (PlayerStates.Spawn_Phase):
                    Debug.Log("PlayerStates=Spawn_Phase");
                    //Spawn Actor Here
                    Instructions.text = "Now is your turn, Spawn-Phase.\n Your monster is spawning into battlefield.";
                    Debug.Log("Spawn A Charcter Name:" + PlayedCardName);
                    if(PlayedCardName=="aaa")
                    {
                        GameObject monsterAAA = Instantiate(MonsterPrefabLists[0]);
                    }
                    else if(PlayedCardName=="bbb")
                    {
                        GameObject monsterBBB = Instantiate(MonsterPrefabLists[1]);
                    }
                    myState = PlayerStates.End;
                    break;
                case (PlayerStates.End):
                    //Return the game control loop to main logic
                    Debug.Log("PlayerStates=End");
                    Instructions.text = "Now is your turn, End-Phase. Switch to next turn.";
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
                myState = PlayerStates.Main_Phase;
                myHandTracker.Event_enableOKtracked.Invoke();
            }
        }
        #endregion

        #region Delegate Handler
        /// <summary>
        /// Invoke when receive delegate from HandTracker
        /// </summary>
        /// <param name="PoseId">
        /// Id=0, OK Pose
        /// Id=1, OpenHand Pose
        /// id=2, Fist Pose
        /// </param>
        private void HandGesture(int PoseId)
        {
            Debug.Log("HandGestrue here!");
            if(PoseId==0)
            {
                if(myState==PlayerStates.Main_Phase)
                {
                    if(myCardTracker.GetStates()==1)
                    {
                        PlayedCardName = myCardTracker.GetTrackingName();
                        myHandTracker.Event_enableFistTracked.Invoke();
                        myHandTracker.Event_enableOpenHandtracked.Invoke();
                        myState = PlayerStates.Confirm_Phase;
                    }
                }
            }
            if(PoseId==1)
            {
                if(myState==PlayerStates.Confirm_Phase)
                {
                    myHandTracker.Event_disableAllTracked.Invoke();
                    myState = PlayerStates.Spawn_Phase;
                }
            }
            if(PoseId==2)
            {
                if(myState==PlayerStates.Confirm_Phase)
                {
                    PlayedCardName = "NON";
                    myState = PlayerStates.Main_Phase;
                }
            }
        }
        #endregion
    }
}


