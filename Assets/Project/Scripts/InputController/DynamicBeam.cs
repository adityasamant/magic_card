using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace InputController
{
    public class DynamicBeam : MonoBehaviour
    {
        #region Public Variables
        public GameObject controller;
        public Vector3 hitPoint;
        #endregion

        #region Private Variables
        [SerializeField, Tooltip("The LineRenderer to show the line from the input to the hit point.")]
        private LineRenderer beamLine;
        #endregion

        #region Unity Methods
        // Update is called once per frame
        void Update()
        {
            transform.position = controller.transform.position;
            transform.rotation = controller.transform.rotation;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                beamLine.useWorldSpace = true;
                beamLine.SetPosition(0, controller.transform.position);
                beamLine.SetPosition(1, hit.point);
                hitPoint = hit.point;
            }

        }
        #endregion
    }
}
