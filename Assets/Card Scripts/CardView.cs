using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer cardImage;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text cost;
    private Card _card;

    public void SetUp(Card card)
    {
        this._card = card;
        cardImage.sprite = card.sprite;
        title.text = card.title;
        cost.text = card.Cost.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _card.PerformEffect();
        Destroy(this.gameObject);
    }
    
}
