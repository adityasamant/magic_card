using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monsters;

public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// All monsters of this player on board.
    /// </summary>
    public HashSet<int> monsterIds;
    public World world;
    public bool isServer;
    // Start is called before the first frame update
    void Start()
    {
        monsterIds.Clear();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
