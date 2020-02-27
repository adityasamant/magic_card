using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monsters
{

    public interface MonsterFunctions
    {
        void Spawn(World world);
        void DecideInOneTurn(World world);
        void Killed(World world);
    }

    public class Monster : MonsterFunctions
    {
        /// <summary>
        /// the total number of all monster in the game
        /// </summary>
        static private int monsterCount = 0;

        /// <summary>
        /// the unique id of this monster 
        /// </summary>
        private int uid;

        /// <summary>
        /// All attributes a monster have
        /// </summary>
        public int hp;
        public int atk;
        public int def;
        public int spd;
        public string name;
        public bool alive;
        
        /// <summary>
        /// Initialization of a monster.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hp"></param>
        /// <param name="atk"></param>
        /// <param name="def"></param>
        /// <param name="spd"></param>
        public Monster(string name="default", int hp=100, int atk=10, int def=5, int spd=1)
        {
            Debug.Log("Created a monster named: "+name);
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.spd = spd;
            this.name = name;
            this.alive = true;
            this.uid = Monster.monsterCount;
            Monster.monsterCount++;
        }

        /// <summary>
        /// get the monster's uid
        /// </summary>
        public int getUId() { return uid; }

        /// <summary>
        ///  monster's movement when it's spawned in game.
        ///  default setting is do nothing.
        /// </summary>
        /// <param name="world"></param>
        public void Spawn(World world)
        {
            return;
        }
        /// <summary>
        /// Monster's movement when it's been killed.
        ///  default setting is do nothing.
        /// </summary>
        /// <param name="world"></param>
        public void Killed(World world)
        {
            return;
        }
        /// <summary>
        /// Monster's movement in one turn.
        /// </summary>
        /// <param name="world"></param>
        public void DecideInOneTurn(World world)
        {
            if (this.alive == false)
                return;
            else
                Debug.Log("I'm a monster and I don't want to do anything!");
            // need inplemented
        }
    } 
}
