using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Network
{
    /// <summary>
    /// 1. Game start, start as client automately, wait 5 seconds(timer)
    /// 2. If it can find the server, join the server and close the menu, switch to game scene
    /// 3. If the timer passed, shut down the client
    /// 4. Start the server and switch to game scene
    /// </summary>
    public class HostJoinSwitcher : MonoBehaviour
    {
        #region Public Variable
        /// <summary>
        /// The total waiting time as a client
        /// </summary>
        public float WaitingTime = 5.0f;
        /// <summary>
        /// A Public Reference to the Bolt Menu
        /// </summary>
        public Menu menu;
        /// <summary>
        /// Init as false, when the client join a session change to true
        /// </summary>
        public bool isJoined;
        #endregion

        #region Private Variable
        /// <summary>
        /// A Timer to countdown the client waiting time
        /// When the timer is over, shutdown the client and start as a server
        /// </summary>
        private float ClientTimer;
        /// <summary>
        /// Init as true, when client shutdown set to true
        /// </summary>
        private bool isClient;
        #endregion

        #region Unity Function
        /// <summary>
        /// Invoke when game start
        /// </summary>
        private void Start()
        {
            isClient = true;
            isJoined = false;
            if(menu!=null)
            {
                ClientTimer = Time.time;
                menu.StartClient();
            }
        }
        /// <summary>
        /// Invoke every frame
        /// </summary>
        private void Update()
        {
            if(isClient && !isJoined)
            {
                if(Time.time-ClientTimer>WaitingTime)
                {
                    menu.ClientShutDown();
                    isClient = false;
                    menu.StartServer();
                }
            }
        }
        #endregion
    }
}

