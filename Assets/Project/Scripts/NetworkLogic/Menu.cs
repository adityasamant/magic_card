using System;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

namespace Project_Network
{
    /// <summary>
    /// Host/Join Menu
    /// </summary>
    public class Menu : Bolt.GlobalEventListener
    {
        #region Public Reference
        public HostJoinSwitcher hostJoinSwitcher;
        #endregion

        #region Public Function
        /// <summary>
        /// To Start as a server
        /// </summary>
        public void StartServer()
        {
            BoltLauncher.StartServer();
        }

        /// <summary>
        /// To Start as a client
        /// </summary>
        public void StartClient()
        {
            BoltLauncher.StartClient();
        }

        /// <summary>
        /// Shut Down
        /// </summary>
        public void ClientShutDown()
        {
            BoltLauncher.Shutdown();
        }
        #endregion

        #region Override Global Event Handle
        /// <summary>
        /// When Bolt (client/server) start
        /// </summary>
        public override void BoltStartDone()
        {
            base.BoltStartDone();
            if (BoltNetwork.IsServer)
            {
                string matchName = "Test Match";
                BoltNetwork.SetServerInfo(matchName, null);
                BoltNetwork.LoadScene("TestMap");
            }
        }

        /// <summary>
        /// When Bolt Client receive the session List
        /// </summary>
        /// <param name="sessionList">The sessions it can reach</param>
        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            base.SessionListUpdated(sessionList);
            foreach (var itr in sessionList)
            {
                UdpSession udpSession = itr.Value as UdpSession;

                if (udpSession.Source == UdpSessionSource.Photon)
                {
                    hostJoinSwitcher.isJoined = true;
                    BoltNetwork.Connect(udpSession);
                }
            }
        }
        #endregion
    }
}

