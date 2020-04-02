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

        #region Unity Function
        private void Start()
        {
            DisplayCard();
            GameObject.Find("HeadposeCanvas").SetActive(false);
            //HideCardUI();
        }
        #endregion

        #region Public Function
        /// <summary>
        /// Hide Card UI.
        /// </summary>
        /// <param name=""></param>
        public void HideCardUI()
        {
            CanvasGroup cg = GameObject.Find("CardCanvas").GetComponent<CanvasGroup>();
            cg.alpha = 0f; //this makes everything transparent
            cg.blocksRaycasts = false; //this prevents the UI element to receive input events
            cg.interactable= false;
            return;
        }
        /// <summary>
        /// Show Card UI.
        /// </summary>
        /// <param name=""></param>
        public void ShowCardUI()
        {
            CanvasGroup cg = GameObject.Find("CardCanvas").GetComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable= true;
            return;
        }
        #endregion

        #region Private Function
        private void DisplayCard()
        {
            // space number is cards.length - 1
            // the while length of the display board is the lastPosition.x - firstPositon.x
            float space = (lastTrans.position.x - firstTrans.position.x) / (cards.Length - 1);

            for (int i = 0; i < cards.Length; i++)
            {
                // Display "Card UI Object" with the same distance
                GameObject newCard = Instantiate(cards[i], new Vector2(firstTrans.position.x + space * i, firstTrans.position.y), Quaternion.identity);
                //newCard.transform.rotation = Quaternion.Euler(90, 0, 0);
                // Card newCard1 = new Card();
                // newCard1 = myDecks.GetCard(cards[i].name);

                //attch to worldCanvas as the parent each GameObject [card]
                newCard.transform.SetParent(worldCanvasTrans);
            }
        }
        #endregion


    }
}
