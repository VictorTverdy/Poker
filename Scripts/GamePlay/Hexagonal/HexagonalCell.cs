using UnityEngine;
using UnityEngine.UI;
using Cards;
using Combination;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Hexagonal {
    public class HexagonalCell : MonoBehaviour,IPointerEnterHandler,IPointerDownHandler,IPointerUpHandler
    {
        [SerializeField]
        private Card myCard;
        [SerializeField]
        private Image imageCell, imageFlashing;
        [SerializeField]
        private Text textScore;
        [SerializeField]
        private ParticleSystemRenderer particleMat;

        public CellData Info;
        private Camera cameraEffect;
        private bool canCombination;
        private eCombination currentMaxCombination;
        private List <eCombination> allCombination = new List<eCombination>();
        private float timeDoubleClick;
        private Animator animator;
        private int currentColorCombination;
        private Vector3 maxSize;

        private void Start()
        {
            cameraEffect = GameObject.Find("CameraEffect").GetComponent<Camera>();
            animator = imageFlashing.gameObject.GetComponent<Animator>();
        }

        public void InitCoordinate(int x,int y)
        {
            Info.myX = x;
            Info.myY = y;            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Info.IsFull)
                BonusManager.Instance.SetCellSwapDown(GetComponent<HexagonalCell>());
        }
   
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0) && Info.IsFull)
                BonusManager.Instance.SetCellSwapEnter(GetComponent<HexagonalCell>());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(GlobalInfo.canCombination)
               AutoRemoveCombination();
        }      

        public void AutoRemoveCombination()
        {
            BonusManager.Instance.SetCellOnClick(GetComponent<HexagonalCell>());
            float curretTime = Time.time;
            if (curretTime - timeDoubleClick < 0.5f)
            {
                if (Info.IsFull && canCombination && currentMaxCombination != eCombination.none)
                    ManagerCombination.Instance.AutoCombinationCheck(this); 
            }
            timeDoubleClick = curretTime;
        } 
        
        public void SetFlashingBack(bool canFlashing)
        {
            imageFlashing.gameObject.SetActive(canFlashing);
            imageFlashing.color = Color.white;
        }      

        public bool GetIsFull()
        {
            return Info.IsFull;
        }     

        public void ShootingCard()
        {
            if (Info.IsFull)
            {
                ClearCell(); 
                myCard.StartShootCard();
                ManagerCombination.Instance.AddScoreFromDestroy();
            }
        }

        public void WindFallingCard(Vector3 _pos)
        {
            if (Info.IsFull)
            {              
                ClearCell();
                myCard.StartWindFalling(_pos);
                ManagerCombination.Instance.AddScoreFromDestroy();
            }
        }

        public void FallingCard()
        {
            ClearCell();
            myCard.StartFalling();
            ManagerCombination.Instance.AddScoreFromDestroy();
        }

        public void AddCombination(eCombination _combination)
        {
            allCombination.Add(_combination);
            allCombination.Sort();
            if ((int)currentMaxCombination > (int)_combination)
                return;             
            canCombination = true;       
            currentMaxCombination = _combination;
        //    imageFlashing.gameObject.SetActive(true);
        //    SetNewColor(_combination);
        }

        public void FlashingCoroutine(eCombination _combination)
        {
            StopCoroutine("BrightFlash");
            StopCoroutine("FadingFlash");
            StopCoroutine("FlashingRoyalFlesh");
            if (_combination == eCombination.royalFlush)
            {
                StartCoroutine("FlashingRoyalFlesh");
            }
            else
            {
                imageFlashing.color = SetNewColor(_combination);
                StartCoroutine("BrightFlash");
            }       
        }

        public void NoFlashing()
        {
            allCombination.Clear();
            currentMaxCombination = eCombination.none;
            canCombination = false;
     //       imageFlashing.gameObject.SetActive(false);
        }

        public void SetEnableCardImage(bool isEnable)
        {
            imageCell.enabled = isEnable;
        }

        public void ClearFromCard(int score,eCombination deleteCombination)
        {
            particleMat.gameObject.SetActive(false);
            Vector3 p = cameraEffect.ScreenToWorldPoint(transform.position);
            p.z = 0;
            particleMat.transform.position = p;
            particleMat.gameObject.SetActive(true);         
            myCard.ClearFromCell();
            textScore.text = score.ToString();
            maxSize = GetScaleColor(deleteCombination);
            textScore.color = SetNewColor(deleteCombination);
            StartCoroutine("ShowTextScore"); 
            ClearCell();
        }

        public eCombination[] GetAllCombination()
        {
            return allCombination.ToArray();
        }

        public eCombination GetCurrentCombination()
        {
            return currentMaxCombination;
        }

        public void InitCardInCellWithCheckClosed(int _color, int _num, eSpecialCard _specialCard,bool isClosed)
        {    
            if(_num == 0 || _color ==0)
            {
                imageCell.enabled = false;
                imageFlashing.gameObject.SetActive(false);
                Info.IsFull = false;
                myCard.ClearFromCell();
                return;
            }
            imageCell.enabled = true;
            imageFlashing.gameObject.SetActive(false);
            Info.IsFull = true;
            if (!isClosed)
            {
                myCard.SetClosed(false);
                string path = myCard.InitCard(_color, _num, _specialCard);
                particleMat.material.mainTexture = Resources.Load<Texture>(path);
            }
            else
            {
                string path = myCard.InitClosed();
                myCard.InitInfoCard(_color, _num, _specialCard);
                particleMat.material.mainTexture = Resources.Load<Texture>(path);
            }
            myCard.InitCoordinate(Info.myX, Info.myY);
        }

        public void InitCardInCell(int _color,int _num,eSpecialCard _specialCard)
        {         
            imageCell.enabled = true;
            imageFlashing.gameObject.SetActive(false);
            Info.IsFull = true;             
            string path = myCard.InitCard(_color, _num, _specialCard);
            particleMat.material.mainTexture = Resources.Load<Texture>(path);
            myCard.InitCoordinate(Info.myX, Info.myY);
        }      

        public Card GetCard()
        {
            return myCard;
        }

        public bool CheckCardData(CardData _info)
        {
            if (Info.IsFull &&_info.myColor == myCard.Info.myColor && _info.myNumCard == myCard.Info.myNumCard)
            {
                return true;
            }
            return false;
        }   

        public void ClearCellAndCard()
        { 
            ClearCell();
            myCard.ClearFromCell();
        }

        public void ChangeColor()
        {
            if(allCombination.Count > 1)
            {
                currentColorCombination++;
                if (currentColorCombination >= allCombination.Count )
                {
                    currentColorCombination = 0;
                }
                imageFlashing.color = SetNewColor(allCombination[currentColorCombination]);
            }
        }

        private void SetNewColorRainbow(int _num)
        {
            if (_num == 0)
            {
                imageFlashing.color = Color.red;
            }else if (_num == 1)
            {
                imageFlashing.color = new Color(1, 0.55f, 0);
            }
            else if (_num == 2)
            {
                imageFlashing.color = Color.yellow;
            }
            else if (_num == 3)
            {
                imageFlashing.color = Color.green;
            }
            else if (_num == 4)
            {
                imageFlashing.color = Color.cyan;
            }
            else if (_num == 5)
            {
                imageFlashing.color = Color.blue;
            }
            else if (_num == 6)
            {
                imageFlashing.color = Color.magenta;
            }
        }

        private Vector3 GetScaleColor(eCombination _combination)
        {
            float _scale = 0;
            if (_combination == eCombination.pair)
            {
                _scale = 0.6f;
            }
            else if (_combination == eCombination.twoPairs)
            {
                _scale = 0.65f;
            }
            else if (_combination == eCombination.set)
            {
                _scale = 0.7f;
            }
            else if (_combination == eCombination.straight)
            {
                _scale = 0.75f;
            }
            else if (_combination == eCombination.flash)
            {
                _scale = 0.8f;
            }
            else if (_combination == eCombination.fullHouse)
            {
                _scale = .85f;
            }
            else if (_combination == eCombination.quads)
            {
                _scale = .9f;
            }
            else if (_combination == eCombination.straightFlush)
            {
                _scale = .95f;
            }
            else if (_combination == eCombination.royalFlush)
            {
                _scale = 1f;
            }           
            return new Vector3(_scale , _scale, _scale);
        }


        private Color SetNewColor(eCombination _combination)
        {
            if (_combination == eCombination.pair)
            {               
                return Color.red;
            }
            else if (_combination == eCombination.twoPairs)
            {               
                return new Color(1, 0.55f, 0);//orange   
            }
            else if (_combination == eCombination.set)
            {              
                return Color.yellow;
            }
            else if (_combination == eCombination.straight)
            {               
                return new Color(.55f, 1, 0);//lightGreen 
            }
            else if (_combination == eCombination.flash)
            {               
                return new Color(0, 0.6f, 0);//green
            }
            else if (_combination == eCombination.fullHouse)
            {              
                return Color.cyan;
            }
            else if (_combination == eCombination.quads)
            {              
                return Color.blue;
            }
            else if (_combination == eCombination.straightFlush)
            {               
                return Color.magenta;
            }
            else if (_combination == eCombination.royalFlush)
            {              
                return Color.white;
            }
            else
            {
                Debug.LogError("unknown  color " + _combination.ToString());
                return Color.white;
            }        
        }

        private void ClearCell() 
        {
            Info.IsFull = false;
            currentMaxCombination = eCombination.none;
            allCombination.Clear();
            canCombination = false;
            imageCell.enabled = false;
            imageFlashing.gameObject.SetActive(false);
        }

        private IEnumerator ShowTextScore()
        {
            Color a = textScore.color;
            Vector3 _scale = new Vector3();
            for (float i = 0.3f; i <= 1; i += 0.01f)
            {
                if (i <= maxSize.x)
                {
                    _scale.x = i;
                    _scale.y = i;
                    _scale.x = i;
                    textScore.transform.localScale = _scale;
                }
                a.a = i;                    
                textScore.color = a;
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.2f);
            for (float i = 1; i >= 0; i -= 0.02f)
            {
                a.a = i;
                textScore.color = a;
                yield return new WaitForSeconds(0.01f);
            }
            textScore.text = "";
        }

        private IEnumerator FlashingRoyalFlesh()
        {
            float step = 0.08f;
            imageFlashing.gameObject.SetActive(true);
            for (int rainbow = 0; rainbow < 7; rainbow++) {
               SetNewColorRainbow(rainbow);
                Color _a = imageFlashing.color;
                for (float i = 0; i <= 1; i = i + step)
                {
                    _a.a = i;
                    imageFlashing.color = _a;
                    yield return new WaitForSeconds(0.01f);
                }
                for (float i = 1; i >= 0; i = i - step)
                {
                    _a.a = i;
                    imageFlashing.color = _a;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            imageFlashing.gameObject.SetActive(false);
        }

        private IEnumerator FadingFlash()
        { 
            Color _a = imageFlashing.color;
            for (float i = 1; i >= 0; i = i - 0.02f)
            { 
                _a.a = i;
                imageFlashing.color = _a;
                yield return new WaitForSeconds(0.01f);
            }         
            imageFlashing.gameObject.SetActive(false);
        } 

        private IEnumerator BrightFlash()
        {
            imageFlashing.gameObject.SetActive(true);
            Color _a = imageFlashing.color;
            for (float i=0; i <= 1; i = i + 0.02f)
            {
                _a.a = i;
                imageFlashing.color = _a;
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.3f); 
            StartCoroutine("FadingFlash");  
        }       
    }
}
