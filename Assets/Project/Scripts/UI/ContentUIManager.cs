using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ContentUIManager : MonoBehaviour
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
        /// Is action displayed on UICanvas or not
        /// </summary>
        public bool ActionIsDisplayed;
        /// <summary>
        /// UI Canvas
        /// </summary>
        public GameObject UICanvas;
        /// <summary>
        /// Instruction UI
        /// </summary>
        public Text InstructionUI;
        /// <summary>
        /// Content UI
        /// </summary>
        public GameObject ContentUI;

        public GameObject ActionBtnGroup;
        public GameObject AttackBtnPrefab;
        public GameObject SkillBtnPrefab;
        public GameObject IdleBtnPrefab;
        public int numOfPortal;
        public int numOfMist;
        public int numOfTor;
        public int numOfFire;
        #endregion

        #region Private Variable
        #endregion

        #region Unity Function
        private void Start()
        {
            CardIsDisplayed = false;
            ActionIsDisplayed = false;
            firstTrans = new Vector3(-0.25f, 0, 0);
            lastTrans = new Vector3(0.25f, 0, 0);
            HideUICanvas();
            HideActionBtn();
            numOfPortal = 0;
            numOfMist = 0;
            numOfTor = 0;
            numOfFire = 0;
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
        /// Show Action btn.
        /// </summary>
        /// <param name=""></param>
        public void ShowActionBtn()
        {
            if (!ActionBtnGroup.activeSelf)
            {
                ActionBtnGroup.SetActive(true);
            }
            ActionIsDisplayed = true;
            return;
        }

        /// <summary>
        /// Hide Action btn.
        /// </summary>
        /// <param name=""></param>
        public void HideActionBtn()
        {
            if (ActionBtnGroup.activeSelf)
            {
                ActionBtnGroup.SetActive(false);
            }
            ActionIsDisplayed = false;
            return;
        }

        /// <summary>
        /// Clear everything on ContentUI.
        /// </summary>
        /// <param name=""></param>
        public void ClearContentUI()
        {
            for (int i = 0; i < ContentUI.transform.childCount; i++)
            {
                if (i == 0) continue;
                GameObject.Destroy(ContentUI.transform.GetChild(i).gameObject);
            }
            // foreach (Transform child in ContentUI.transform)
            // {
            //     GameObject.Destroy(child.gameObject);
            // }
            HideActionBtn();
            CardIsDisplayed = false;
            ActionIsDisplayed = false;
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
            ActionIsDisplayed = false;
        }

        /// <summary>
        /// Display Cards on UICanvas.
        /// </summary>
        /// <param name=""></param>
        public void DisplayCard()
        {
            if (CardIsDisplayed)
                return;
            ClearContentUI();
            // space number is cards num on deck - 1
            // the length of the display board is the lastPosition.x - firstPositon.x
            int numOfCardsOnDeck = 2;
            float space = (lastTrans.x - firstTrans.x) / (numOfCardsOnDeck - 1);

            for (int i = 0; i < numOfCardsOnDeck; i++)
            {
                GameObject newCard;
                if (i == 0)
                {
                    if (numOfPortal < 2)
                    {
                        newCard = Instantiate(cards[7]);
                        numOfPortal++;
                    }else if(numOfFire < 1){
                        newCard = Instantiate(cards[6]);
                        numOfFire++;
                    }else if(numOfMist < 1){
                        newCard = Instantiate(cards[9]);
                        numOfMist++;
                    }else if(numOfTor < 1){
                        newCard = Instantiate(cards[8]);
                        numOfTor++;
                    }else{
                        newCard = Instantiate(cards[Random.Range(0, 6)]);
                    }
                }
                else
                {
                    // Get one card from card deck randomly
                    newCard = Instantiate(cards[Random.Range(0, 6)]);
                }

                //attch each card to ContentUI as the parent 
                newCard.transform.SetParent(ContentUI.transform);
                newCard.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
                newCard.transform.localPosition = new Vector3(firstTrans.x + space * i, firstTrans.y, 0);
                newCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            CardIsDisplayed = true;
        }

        /// <summary>
        /// Display Action Button.
        /// </summary>
        /// <param name=""></param>
        public void DisplayAction()
        {
            if (ActionIsDisplayed)
                return;
            ActionBtnGroup.SetActive(true);
            ActionIsDisplayed = true;
        }
        #endregion

        #region Private Function
        #endregion


    }
}