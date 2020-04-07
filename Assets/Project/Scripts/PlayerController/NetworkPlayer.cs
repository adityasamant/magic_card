using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Project_Network;

namespace GameLogic
{
    /// <summary>
    /// This Player is replicated from server
    /// Only deal with replicated event, Don't deal with real function
    /// </summary>
    public class NetworkPlayer : EntityEventListener<INetworkPlayer>
    {
        #region Public Reference
        /// <summary>
        /// Public Reference of the playerGameobject
        /// All real function should be call by GameObject
        /// </summary>
        [Tooltip("Public Reference of the playerGameObject")]
        public Player PlayerGameObject;
        #endregion
        #region Unity Function
        /// <summary>
        /// Just Like Monobehavior.Start()
        /// Invoked when Bolt is aware of this entity and all internal state has been setup
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
            }

            state.AddCallback("PlayerId", PlayerIdChange);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {

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
        #endregion
    }
}


