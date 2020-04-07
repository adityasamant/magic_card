using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardInfo;

namespace GameLogic
{
    /// <summary>
    /// Using Input from Network
    /// </summary>
    public class RemotePlayer : Player
    {
        #region Private Reference
        /// <summary>
        /// CardInfo Database
        /// </summary>
        [SerializeField, Tooltip("CardInfo Database")]
        private GetCardInstruction myCardDataBase;
        #endregion

        #region Private Variable
        /// <summary>
        /// myState record the states of the player
        /// </summary>
        private PlayerStates myState = PlayerStates.Init;
        /// <summary>
        /// The Card Player played
        /// </summary>
        private string PlayedCardName;
        ///<summary>
        /// The ID of one hex to place new monster
        ///</summary>
        private int targetHexId;
        #endregion

        #region Network Function
        /// <summary>
        /// Public Reference and Interface of NetworkPlayer
        /// Network Event Receiver
        /// </summary>
        public NetworkPlayer networkPlayer = null;
        /// <summary>
        /// Binding the NetworkPlayer Delegate with local Function
        /// </summary>
        public void NetworkSetUp()
        {
            if (networkPlayer == null) return;
            networkPlayer.Network_ClickOnCard += ClickOnCardInvoked;
            networkPlayer.Network_ClickOnHex += ClickOnHexInvoked;
            networkPlayer.Network_PlayCard += Network_PlayCardInvoked;
            networkPlayer.Network_PlayerTurnEnd += Network_PlayerTurnEndInvoked;
            myState = PlayerStates.WaitForStart;
        }
        /// <summary>
        /// Invoke when receive the Network ClickOnCard Event
        /// </summary>
        /// <param name="CardName">Name of used cards</param>
        private void ClickOnCardInvoked(string CardName)
        {
            if(myState == PlayerStates.Main_Phase)
            {
                PlayedCardName = CardName;
                Debug.LogFormat("Now Player {0} want to use {1}", PlayerId, PlayedCardName);
                myState = PlayerStates.Confirm_Phase;
            }
            return;
        }
        /// <summary>
        /// Invoke when receive the Network ClickOnHex Event
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnHexInvoked(int HexTileID)
        {
            if (myState == PlayerStates.Confirm_Phase)
            {
                targetHexId = HexTileID;
                Debug.LogFormat("Now Player {0} want to Play Card in Hex {1}", PlayerId, HexTileID);
                myState = PlayerStates.Spawn_Phase;
            }
            return;
        }
        /// <summary>
        /// Invoke when receive the Network PlayCard Event
        /// </summary>
        /// <param name="PlayerId">Player Index</param>
        /// <param name="CardIndex">Card Index</param>
        /// <param name="HexIndex">Hex Index</param>
        private void Network_PlayCardInvoked(int PlayerId, int CardIndex, int HexIndex)
        {
            if (PlayerId != this.PlayerId) return;
            this.PlayedCard.Invoke(PlayerId, CardIndex, HexIndex);
            myState = PlayerStates.End;
            return;
        }
        /// <summary>
        /// Invoke when receive the Network PlayerTurnEnd Event
        /// Set my state to WaitForStart
        /// </summary>
        /// <param name="PlayerId">The Player Index</param>
        private void Network_PlayerTurnEndInvoked(int PlayerId)
        {
            if (PlayerId != this.PlayerId) return;
            PlayerEnd(PlayerId);
            myState = PlayerStates.WaitForStart;
            return;
        }
        #endregion

        #region Unity Function
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public virtual void Start()
        {
            Event_PlayerTurnStart.AddListener(PlayerTurnStartInvoke);
            PlayedCard += PlayedCardInvoked;
        }

        // Update is called once per frame
        void Update()
        {
            //StateMachine Related
            switch (myState)
            {
                case (PlayerStates.Init):
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    break;
                case (PlayerStates.Main_Phase): // Wait For Network_ClickOnCard Event
                    break;
                case (PlayerStates.Confirm_Phase): // Wait For Network_ClickOnHex Event
                    break;
                case (PlayerStates.Spawn_Phase):// Wait For Network_PlayCard Event
                    break;
                case (PlayerStates.End): //Wait For Netowrk_PlayerTurnEnd Event
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Event Handle
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
        /// Invoked when the player is playing card
        /// </summary>
        /// <param name="PlayerId">The Player Id of the played card.</param>
        /// <param name="CardIndex">The played card index</param>
        /// <param name="HexIndex">The hex that card has been place on</param>
        private void PlayedCardInvoked(int PlayerId, int CardIndex, int HexIndex)
        {
            Debug.LogFormat("RemotePlayer: Player {0} playing card {1} in hex {2}", PlayerId, CardIndex, HexIndex);
            return;
        }
        #endregion
    }
}


