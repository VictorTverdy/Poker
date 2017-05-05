using System;
using System.Linq;
using UnityEngine;
using Hexagonal;
using Cards;
using Combination;
public class BonusManager : MonoBehaviour {

    public static BonusManager Instance;

    [SerializeField]
    private GameObject prefabBonus;
    [SerializeField]
    private Transform _parent;
    [SerializeField]
    private BonusSlot[] bonusSlots;

    private BonusSlot currentActivSlot;
    private BonusInfo InfoBonus;
    private BonusItem[] bonusItems;
    private bool isNeedActivateBonus;
    private HexagonalCell swapCell;
    private ActivCardMoving activCard;
    private ChameleonPanel chameleonPanel;
    private int canUppend;
    private eSpecialCard currentBonus;

    // Use this for initialization
    void Start ()
    {      
		if(Instance == null)
        {         
            Instance = this;
            activCard = FindObjectOfType<ActivCardMoving>();
            chameleonPanel = FindObjectOfType<ChameleonPanel>();
            LoadInfo();
            CreatePoll();
        }
    }

    public void BuyBonus(int _bonus)
    {
        BonusSlot _freeSlot = GetFreeBonusSlot();
        if(_freeSlot != null)
        {           
            _freeSlot.InitSlot((eSpecialCard)_bonus);
        } 
    }

    public void CreateBonus(HexagonalCell _cell, CombinationData _combination)
    {
        int random = UnityEngine.Random.Range(0, 100);
        if (random > GlobalInfo.changeDropBonus)
        {
            return;
        }
          
        int maxNumCard = GetMaxNumCard(_combination);
        string _combo = _combination.currentCombination.ToString() + maxNumCard.ToString();      
        eSpecialCard _bonus = GetBonusReward(_combo); 
        if (_bonus != eSpecialCard.none)
        {
            BonusItem _items = GetNotActiveBonusItem(); 
            _items.gameObject.SetActive(true);
            _items.transform.position = _cell.transform.position;
            _items.Init(_bonus);       
            BonusSlot _freeslot = GetFreeBonusSlot();
            if (_freeslot != null)
            { 
                _items.MoveFreeSlot(_freeslot);
            } else 
            { 
              //  _freeslot = GetSameBonusInSlot(_bonus);
                if (_freeslot != null)
                    _items.MoveFreeSlot(_freeslot);
                else
                    _items.InvokeDeactivate();
            }
        }
        else
        { 
            Debug.LogError("_bonus == eSpecialCard.none");
        }
    }

    public void DeactivateBonus()
    {
        for (int i = 0; i < bonusItems.Length; i++)
        {
            if (bonusItems[i].gameObject.activeInHierarchy) 
                bonusItems[i].Deactivate();
        }
    }

    public void ActivateBonus(BonusSlot _bonusSlot)
    {
        if (_bonusSlot == null || _bonusSlot.GetBonus() == eSpecialCard.none || activCard.myCard.Info.mySpecialCard != eSpecialCard.none || isNeedActivateBonus)
        {
            return;
        }       
        eSpecialCard _currentBonus = _bonusSlot.GetBonus();
        SoundManager.Instance.PlayBonusStart(_currentBonus,true);
        isNeedActivateBonus = false;    
        if (_currentBonus == eSpecialCard.Joker || _currentBonus.ToString().Contains("bullet") )
        {  
            if (!activCard.GetIsMoving())
            {
                activCard.SetReserveCardToPoll();
                activCard.InitCard(0, 0, _currentBonus); 
                _bonusSlot.ClearSlot();
            }    
        }
        else if(_currentBonus.ToString().Contains("wind") || _currentBonus.ToString().Contains("fireworks"))
        {
            if (!activCard.GetIsMoving())
            {
                Transform card = _bonusSlot.GetMyCard().GetTransformImageCard();
                Vector3 pos = activCard.transform.position;
                activCard.SetEnableImageCard(false);
                card.position = pos;
                NeedActivateBonus(_bonusSlot,false);
            }
        }
        else if (_currentBonus == eSpecialCard.upend1|| _currentBonus == eSpecialCard.upend2|| _currentBonus == eSpecialCard.upend3)
        {             
            HexagonalCell[] closedCells = HexagonalGrid.Instance.GetAllClosedCard();
            for (int i = 0; i < closedCells.Length; i++)
            {
                closedCells[i].SetFlashingBack(true);
            }
            canUppend =(int)_currentBonus;
            NeedActivateBonus(_bonusSlot,true);
        }
        else if (_currentBonus == eSpecialCard.upend4)
        {           
            HexagonalCell[] closedCells = HexagonalGrid.Instance.GetAllClosedCard();
            for (int i=0; i < closedCells.Length;i++)
            {
                closedCells[i].GetCard().SetIsClosed(false);
            }
            _bonusSlot.ClearSlot();
        }
        else if (_currentBonus == eSpecialCard.hurricane)
        {
            HexagonalGrid.Instance.MixAllCards(); 
            _bonusSlot.ClearSlot();
        }else if (_currentBonus == eSpecialCard.free)
        {
            activCard.SetCanFlight();
            _bonusSlot.ClearSlot();
        }
        else
        {
            NeedActivateBonus(_bonusSlot,true);
        }
    } 

    private void NeedActivateBonus(BonusSlot _bonusSlot, bool imageEnable)
    {
        currentActivSlot = _bonusSlot;
        currentBonus = currentActivSlot.GetBonus(); 
        currentActivSlot.ActivateBackImageCard(imageEnable);
        isNeedActivateBonus = true;
    }

    public void ClearCurrentActivSlot()
    {
        isNeedActivateBonus = false;
        currentActivSlot = null;
    }

    public void SetCellOnClick(HexagonalCell _cell)
    {
        if (isNeedActivateBonus)
        {
            if (currentActivSlot != null)
                currentActivSlot.ActivateBackImageCard(false);
            isNeedActivateBonus = false;
            MechanicsBonuses(_cell);
        }
        else
        {
            currentActivSlot = null;
        }
    }  

    private void MechanicsBonuses(HexagonalCell _cell)
    {
        SoundManager.Instance.PlayBonusStart(currentBonus, false);
        string strCurrentBonus = currentBonus.ToString();
        if (strCurrentBonus.Contains("upend"))
        {
            isNeedActivateBonus = true;
            if (!_cell.GetCard().GetIsClosed())
                return;
            currentActivSlot.ActivateBackImageCard(true);
            _cell.SetFlashingBack(false);
            _cell.GetCard().SetIsClosed(false);
            canUppend--;           
            ManagerCombination.Instance.ClearMatchCells();
            if (canUppend <= 0)
            {
                currentActivSlot.ClearSlot();
                isNeedActivateBonus = false;
                HexagonalCell[] closedCells = HexagonalGrid.Instance.GetAllClosedCard();
                for (int i = 0; i < closedCells.Length; i++)
                {
                    closedCells[i].SetFlashingBack(false);
                }
            }        
        }
        else if (strCurrentBonus.Contains("wind"))
        {
            int step = 0;
            if (currentBonus == eSpecialCard.wind1)
                step = 1;
            if (currentBonus == eSpecialCard.wind2)
                step = 2;
            if (currentBonus == eSpecialCard.wind3)
                step = 3;
            currentActivSlot.GetMyCard().SetMovePositionCallBack(_cell.transform.position, ( delegate() {
                HexagonalGrid.Instance.WindFallNearCard(_cell.Info.myX, _cell.Info.myY, step); 
                BonusManager.Instance.ClearCurrentBonusSlot();              
                ManagerCombination.Instance.ClearMatchCells();
                activCard.SetEnableImageCard(true);
            }  ));
        }
        else if (strCurrentBonus.Contains("fireworks"))
        { 
            int step = 0;           
            if (currentBonus == eSpecialCard.fireworks2)
                step = 1; 
            if (currentBonus == eSpecialCard.fireworks3)
                step = 2;       
            currentActivSlot.GetMyCard().SetMovePositionCallBack(_cell.transform.position, (delegate () {
                HexagonalGrid.Instance.FallNearCard(_cell.Info.myX, _cell.Info.myY, step);
                BonusManager.Instance.ClearCurrentBonusSlot();
                ManagerCombination.Instance.ClearMatchCells();
                activCard.SetEnableImageCard(true);
            }));
        }
        else if (currentBonus == eSpecialCard.copy && !_cell.GetCard().GetIsClosed())
        {
            if (!activCard.GetIsMoving())
            {
                activCard.AppearanceInit((int)_cell.GetCard().Info.myColor, _cell.GetCard().Info.myNumCard, eSpecialCard.none);
                currentActivSlot.ClearSlot();
            }
        }
        else if (currentBonus == eSpecialCard.chameleon &&  !_cell.GetCard().GetIsClosed() && _cell.GetCard().Info.mySpecialCard == eSpecialCard.none)
        {
            chameleonPanel.ActivatePanel(_cell);
            currentActivSlot.ClearSlot();
        }
        else if (currentBonus == eSpecialCard.decrease && !_cell.GetCard().GetIsClosed() && _cell.GetCard().Info.myNumCard > 2)
        {
            _cell.GetCard().DecreaseCard();
            currentActivSlot.ClearSlot();
        }
        else if (currentBonus == eSpecialCard.increase && !_cell.GetCard().GetIsClosed() && _cell.GetCard().Info.myNumCard < 14)
        {
            _cell.GetCard().IncreaseCard(); 
            currentActivSlot.ClearSlot();
        }
    }

    public void ClearCurrentBonusSlot()
    {
        if(currentActivSlot != null)
           currentActivSlot.ClearSlot(); 
        isNeedActivateBonus = false;
    }

    public BonusSlot GetCurrentActivSlot()
    {
        return currentActivSlot;
    }

    public void SetCellSwapDown(HexagonalCell _cell)
    {
        if (currentActivSlot != null && currentActivSlot.GetBonus() == eSpecialCard.swap)
        {
            swapCell = _cell;
        }
    }

    public void SetCellSwapEnter(HexagonalCell _cell)
    {
        if (isNeedActivateBonus && currentActivSlot != null && currentActivSlot.GetBonus() == eSpecialCard.swap && swapCell != null)
        {
            if (!HexagonalGrid.Instance.GetNearFullCells(swapCell.Info.myX, swapCell.Info.myY).Contains(_cell))
            {
                swapCell = null;
                return;
            }
            SoundManager.Instance.PlayBonusStart(eSpecialCard.swap,false);
            GlobalInfo.canCombination = false;
            CardData swapCard = new CardData();
            swapCard.myColor = swapCell.GetCard().Info.myColor;
            swapCard.myNumCard = swapCell.GetCard().Info.myNumCard;
            swapCard.mySpecialCard = swapCell.GetCard().Info.mySpecialCard;
            swapCard.isClosed = swapCell.GetCard().GetIsClosed();
            swapCell.GetCard().SetMovePositionCallBack(_cell.transform.position, (delegate ()
        {
            swapCell.InitCardInCellWithCheckClosed((int)_cell.GetCard().Info.myColor, _cell.GetCard().Info.myNumCard, _cell.GetCard().Info.mySpecialCard, _cell.GetCard().GetIsClosed());
        }));

            _cell.GetCard().SetMovePositionCallBack(swapCell.transform.position, (delegate ()
            {
                _cell.InitCardInCellWithCheckClosed((int)swapCard.myColor, swapCard.myNumCard, swapCard.mySpecialCard, swapCard.isClosed);
                swapCell = null;
                ManagerCombination.Instance.ClearMatchCells();
                GlobalInfo.canCombination = true;
            }));
            currentActivSlot.ClearSlot();
        }       
    }

    private eSpecialCard GetBonusReward(string _combo)
    {       
        int _random = UnityEngine.Random.Range(0, 100);  
        BonusData _bonus = InfoBonus.Info.FirstOrDefault(x => x.ComboName == _combo);     
        if (string.IsNullOrEmpty(_bonus.ComboName))
        {
            Debug.LogError("noy found combo -"+ _combo);
            return eSpecialCard.none;
        }
        int currentNum = 0;
        for (int i=0;i< _bonus.ChangeDrop.Length;i++)
        {
            currentNum += _bonus.ChangeDrop[i];
            if(_random < currentNum)
            {
                return (eSpecialCard)Enum.Parse(typeof(eSpecialCard), _bonus.MySpecial[i]);              
            }
        }   
        return eSpecialCard.none;
    }

    private BonusSlot GetSameBonusInSlot(eSpecialCard _bonus)
    {
        for (int i = 0; i < bonusSlots.Length; i++)
        {
            eSpecialCard currentBonus = bonusSlots[i].GetBonus();
            if (_bonus.ToString().Contains("upend") && currentBonus.ToString().Contains("upend"))
            {
                if (currentBonus != eSpecialCard.upend4)
                    return bonusSlots[i];
            }
            if (_bonus.ToString().Contains("bullet") && currentBonus.ToString().Contains("bullet"))
            {
                if (currentBonus != eSpecialCard.bullet5)
                    return bonusSlots[i];
            }
            if (_bonus.ToString().Contains("fireworks") && currentBonus.ToString().Contains("fireworks"))
            {
                if (currentBonus != eSpecialCard.fireworks3)
                    return bonusSlots[i];
            }
            if (_bonus.ToString().Contains("wind") && currentBonus.ToString().Contains("wind"))
            {
                if (currentBonus != eSpecialCard.wind3)
                    return bonusSlots[i];
            }
        }
        return null;
    }

    public bool GetIsFreeBonusSlot()
    {
        for (int i = 0; i < bonusSlots.Length; i++)
        {
            if (!bonusSlots[i].GetisFull())
            {
                return true;
            }
        }
        return false;
    }

    private BonusSlot GetFreeBonusSlot()
    {
        for (int i=0;i<bonusSlots.Length;i++)
        {
            if (!bonusSlots[i].GetisFull())
            {
                bonusSlots[i].SetisFull();
                return bonusSlots[i];
            }
        }
        return null;
    }

    private BonusItem GetNotActiveBonusItem()
    {
        return bonusItems.First(x => !x.isActiveAndEnabled);

    }

    private int GetMaxNumCard(CombinationData _combination)
    {
        int maxNumCard = 0;
        for (int i = 0; i < _combination.cards.Length; i++)
        {
            if (maxNumCard < _combination.cards[i].Info.myNumCard)
            {
                maxNumCard = _combination.cards[i].Info.myNumCard;
            }
        }
        return maxNumCard;
    }

    private void CreatePoll()
    { 
        int countBonus = 10;
        bonusItems = new BonusItem[countBonus];
        for (int i=0;i< countBonus;i++)
        {
            GameObject _clone = Instantiate(prefabBonus, _parent) as GameObject;
            _clone.transform.localScale = Vector3.one;
            bonusItems[i] = _clone.GetComponent<BonusItem>();
            _clone.SetActive(false);
        }
    }

    private void LoadInfo()
    {       
        TextAsset targetFile = Resources.Load<TextAsset>("BonusInfo");
        InfoBonus =  JsonUtility.FromJson<BonusInfo>(targetFile.text);       
    }
}


