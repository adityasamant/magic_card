using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardUIManager : MonoBehaviour
    {
        #region Public Property
        /// <summary>
        /// First Card Position
        /// </summary>
        public Vector3 firstTrans;
        /// <summary>
        /// Last Card Position
        /// </summary>
        public Vector3 lastTrans;
        /// <summary>
        /// All Card prefabs (Deck)
        /// </summary>
        public GameObject[] cards;
        /// <summary>
        /// Container Canvas
        /// </summary>
        public Transform worldCanvasTrans;
        /// <summary>
        /// Is card displayed on UICanvas or not
        /// </summary>
        public bool CardIsDisplayed;
        /// <summary>
        /// UI Canvas
        /// </summary>
        public GameObject UICanvas;
        /// <summary>
        /// Instruction UI
        /// </summary>
        public Text InstructionUI;
        /// <summary>
        /// Card UI
        /// </summary>
        public GameObject CardUI;
        #endregion

        #region Private Variable
        #endregion

        #region Unity Function
        private void Start()
        {
            CardIsDisplayed = false;
            firstTrans = new Vector3(-0.25f, 0, 0);
            lastTrans = new Vector3(0.25f, 0, 0);
            HideUICanvas();
        }
        #endregion

        #region Public Function
        /// <summary>
        /// Hide UI.
        /// </summary>
        /// <param name=""></param>
        public void HideUICanvas()
        {
            if (UICanvas.activeSelf)
            {
                UICanvas.SetActive(false);
            }
            return;
        }

        /// <summary>
        /// Show UI.
        /// </summary>
        /// <param name=""></param>
        public void ShowUICanvas()
        {
            if (!UICanvas.activeSelf)
            {
                UICanvas.SetActive(true);
            }
            return;
        }

        /// <summary>
        /// Clear everything on CardUI.
        /// </summary>
        /// <param name=""></param>
        public void ClearCardUI()
        {
            foreach (Transform child in CardUI.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            CardIsDisplayed = false;
        }

        /// <summary>
        /// Clear everything on InstructionUI.
        /// </summary>
        /// <param name=""></param>
        public void ClearInstructionUI()
        {
            InstructionUI.text = "";
        }

        /// <summary>
        /// Clear everything on UICanvas.
        /// </summary>
        /// <param name=""></param>
        public void ClearUICanvas()
        {
            foreach (Transform child in UICanvas.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            CardIsDisplayed = false;
        }

        /// <summary>
        /// Display Cards on UICanvas.
        /// </summary>
        /// <param name=""></param>
        public void DisplayCard()
        {
            if (CardIsDisplayed)
                return;
            ClearCardUI();
            // space number is cards num on deck - 1
            // the length of the display board is the lastPosition.x - firstPositon.x
            int numOfCardsOnDeck = 2;
            float space = (lastTrans.x - firstTrans.x) / (numOfCardsOnDeck - 1);

            for (int i = 0; i < numOfCardsOnDeck; i++)
            {
                // Get one card from card deck randomly
                GameObject newCard = Instantiate(cards[Random.Range(0, 2)]);

                //attch each card to CardUI as the parent 
                newCard.transform.SetParent(CardUI.transform);
                newCard.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
                newCard.transform.localPosition = new Vector3(firstTrans.x + space * i, firstTrans.y, 0);
                newCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            CardIsDisplayed = true;
        }
        #endregion

        #region Private Function
        #endregion


    }
}
