using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using HighlightingSystem;

namespace InputController
{
    public class DynamicBeam : MonoBehaviour
    {
        #region Public Variables
        public GameObject controller;
        public Vector3 hitPoint;
        // Hover color
        public Color hoverColor = Color.red;

        public Highlighter highlighter;
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

            if (true || Physics.Raycast(transform.position, transform.forward, out hit))
            {
                beamLine.useWorldSpace = true;
                beamLine.SetPosition(0, controller.transform.position);
                beamLine.SetPosition(1, transform.position + transform.forward * 5);
                if(Physics.Raycast(transform.position, transform.forward, out hit)){
                    hitPoint = hit.point;
                    OnHover(hit);
                }
            }

        }
        #endregion

        #region Public Methods
        // RaycastController should trigger this method via onHover event
        public void OnHover(RaycastHit hitInfo)
        {
            Transform tr = hitInfo.collider.transform;
            if (tr == null) { return; }

            highlighter = tr.GetComponentInParent<Highlighter>();
            if (highlighter == null) { return; }

            // Hover
            highlighter.Hover(hoverColor);

            // Switch tween
            // if (Input.GetButtonDown(buttonFire1))
            // {
            // 	highlighter.tween = !highlighter.tween;
            // }

            // Toggle overlay
            // if (Input.GetButtonUp(buttonFire2))
            // {
            // 	highlighter.overlay = !highlighter.overlay;
            // }
        }
        #endregion
    }
}
