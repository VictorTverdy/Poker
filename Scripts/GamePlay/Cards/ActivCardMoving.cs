using UnityEngine;
using Hexagonal;
using Combination;
using System.Collections;
using UnityEngine.EventSystems;
namespace Cards
{
    public class ActivCardMoving : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Transform leftBorder, rightBorder, downBorder;
        [SerializeField]
        private ReplacementCard replacementCard;
        [SerializeField]
        private GameObject backLight;

        public Card myCard;
        public bool canSwap;
        public GameObject wings;

        private Transform myTransform;
        private bool canMoving = true;
        private bool isMoving;
        private bool isTouch;
        private bool isFlight;
        private bool needReserveCard;
        private float currentSpeedMove;
        private float slowSpeedMove = 1.5f;
        private float speedMoveFlight = 0.05f;
        private float timeMoving;
        private HexagonalCell lastConnect;
        private HexagonalCell flightCell;
        private Vector3 vectorMove;
        private Vector3 startPos;
        private HudUI hud;
        private TurnBack turnBack;
        // Use this for initialization
        void Start()
        {
            SoundManager.Instance.PlayMixStart();
            turnBack = FindObjectOfType<TurnBack>();
            hud = FindObjectOfType<HudUI>();
            myTransform = transform;
            startPos = transform.position;
            myCard = GetComponent<Card>();
            if (LoadManager.Instance.needLoad)
            {
                Invoke("LoadNewCard", .01f);  
            }
            else
            {
                Invoke("CreateNewCard", .01f);
            }
        }

        void OnDisable()
        {
            string jsonSave = JsonUtility.ToJson(myCard.Info);
            PlayerPrefs.SetString("ActivCard", jsonSave);
        }

        // Update is called once per frame
        void Update() 
        {
            FindVectroMove();
            CurrentMoving();
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!isMoving)
                transform.position = Input.mousePosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isMoving)
                myCard.imageCard.raycastTarget = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isMoving)
            {
                transform.position = startPos;
                var obj = eventData.pointerCurrentRaycast;
                myCard.imageCard.raycastTarget = true;
                if (obj.gameObject == null)
                    return;
                Transform _parent = obj.gameObject.transform.parent;
                if (_parent == null)
                    return;
                var _replacementCard = _parent.GetComponent<ReplacementCard>();
                if (_replacementCard != null)
                {
                    _replacementCard.AddCard();
                }
            }        
        }

        public void SetEnableImageCard(bool _enable)
        {
            myCard.imageCard.enabled = _enable;
            canMoving = _enable;
        }

        public void SetCanMoving()
        {
            canMoving = true;         
        }      

        // invoke event trigger
        public void MouseUp()
        {
            //  Invoke("ActivateReplacement",0.1f);
        }
        public void LoadNewCard()
        {
            var save = JsonUtility.FromJson<CardData>(PlayerPrefs.GetString("ActivCard"));
            InitCard((int)save.myColor, save.myNumCard, save.mySpecialCard);
            hud.AddTurn(-1);
            ManagerCombination.Instance.ClearMatchCells();   
        }

        public void CreateNewCard()
        {
            CardData _card;
            if (needReserveCard)
            {
                _card = PollCards.Instance.GetReserveCard();
                needReserveCard = false;
            }
            else
            {
                hud.AddTurn(-1);
                BonusManager.Instance.DeactivateBonus();
                _card = PollCards.Instance.GetNewCard();
            }
            if (_card == null)
                Debug.LogError("Create New Card == null");
            InitCard((int)_card.myColor, _card.myNumCard, _card.mySpecialCard);
            ManagerCombination.Instance.ClearMatchCells();
            ManagerCombination.Instance.SetLastConnect(lastConnect);
            ManagerClosedCard.Instance.UpCurrentProgress();
            turnBack.AddNewTurnInfo();
        } 

        public void SetReserveCardToPoll()
        {
            PollCards.Instance.SetReserveCard(myCard.Info.myColor,myCard.Info.myNumCard, myCard.Info.mySpecialCard);
            needReserveCard = true;
        }

        public void AppearanceInit(int color, int num, eSpecialCard _specialCard)
        {
            myCard.imageCard.raycastTarget = true;
            myCard.InitCard(color, num, _specialCard);
            RestartCard();
            myCard.StartAppearanceCard();
        } 

        public void InitCard(int color,int num,eSpecialCard _specialCard)
        {
            myCard.imageCard.raycastTarget = true;
            myCard.InitCard(color, num, _specialCard);
            RestartCard();
        }

        public void SetIsTouchTrue()
        {          
            if (isFlight && canMoving)
            {
                SoundManager.Instance.PlayBonusStart(eSpecialCard.free,false);
                canMoving = false;
                isFlight = false;
                StartCoroutine("FlightNewPosition", Input.mousePosition);
            }else
            {
                isTouch = true;
            }
        }

        public bool GetIsMoving()
        {
            return isMoving;
        }

        public void SetCanFlight()
        {
            isFlight = true;
            wings.SetActive(true);
        }

        private void ActivateReplacement()
        {
            isTouch = true;
            if (isTouch && !isMoving && !replacementCard.canSwap)
            {
                replacementCard.ActivateImageBack(true);
                myCard.ActivateImageBack(true);
                canSwap = true; 
            }
        }

        private void CurrentMoving()
        {
            if (isMoving)
            {
                if (myTransform.position.x < leftBorder.position.x) 
                {                
                    vectorMove.x *= -1;
                }
                if (myTransform.position.x > rightBorder.position.x)
                {                  
                    vectorMove.x *= -1;
                }
                if (myTransform.position.y < downBorder.position.y)
                {
                    vectorMove.y *= -1;
                }
                myTransform.position -= vectorMove * Time.deltaTime * currentSpeedMove;
                timeMoving += Time.deltaTime;
                if (timeMoving > 3.5f)
                {
                    CreateNewCard();
                }
            }
        }
            
        private void FindVectroMove()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (canMoving && isTouch)
                {
                    if (myCard.Info.mySpecialCard == eSpecialCard.Joker || myCard.Info.mySpecialCard.ToString().Contains("bullet"))
                        SoundManager.Instance.PlayBonusStart(myCard.Info.mySpecialCard, false);
                    VectorMove(Input.mousePosition);
                }
                if (isFlight && flightCell != null)
                {
                    FillCell(flightCell);
                    flightCell = null;
                    RestartCard();                  
                }           
            }   
        }     

        private void OnTriggerEnter2D(Collider2D obj)
        {
            if (isFlight)
            {
                HexagonalCell _cell = obj.GetComponent<HexagonalCell>();
                if (_cell != null && !_cell.GetIsFull())
                {
                    flightCell = _cell;
                }
            }
            if (isMoving)
            {
                HexagonalCell _cell = obj.GetComponent<HexagonalCell>();
                if (_cell != null)
                {
                    if (_cell.Info.myY == GlobalInfo.column - 1 && _cell.GetIsFull() && myCard.Info.mySpecialCard == eSpecialCard.none)
                    {
                        isMoving = false;
                        hud.Lose();
                        return;
                    }
                    CollisionCell(_cell);
                }
            }        
        }

        private void CollisionCell(HexagonalCell _cell)
        {
            if (_cell.GetIsFull())
            {
                if ((int)myCard.Info.mySpecialCard >= (int)eSpecialCard.bullet1 && (int)myCard.Info.mySpecialCard <= (int)eSpecialCard.bullet5)
                {
                    _cell.ShootingCard();
                    if (myCard.Info.mySpecialCard == eSpecialCard.bullet1)
                    {
                        CreateNewCard();
                        return;
                    }
                    else
                    {
                        myCard.Info.mySpecialCard = (eSpecialCard)((int)myCard.Info.mySpecialCard - 1);
                    } 
                }
                else
                {
                    HexagonalCell[] nearEmptyCell = HexagonalGrid.Instance.GetNearEmpyCells(_cell.Info.myX, _cell.Info.myY);
                    float[] distances = new float[nearEmptyCell.Length]; 
                    int min = 0;
                    float minvalue = 999;
                    for (int i=0;i< nearEmptyCell.Length;i++)
                    {
                        distances[i] = Vector3.Distance(transform.position, nearEmptyCell[i].transform.position);    
                        if(minvalue > distances[i])
                        {
                            min = i;
                            minvalue = distances[i];
                        }                   
                    }
                    FillCell(nearEmptyCell[min]);
                    return;
                }
            }          
            if (_cell.Info.myY == 0)
            {
                if (!_cell.GetIsFull()) { 
                    if(myCard.Info.mySpecialCard == eSpecialCard.none || myCard.Info.mySpecialCard == eSpecialCard.Joker)
                         FillCell(_cell);
                    else
                        CreateNewCard();
                }
                else
                {
                    CreateNewCard();
                }
            }           
        }    

        private void FillCell(HexagonalCell _cell)
        {
            /*      
            if ( HexagonalGrid.Instance.CheckSameCard(_cell.myX, _cell.myY, myCard.info))
            {              
                Debug.Log("same card");
                SoundManager.Instance.PlaySameCard();
                RestartCard(); 
            }
            else
            {                
            } 
            */
            lastConnect = _cell;
            _cell.InitCardInCell((int)myCard.Info.myColor, myCard.Info.myNumCard, myCard.Info.mySpecialCard);
            CreateNewCard();
        }

        private void RestartCard()
        {
            myTransform.position = startPos;
            canMoving = true;
            myCard.ActivateImageBack(false);
            replacementCard.ActivateImageBack(false);
            isMoving = false; 
            isTouch = false;
            isFlight = false;
            canMoving = true;
            backLight.gameObject.SetActive(false);
            wings.SetActive(false);
            timeMoving = 0;
        }

        private void VectorFlight(Vector3 _destination)
        {
            currentSpeedMove = slowSpeedMove * ((float)Screen.height / _destination.y);
            vectorMove = myTransform.position - _destination;
            isMoving = true;
            canMoving = false;
            isTouch = false;
        }

        private void VectorMove(Vector3 _destination)
        {
            if (_destination.y > 135)
            {
                SoundManager.Instance.PlayDealingCard();
                currentSpeedMove = slowSpeedMove * ((float)Screen.height / _destination.y);
                vectorMove = myTransform.position - _destination;
                isMoving = true;
                canMoving = false;
                isTouch = false;
                myCard.ActivateImageBack(false);
                replacementCard.ActivateImageBack(false);
                if ((int)myCard.Info.mySpecialCard < (int)eSpecialCard.bullet1 && (int)myCard.Info.mySpecialCard > (int)eSpecialCard.bullet5)
                    backLight.gameObject.SetActive(true);
            }
        }

        private IEnumerator FlightNewPosition(Vector3 _pos)
        {         
            for (;;)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, _pos, speedMoveFlight);
                float distance = Vector3.Distance(myTransform.position, _pos);
                if (distance < 15)
                    break;
                yield return new WaitForSeconds(0.01f);
            }
            FillCell(HexagonalGrid.Instance.GetEmptyCellFromNearVector(_pos));
        }
    }
}
