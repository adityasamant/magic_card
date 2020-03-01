using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace GameLogic
{
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
    /// When this player is end its turn, it will pop a delegate to Main Logic
    /// </summary>
    /// <param name="id">
    /// The playerid, should be unique for each Player
    /// </param>
    public delegate void PlayerEnd(int id);

    public class Player : MonoBehaviour
    {

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

        #region Public Delegate
        /// <summary>
        /// Define the object for delegate
        /// </summary>
        public PlayerEnd PlayerEnd;
        #endregion

        #region Public Property
        /// <summary>
        /// Player Identity, should be unique for each player
        /// </summary>
        public int PlayerId;
        #endregion

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
