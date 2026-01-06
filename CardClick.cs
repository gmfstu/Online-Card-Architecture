using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CardClick : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler,IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform originalPosition;
    public Transform handPosition;
    public OldHand hand;
    public bool held = false;
    public bool onBoard = true;
    private Vector3 offset = new Vector3(0.06f, 0.0025f, 0.04f); 
    // public Transform cam;
    private List<Transform> cardsStacked = new List<Transform>();
    public bool stacked = false;
    private Vector3 stackedViewPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    void Update() {
        if (hand.inHand && Input.GetKeyDown(KeyCode.LeftShift)) {
            transform.position = originalPosition.position;
            hand.inHand = false;
            hand.pickedCard = null;
        }

        if (Input.GetKeyDown(KeyCode.T) && onBoard && stackedViewPosition != Vector3.zero) {
            transform.position = stackedViewPosition;
            stackedViewPosition = Vector3.zero;
        }
        // if (held) {
        //     transform.rotation = Quaternion.LookRotation(cam.position);
        // } else {
        //     transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        // }
    }

    public void DisplayView() {
        stackedViewPosition = transform.position;;
        foreach (Transform card in GetComponentInChildren<Transform>()) {
            card.GetComponent<CardClick>().DisplayView();
        }
        if (transform.parent.tag == "Slot") {

        } else {
            transform.position += Vector3.right * 0.1f; 
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onBoard && Input.GetKey(KeyCode.R)) {
            DisplayView();
        }
        else if (hand.inHand) {
            Debug.Log("here");
            hand.pickedCard.GetComponent<CardClick>().held = false;
            Debug.Log(hand.pickedCard);
            hand.pickedCard.transform.position = transform.position + offset;
            hand.pickedCard.transform.parent = transform;
            cardsStacked.Add(hand.pickedCard.transform);
            hand.pickedCard = null;
            hand.inHand = false;
        } else {
            transform.parent = hand.transform;
            transform.position = handPosition.position;
            hand.inHand = true;
            hand.pickedCard = gameObject;
            held = true;
        }
        // eventData.pointerDrag = gameObject;
        // Debug.Log("Pointer Clicked");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("Pointer Down");
    }

    public void OnBeginDrag(PointerEventData eventData) {
        // BeginDragEvent.Invoke(this); // this event exists to tell the other scripts the card is being dragged (cardvisual and horizontal card layout scripts)
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // TODO this but its 3d
    }

    public void OnDrag(PointerEventData eventData) {

    }

    public void OnEndDrag(PointerEventData eventData) {

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
