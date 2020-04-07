using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bolt;

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
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {

        }
        
        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {

        }
        #endregion
    }
}


