using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace CardInfo
{
    /// <summary>
    /// Cards is a class to store data of a card
    /// </summary>
    public class Cards
    {
        public int id;
        public string CardName;
        public int Attack;
        public int HP;
        public int Speed;
        public string SpecialEffect;
    }

    /// <summary>
    /// GetCardInsturction read "CardData.csv" when game started
    /// Using GetCard(CardName) return a Cards
    /// </summary>
    public class GetCardInstruction : MonoBehaviour
    {
        /// <summary>
        /// 2D Array to storage Card Infomation
        /// </summary>
        string[][] CardInfo;
        string[] keyName;

        #region Private Variable
        /// <summary>
        /// File Path for data 
        /// </summary>
        private string filePath ="Assets/Project/Data/CardData.csv";


        #endregion

        #region Unity Function
        // Start is called before the first frame update
        /// <summary>
        /// Init The CardInfo and keyName
        /// </summary>
        void Start()
        {
            string[] fileData = File.ReadAllLines(filePath);
            keyName = fileData[0].Split(',');

            CardInfo = new string[fileData.Length - 1][];

            for(int i=1;i<fileData.Length;i++)
            {
                string[] tempArr = fileData[i].Split(',');
                CardInfo[i - 1] = new string[tempArr.Length];
                for(int j=0;j<tempArr.Length;j++)
                {
                    CardInfo[i - 1][j] = tempArr[j];
                }
            }
        }
        #endregion

        #region Public Function

        public Cards GetCard(string CardName)
        {
            Cards newCard = new Cards();
            for(int i=0;i<CardInfo.Length;i++)
            {
                if(CardInfo[i][1]==CardName)
                {
                    newCard.id = int.Parse(CardInfo[i][0]);
                    newCard.CardName = CardInfo[i][1];
                    newCard.Attack = int.Parse(CardInfo[i][2]);
                    newCard.HP = int.Parse(CardInfo[i][3]);
                    newCard.Speed = int.Parse(CardInfo[i][4]);
                    newCard.SpecialEffect = CardInfo[i][5];
                }
            }
            return newCard;
        }
        #endregion

    }
}