using UnityEngine;
using Cards;
using System.Collections.Generic;
using System.Linq;

namespace Hexagonal {
    public class HexagonalGrid : MonoBehaviour
    {
        public static HexagonalGrid Instance;

        [SerializeField]
        private GameObject prefabCell;

        private HexagonalCell[,] cells;
        private int startInitCell = 0;

        // Use this for initialization
        void Start()
        {
            if (Instance == null)
                Instance = this;
            cells = new HexagonalCell[GlobalInfo.row, GlobalInfo.column];
            BuildCells();
        }       

        public HexagonalCell[]  GetAllCells()
        {
            List<HexagonalCell> allCells = new List<HexagonalCell>();
            for (int y = 0; y < GlobalInfo.column; y++)
            {
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    HexagonalCell _cell = GetCell(x, y);
                    if (_cell != null)
                    {
                        allCells.Add(_cell);
                    }
                }
            }
            return allCells.ToArray();
        }

        public HexagonalCell[] GetAllClosedCard()
        {
            List< HexagonalCell > allClosed = new List<HexagonalCell> ();
            for (int y = 0; y < GlobalInfo.column; y++)
            {
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    HexagonalCell _cell = GetCell(x, y);
                    if (_cell != null)
                    {
                        _cell.SetFlashingBack(false);
                        if ( _cell.GetIsFull() && _cell.GetCard().GetIsClosed())
                            allClosed.Add(_cell);
                    }                
                }
            }
            return allClosed.ToArray();
        }

        public HexagonalCell GetEmptyCellFromNearVector(Vector3 _pos)
        {
            float distanceMin = 999; 
            HexagonalCell cellNear = null;
            for (int y = 0; y < GlobalInfo.column; y++)
            {
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    HexagonalCell _cell = GetCell(x, y);
                    if (_cell != null && !_cell.GetIsFull())
                    {
                        float distanceCurrent = Vector3.Distance(_cell.transform.position, _pos);
                        if (distanceMin > distanceCurrent)
                        {
                            distanceMin = distanceCurrent;
                            cellNear = _cell;
                            if (distanceMin < 20)
                                break;
                        }
                    }
                }
            }
            return cellNear;
        }

        public HexagonalCell GetCell(int x,int y)
        {
            if (ExistCell(x, y))
            {
                return cells[x, y];
            }
            else
            {
                return null;
            }     
        }

        public HexagonalCell GetRandomEmptyCellNearFull()
        {
            HexagonalCell _cell = null;
            int _random = Random.Range(1, GlobalInfo.row);
            for (int y = 0; y < GlobalInfo.column; y++)
            {
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    if (ExistCell(x, y) && !cells[x, y].GetIsFull() && GetNearIsFull(x, y))
                    {
                        _cell = cells[x, y];
                        _random--;
                        if (_random <= 0)
                        {
                            return _cell;
                        }
                    }
                }
            }
            return _cell;
        }

        public bool CheckSameCard(int x, int y, CardData _info)
        {  
            if (x-1 >= 0 && cells[x - 1, y].CheckCardData(_info))
                return true;
            if (y - 1 >= 0 && cells[x, y - 1].CheckCardData(_info))
                return true;
            if (y + 1 < GlobalInfo.column && cells[x, y + 1].CheckCardData(_info))
                return true;
            if (y % 2 == 0) 
            {
                if (x - 1 >= 0)
                {
                    if (y - 1 >= 0 && cells[x - 1, y - 1].CheckCardData(_info))
                        return true;
                    if (y < GlobalInfo.column && cells[x - 1, y + 1].CheckCardData(_info))
                        return true;
                } 
                if (x + 1 < GlobalInfo.row && cells[x + 1, y].CheckCardData(_info))
                    return true;
            }
            else
            {
                if (x + 1 < GlobalInfo.row-1)
                {
                    if (y - 1 >= 0 && cells[x + 1, y - 1].CheckCardData(_info))
                        return true;
                    if (y + 1 < GlobalInfo.column && cells[x + 1, y + 1].CheckCardData(_info))
                         return true;
                }
                if (x + 1 < GlobalInfo.row -1 && cells[x + 1, y].CheckCardData(_info))
                    return true;
            }
            return false;
        }

        public void NoFlashingAllCells()
        {
            for (int y = 0; y < GlobalInfo.column; y++)
            {             
                for (int x = 0; x < GlobalInfo.row; x++) 
                {
                    if (ExistCell(x, y) && cells[x, y].GetIsFull())
                        cells[x, y].NoFlashing();
                }
            }
        }

        public void MixAllCards()
        {
            List<CardData> allCards = new List<CardData>();
            for (int y = 0; y < GlobalInfo.column; y++)
            {
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    if (ExistCell(x, y) && cells[x, y].GetIsFull())
                    {
                        CardData info = new CardData();
                        info.myColor = cells[x, y].GetCard().Info.myColor;
                        info.myNumCard = cells[x, y].GetCard().Info.myNumCard;
                        info.mySpecialCard = cells[x, y].GetCard().Info.mySpecialCard;
                        info.myX = cells[x, y].GetCard().Info.myX;
                        info.myY = cells[x, y].GetCard().Info.myY;
                        info.isClosed = cells[x, y].GetCard().GetIsClosed();
                        allCards.Add(info); 
                        cells[x, y].ClearCellAndCard();
                    }
                }
            }
            for (int y = 0; y < GlobalInfo.column; y++)
            {
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    if (ExistCell(x, y))
                    {
                        int _random = Random.Range(0, allCards.Count);                                       
                        cells[x, y].InitCardInCellWithCheckClosed((int)allCards[_random].myColor, allCards[_random].myNumCard, allCards[_random].mySpecialCard, allCards[_random].isClosed);
                        allCards.RemoveAt(_random);
                        if (allCards.Count <= 0)
                        {
                            Combination.ManagerCombination.Instance.ClearMatchCells();
                            return;
                        }                            
                    }
                }
            }          
        } 

        public void WindFallNearCard(int x, int y, int step)
        {
            cells[x, y].WindFallingCard(new Vector3(0,100, 0) );
            int xStepPlus = x;
            int xStepMinus = x;
            int distanceWind = 400;
            for (int iStep = 1; iStep <= step; iStep++)
            {
                if (ExistCell(x + iStep, y))
                    cells[x + iStep, y].WindFallingCard(new Vector3(distanceWind, 0, 0));
                if (ExistCell(x - iStep, y))
                    cells[x - iStep, y].WindFallingCard(new Vector3(-distanceWind, 0, 0));              

                if ((y + iStep) % 2 == 0)
                    xStepPlus++;
                else
                    xStepMinus--;

                if (ExistCell(xStepPlus, y + iStep))
                    cells[xStepPlus, y + iStep].WindFallingCard(new Vector3(distanceWind, -distanceWind, 0));
                if (ExistCell(xStepPlus, y - iStep))
                    cells[xStepPlus, y - iStep].WindFallingCard(new Vector3(distanceWind, distanceWind, 0)); 
                if (ExistCell(xStepMinus, y - iStep))
                    cells[xStepMinus, y - iStep].WindFallingCard(new Vector3(-distanceWind, distanceWind, 0));
                if (ExistCell(xStepMinus, y + iStep))
                    cells[xStepMinus, y + iStep].WindFallingCard(new Vector3(-distanceWind, -distanceWind, 0));
            }
        }

        public void FallNearCard(int x, int y,int step)
        {
            cells[x, y].FallingCard();
            int xStepPlus = x;
            int xStepMinus = x;
            for (int iStep=1;iStep <= step;iStep++)
            {
                if (ExistCell(x + iStep,y))
                    cells[x  + iStep, y].FallingCard();
                if (ExistCell(x - iStep, y))
                    cells[x - iStep, y].FallingCard();              

                if ((y + iStep) % 2 == 0)
                    xStepPlus ++;
                else
                    xStepMinus --;

                if (ExistCell(xStepPlus, y + iStep))
                    cells[xStepPlus, y + iStep].FallingCard();
                if (ExistCell( xStepPlus, y - iStep))
                    cells[ xStepPlus, y - iStep].FallingCard();
                if (ExistCell(xStepMinus, y - iStep))
                    cells[ xStepMinus, y - iStep].FallingCard();
                if (ExistCell(xStepMinus, y + iStep))
                    cells[ xStepMinus, y + iStep].FallingCard();
            } 
        }

        public void DetectFallingCell()
        {
            for (int y = 1; y < GlobalInfo.column; y++)
            {
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    if (ExistCell(x, y) && cells[x, y].GetIsFull())
                    {
                        if (!LinearSearchPath(x, y))
                        {
                            cells[x, y].FallingCard();
                        }
                    }
                }
            }
        }

        public bool LinearSearchPath(int x, int y)
        {
            List<HexagonalCell> allCheck = new List<HexagonalCell>();
            allCheck.AddRange(GetNearFullCells(x, y));
            if (allCheck.Count == 0)
                return false;
            for (int i = 0; i < allCheck.Count; i++)
            {
                HexagonalCell[] cells = GetNearFullCells(allCheck[i].Info.myX, allCheck[i].Info.myY);
                for (int z = 0; z < cells.Length; z++)
                {
                    if (cells[z].Info.myY == 0)
                        return true;
                    if (!allCheck.Contains(cells[z]))
                        allCheck.Add(cells[z]);                    
                }
            }          
            return false;
        }     
        
        private bool CheckYTop(HexagonalCell[] cells)
        {
            int lenght = cells.Length;
            for (int i = 0; i < lenght; i++)
            {
                if (cells[i].Info.myY == 0)
                {
                    return true;
                }
            }
            return false;
        }        

        public HexagonalCell[] GetNearIsClosed(int x, int y)
        {
            List<HexagonalCell> clodesCell = new List<HexagonalCell>();
            if (ExistCell(x + 1, y) && cells[x + 1, y].GetCard().GetIsClosed())
            {
                clodesCell.Add(cells[x + 1, y]);
            }
            if (ExistCell(x - 1, y) && cells[x - 1, y].GetCard().GetIsClosed())
            {
                clodesCell.Add(cells[x - 1, y]);
            }
            if (ExistCell(x, y - 1) && cells[x, y - 1].GetCard().GetIsClosed())
            {
                clodesCell.Add(cells[x, y - 1]);
            }
            if (ExistCell(x, y + 1) && cells[x, y + 1].GetCard().GetIsClosed())
            {
                clodesCell.Add(cells[x, y + 1]);
            }
            if (y % 2 == 0)
            {
                if (ExistCell(x - 1, y - 1) && cells[x - 1, y - 1].GetCard().GetIsClosed())
                    clodesCell.Add(cells[x - 1, y - 1]);
                if (ExistCell(x - 1, y + 1) && cells[x - 1, y + 1].GetCard().GetIsClosed())
                    clodesCell.Add(cells[x - 1, y + 1]);
            }
            else
            {
                if (ExistCell(x + 1, y - 1) && cells[x + 1, y - 1].GetCard().GetIsClosed())
                    clodesCell.Add(cells[x + 1, y - 1]);
                if (ExistCell(x + 1, y + 1) && cells[x + 1, y + 1].GetCard().GetIsClosed())
                    clodesCell.Add(cells[x + 1, y + 1]);
            }
            return clodesCell.ToArray();
        }

        public HexagonalCell[] GetNearEmpyCells(int x, int y)
        {
            List<HexagonalCell> clodesCell = new List<HexagonalCell>();
            if (ExistCell(x + 1, y) && !cells[x + 1, y].GetIsFull())
            {
                clodesCell.Add(cells[x + 1, y]);
            }
            if (ExistCell(x - 1, y) && !cells[x - 1, y].GetIsFull())
            {
                clodesCell.Add(cells[x - 1, y]);
            }
            if (ExistCell(x, y - 1) && !cells[x, y-1].GetIsFull())
            {
                clodesCell.Add(cells[x, y - 1]);
            }
            if (ExistCell(x, y +1) && !cells[x, y+1].GetIsFull())
            {
                clodesCell.Add(cells[x, y +1]); 
            }
            if (y % 2 == 0)
            {
                if (ExistCell(x - 1, y - 1) && !cells[x - 1, y - 1].GetIsFull())
                    clodesCell.Add(cells[x - 1, y - 1]);
                if (ExistCell(x - 1, y + 1) && !cells[x - 1, y + 1].GetIsFull())
                    clodesCell.Add(cells[x - 1, y +1]);
            }
            else
            {
                if (ExistCell(x + 1, y - 1) && !cells[x + 1, y - 1].GetIsFull())
                    clodesCell.Add(cells[x + 1, y - 1]);
                if (ExistCell(x + 1, y + 1) && !cells[x + 1, y +1].GetIsFull())
                    clodesCell.Add(cells[x + 1, y + 1]);
            }

            return clodesCell.ToArray();
        }

        public HexagonalCell[] GetNearFullCells(int x, int y)
        {
            List<HexagonalCell> fullCells = new List<HexagonalCell>();
            if (ExistCell(x, y - 1) && cells[x, y - 1].GetIsFull())
            {
                fullCells.Add(cells[x, y - 1]);
            }          
            if (ExistCell(x + 1, y) && cells[x + 1, y].GetIsFull())
            {
                fullCells.Add(cells[x + 1, y]);
            }
            if (ExistCell(x - 1, y) && cells[x - 1, y].GetIsFull())
            {
                fullCells.Add(cells[x - 1, y]);
            }         
            if (y % 2 == 0)
            {
                if (ExistCell(x - 1, y - 1) && cells[x - 1, y - 1].GetIsFull())
                    fullCells.Add(cells[x - 1, y - 1]);
                if (ExistCell(x - 1, y + 1) && cells[x - 1, y + 1].GetIsFull())
                    fullCells.Add(cells[x - 1, y + 1]);
            }
            else
            {
                if (ExistCell(x + 1, y - 1) && cells[x + 1, y - 1].GetIsFull())
                    fullCells.Add(cells[x + 1, y - 1]);
                if (ExistCell(x + 1, y + 1) && cells[x + 1, y + 1].GetIsFull())
                    fullCells.Add(cells[x + 1, y + 1]);
            }
            if (ExistCell(x, y + 1) && cells[x, y + 1].GetIsFull())
            {
                fullCells.Add(cells[x, y + 1]);
            }
            return fullCells.ToArray();
        }

        private bool GetNearIsFull(int x, int y)
        {
            if (ExistCell(x + 1, y) && cells[x + 1, y].GetIsFull())
            {
                return true;
            }
            if (ExistCell(x - 1, y) && cells[x - 1, y].GetIsFull())
            {
                return true;
            }
            if (ExistCell(x, y - 1) && cells[x, y - 1].GetIsFull())
            {
                return true;
            }
            if (y % 2 == 0)
            {
                if (ExistCell(x - 1, y - 1) && cells[x - 1, y - 1].GetIsFull())
                    return true;
            }
            else
            {
                if (ExistCell(x + 1, y - 1) && cells[x + 1, y - 1].GetIsFull())
                    return true;
            }

            return false;
        }

        private bool ExistCell(int x, int y)
        {       
            if (y % 2 == 0)
            {
                if (x >= GlobalInfo.row || x < 0 || y < 0 || y >= GlobalInfo.column)
                    return false;
            }
            else
            {
                if (x >= GlobalInfo.row -1 || x < 0 || y < 0 || y >= GlobalInfo.column)
                    return false;
            }   
            return true;
        }    

        private void BuildCells()
        {
            float screen = (float)Screen.height / (float)Screen.width;
            float distanseX = Screen.width / GlobalInfo.row;
            float distanseY = (Screen.height / (GlobalInfo.column + 3));
            if (screen > 1.4f && screen <= 1.55f && Screen.height > 400 && Screen.height < 600)
                distanseY -= 1f;
            if (screen > 1.55f && screen <= 1.65f && Screen.height > 1100)
                distanseY -= 9;
            if (screen > 1.7f && Screen.height > 1100)
                distanseY -= 9;
            if (screen > 1.65f && Screen.height >= 850)
                distanseY -= 12;
            if (screen > 1.65f && Screen.height < 850)
                distanseY -= 7;

            float startX = transform.position.x + 0.5f;
            float posX = startX;
            float posY = transform.position.y;
            float offset = ((distanseX / 2));
            for (int y = 0; y < GlobalInfo.column; y++)
            {
                posY -= distanseY;
                for (int x = 0; x < GlobalInfo.row; x++)
                {
                    if (y % 2 == 1 && x == GlobalInfo.row - 1)
                        continue;
                    InitCells(posX, posY, x, y);

                    posX += distanseX;
                }
                if (y % 2 == 0)
                    posX = startX + offset;
                else
                    posX = startX;
            }
            if (LoadManager.Instance != null && !LoadManager.Instance.needLoad)
                ManagerClosedCard.Instance.CreateStartClosedCard();
        }    

        private void InitCells(float distanceX, float distanceY,int x,int y)
        {
            GameObject obj = Instantiate(prefabCell, new Vector3(distanceX, distanceY, 0), Quaternion.identity) as GameObject;         
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.name = x.ToString()+"x " + y.ToString() + "y";
            HexagonalCell _cell = obj.GetComponent<HexagonalCell>();
            cells[x, y] = _cell; 
            _cell.InitCoordinate(x, y);

            if (LoadManager.Instance != null && LoadManager.Instance.needLoad)
            {
                CardData _card = LoadManager.Instance.SaveInfo.Cards.FirstOrDefault(card => card.myX == x && card.myY == y);
                if(_card != null && _card.myColor != 0 && _card.myColor != 0 )
                   _cell.InitCardInCellWithCheckClosed((int)_card.myColor, _card.myNumCard, _card.mySpecialCard, _card.isClosed);
            }
            else
            {
                if (y <= startInitCell)
                {
                    InitCardInCell(_cell);
                }
            }             
        }

        private void InitCardInCell(HexagonalCell _cell)
        {
            CardData _card = PollCards.Instance.GetNewCard();
            _cell.InitCardInCell((int)_card.myColor, _card.myNumCard, _card.mySpecialCard);
        }

    }
}
