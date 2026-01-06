using UnityEngine;
using UnityEngine.EventSystems;

public class TableClick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler,IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool inHand = false;
    public Transform originalPosition;
    public Transform handPosition;
    public OldHand hand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    void Update() {
        // if (inHand && Input.GetKeyDown(KeyCode.LeftShift)) {
        //     transform.position = originalPosition.position;
        //     inHand = false;
        //     hand.pickedCard = null;
        // }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hand.pickedCard != null) {
            hand.pickedCard.transform.position = eventData.pointerCurrentRaycast.worldPosition + 0.1f * Vector3.up; // TODO: fix
            hand.pickedCard.GetComponent<CardClick>().held = false;
            hand.pickedCard.GetComponent<CardClick>().onBoard = true;
            hand.pickedCard.transform.parent = transform;
            hand.pickedCard = null;
            hand.inHand = false;
        } 
        // eventData.pointerDrag = gameObject;
        // Debug.Log("Pointer Clicked");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("Pointer Down");
        
        // if (inHand)
        // {
        //     transform.position = eventData.pointerCurrentRaycast.worldPosition;
        //     inHand = false;
        // }
        // else
        // {
        //     transform.position = handPosition.position;
        //     inHand = true;
        // }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("Pointer Up");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("On Pointer Enter" + eventData.pointerDrag);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("Pointer Exited");
    }
}
