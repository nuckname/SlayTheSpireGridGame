using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Card Data", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName = "Blank Card";
    
    [TextArea(3, 5)]
    public string cardDescription;
    
    public Sprite cardArtwork;

    [Header("Shape Settings")]
    [Tooltip("Grid coordinates relative to the pivot. (0,0) is the starting cell.")]
    public List<Vector2Int> shapeCoordinates = new List<Vector2Int> { new Vector2Int(0, 0) };
}