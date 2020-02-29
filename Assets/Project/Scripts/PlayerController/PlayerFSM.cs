using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CardInfo;
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

        /// <summary>
        /// This is a pointer to the Monster in UI
        /// </summary>
        private GameObject UIMonsterpreview;

        ///<summary>
        ///This is a string recording the monster name
        ///</summary>
        private string UIMonsterName;

        #endregion

        #region Event Define
        ///<summary>
        /// The MainLogic will give a event to the player when scan finished.
        ///</summary>
        public UnityEvent Event_ScanFinished;

        /// <summary>
        /// The MainLogic will give a event to the player when player turn start.
        /// </summary>
        public UnityEvent Event_PlayerTurnStart;

        #endregion

        #region Public Variable
        
        /// <summary>
        /// Preview GameObject under this gameobject
        /// </summary>
        public GameObject MonsterPreviewField;

        /// <summary>
        /// CardInfo Reader
        /// </summary>
        public GetCardInstruction myCardDataBase;

        /// <summary>
        /// The Text box in UI show how many card you are tracking.
        /// </summary>
        [Tooltip("The Text box in UI show how many card you are tracking.")]
        public Text NumberOfCard;

        /// <summary>
        /// The Text box in UI show which card you are tracking.
        /// </summary>
        [Tooltip("The Text box in UI show which card you are tracking.")]
        public Text CardInfo;

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
        /// The Text box in UI show Instructions based on states.
        /// </summary>
        [Tooltip("The Text box in UI show Instructions based on states.")]
        public Text Instructions;

        /// <summary>
        /// The Array of HexMap
        /// </summary>
        public Transform HexMap;

        /// <summary>
        /// GameObject of MainCamera
        /// </summary>
        public GameObject MainCamera;
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
            if(Event_ScanFinished==null)
            {
                Event_ScanFinished = new UnityEvent();
            }
            Event_ScanFinished.AddListener(ScanFinishedInvoke);
        }

        ///<summary>
        ///Update is called once per frame
        ///</summary> 
        void Update()
        {
            //StateMachine Related
            switch (myState)
            {
                case (PlayerStates.Init):
                    //Debug.Log("PlayerStates=Init");
                    Instructions.text = "Now is Init States. \n Please scanning the room around you. \n Press trigger to finish scanning.";
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
                    }
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    //Debug.Log("PlayerStates=WaitForStart");
                    Instructions.text = "Now is not your turn. Please wait for the game going.";
                    break;
                case (PlayerStates.Main_Phase): // Wait For Hand Event From Hand Tracker
                    //Debug.Log("PlayerStates=Main_Phase");
                    Instructions.text = "Now is your turn, Main-Phase.\n Please choose a card. \n And shape your right hand to OK pose to confirm.";
                    if (NumberOfCard)
                    {
                        NumberOfCard.text = "Now you are tracking " + myCardTracker.GetStates().ToString() + "Cards.";
                    }
                    if (myCardTracker.GetStates() == 0)
                    {
                        CardInfo.text = "Non Card Been Tracked.";
                    }
                    else if (myCardTracker.GetStates() > 1)
                    {
                        CardInfo.text = "More than 1 card been tracked. \n Please only use 1 card per turn.";
                    }
                    else
                    {
                        if (myCardDataBase)
                        {
                            Cards myCard = myCardDataBase.GetCard(myCardTracker.GetTrackingName());
                            CardInfo.text = "CardName:" + myCard.CardName + "\n"
                                + "Attack:" + myCard.Attack.ToString() + "\n"
                                + "HP:" + myCard.HP.ToString() + "\n"
                                + "Speed:" + myCard.Speed.ToString() + "\n"
                                + "Special Effect:\n"
                                + myCard.SpecialEffect;

                            if (UIMonsterName != myCard.CardName)
                            {
                                Destroy(UIMonsterpreview);
                                UIMonsterName = myCard.CardName;
                                Debug.Log(myCard.PrefabPath);
                                UIMonsterpreview = Object.Instantiate(Resources.Load<GameObject>(myCard.PrefabPath), MonsterPreviewField.transform);
                            }
                        }
                    }
                    break;
                case (PlayerStates.Confirm_Phase): // Wait For Hand (OpenHand or Fist) From Hand Tracker
                    //Debug.Log("PlayerStates=Confirm_Phase");
                    Instructions.text = "Now is your turn, Confirm-Phase.\n Please use Open-Hand pose to comfirm. \n Or use Fist pose to go back.";
                    if (NumberOfCard)
                    {
                        NumberOfCard.text = "Now you want to use " + PlayedCardName + ".";
                    }
                    if (myCardDataBase)
                    {
                        Cards myCard = myCardDataBase.GetCard(PlayedCardName);
                        CardInfo.text = "CardName:" + myCard.CardName + "\n"
                            + "Attack:" + myCard.Attack.ToString() + "\n"
                            + "HP:" + myCard.HP.ToString() + "\n"
                            + "Speed:" + myCard.Speed.ToString() + "\n"
                            + "Special Effect:\n"
                            + myCard.SpecialEffect;

                        if (UIMonsterName != myCard.CardName)
                        {
                            Destroy(UIMonsterpreview);
                            Debug.Log(myCard.PrefabPath);
                            UIMonsterpreview = Object.Instantiate(Resources.Load<GameObject>(myCard.PrefabPath), MonsterPreviewField.transform);
                            UIMonsterName = myCard.CardName;
                        }
                    }
                    break;
                case (PlayerStates.Spawn_Phase):
                    //Debug.Log("PlayerStates=Spawn_Phase");
                    //Spawn Actor Here
                    Instructions.text = "Now is your turn, Spawn-Phase.\n Your monster is spawning into battlefield.";
                    //Debug.Log("Spawn A Charcter Name:" + PlayedCardName);
                    if (myCardDataBase)
                    {
                        Cards myCard = myCardDataBase.GetCard(PlayedCardName);
                        GameObject monsterClass = Resources.Load<GameObject>(myCard.PrefabPath);
                        float myDis = 9999.9f;
                        int index = 0;
                        for(int i=0;i<HexMap.childCount;i++)
                        {
                            Transform hex = HexMap.GetChild(i);
                            if((MainCamera.transform.position - hex.position).magnitude<myDis)
                            {
                                myDis = (MainCamera.transform.position - hex.position).magnitude;
                                index = i;
                            }
                        }
                        GameObject NewMonster = Instantiate(monsterClass, HexMap.GetChild(index));
                    }
                    myState = PlayerStates.End;
                    break;
                case (PlayerStates.End):
                    //Return the game control loop to main logic
                    //Debug.Log("PlayerStates=End");
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


