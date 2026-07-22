using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

//https://www.youtube.com/watch?v=I1dAZuWurw4
//https://github.com/mixandjam/balatro-feel

public class CardAnimator : MonoBehaviour
{
    private bool initalize = false;

    [Header("Card")]
    public CardMovement parentCardMovement;
    private Transform cardTransform;
    private Vector3 rotationDelta;
    private int savedIndex;
    Vector3 movementDelta;
    private Canvas canvas;

    [Header("References")]
    public Transform visualShadow;
    private float shadowOffset = 20;
    private Vector2 shadowDistance;
    private Canvas shadowCanvas;
    [SerializeField] private Transform shakeParent;
    [SerializeField] private Transform tiltParent;
    [SerializeField] private Image cardImage;

    [Header("Follow Parameters")]
    [SerializeField] private float followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 20;
    [SerializeField] private float rotationSpeed = 20;
    [SerializeField] private float autoTiltAmount = 30;
    [SerializeField] private float manualTiltAmount = 20;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] private bool scaleAnimations = true;
    [SerializeField] private float scaleOnHover = 1.15f;
    [SerializeField] private float scaleOnSelect = 1.25f;
    [SerializeField] private float scaleTransition = .15f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    [Header("Select Parameters")]
    [SerializeField] private float selectPunchAmount = 20;

    [Header("Hober Parameters")]
    [SerializeField] private float hoverPunchAngle = 5;
    [SerializeField] private float hoverTransition = .15f;

    [Header("Swap Parameters")]
    [SerializeField] private bool swapAnimations = true;
    [SerializeField] private float swapRotationAngle = 30;
    [SerializeField] private float swapTransition = .15f;
    [SerializeField] private int swapVibrato = 5;

    [Header("Curve")]
    [SerializeField] private CurveParameters curve;

    private float curveYOffset;
    private float curveRotationOffset;
    private Coroutine pressCoroutine;

    private int _currentHandLength;
    private void Start()
    {
        shadowDistance = visualShadow.localPosition;
    }

    public void Initialize(CardMovement target, int index = 0)
    {
        //Declarations
        parentCardMovement = target;
        cardTransform = target.transform;
        canvas = GetComponent<Canvas>();
        shadowCanvas = visualShadow.GetComponent<Canvas>();

        //Event Listening
        parentCardMovement.PointerEnterEvent.AddListener(PointerEnter);
        parentCardMovement.PointerExitEvent.AddListener(PointerExit);
        parentCardMovement.BeginDragEvent.AddListener(BeginDrag);
        parentCardMovement.EndDragEvent.AddListener(EndDrag);
        parentCardMovement.PointerDownEvent.AddListener(PointerDown);
        parentCardMovement.PointerUpEvent.AddListener(PointerUp);
        parentCardMovement.SelectEvent.AddListener(Select);

        //Initialization
        initalize = true;
    }

    public void UpdateLength(int length)
    {
        _currentHandLength = length;
    }
    
    private void HandPositioning(int length)
    {
        curveYOffset = (curve.positioning.Evaluate(parentCardMovement.NormalizedPosition()) * curve.positioningInfluence) * _currentHandLength;
    }

    void Update()
    {
        if (!initalize || parentCardMovement == null) return;

        HandPositioning();
        SmoothFollow();
        FollowRotation();
        CardTilt();

    }

    private void HandPositioning()
    {
        // Replaced parentCardMovement.SiblingAmount() with currentHandLength
        curveYOffset = (curve.positioning.Evaluate(parentCardMovement.NormalizedPosition()) * curve.positioningInfluence) * _currentHandLength;
        curveYOffset = _currentHandLength < 5 ? 0 : curveYOffset;
        curveRotationOffset = curve.rotation.Evaluate(parentCardMovement.NormalizedPosition());
    }

    
    private void SmoothFollow()
    {
        Vector3 verticalOffset = (Vector3.up * (parentCardMovement.isDragging ? 0 : curveYOffset));
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, followSpeed * Time.deltaTime);
    }

    private void FollowRotation()
    {
        Vector3 movement = (transform.position - cardTransform.position);
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        
        // IMPORTANT: We divide by 100f here because 'movement' is now in pixels (hundreds of units)
        // instead of world space (single units). This prevents extreme rotation snapping.
        Vector3 movementRotation = (parentCardMovement.isDragging ? movementDelta : movement) * (rotationAmount / 100f); 
        
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
    }

    private void CardTilt()
    {
        savedIndex = parentCardMovement.isDragging ? savedIndex : parentCardMovement.ParentIndex();
        float sine = Mathf.Sin(Time.time + savedIndex) * (parentCardMovement.isHovering ? .2f : 1);
        float cosine = Mathf.Cos(Time.time + savedIndex) * (parentCardMovement.isHovering ? .2f : 1);

        Vector3 offset = transform.position - Input.mousePosition;
        
        // Scale down the pixel offset so the rotation math doesn't explode
        float pixelScaleFactor = 100f; 

        float tiltX = parentCardMovement.isHovering ? (((offset.y / pixelScaleFactor) * -1) * manualTiltAmount) : 0;
        float tiltY = parentCardMovement.isHovering ? (((offset.x / pixelScaleFactor)) * manualTiltAmount) : 0;
        float tiltZ = parentCardMovement.isDragging ? tiltParent.eulerAngles.z : (curveRotationOffset * (curve.rotationInfluence * parentCardMovement.SiblingAmount()));

        float lerpX = Mathf.LerpAngle(tiltParent.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(tiltParent.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

        tiltParent.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    private void Select(CardMovement cardMovement, bool state)
    {
        DOTween.Kill(2, true);
        float dir = state ? 1 : 0;
        shakeParent.DOPunchPosition(shakeParent.up * selectPunchAmount * dir, scaleTransition, 10, 1);
        shakeParent.DOPunchRotation(Vector3.forward * (hoverPunchAngle/2), hoverTransition, 20, 1).SetId(2);

        if(scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);

    }

    public void Swap(float dir = 1)
    {
        if (!swapAnimations)
            return;

        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation((Vector3.forward * swapRotationAngle) * dir, swapTransition, swapVibrato, 1).SetId(3);
    }

    private void BeginDrag(CardMovement cardMovement)
    {
        /*
        if(scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);
     
        canvas.overrideSorting = true;
        */
    }

    private void EndDrag(CardMovement cardMovement)
    {
        /*
        canvas.overrideSorting = false;
        transform.DOScale(1, scaleTransition).SetEase(scaleEase);
        */
    }

    private void PointerEnter(CardMovement cardMovement)
    {
        if(scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);

        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
    }

    private void PointerExit(CardMovement cardMovement)
    {
        if (!parentCardMovement.wasDragged)
            transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    private void PointerUp(CardMovement cardMovement, bool longPress)
    {
        if(scaleAnimations)
            transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase);
        canvas.overrideSorting = false;

        visualShadow.localPosition = shadowDistance;
        shadowCanvas.overrideSorting = true;
        
        Debug.Log("Pointer Up: " + cardMovement.name + ", Long Press: " + longPress);
    }

    private void PointerDown(CardMovement cardMovement)
    {
        if(scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);
            
        visualShadow.localPosition += (-Vector3.up * shadowOffset);
        shadowCanvas.overrideSorting = false;
        
        Debug.Log("Pointer Down: " + cardMovement.name);
    }

}
