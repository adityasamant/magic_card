using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardInfo;
using Monsters;

namespace GameLogic
{
    /// <summary>
    /// Using Input from Network
    /// </summary>
    public class RemotePlayer : Player
    {
        #region Private Reference
        /// <summary>
        /// CardInfo Database
        /// </summary>
        [SerializeField, Tooltip("CardInfo Database")]
        private NewCardDB myCardDataBase;
        #endregion

        #region Private Variable
        /// <summary>
        /// myState record the states of the player
        /// </summary>
        private PlayerStates myState = PlayerStates.Init;
        /// <summary>
        /// The Card Player played
        /// </summary>
        private string PlayedCardName;
        ///<summary>
        /// The ID of one hex to place new monster
        ///</summary>
        private int targetHexId;

        /// <summary>
        /// The begin time of this state started.
        /// </summary>
        private float stateBeginTime;

        /// <summary>
        /// Current Control Monster
        /// </summary>
        private Monster currMonster;
        /// <summary>
        /// Attacking Target Monster
        /// </summary>
        private Monster targetMonster;
        /// <summary>
        /// Store player's monster when card selection finish
        /// </summary>
        private List<Monster> myMonsters = new List<Monster>();
        #endregion

        #region Network Function
        /// <summary>
        /// Public Reference and Interface of NetworkPlayer
        /// Network Event Receiver
        /// </summary>
        public NetworkPlayer networkPlayer = null;
        /// <summary>
        /// Binding the NetworkPlayer Delegate with local Function
        /// </summary>
        public void NetworkSetUp()
        {
            if (networkPlayer == null) return;
            networkPlayer.Network_ClickOnCard += Network_ClickOnCardInvoked;
            networkPlayer.Network_ClickOnHex += Network_ClickOnHexInvoked;
            networkPlayer.Network_PlayCard += Network_PlayCardInvoked;
            networkPlayer.Network_PlayerTurnEnd += Network_PlayerTurnEndInvoked;
            networkPlayer.Network_ClickOnMonster += Network_ClickOnMonsterInvoked;
            networkPlayer.Network_ClickOnBtn += Network_ClickOnBtnInvoked;
            networkPlayer.Network_ChangeToAction += Network_ChangeToActionInvoked;
            myState = PlayerStates.WaitForStart;
        }
        /// <summary>
        /// Invoke when receive the Network ClickOnCard Event
        /// </summary>
        /// <param name="CardName">Name of used cards</param>
        private void Network_ClickOnCardInvoked(string CardName)
        {
            if (myState == PlayerStates.Main_Phase)
            {
                PlayedCardName = CardName;
                Debug.LogFormat("Now Player {0} want to use {1}", PlayerId, PlayedCardName);
                ChangeState(PlayerStates.ConfirmSpawnPosition_Phase);
            }
            return;
        }
        /// <summary>
        /// Invoke when receive the Network ClickOnHex Event
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void Network_ClickOnHexInvoked(int HexTileID)
        {
            if (myState == PlayerStates.ConfirmSpawnPosition_Phase)
            {
                targetHexId = HexTileID;
                ChangeState(PlayerStates.Spawn_Phase);
            }
            if (myState == PlayerStates.Move_Phase)
            {
                if (currMonster.CanReach(HexTileID))
                {
                    MoveDelegate(HexTileID);
                    ChangeState(PlayerStates.Moved_Phase);
                }
            }
            return;
        }
        /// <summary>
        /// Invoke when receive the Network PlayCard Event
        /// </summary>
        /// <param name="PlayerId">Player Index</param>
        /// <param name="CardIndex">Card Index</param>
        /// <param name="HexIndex">Hex Index</param>
        private void Network_PlayCardInvoked(int PlayerId, int CardIndex, int HexIndex)
        {
            if (PlayerId != this.PlayerId) return;
            this.PlayedCard.Invoke(PlayerId, CardIndex, HexIndex);
            ChangeState(PlayerStates.End);
            return;
        }
        /// <summary>
        /// Invoke when receive the Network PlayerTurnEnd Event
        /// Set my state to WaitForStart
        /// </summary>
        /// <param name="PlayerId">The Player Index</param>
        private void Network_PlayerTurnEndInvoked(int PlayerId)
        {
            if (PlayerId != this.PlayerId) return;
            PlayerEnd(PlayerId);
            ChangeState(PlayerStates.WaitForStart);
            return;
        }

        /// <summary>
        /// Invoke when receive the Network Click On Monster Event
        /// Transform MonsterId to clickedMonster
        /// </summary>
        private void Network_ClickOnMonsterInvoked(int MonsterId)
        {
            //Transform MonsterId to Monster
            Monster clickedMonster = world.getMonsterByID(MonsterId);

            //choose an action
            if (myState == PlayerStates.Action_Phase)
            {
                currMonster = clickedMonster;
                if ((!currMonster.isIdle) && currMonster.isAlive && currMonster.monsterOwner.GetPlayerId() == PlayerId)
                {
                    ChangeState(PlayerStates.Move_Phase);
                }
            }

            //confirm your attack target
            if (myState == PlayerStates.ChooseTarget_Phase)
            {
                targetMonster = clickedMonster;
                //TODO check attack range
                if (targetMonster.isAlive && targetMonster.monsterOwner.GetPlayerId() != PlayerId)
                {
                    AttackDelegate(PlayerId, currMonster, targetMonster);
                }
                else
                {
                    return;
                }
                //Should go to next monster
                ChangeState(PlayerStates.Idle_Phase);
            }
            return;
        }

        /// <summary>
        /// Invoke when receive the Network Click On Btn Event
        /// </summary>
        private void Network_ClickOnBtnInvoked(string BtnName)
        {
            if (myState == PlayerStates.Move_Phase || myState == PlayerStates.Moved_Phase)
            {
                if (currMonster.isAlive && currMonster.monsterOwner.GetPlayerId() == PlayerId)
                {
                    switch (BtnName)
                    {
                        case ("AttackBtn"):
                            currMonster.isIdle = true;
                            ChangeState(PlayerStates.Attack_Phase);
                            break;
                        case ("SkillBtn"):
                            // InstructionUI.text = "Choose Skill Target";
                            // numOfMonsterCouldUse--;
                            // currMonster.isIdle = true;
                            // ContentUIManager.HideActionBtn();
                            break;
                        case ("IdleBtn"):
                            currMonster.isIdle = true;
                            ChangeState(PlayerStates.Idle_Phase);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Debug.Log("Current Monster doesn't match UI!");
                }
            }
            return;
        }
        /// <summary>
        /// Invoke when receive the Network ChangeToAction Event
        /// Set my state to Action Phase
        /// </summary>
        private void Network_ChangeToActionInvoked()
        {
            ChangeState(PlayerStates.Action_Phase);
        }
        #endregion

        #region Unity Function
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public virtual void Start()
        {
            Event_PlayerTurnStart.AddListener(PlayerTurnStartInvoke);
            PlayedCard += PlayedCardInvoked;
        }

        // Update is called once per frame
        void Update()
        {
            //StateMachine Related
            switch (myState)
            {
                case (PlayerStates.Init):
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    break;
                case (PlayerStates.Main_Phase): // Wait For Network_ClickOnCard Event
                    break;
                case (PlayerStates.ConfirmSpawnPosition_Phase): // Wait For Network_ClickOnHex Event
                    break;
                case (PlayerStates.Spawn_Phase):// Wait For Network_PlayCard Event
                    break;
                case (PlayerStates.Action_Phase):// Wait For Select Monster
                    break;
                case (PlayerStates.Move_Phase): //Wait For Move
                    break;
                case (PlayerStates.Moved_Phase): //Wait for Other Btn Function(Attack,Skill,Idle)
                    break;
                case (PlayerStates.Attack_Phase):
                    ChangeState(PlayerStates.ChooseTarget_Phase);
                    break;
                case (PlayerStates.ChooseTarget_Phase):
                    break;
                case (PlayerStates.End): //Wait For Netowrk_PlayerTurnEnd Event
                    break;
                case (PlayerStates.Idle_Phase):
                    ChangeState(PlayerStates.Main_Phase);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Event Handle
        /// <summary>
        /// Call this function when Event_PlayerTurnStart invoke
        /// Change User state from WaitForStart to Main_Phase
        /// </summary>
        void PlayerTurnStartInvoke()
        {
            if (myState == PlayerStates.WaitForStart)
            {
                myMonsters.Clear();
                foreach (KeyValuePair<int, Monster> monsterPair in world.monsters)
                {
                    Monster thisMonster = monsterPair.Value;
                    if (thisMonster.monsterOwner.GetPlayerId() == PlayerId)
                    {
                        myMonsters.Add(thisMonster);
                    }
                }
                foreach (Monster m in myMonsters)
                {
                    m.isIdle = false;
                }
                ChangeState(PlayerStates.Main_Phase);
            }
        }
        /// <summary>
        /// Invoked when the player is playing card
        /// </summary>
        /// <param name="PlayerId">The Player Id of the played card.</param>
        /// <param name="CardIndex">The played card index</param>
        /// <param name="HexIndex">The hex that card has been place on</param>
        private void PlayedCardInvoked(int PlayerId, int CardIndex, int HexIndex)
        {
            Debug.LogFormat("RemotePlayer: Player {0} playing card {1} in hex {2}", PlayerId, CardIndex, HexIndex);
            return;
        }
        #endregion

        #region Private Function
        /// <summary>
        /// Change Player State
        /// </summary>
        /// <param name="dstStates">Target State</param>
        private void ChangeState(PlayerStates dstStates)
        {
            //Debug.LogFormat("PlayerStates: "+dstStates);
            myState = dstStates;
            stateBeginTime = Time.time;
        }
        #endregion
    }
}


