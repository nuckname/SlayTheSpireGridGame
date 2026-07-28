using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

// https://www.youtube.com/watch?v=I1dAZuWurw4
// https://github.com/mixandjam/balatro-feel

public class HorizontalCardHolder : MonoBehaviour
{
    public static HorizontalCardHolder Instance { get; private set; }

    [SerializeField] private CardMovement selectedCardMovement;
    [SerializeReference] private CardMovement hoveredCardMovement;

    [SerializeField] private GameObject slotPrefab;
    private RectTransform rect;

    [Header("Spawn Settings")]
    [SerializeField] private int cardsToSpawn = 7;
    [Tooltip("Populate this in the inspector with your unique CardData Scriptable Objects")]
    [SerializeField] private List<CardData> startingDeck = new List<CardData>();
    public List<CardMovement> cardsInHand;

    bool isCrossing = false;
    [SerializeField] private bool tweenCardReturn = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // Spawn initial cards
        for (int i = 0; i < cardsToSpawn; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, transform);
            
            // Look for the data container we made earlier on the spawned card
            Card spawnedCard = newSlot.GetComponentInChildren<Card>();
            
            // Assign a unique Scriptable Object from the deck list
            if (spawnedCard != null && i < startingDeck.Count)
            {
                spawnedCard.cardData = startingDeck[i];
                spawnedCard.UpdateVisuals(); 
            }
        }

        cardsInHand = GetComponentsInChildren<CardMovement>().ToList();

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            RegisterCardEvents(cardsInHand[i], i);
        }

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            
            RebuildHandVisuals();
        }
    }

    private void RegisterCardEvents(CardMovement card, int index)
    {
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.name = index.ToString();
    }

    void CardPointerEnter(CardMovement cardMovement)
    {
        hoveredCardMovement = cardMovement;
    }

    void CardPointerExit(CardMovement cardMovement)
    {
        hoveredCardMovement = null;
    }

    /// <summary>
    /// Safely wraps a floating card into a layout slot when it is finally dropped in the hand.
    /// </summary>
    public void AssignSlotToCard(CardMovement card)
    {
        card.transform.SetParent(transform);
        RebuildHandVisuals();
    }

    /// <summary>
    /// Evaluates the current slots in the hand and updates all visual cards 
    /// with their correct sibling index and the new total hand length.
    /// </summary>
    public void RebuildHandVisuals()
    {
        // Wipe the list completely clean
        cardsInHand.Clear();

        // Count the physical slots remaining in the hand
        int currentLength = transform.childCount;

        for (int i = 0; i < currentLength; i++)
        {
            Transform slot = transform.GetChild(i);
            CardMovement cardSlot = slot.GetComponentInChildren<CardMovement>();

            // Re-add only the valid cards that are actually in the slots right now
            if (cardSlot != null)
            {
                cardsInHand.Add(cardSlot);

                if (cardSlot.cardAnimator != null)
                {
                    // Pass the new length to the animator for curve/spacing math
                    cardSlot.cardAnimator.UpdateLength(currentLength);
                }
            }
        }
    }
}