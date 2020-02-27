using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monsters;

public class World
{
    /// <summary>
    /// data structure of all monsters
    /// </summary>
    private Dictionary<int, Monster> monsters;

    /// <summary>
    /// get monster instance by its uid
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    /// return the monster if it exists, otherwise return null 
    public Monster getMonsterByID(int uid)
    {
        if (monsters.ContainsKey(uid) == false)
        {
            Debug.LogErrorFormat("Error! No monster with id %d", uid);
            return null;
        }
        else
            return monsters[uid];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="monster"></param>
    /// <returns></returns>
    public int uploadMonsterInWorld(Monster monster)
    {
        int uid = monster.getUId();
        if(monsters.ContainsKey(uid) == true)
        {
            Debug.LogErrorFormat("Error! Monster with uid %d already exists", uid);
            return -1;
        }
        monsters[uid] = monster;
        return uid;
    }

    // Start is called before the first frame update
    void Start()
    {
        monsters.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
