using UnityEngine;
using UnityEngine.EventSystems;
using Cards;
using UnityEngine.UI;
using System.Collections;

public class BonusItem : MonoBehaviour,  IDragHandler, IBeginDragHandler,IEndDragHandler {

    [SerializeField]
    private Image closed, open, bonus;
    private eSpecialCard mySpecial;
    private float speedMove = 2;
    private Vector3 posMove;
    private bool canMove;
    private BonusSlot destination;
    private Animator animator;

    private void Start()
    {
        animator = bonus.GetComponent<Animator>();
    }

    private void OnEnable()
    {
     //   StopCoroutine("DeactivateTransparent");
     //   CancelInvoke("Deactivate");
    }

    private void Update()
    {
        if (canMove)
        {
            bonus.transform.position = Vector3.Lerp(bonus.transform.position, posMove, speedMove * Time.deltaTime);
            float distance = Vector3.Distance(bonus.transform.position, posMove);
            if (distance < 30)
            {
                destination.InitSlot(mySpecial);
                canMove = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void Init(eSpecialCard _special)
    {     
        Color _color = bonus.color;
        _color.a = 1;
        bonus.color = _color;
        mySpecial = _special;
        InitBonusInfo(_special);
        StartCoroutine("ActivateChest");
    }

    public void MoveFreeSlot(BonusSlot slot)
    {
        posMove = slot.transform.position;
        destination = slot;
        Invoke("InvokeMoveFreeSlot",1);
    } 

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    } 

    public void OnBeginDrag(PointerEventData eventData)
    {
        bonus.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bonus.raycastTarget = true;
        var obj = eventData.pointerCurrentRaycast;
        if(obj.gameObject.name == "ImageCell")
        {
            BonusSlot _slot = obj.gameObject.transform.parent.GetComponent<BonusSlot>();
            if (_slot != null)
            {
                _slot.SwapSlot(mySpecial);
          //      StopCoroutine("DeactivateTransparent");
           //     CancelInvoke("Deactivate");
                gameObject.SetActive(false);
            }
        }
    }  

    public void InvokeDeactivate()
    {
    //    Invoke("Deactivate",5);
    }

    public void InitBonusInfo(eSpecialCard _specialCard)
    {
        string path = "";
        bonus.transform.localPosition = Vector3.zero;
        if (animator == null)
            animator = bonus.GetComponent<Animator>();
        animator.enabled = false;
        if (_specialCard == eSpecialCard.Joker)
        {
            int rand = Random.Range(0, 4);
            path = "Card/Special/" + _specialCard.ToString() + rand.ToString();
        }
        else if (_specialCard == eSpecialCard.chameleon)
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
            return;
        }
        else if (_specialCard == eSpecialCard.wind2)
        {
            if (animator != null)
            {
                animator.enabled = true;
                animator.Play("Wind2");
            }
            return;
        }
        else
        {
            path = "Card/Special/" + _specialCard.ToString();
        }
        Sprite _sprite = Resources.Load<Sprite>(path);
        bonus.sprite = _sprite;
    }

    private void InvokeMoveFreeSlot()
    {        
        canMove = true;
    }

    private IEnumerator ActivateChest()
    {
        bonus.enabled = false;
        closed.enabled = true;
        Color _color = closed.color;
        for (float f = 0; f <= 1; f += 0.02f)
        {
            _color.a = f;
            closed.color = _color;
            yield return new WaitForSeconds(0.01f);
        }
        closed.enabled = false;
        bonus.enabled = true;
    }

    public void Deactivate()
    {
        StartCoroutine("DeactivateTransparent");
    }

    private IEnumerator DeactivateTransparent()
    {
        Color _color = bonus.color;
        for (float f = 1; f > 0; f -= 0.01f)
        {          
            _color.a = f;
            bonus.color = _color;
            yield return new WaitForSeconds(0.01f);
        }    
        _color.a = 1;
        bonus.color = _color;
        gameObject.SetActive(false);
    }

   
}
