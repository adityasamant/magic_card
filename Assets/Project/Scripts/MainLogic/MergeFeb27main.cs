using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;

public class MergeFeb27main : MonoBehaviour
{
    #region Public Variable
    /// <summary>
    /// Store a Pointer to ScanMesh to capture the ScanFinished event
    /// </summary>
    public meshCode ScanMesh;

    /// <summary>
    /// Store a Pointer to Player1 to send playerstart event and scanfinished event
    /// </summary>
    public PlayerFSM Player1;
    #endregion

    #region Private Variable
    /// <summary>
    /// Store the time that get ScanFinished event
    /// </summary>
    private float Player1WaitTime;

    /// <summary>
    /// Store the time that get ScanFinished event
    /// </summary>
    private bool Player1Start = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ScanMesh.ScanFinished += ScanFinshedUpdate;
        Player1.PlayerEnd += PlayerEndUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player1Start == false)
        {
            float nowTime = Time.time;
            if (nowTime - Player1WaitTime > 5)
            {
                Debug.Log("Event Finished");
                Player1.Event_PlayerTurnStart.Invoke();
                Player1Start = true;
            }
        }
    }

    void ScanFinshedUpdate()
    {
        Debug.Log("ScanFinished");
        Player1.Event_ScanFinished.Invoke();
        Player1WaitTime = Time.time;
        Player1Start = false;
        return;
    }

    void PlayerEndUpdate(int PlayerId)
    {
        if (PlayerId == 1)
        {
            Player1WaitTime = Time.time;
            Player1Start = false;
            return;
        }
    }
}
