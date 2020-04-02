using System;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public class Menu : Bolt.GlobalEventListener
{
    public void StartServer()
    {
        BoltLauncher.StartServer();
    }

    public void StartClient()
    {
        BoltLauncher.StartClient();
    }

    public override void BoltStartDone()
    {
        base.BoltStartDone();
        if(BoltNetwork.IsServer)
        {
            string matchName = "Test Match";
            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene("TestMap");
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        base.SessionListUpdated(sessionList);
        foreach(var itr in sessionList)
        {
            UdpSession udpSession = itr.Value as UdpSession;

            if(udpSession.Source==UdpSessionSource.Photon)
            {
                BoltNetwork.Connect(udpSession);
            }
        }
    }
}
