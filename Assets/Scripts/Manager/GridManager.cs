using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;
    [Tooltip("The parent transform that holds the grid layout")]
    [SerializeField] private Transform gridParent;
    
    [Tooltip("The UI square prefab to spawn")]
    [SerializeField] private Image squarePrefab;

    private Image[,] _gridCells;
    private bool[,] _occupiedCells;

    private Dictionary<CardMovement, List<Image>> _cardSquareMap = new Dictionary<CardMovement, List<Image>>();
    
    // Tracks the order in which cards were selected to allow auto-packing
    private List<CardMovement> _activeCards = new List<CardMovement>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        _gridCells = new Image[gridWidth, gridHeight];
        _occupiedCells = new bool[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // Spawn empty grid cells and make them invisible/transparent to start
                Image newSquare = Instantiate(squarePrefab, gridParent);
                newSquare.color = new Color(1, 1, 1, 0.1f); 
                _gridCells[x, y] = newSquare;
            }
        }
    }

    public void ToggleSquare(Card card, CardMovement cardMovement, bool isSelected)
    {
        if (card == null || card.cardData == null) 
        {
            Debug.LogWarning("This CardMovement doesn't have a Card data container attached!");
            return;
        }

        if (isSelected)
        {
            if (!_activeCards.Contains(cardMovement))
            {
                _activeCards.Add(cardMovement);
            }
        }
        else
        {
            if (_activeCards.Contains(cardMovement))
            {
                _activeCards.Remove(cardMovement);
            }
        }

        RepackGrid();
    }

    private void RepackGrid()
    {
        // Wipe the entire grid visually and logically
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                _occupiedCells[x, y] = false;
                _gridCells[x, y].color = new Color(1, 1, 1, 0.1f);
            }
        }
        
        _cardSquareMap.Clear();

        List<CardMovement> cardsToDeselect = new List<CardMovement>();

        // Re-place all active cards in the exact order they were clicked
        foreach (CardMovement activeCard in _activeCards)
        {
            List<Vector2Int> validPlacement = FindFirstAvailableSpace(activeCard.card.cardData.shapeCoordinates);

            if (validPlacement != null)
            {
                List<Image> occupiedImages = new List<Image>();

                foreach (Vector2Int pos in validPlacement)
                {
                    _occupiedCells[pos.x, pos.y] = true;
                    Image cellImage = _gridCells[pos.x, pos.y];
                    cellImage.color = new Color(1, 1, 1, 1f); 
                    occupiedImages.Add(cellImage);
                }

                _cardSquareMap.Add(activeCard, occupiedImages);
            }
            else
            {
                Debug.LogWarning("Not enough space on the grid for this shape! Pushing it back out.");
                cardsToDeselect.Add(activeCard);
            }
        }

        // Clean up any cards that no longer fit on the board after shifting
        foreach (CardMovement failedCard in cardsToDeselect)
        {
            _activeCards.Remove(failedCard);
            failedCard.selected = false;
            failedCard.transform.localPosition = Vector3.zero; 
        }
    }

    private List<Vector2Int> FindFirstAvailableSpace(List<Vector2Int> shapeOffsets)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (CanShapeFitAt(shapeOffsets, x, y))
                {
                    List<Vector2Int> actualPositions = new List<Vector2Int>();
                    foreach (Vector2Int offset in shapeOffsets)
                    {
                        actualPositions.Add(new Vector2Int(x + offset.x, y + offset.y));
                    }
                    return actualPositions;
                }
            }
        }
        return null; 
    }

    private bool CanShapeFitAt(List<Vector2Int> shapeOffsets, int startX, int startY)
    {
        foreach (Vector2Int offset in shapeOffsets)
        {
            int targetX = startX + offset.x;
            int targetY = startY + offset.y;

            if (targetX < 0 || targetX >= gridWidth || targetY < 0 || targetY >= gridHeight)
                return false;

            if (_occupiedCells[targetX, targetY])
                return false;
        }

        return true;
    }
}