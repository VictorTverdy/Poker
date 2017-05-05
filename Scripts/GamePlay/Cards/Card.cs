using Combination;
using Hexagonal;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Cards
{
    public class Card : MonoBehaviour
    {
        public delegate void OnEndEvent();
        protected OnEndEvent callbackFct;
      
        public Image imageCard;
        [SerializeField]
        private Image imageBack, bulletHole;
        [SerializeField]
        private Text textCard;

        public CardData Info;

        private float speedFalling = 10f;
        private bool isInverted;
        private float speedMove = 0.05f;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent <Animator>();
            if(animator != null)
               animator.enabled = false;
        }

        public void SetColorCard(Color _color)
        {
            imageCard.color = _color;
        }

        public void ActivateImageBack(bool isActivate)
        {
            if (imageBack != null)
                imageBack.enabled = isActivate;
        }

        public Transform GetTransformImageCard()
        {
            return imageCard.transform;
        }

        public void IncreaseCard()
        {
            StartCoroutine("SetIncreaseCard");
        }

        public void DecreaseCard()
        {
            StartCoroutine("SetDecreaseCard");     
        }

        public string InitCard(int _color, int _num, eSpecialCard _specialCard)
        {
            if (tag == "CellLevel")
            {
                StopAllCoroutines();
                imageCard.transform.localEulerAngles = Vector3.zero;
                imageCard.transform.localPosition = Vector3.zero;
                imageCard.transform.localScale = Vector3.one;
            }
            InitInfoCard(_color,_num, _specialCard);
            return InitBonusInfo(Info.mySpecialCard);
        }   

        public void InitInfoCard(int _color, int _num, eSpecialCard _specialCard)
        {
            eCards _Mycolor = (eCards)_color;
            Info.myColor = _Mycolor;
            Info.myNumCard = _num;
            Info.mySpecialCard = _specialCard;
        }

        public void AnimatorDesable()
        {
            if (animator != null)
                animator.enabled = false;
        }

        public string InitBonusInfo(eSpecialCard _specialCard)
        {        
            string path = "Card/" + Info.myColor.ToString() + "/" + Info.myNumCard.ToString();
            if (_specialCard != eSpecialCard.none)
            {
                if (_specialCard == eSpecialCard.Joker)
                {
                    int rand = Random.Range(0, 4);
                    path = "Card/Special/" + _specialCard.ToString() + rand.ToString();
                } else if (_specialCard == eSpecialCard.chameleon)
                {
                    int rand = Random.Range(1, 3);
                    path = "Card/Special/" + _specialCard.ToString() + rand.ToString();
                }
                else if (_specialCard == eSpecialCard.wind1)
                {
                    if (animator != null)
                    {
                        animator.enabled = true;
                        animator.Play("Wind1");
                    }
                    imageCard.enabled = true;
                    return null;
                }
                else if (_specialCard == eSpecialCard.wind2)
                {
                    if (animator != null)
                    {
                        animator.enabled = true; 
                        animator.Play("Wind2");
                    }
                    imageCard.enabled = true;
                    return null;
                }
                else
                {
                    path = "Card/Special/" + _specialCard.ToString();
                }
            }
            Sprite _sprite = Resources.Load<Sprite>(path);
            if (_sprite != null)
            {
                imageCard.enabled = true;
                imageCard.sprite = _sprite;
                textCard.text = "";
            }
            else
            {
                imageCard.enabled = false;
                if (_specialCard == eSpecialCard.none)
                {
                    textCard.text = "";
                }
                else
                {
                    textCard.text = _specialCard.ToString();
                }
            }
            return path;
        }

        public void SetClosed(bool _isClosed)
        { 
            Info.isClosed = _isClosed;
        }

        public void SetIsClosed(bool _isClosed)
        {
            if (_isClosed)
            {
                InitClosed();
                StartCoroutine("ActivateClosedCard");
            }
            else
            {
                bulletHole.enabled = false;
                Info.isClosed = false;
                StartCoroutine("RevertClosedCard");
            }
        }

        public string InitClosed()
        {
            imageCard.enabled = true;
            bulletHole.enabled = false;
            Info.isClosed = true;
            Sprite ClosedSprite = Resources.Load<Sprite>("Card/closed"); ;
            imageCard.sprite = ClosedSprite;
            return "Card/closed";
        }     

        public bool GetIsClosed()
        {
            return Info.isClosed;
        }

        public bool GetIsInverted()
        {
            return isInverted;
        }

        public void StartShootCard()
        {
            bulletHole.enabled = true;
            int randomHole = Random.Range(1, 3);
            bulletHole.sprite = Resources.Load<Sprite>("bulletHole" + randomHole.ToString());
            int randomX = Random.Range(-20, 20);
            int randomY = Random.Range(-15, 15);
            bulletHole.transform.localPosition = new Vector3(randomX, randomY, 0);
            StartCoroutine("ShootFallingCard");
        }

        public void StartAppearanceCard()
        {            
            StartCoroutine("AppearanceCard");
        }

        public void StartWindFalling(Vector3 _pos)
        {
            StartCoroutine("WindFallingCard", _pos);
        }

        public void StartFalling()
        {
            StartCoroutine("FallingCard");
        }

        public void SetMovePositionCallBack(Vector3 _pos, OnEndEvent CallBackFunction)
        {
            callbackFct = CallBackFunction;
            StartCoroutine("MoveNewPosition", _pos);
        }      

        public void SetMovePosition(Vector3 _pos)
        {
            StartCoroutine("MovePosition", _pos);
        }

        public void ClearFromCell()
        {
            transform.localPosition = Vector3.zero;
            Info.isClosed = false;
            imageCard.enabled = false;
            Info.myColor = eCards.none;
            Info.myNumCard = 0;
            textCard.text = "";
            StopAllCoroutines();
        }

        public void InitCoordinate(int _x, int _y)
        {
            Info.myX = _x;
            Info.myY = _y;
        }

        private void OnTriggerEnter2D(Collider2D obj)
        {
            if(tag == "special")
            {
                HexagonalCell _cell = obj.GetComponent<HexagonalCell>();
                if (_cell != null && _cell.GetIsFull())
                {
                    eSpecialCard currentBonus = BonusManager.Instance.GetCurrentActivSlot().GetBonus();                  
                    int step = 0;
                    if (currentBonus == eSpecialCard.wind1)
                        step = 1;
                    if (currentBonus == eSpecialCard.wind2)
                        step = 2;
                    if (currentBonus == eSpecialCard.wind3)
                        step = 3;
                    if (step == 0)
                        return; 
                    StopCoroutine("MoveNewPosition");
                    imageCard.transform.localPosition = Vector3.zero;
                    HexagonalGrid.Instance.WindFallNearCard(_cell.Info.myX, _cell.Info.myY, step);
                    BonusManager.Instance.ClearCurrentBonusSlot();
                    ManagerCombination.Instance.ClearMatchCells();
                    FindObjectOfType<ActivCardMoving>().SetEnableImageCard(true);
                }
            }          
        }

        private IEnumerator SetIncreaseCard()
        {
            Vector3 pos = new Vector3(imageCard.transform.position.x, imageCard.transform.position.y +110, imageCard.transform.position.z);
            GameObject clone = Instantiate(imageCard.gameObject, pos,Quaternion.identity,imageCard.transform) as GameObject;
            string path = "Card/" + Info.myColor.ToString() + "/" + (Info.myNumCard+1).ToString();
            clone.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
            imageCard.gameObject.AddComponent<Mask>();
            for (; ; )
            {
                if (clone.transform.localPosition.y < 1)
                    break;
                clone.transform.localPosition += Vector3.down * 2f;
                yield return new WaitForSeconds(0.01f);
            }
            Destroy(clone);
            Destroy(imageCard.gameObject.GetComponent<Mask>());        
            InitCard((int)Info.myColor, Info.myNumCard + 1, eSpecialCard.none);
            ManagerCombination.Instance.ClearMatchCells();
        }

        private IEnumerator SetDecreaseCard()
        {
            Vector3 pos = new Vector3(imageCard.transform.position.x, imageCard.transform.position.y - 110, imageCard.transform.position.z);
            GameObject clone = Instantiate(imageCard.gameObject, pos, Quaternion.identity, imageCard.transform) as GameObject;
            string path = "Card/" + Info.myColor.ToString() + "/" + (Info.myNumCard - 1).ToString();
            clone.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
            imageCard.gameObject.AddComponent<Mask>();
            for (;;)
            {
                if (clone.transform.localPosition.y > -1)
                    break;
                clone.transform.localPosition -= Vector3.down * 2f;
                yield return new WaitForSeconds(0.01f);
            }
            Destroy(clone);
            Destroy(imageCard.gameObject.GetComponent<Mask>());
            InitCard((int)Info.myColor, Info.myNumCard - 1, eSpecialCard.none);
            ManagerCombination.Instance.ClearMatchCells();
        }

        private IEnumerator UpendCard()
        { 
            for (float f = 180; f >= 0; f -= 6f)
            {
                imageCard.transform.eulerAngles = new Vector3(0, 0, f);
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator AppearanceCard()
        {
            Color _c = imageCard.color;
            for (float f = 0; f <= 1; f += 0.01f)
            {
                _c.a = f;
                imageCard.color = _c;
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator RevertClosedCard()
        {
            for (float f = 0; f <= 90; f += 3f)
            {
                imageCard.transform.eulerAngles = new Vector3(0, f, 0);
                yield return new WaitForSeconds(0.01f);
            }
            string path = "Card/" + Info.myColor.ToString() + "/" + Info.myNumCard.ToString();
            Sprite _sprite = Resources.Load<Sprite>(path);
            imageCard.enabled = true;
            imageCard.sprite = _sprite;
            for (float f = 90; f >= 0; f -= 3f)
            {
                imageCard.transform.eulerAngles = new Vector3(0, f, 0);
                yield return new WaitForSeconds(0.01f);
            }
            InitCard((int)Info.myColor, Info.myNumCard, Info.mySpecialCard);
            ManagerCombination.Instance.ClearMatchCells();
        }

        private IEnumerator MovePosition(Vector3 _pos)
        {
            for (;;)
            {
                imageCard.transform.position = Vector3.Lerp(imageCard.transform.position, _pos, speedMove);
                float distance = Vector3.Distance(imageCard.transform.position, _pos);
                if (distance < 15)
                    break;
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator MoveNewPosition(Vector3 _pos)
        {
            for (; ; )
            {
                imageCard.transform.position = Vector3.Lerp(imageCard.transform.position, _pos, speedMove);
                float distance = Vector3.Distance(imageCard.transform.position, _pos);
                if (distance < 20)
                    break;
                yield return new WaitForSeconds(0.01f);
            }
            imageCard.transform.localPosition = Vector3.zero;
            callbackFct();          
        }  

        private IEnumerator ActivateClosedCard()
        {
            for (float f = 0; f <= 1; f += 0.03f)
            {
                imageCard.transform.localScale =  new  Vector3(f,f,1); 
                yield return new WaitForSeconds(0.01f);
            }                      
        }

        private IEnumerator ShootFallingCard()
        {
            for (float f = 0; f <= 180; f += 5f)
            {
                imageCard.transform.eulerAngles = new Vector3(0, 0, f);
                yield return new WaitForSeconds(0.01f);
            }
            bulletHole.enabled = false;
            ClearFromCell();
        }

        private IEnumerator WindFallingCard(Vector3 _pos)
        {
            for (float f = 0; f < 1; f += 0.02f)
            {
                imageCard.transform.localPosition = Vector3.Lerp(imageCard.transform.localPosition, _pos, speedMove);
                imageCard.transform.Rotate(5, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            ClearFromCell();
        }

        private IEnumerator FallingCard()
        {
            for (float f = 0; f < 2; f += 0.05f)
            {
                imageCard.transform.localPosition += Vector3.down * speedFalling;               
                yield return new WaitForSeconds(0.01f);
            }
            ClearFromCell();
        }

    }
}
