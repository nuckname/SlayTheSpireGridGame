using UnityEngine;

[CreateAssetMenu(fileName = "NewPinCard", menuName = "Game Data/Pin Card Data")]
public class PinCardData : ScriptableObject
{
    [Header("Card Identity")]
    public string cardTitle;
    [TextArea] public string cardDescription;
    public Color accentColor = Color.white;
    
    [Header("Game Data")]
    public GameObject pinPrefab;
    public Sprite sprite;
    public int cost = 0;
}