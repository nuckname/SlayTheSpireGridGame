using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private readonly PinCardData cardData;

    public Card(PinCardData cardData)
    {
        this.cardData = cardData;
        Cost = cardData.cost;
    }
    
    public Sprite sprite { get =>cardData.sprite; }
    
    public string title { get =>cardData.name; }
    
    public int Cost { get; set; }
    
    public void PerformEffect()
    {
        // Implement the effect of the card here, using cardData.pinPrefab or other properties as needed.
        Debug.unityLogger.Log("Perform Effect");
    }
}
