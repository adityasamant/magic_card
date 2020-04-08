using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class CardUIManager : MonoBehaviour
    {
        #region Public Property
        /// <summary>
        /// Reference of CardInfo Reader
        /// </summary>
        [Tooltip("Reference of CardInfo Reader")]
        //public CardDB myDecks;
        /// <summary>
        /// First Card Position
        /// </summary>
        public Transform firstTrans;
        /// <summary>
        /// Last Card Position
        /// </summary>
        public Transform lastTrans;
        /// <summary>
        /// All Card prefabs (Deck)
        /// </summary>
        public GameObject[] cards;
        /// <summary>
        /// Container Canvas
        /// </summary>
        public Transform worldCanvasTrans;
        #endregion

        #region Public Variable
        /// <summary>
        /// Card UI Canvas
        /// </summary>
        private GameObject CardUICanvas;
        #endregion

        #region Unity Function
        private void Start()
        {
            DisplayCard();
            CardUICanvas = GameObject.Find("CardCanvas");
            HideCardUI();
        }
        #endregion

        #region Public Function
        /// <summary>
        /// Hide Card UI.
        /// </summary>
        /// <param name=""></param>
        public void HideCardUI()
        {
            // CanvasGroup cg = GameObject.Find("CardCanvas").GetComponent<CanvasGroup>();
            // cg.alpha = 0f; //this makes everything transparent
            // cg.blocksRaycasts = true; //this prevents the UI element to receive input events
            // cg.interactable= false;
            CardUICanvas.SetActive(false);
            return;
        }
        /// <summary>
        /// Show Card UI.
        /// </summary>
        /// <param name=""></param>
        public void ShowCardUI()
        {
            // CanvasGroup cg = GameObject.Find("CardCanvas").GetComponent<CanvasGroup>();
            // cg.alpha = 1f;
            // cg.blocksRaycasts = false;
            // cg.interactable= true;
            CardUICanvas.SetActive(true);
            return;
        }
        #endregion

        #region Private Function
        private void DisplayCard()
        {
            // space number is cards num on deck - 1
            // the while length of the display board is the lastPosition.x - firstPositon.x
            int numOfCardsOnDeck = 2;
            float space = (lastTrans.position.x - firstTrans.position.x) / (numOfCardsOnDeck - 1);

            for (int i = 0; i < numOfCardsOnDeck; i++)
            {
                // Display "Card UI Object" with the same distance
                GameObject newCard = Instantiate(cards[Random.Range(0,6)], new Vector2(firstTrans.position.x + space * i, firstTrans.position.y), Quaternion.identity);

                //attch to worldCanvas as the parent each GameObject [card]
                newCard.transform.SetParent(worldCanvasTrans);
            }
        }
        #endregion


    }
}
