/// This C# Code is from http://www.theappguruz.com/blog/roll-a-dice-unity-3d website

using UnityEngine;
using System.Collections;

namespace DicePackage
{
    /// <summary>
    /// Pop a delegate when Dice stop rolling
    /// </summary>
    public delegate void Dice_stop();

    /// <summary>
    /// Dice is a class that can create a dice, roll a dice and return the dice value
    /// Ref: http://www.theappguruz.com/blog/roll-a-dice-unity-3d
    /// </summary>
    public class Dice : MonoBehaviour
    {
        #region Private Variable
        /// <summary>
        /// Store the Dice Count
        /// </summary>
        private int diceCount;

        /// <summary>
        /// Store the initial Position
        /// </summary>
        private Vector3 initPos;

        ///<summary>
        ///Store the start rolling time
        ///</summary>
        private float startRollingTime;

        ///<summary>
        ///A boolean value to check if the dice is still rolling
        ///</summary>
        private bool bStartRolling;
        #endregion

        #region Public Property
        /// <summary>
        /// Store the maximum time that allow the dice to roll.
        /// </summary>
        [Tooltip("Store the maximum time that allow the dice to roll.")]
        public float MaxRollingTime = 5.0f;

        /// <summary>
        /// The Delegate Handle
        /// </summary>
        public Dice_stop Dice_stop;
        #endregion

        #region Unity Function
        /// <summary>
        /// Invoke when the dice been create
        /// </summary>
        void Start()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.solverIterations = 250;
            //rb.constraints = RigidbodyConstraints.FreezeAll;
            initPos = transform.position;
            Vector3 lastPos = new Vector3(Random.value, Random.value, Random.value);
            Dice_stop += regularDiceCount;
        }

        /// <summary>
        /// Invoke when the dice been enable
        /// </summary>
        void OnEnable()
        {
            initPos = transform.position;
        }

        ///<summary>
        /// Invoke every frame
        ///</summary>
        private void Update()
        {
            float nowTime = Time.time;
            if(bStartRolling && nowTime-startRollingTime>MaxRollingTime)
            {
                bStartRolling = false;
                Dice_stop();
            }
            return;
        }
        #endregion

        #region Public Function
        /// <summary>
        /// Return a value that indicated the dice count
        /// </summary>
        /// <returns>
        /// The upside of the dice.
        /// </returns>
        public int GetDiceCount()
        {
            diceCount = 0;
            regularDiceCount();
            return diceCount;
        }

        public void ResetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
            initPos = newPosition;
            bStartRolling = true;
            startRollingTime = Time.time;
            Rigidbody rb = GetComponent<Rigidbody>();
            //rb.constraints = RigidbodyConstraints.None;
            Vector3 lastPos = new Vector3(Random.value, Random.value, Random.value);
            addForce(lastPos);
        }
        #endregion

        #region Private Function
        /// <summary>
        /// Get the Dice Upface Count, Set diceCount
        /// </summary>
        private void regularDiceCount()
        {
            if (Vector3.Dot(transform.forward, Vector3.up) > 0.6f)
                diceCount = 5;
            if (Vector3.Dot(-transform.forward, Vector3.up) > 0.6f)
                diceCount = 2;
            if (Vector3.Dot(transform.up, Vector3.up) > 0.6f)
                diceCount = 3;
            if (Vector3.Dot(-transform.up, Vector3.up) > 0.6f)
                diceCount = 4;
            if (Vector3.Dot(transform.right, Vector3.up) > 0.6f)
                diceCount = 6;
            if (Vector3.Dot(-transform.right, Vector3.up) > 0.6f)
                diceCount = 1;
        }

        /// <summary>
        /// Create a Force to roll the dice
        /// </summary>
        /// <param name="lastPos">
        /// A random Vector3 to randomize the force
        /// </param>
        void addForce(Vector3 lastPos)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddTorque(Vector3.Cross(lastPos, initPos) * 1000, ForceMode.Impulse);
            lastPos.y += 12;
            rb.AddForce(((lastPos - initPos).normalized) * (Vector3.Distance(lastPos, initPos)) * 30 * rb.mass);
        }
        #endregion
    }
}
