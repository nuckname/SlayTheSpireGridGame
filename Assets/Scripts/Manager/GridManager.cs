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

    // 2D array to hold our pre-generated grid cells
    private Image[,] _gridCells;
    private bool[,] _occupiedCells;

    // Maps a card to the specific list of grid cell images it occupies
    private Dictionary<CardMovement, List<Image>> _cardSquareMap = new Dictionary<CardMovement, List<Image>>();

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
                newSquare.color = new Color(1, 1, 1, 0.1f); // Dimmed for empty space
                _gridCells[x, y] = newSquare;
            }
        }
    }

    // Handles adding/removing squares via the card selection and tracks them
    // Now accepts an optional specific X/Y coordinate to place the card at
    public void ToggleSquare(CardMovement cardMovement, bool isSelected, int specificX = -1, int specificY = -1)
    {
        Card card = cardMovement.GetComponentInChildren<Card>();
       // Card card = cardMovement.GetComponent<Card>();
        if (card == null || card.cardData == null) 
        {
            Debug.LogWarning("This CardMovement doesn't have a Card data container attached!");
            return;
        }

        if (isSelected)
        {
            List<Vector2Int> validPlacement = null;

            // If we passed in a specific grid coordinate, check if the shape fits right there
            if (specificX != -1 && specificY != -1)
            {
                if (CanShapeFitAt(card.cardData.shapeCoordinates, specificX, specificY))
                {
                    validPlacement = new List<Vector2Int>();
                    foreach (Vector2Int offset in card.cardData.shapeCoordinates)
                    {
                        validPlacement.Add(new Vector2Int(specificX + offset.x, specificY + offset.y));
                    }
                }
            }
            else
            {
                // Otherwise fall back to auto-finding the first available space
                validPlacement = FindFirstAvailableSpace(card.cardData.shapeCoordinates);
            }

            if (validPlacement != null)
            {
                List<Image> occupiedImages = new List<Image>();

                foreach (Vector2Int pos in validPlacement)
                {
                    _occupiedCells[pos.x, pos.y] = true;
                    Image cellImage = _gridCells[pos.x, pos.y];
                    cellImage.color = new Color(1, 1, 1, 1f); // Highlight to show it's occupied
                    occupiedImages.Add(cellImage);
                }

                _cardSquareMap.Add(cardMovement, occupiedImages);
            }
            else
            {
                Debug.LogWarning("Not enough space on the grid for this shape at the target location!");
                
                // Force deselect the card so it visually pops back down if it fails to place
                cardMovement.selected = false;
                cardMovement.transform.localPosition = Vector3.zero; 
            }
        }
        else
        {
            if (_cardSquareMap.ContainsKey(cardMovement))
            {
                List<Image> squaresToClear = _cardSquareMap[cardMovement];
                
                // Find the coordinates of these images and clear them
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        if (squaresToClear.Contains(_gridCells[x, y]))
                        {
                            _occupiedCells[x, y] = false;
                            _gridCells[x, y].color = new Color(1, 1, 1, 0.1f); // Return to dimmed state
                        }
                    }
                }
                
                _cardSquareMap.Remove(cardMovement);
            }
        }
    }

    /// <summary>
    /// Scans the grid left-to-right, top-to-bottom to find a valid anchor point for the shape.
    /// </summary>
    private List<Vector2Int> FindFirstAvailableSpace(List<Vector2Int> shapeOffsets)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (CanShapeFitAt(shapeOffsets, x, y))
                {
                    // Return the exact absolute grid coordinates this shape will consume
                    List<Vector2Int> actualPositions = new List<Vector2Int>();
                    foreach (Vector2Int offset in shapeOffsets)
                    {
                        actualPositions.Add(new Vector2Int(x + offset.x, y + offset.y));
                    }
                    return actualPositions;
                }
            }
        }
        return null; // No space found
    }

    private bool CanShapeFitAt(List<Vector2Int> shapeOffsets, int startX, int startY)
    {
        foreach (Vector2Int offset in shapeOffsets)
        {
            int targetX = startX + offset.x;
            int targetY = startY + offset.y;

            // Check out of bounds
            if (targetX < 0 || targetX >= gridWidth || targetY < 0 || targetY >= gridHeight)
                return false;

            // Check if cell is already taken
            if (_occupiedCells[targetX, targetY])
                return false;
        }

        return true;
    }
}