using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

// https://www.youtube.com/watch?v=I1dAZuWurw4
// https://github.com/mixandjam/balatro-feel

public class CardMovement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image _imageComponent;
    
    private Vector3 offset; 

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 3000; 

    [Header("Selection")]
    public bool selected;
    public float selectionOffset = 50;
    private float _pointerDownTime;
    private float _pointerUpTime;

    [Header("Visual")]
    public CardAnimator cardAnimator;

    [Header("Play Area Threshold")]
    [Tooltip("How high up the screen we the spawn the pin, in pixels")]
    [SerializeField] private float playAreaThresholdY = 500f; 
    
    [HideInInspector] public bool isPreviewingInWorld = false;

    [Header("States")]
    public bool isHovering;
    public bool isDragging;
    [HideInInspector] public bool wasDragged;

    [Header("Events")]
    [HideInInspector] public UnityEvent<CardMovement> PointerEnterEvent;
    [HideInInspector] public UnityEvent<CardMovement> PointerExitEvent;
    [HideInInspector] public UnityEvent<CardMovement, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<CardMovement> PointerDownEvent;
    [HideInInspector] public UnityEvent<CardMovement> BeginDragEvent;
    [HideInInspector] public UnityEvent<CardMovement> EndDragEvent;
    [HideInInspector] public UnityEvent<CardMovement, bool> SelectEvent;
    
    private void Awake()
    {
        _imageComponent = GetComponent<Image>();
        if (cardAnimator == null) cardAnimator = GetComponentInChildren<CardAnimator>();
    }

    void Start()
    {
        if (cardAnimator != null)
            cardAnimator.Initialize(this);
    }

    // Fires the enter event and marks the card as hovering.
    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    // Fires the exit event and removes the hovering state.
    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
        isHovering = false;
    }

    // Select a card
    // Records the start time of a left mouse click for tap vs. drag detection.
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            Debug.LogError("returned");
            return;
        }

        PointerDownEvent.Invoke(this);
        _pointerDownTime = Time.time;
    }

    // Determines if a click was a quick tap to toggle selection and fires relevant events.
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _pointerUpTime = Time.time;

        PointerUpEvent.Invoke(this, _pointerUpTime - _pointerDownTime > .2f);

        if (_pointerUpTime - _pointerDownTime > .2f)
            return;

        if (wasDragged)
            return;

        selected = !selected;
        SelectEvent.Invoke(this, selected);
        
        // Card selected here
        if (GridManager.Instance != null)
        {
            GridManager.Instance.ToggleSquare(this, selected);
        }

        if (selected)
            transform.localPosition += (cardAnimator.transform.up * selectionOffset);
        else
            transform.localPosition = Vector3.zero;
    }

    // Returns the total number of sibling slots in the parent container.
    public int SiblingAmount()
    {
        return transform.parent != null ? transform.parent.childCount - 1 : 0;
    }

    // Gets the current index of this card's UI slot within its parent container.
    public int ParentIndex()
    {
        return transform.parent != null ? transform.GetSiblingIndex() : 0;
    }

    // Calculates the card's relative position (0.0 to 1.0) within the hand.
    public float NormalizedPosition()
    {
        return transform.parent != null && SiblingAmount() > 0 ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)SiblingAmount(), 0, 1) : 0;
    }

    // Cleans up the linked visual animator object when this card is destroyed.
    private void OnDestroy()
    {
        // Fallback cleanup: If the card is completely destroyed while selected, ensure its square gets deleted too
        if (selected && GridManager.Instance != null)
        {
            GridManager.Instance.ToggleSquare(this, false);
        }
    } 
}