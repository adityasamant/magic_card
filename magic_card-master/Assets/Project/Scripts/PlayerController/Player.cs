﻿using System.Collections;
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
        ImageTrackingStart,
        Main_Phase,
        Confirm_Phase,
        Spawn_Phase,
        ImageTrackingStop,
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

    /// <summary>
    /// When this player is playing a card, it will pop a delegate to Main Logic
    /// </summary>
    /// <param name="PlayerId">The Player Id of the card.</param>
    /// <param name="CardIndex">The Card Index of the played card.</param>
    /// <param name="HexIndex">The Hex Index of the player camera.</param>
    public delegate void PlayedCard(int PlayerId, int CardIndex, int HexIndex);

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
        /// Define the interface for delegate PlayerEnd
        /// </summary>
        public PlayerEnd PlayerEnd;

        /// <summary>
        /// Define the interface for delegate PlayedCard
        /// </summary>
        public PlayedCard PlayedCard;
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
