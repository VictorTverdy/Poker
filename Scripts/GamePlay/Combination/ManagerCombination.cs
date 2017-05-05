using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cards;
using Hexagonal;
namespace Combination
{
    public class ManagerCombination : MonoBehaviour
    {
        public static ManagerCombination Instance;

        private HexagonalCell lastCellConnect;
        private List<CombinationData> allCombinations = new List<CombinationData>();
        private HudUI score;
        private float comboBonus = .05f;
        private int pointFromDestroy;
        private int[] collectedCombo = new int[9];

        // Use this for initialization
        void Start()
        {
            if (Instance == null)
                Instance = this;
            score = FindObjectOfType<HudUI>();          
            StartCoroutine("FlashCombinationAlternately");
        }  

        public void AutoCombinationCheck(HexagonalCell _card)
        {
            for (int i = 0; i < allCombinations.Count; i++)
            {
                if (allCombinations[i].currentCombination == _card.GetCurrentCombination())
                { 
                    int lenght = allCombinations[i].cards.Length;
                    for (int y = 0; y < lenght; y++)
                    {
                        if (allCombinations[i].cards[y].Info.myX == _card.Info.myX && allCombinations[i].cards[y].Info.myY == _card.Info.myY)
                        {
                            AutoAddComboCombination(allCombinations[i]);
                            return;
                        }
                    }
                }
            }
        }

        public int GetLenghtAllCombination()
        {
            return allCombinations.Count; 
        }

        public void CheckCombinationCell() 
        { 
            for (int y = 0; y < GlobalInfo.column; y++)
            {
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    HexagonalCell _cell = HexagonalGrid.Instance.GetCell(x, y);
                    if (_cell != null && _cell.GetIsFull())
                    {
                        CheckCardsIn5Cells(x, y);
                    }
                }
            }
        } 
        
        public void AddScoreFromDestroy()
        {         
            pointFromDestroy += GlobalInfo.pointFromDestroy;          
        }    

        public void ClearPointDestroy()
        {
            if (pointFromDestroy > 0)
            {
                int addPoint = (pointFromDestroy * 2) + GlobalInfo.pointStartDestroy;
                score.AddScore(addPoint);
                pointFromDestroy = 0;
            }
        }

        public void ClearMatchCells()
        {          
            HexagonalGrid.Instance.DetectFallingCell();
            ClearPointDestroy();
            allCombinations.Clear(); 
            HexagonalGrid.Instance.NoFlashingAllCells(); 
            CheckCombinationCell();          
        }

        public void SetLastConnect(HexagonalCell last)
        {
            lastCellConnect = last;
            if (lastCellConnect == null)
                return;
            if (allCombinations.Count >= 1)
            { 
                for (int _combo = 0; _combo < allCombinations.Count; _combo++)
                {
                    if (allCombinations[_combo].currentCombination == last.GetCurrentCombination())
                    {
                        int lenght = allCombinations[_combo].cards.Length;
                        for (int _card = 0; _card < lenght; _card++)
                        {
                            if (allCombinations[_combo].cards[_card].Info.myX == lastCellConnect.Info.myX &&
                                allCombinations[_combo].cards[_card].Info.myY == lastCellConnect.Info.myY)
                            {
                                var _swap = allCombinations[_combo];
                                allCombinations[_combo] = allCombinations[0];
                                allCombinations[0] = _swap;
                                StopCoroutine("FlashCombinationAlternately");
                                StartCoroutine("FlashCombinationAlternately");
                            }
                        }
                    }
                }              
            }
        }

        private void FlashingCoroutineInCell(CombinationData _combination)
        {
            int lengCard = _combination.cards.Length;
            for (int y = 0; y < lengCard; y++)
            {
                eCombination _comb = _combination.currentCombination;
                HexagonalCell _cell = HexagonalGrid.Instance.GetCell(_combination.cards[y].Info.myX, _combination.cards[y].Info.myY);
                _cell.FlashingCoroutine(_comb);
            }
        }

        private IEnumerator FlashCombinationAlternately() 
        {
            for (int i = 0; ; i++)
            {
                int allCombinationsLenght = allCombinations.Count;
                if (allCombinationsLenght > 0)
                {
                    FlashingCoroutineInCell(allCombinations[i]);                
                }
                yield return new WaitForSeconds(.5f);
                if (i >= allCombinations.Count - 1)
                {
                    if (allCombinationsLenght == 1)
                        yield return new WaitForSeconds(3f);
                    if (allCombinationsLenght == 2)
                        yield return new WaitForSeconds(2f);
                    if (allCombinationsLenght == 3)
                        yield return new WaitForSeconds(1f);
                    i = -1;
                }
            }
        }

        private void AutoAddComboCombination(CombinationData _combination)
        {
            List<CombinationData> _combo = new List<CombinationData>();
            _combo.Add(_combination);     
            int lenghAll = allCombinations.Count;
            if (lenghAll < 1)
                return;
            for (int all = 0; all < lenghAll; all++)
            {
                if (!_combo.Contains(allCombinations[all]))
                {
                    for (int current = 0; current < _combo.Count; current++)
                    {
                        int lenghCard = _combo[current].cards.Length;
                        for (int card = 0; card < lenghCard; card++)
                        {                           
                            if (allCombinations[all].cards.Contains(_combo[current].cards[card]))
                            {
                                HexagonalCell _cell = HexagonalGrid.Instance.GetCell(_combo[current].cards[card].Info.myX, _combo[current].cards[card].Info.myY);
                                BonusManager.Instance.CreateBonus(_cell, allCombinations[all]);
                                _combo.Add(allCombinations[all]);
                                card = lenghCard;
                                current = _combo.Count;
                                all = 0;
                            }
                        }
                    }
                }
            }  
            AddScores(_combo.ToArray());
        }          

        private void CheckCardsIn5Cells(int _x, int _y)
        {
            List<Card> _cardsX = new List<Card>();
            List<Card> _cardsY = new List<Card>();
            List<Card> _cardsYX = new List<Card>();
            for (int i = 0; i < 5; i++) 
            {
                HexagonalCell _cell = HexagonalGrid.Instance.GetCell(_x + i, _y);
                if (_cell != null && _cell.GetIsFull() && !_cell.GetCard().GetIsClosed())
                {
                    _cardsX.Add(_cell.GetCard());
                }
                else
                {
                    break;
                }
            }
            int myXminus = _x;
            int myYleft = _y;
            for (int i = 0; i < 5; i++)
            {
                if (i > 0)
                {
                    if ((myYleft + i) % 2 == 0)
                    {                     
                    }
                    else
                    {
                        myXminus--;
                    }
                }
                         
                HexagonalCell _cell = HexagonalGrid.Instance.GetCell(myXminus, myYleft + i);
                if (_cell != null && _cell.GetIsFull() && !_cell.GetCard().GetIsClosed())
                {
                    _cardsY.Add(_cell.GetCard());
                }
                else
                {
                    break;
                }
            }
            int myXplus = _x;
            int myYright = _y;
            for (int i =0; i < 5; i++)
            {            
                if (i > 0)
                {
                    if ((myYright + i) % 2 == 0)                    
                        myXplus++;                             
                }

                HexagonalCell _cell = HexagonalGrid.Instance.GetCell(myXplus, myYright + i);
                if (_cell != null && _cell.GetIsFull() && !_cell.GetCard().GetIsClosed())
                {
                    _cardsYX.Add(_cell.GetCard());
                }
                else
                {
                    break;
                }
            }
            CheckCombinationIn5Cards(_cardsX.ToArray());
            CheckCombinationIn5Cards(_cardsY.ToArray());
            CheckCombinationIn5Cards(_cardsYX.ToArray());
        }

        private void CheckCombinationIn5Cards(Card[] _cards)
        {          
            int countCard = _cards.Length-1;
            if (countCard <= 0 )//|| HexagonalGrid.Instance.GetCell(_cards[0].info.myX, _cards[0].info.myY).GetCurrentCombination() != eCombination.none)
                return;
            int matchColor = 0;
            int matchStrGrowth = 0;
            int matchStrDecrease = 0;
            int matchSet = 0;
            int matchTwoPair = 0;
            int matchFullHouse = 0;
            int match1FullHouse = 0;
            int matchQuads = 0;
         
            for (int i = 0; i < countCard; i++)
            {               
                if (_cards[i].Info.myNumCard == _cards[i + 1].Info.myNumCard ^ _cards[i].Info.mySpecialCard == eSpecialCard.Joker  || _cards[i+1].Info.mySpecialCard == eSpecialCard.Joker)
                {
                    if (i < 2 && countCard >= 2)
                    {
                        if(_cards[0].Info.myNumCard == _cards[2].Info.myNumCard || _cards[0].Info.myNumCard == _cards[1].Info.myNumCard ||_cards[1].Info.myNumCard == _cards[2].Info.myNumCard)
                          matchSet++;
                    }
                    if (i < 3 && i != 1)
                        matchTwoPair++;
                    if (i < 3)
                        matchQuads++;
                    if (i != 1 && countCard > 3 && _cards[0].Info.myNumCard != _cards[3].Info.myNumCard) 
                            matchFullHouse++;
                    if (i != 2 && countCard > 3 && _cards[0].Info.myNumCard != _cards[3].Info.myNumCard)
                        match1FullHouse++;
                }
                if (_cards[i].Info.myColor == _cards[i + 1].Info.myColor)
                {
                    matchColor++;
                }
                if (_cards[i].Info.myNumCard + 1 == _cards[i + 1].Info.myNumCard )
                {
                    matchStrGrowth++;
                }
                if (_cards[i].Info.myNumCard - 1 == _cards[i + 1].Info.myNumCard)
                {
                    matchStrDecrease++;
                }
                if (_cards[i].Info.mySpecialCard == eSpecialCard.Joker)
                {
                    if (i == 0 || i == 3)
                    {
                        matchColor += 2;
                        matchStrGrowth += 2;
                        matchStrDecrease += 2;
                    }
                    else
                    {
                        if (countCard >= 2 && _cards[i - 1].Info.myColor == _cards[i + 1].Info.myColor)
                            matchColor += 2;
                        if (countCard >= 2 && _cards[i - 1].Info.myNumCard + 2 == _cards[i + 1].Info.myNumCard || _cards[i - 1].Info.myNumCard - 2 == _cards[i + 1].Info.myNumCard)
                        {
                            matchStrGrowth += 2;
                            matchStrDecrease += 2;
                        }
                    }
                }
            }
            if (matchColor == 4 && CanAddFlaghingCombination(_cards, eCombination.flash))
            {
                if (matchStrGrowth == 4 || matchStrDecrease == 4)
                {
                    if (_cards[0].Info.myNumCard == 10 || _cards[0].Info.myNumCard == 14 || _cards[0].Info.mySpecialCard == eSpecialCard.Joker ||
                 _cards[4].Info.mySpecialCard == eSpecialCard.Joker)
                    {
                        AddCombinations(_cards, eCombination.royalFlush);
                    }
                    else
                    {
                        AddCombinations(_cards, eCombination.straightFlush);
                    }
                }
                else
                {
                    AddCombinations(_cards, eCombination.flash);
                }
            }
            else if (matchStrGrowth == 4 || matchStrDecrease == 4 && CanAddFlaghingCombination(_cards, eCombination.straight))
            {
                AddCombinations(_cards, eCombination.straight);
            }
            else if (matchFullHouse == 3 || match1FullHouse == 3 && CanAddFlaghingCombination(_cards, eCombination.fullHouse))
            {
                AddCombinations(_cards, eCombination.fullHouse);
            }
            else if (matchQuads == 3 && CanAddFlaghingCombination(new Card[] { _cards[0], _cards[1], _cards[2], _cards[3] }, eCombination.quads))
            {
                AddCombinations(new Card[] { _cards[0], _cards[1], _cards[2], _cards[3] }, eCombination.quads);
            }
            else if (matchTwoPair == 2 && _cards[0].Info.myNumCard != _cards[3].Info.myNumCard &&
                CanAddFlaghingCombination(new Card[] { _cards[0], _cards[1], _cards[2], _cards[3] }, eCombination.twoPairs))
            {
                AddCombinations(new Card[] { _cards[0], _cards[1], _cards[2], _cards[3] }, eCombination.twoPairs);
            }
            else if (matchSet == 2 && CanAddFlaghingCombination(new Card[] { _cards[0], _cards[1], _cards[2] }, eCombination.set))
            {
                AddCombinations(new Card[] { _cards[0], _cards[1], _cards[2] }, eCombination.set);
            }
            else if (_cards[0].Info.myNumCard == _cards[1].Info.myNumCard&& !_cards[0].GetIsClosed() && !_cards[1].GetIsClosed() && CanAddFlaghingCombination(new Card[] { _cards[0], _cards[1] }, eCombination.pair))
            {
                AddCombinations(new Card[] { _cards[0], _cards[1] }, eCombination.pair);
            }
            else if (_cards[0].Info.mySpecialCard == eSpecialCard.Joker || _cards[1].Info.mySpecialCard == eSpecialCard.Joker)
            {
                if (!_cards[0].GetIsClosed() && !_cards[1].GetIsClosed() && CanAddFlaghingCombination(new Card[] { _cards[0], _cards[1] }, eCombination.pair))
                    AddCombinations(new Card[] { _cards[0], _cards[1] }, eCombination.pair);
            }
        }
         
        private bool CanAddFlaghingCombination(Card[] _cards, eCombination currentAdd)
        {      
            int lengAdd = _cards.Length;
            for (int i = 0; i < allCombinations.Count; i++)
            {
                int lengExist = 0;
                for (int y = 0; y < lengAdd; y++)
                {
                    if (allCombinations[i].cards.Contains(_cards[y])) 
                    {
                        lengExist++;
                        if (lengExist == lengAdd) 
                            return false;
                        if(lengAdd >= 3 && lengExist > 1)
                            return false;
                        if (currentAdd == eCombination.fullHouse && lengExist >= 0)
                            return false;
                        if (currentAdd == eCombination.set && lengExist >=2)
                            return false;
                    }
                }
            }
            return true;
        }
      
        private void AddCombinations(Card[] _cards,eCombination _Ecombo)
        {
            CombinationData _comb = new CombinationData();
            _comb.cards = _cards;
            _comb.currentCombination = _Ecombo;
            allCombinations.Add(_comb);
            FlashingCellsAnimation(_cards, _Ecombo);
        }

        private void FlashingCellsAnimation(Card[] _card, eCombination _combination)
        {
            for (int i = 0; i < _card.Length; i++)
            {             
                HexagonalGrid.Instance.GetCell(_card[i].Info.myX, _card[i].Info.myY).AddCombination(_combination);
            }
        }      

        private void AddScores(CombinationData[] _currentCombinations)
        {
            SoundManager.Instance.PlayDeleteCombo();
            float _addScore = 0;
            for (int comb = 0; comb < _currentCombinations.Length; comb++)
            {
                AddCollectedCombo(_currentCombinations[comb].currentCombination);
                CheckClosedCard(_currentCombinations[comb]);             
                int scoreCombination = GetMaxNumCard(_currentCombinations[comb]) + (int)_currentCombinations[comb].currentCombination;
                _addScore += scoreCombination;
                ClearCellFromCard(scoreCombination, _currentCombinations[comb]);
            }
            if (_currentCombinations.Length > 1)  
            {
                float _comboBonus = _addScore *(_currentCombinations.Length * comboBonus);
                score.SetTextInfo(string.Format("{0} combo bonus", (int)_comboBonus));
                _addScore += _comboBonus;
            } 
            score.AddScore((int)_addScore);         
            ClearMatchCells();
            ManagerClosedCard.Instance.NullCurrentProgress();
        }

        private void AddCollectedCombo(eCombination _combo)
        {
            if(_combo == eCombination.pair)
            {
                collectedCombo[0]++;
                if (collectedCombo[0] >= GlobalInfo.collectPare)
                {
                    score.AddTurn(GlobalInfo.turnPare);
                    collectedCombo[0] = 0;
                }
            }
           else if (_combo == eCombination.twoPairs)
            {
                collectedCombo[1]++;
                if (collectedCombo[1] >= GlobalInfo.collectTwoPares)
                {
                    score.AddTurn(GlobalInfo.turnTwoPares);
                    collectedCombo[1] = 0;
                }
            }
            else if (_combo == eCombination.set)
            {
                collectedCombo[2]++;
                if (collectedCombo[2] >= GlobalInfo.collectSet)
                {
                    score.AddTurn(GlobalInfo.turnSet);
                    collectedCombo[2] = 0;
                }
            }
            else if (_combo == eCombination.straight)
            {
                collectedCombo[3]++;
                if (collectedCombo[3] >= GlobalInfo.collectStraight)
                {
                    score.AddTurn(GlobalInfo.turnStraight);
                    collectedCombo[3] = 0;
                }
            }
            else if (_combo == eCombination.flash)
            {
                collectedCombo[4]++;
                if (collectedCombo[4] >= GlobalInfo.collectFlash)
                {
                    score.AddTurn(GlobalInfo.turnFlash);
                    collectedCombo[4] = 0;
                }
            }
            else if (_combo == eCombination.fullHouse)
            {
                collectedCombo[5]++;
                if (collectedCombo[5] >= GlobalInfo.collectHouse)
                {
                    score.AddTurn(GlobalInfo.turnHouse);
                    collectedCombo[5] = 0;
                }
            }
            else if (_combo == eCombination.quads)
            {
                collectedCombo[6]++;
                if (collectedCombo[6] >= GlobalInfo.collectQuadro)
                {
                    score.AddTurn(GlobalInfo.turnQuadro);
                    collectedCombo[6] = 0;
                }
            }
            else if (_combo == eCombination.straightFlush)
            {
                collectedCombo[7]++;
                if (collectedCombo[7] >= GlobalInfo.collectStrFlash)
                {
                    score.AddTurn(GlobalInfo.turnStrFlash);
                    collectedCombo[7] = 0;
                }
            }
            else if (_combo == eCombination.royalFlush)
            {
                collectedCombo[8]++;
                if (collectedCombo[8] >= GlobalInfo.collectRoyal)
                {
                    score.AddTurn(GlobalInfo.turnRoyal);
                    collectedCombo[8] = 0;
                }
            }
        }

        private void CheckClosedCard(CombinationData _combinations)
        {
            for (int i = 0; i < _combinations.cards.Length; i++)
            {
                HexagonalCell[] closedCell = HexagonalGrid.Instance.GetNearIsClosed(_combinations.cards[i].Info.myX, _combinations.cards[i].Info.myY);
                for (int _card = 0; _card < closedCell.Length; _card++)
                {
                    closedCell[_card].GetCard().SetIsClosed(false);
                }
            }   
        }

        private void ClearCellFromCard(float maxScore, CombinationData _combinations)
        {
            int lenght = _combinations.cards.Length;
            float scoreOnCell = maxScore / lenght;
            for (int i = 0; i < lenght; i++)
            {
                HexagonalCell _cell = HexagonalGrid.Instance.GetCell(_combinations.cards[i].Info.myX, _combinations.cards[i].Info.myY);
                _cell.ClearFromCard((int)scoreOnCell, _combinations.currentCombination);               
            }
        }

        private int GetMaxNumCard(CombinationData _combinations)
        {         
            int maxNumCard = 0;
            int lenght = _combinations.cards.Length;
            for (int i = 0; i < lenght; i++)
            {
                if (maxNumCard < _combinations.cards[i].Info.myNumCard)
                {
                    maxNumCard = _combinations.cards[i].Info.myNumCard;
                }              
            }
            return maxNumCard;
        }

    }    
}
