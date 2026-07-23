using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Card : MonoBehaviour
{
    [Header("Card Reference")]
    public CardData cardData;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image artworkImage;

    public void UpdateVisuals()
    {
        if (cardData == null) return;

        if (titleText != null) titleText.text = cardData.cardName;
        if (descriptionText != null) descriptionText.text = cardData.cardDescription;
        if (artworkImage != null) artworkImage.sprite = cardData.cardArtwork;
    }
}