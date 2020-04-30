using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using GameWorld;

namespace Monsters
{
    public class MonsterTestManager : MonoBehaviour
    {
                /// <summary>
        /// The link to the game world
        /// </summary>
        [Tooltip("A link to the game world.")]
        public World world;
        public Monster monster0;
        public Maria_Brute monster_brute;
        public Jolleen_Knight monster_jolleen;
        public Erica_Surviver monster_erica;
        MLInputController _controller;

        // Start is called before the first frame update
        void Start()
        {
            //monster0.MonsterReset();            
            //Start receiving input by the Control
            MLInput.Start();
            MLInput.OnControllerButtonDown += OnButtonDown;
            _controller = MLInput.GetController(MLInput.Hand.Left);
        }

        // Update is called once per frame
        void Update()
        {
            CheckTrigger();
        }

        void CheckTrigger() 
        {
            if (_controller.TriggerValue > 0.2f) {
                monster_jolleen.Attack(monster_brute,4);
            } else {
                
            }
        }

        void OnButtonDown(byte controller_id, MLInputControllerButton button)
        {
            if (button == MLInputControllerButton.Bumper)
            {
                
                //monster_erica.Move(181);
                //monster_erica.Move(180);
               //monster_erica.Move(179);
                //monster_erica.DoubleShot(monster_brute, 9);
                
                monster_jolleen.Attack(monster_erica,2);
            }

            if (button == MLInputControllerButton.HomeTap)
            {
                //monster_jolleen.Attack(monster0, 4);
                //monster_jolleen.Attack(monster_erica, 4);
                //monster0.Attack(monster_jolleen, 4);
                //monster_erica.Attack(monster_brute, 4);
                // int dest = 92;
                List<int> path = new List<int>();
                // List<int> path2 = new List<int>();
                // path2 = world.tileMap.getShortestPath(182, dest);

                monster_brute.Attack(monster_jolleen,2);
                // path.Add(180);
                // path.Add(179);
                // path.Add(178);
                // path.Add(165);
                // monster_erica.Move(path);
            }
        }
    }
}
