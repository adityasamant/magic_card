using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Project_Network;

namespace GameLogic
{
    /// <summary>
    /// Invoke when receive a Network ClickOnHex Event
    /// </summary>
    /// <param name="HexID">The chosen hex ID.</param>
    public delegate void Network_ClickOnHex(int HexID);
    /// <summary>
    /// Invoke when receive a Network ClickOnCard Event
    /// </summary>
    /// <param name="CardName">The chosen card name.</param>
    public delegate void Network_ClickOnCard(string CardName);
    /// <summary>
    /// Invoke when receive a Player Played Card Event
    /// </summary>
    /// <param name="PlayerId">Player Id</param>
    /// <param name="CardIndex">Card Id</param>
    /// <param name="HexIndex">HexTile Id</param>
    public delegate void Network_PlayCard(int PlayerId, int CardIndex, int HexIndex);
    /// <summary>
    /// Global Event, Send only from server
    /// </summary>
    /// <param name="PlayerId">The Active Player Id</param>
    public delegate void Network_PlayerTurnStart(int PlayerId);
    /// <summary>
    /// Invoke when receive a Network PlayerTurnEnd Event
    /// </summary>
    /// <param name="PlayerId">The Index of the comming player</param>
    public delegate void Network_PlayerTurnEnd(int PlayerId);

    /// <summary>
    /// This Player is replicated from server
    /// Only deal with replicated event, Don't deal with real function
    /// </summary>
    public class NetworkPlayer : EntityEventListener<INetworkPlayer>
    {
        #region Private Reference
        /// <summary>
        /// Private Reference of the playerGameobject
        /// All real function should be call by GameObject
        /// </summary>
        private Player PlayerGameObject;
        #endregion
        #region Event Definition
        /// <summary>
        /// Invoke when receive a Network ClickOnHex Event
        /// </summary>
        public Network_ClickOnCard Network_ClickOnCard;
        /// <summary>
        /// Invoke when receive a Network ClickOnCard Event
        /// </summary>
        public Network_ClickOnHex Network_ClickOnHex;
        /// <summary>
        /// Invoke when receive a Network PlayCard Event
        /// </summary>
        public Network_PlayCard Network_PlayCard;
        /// <summary>
        /// Invoke when receive a Network PlayerTurnEnd Event
        /// </summary>
        public Network_PlayerTurnEnd Network_PlayerTurnEnd;
        #endregion

        #region Unity Function
        /// <summary>
        /// Just Like Monobehavior.Start()
        /// Invoked when Bolt is aware of this entity and all internal state has been setup
        /// Bind the PlayerGameObject
        /// </summary>
        public override void Attached()
        {
            base.Attached();

            if (entity.IsOwner)
            {
                if(BoltNetwork.IsServer)
                {
                    state.PlayerId = 0;
                }
                else
                {
                    state.PlayerId = 1;
                }
                PlayerGameObject = GameObject.Find("HumanPlayer").GetComponent<HumanPlayer>();
                HumanPlayer human = PlayerGameObject as HumanPlayer;
                human.networkPlayer = this;
                
            }
            else
            {
                PlayerGameObject = GameObject.Find("RemotePlayer").GetComponent<RemotePlayer>();
                RemotePlayer remote = PlayerGameObject as RemotePlayer;
                remote.networkPlayer = this;
                remote.NetworkSetUp();
            }

            state.AddCallback("PlayerId", PlayerIdChange);
        }
        #endregion

        #region Event Callback
        /// <summary>
        /// Set the Real gameobject Player Id, 0 is server, 1 is client
        /// </summary>
        private void PlayerIdChange()
        {
            PlayerGameObject.PlayerId = state.PlayerId;
        }
        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(ClickOnCard evnt)
        {
            base.OnEvent(evnt);
            this.Network_ClickOnCard.Invoke(evnt.CardName);
        }
        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(ClickOnHex evnt)
        {
            base.OnEvent(evnt);
            this.Network_ClickOnHex.Invoke(evnt.HexTileID);
        }
        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(PlayCard evnt)
        {
            base.OnEvent(evnt);
            if (evnt.PlayerId != state.PlayerId) return;
            this.Network_PlayCard.Invoke(evnt.PlayerId, evnt.CardIndex, evnt.HexIndex);
        }
        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(PlayerTurnEnd evnt)
        {
            base.OnEvent(evnt);
            if (evnt.PlayerId != state.PlayerId) return;
            this.Network_PlayerTurnEnd.Invoke(evnt.PlayerId);
        }
        #endregion

        #region Event Sender
        /// <summary>
        /// Send ClickOnCard Event to other player
        /// Only Owner allow to send event
        /// </summary>
        /// <param name="CardName">The Name of Card</param>
        public void Send_ClickOnCard(string CardName)
        {
            if(entity.IsOwner)
            {
                var newEvent = ClickOnCard.Create(entity);
                newEvent.CardName = CardName;
                newEvent.Send();
            }
        }

        /// <summary>
        /// Send ClickOnHex Event to other player
        /// Only Owner allow to send event
        /// </summary>
        /// <param name="HexTileID">HexTile Index</param>
        public void Send_ClickOnHex(int HexTileID)
        {
            if(entity.IsOwner)
            {
                var newEvent = ClickOnHex.Create(entity);
                newEvent.HexTileID = HexTileID;
                newEvent.Send();
            }
        }
        #endregion
    }
}


