using System;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    [Tooltip("The parent transform that holds the grid layout")]
    [SerializeField] private Transform gridParent;
    
    [Tooltip("The UI square prefab to spawn")]
    [SerializeField] private Image squarePrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddSquare();
        }
    }

    public void AddSquare()
    {
        if (squarePrefab != null && gridParent != null)
        {
            Instantiate(squarePrefab, gridParent);
        }
        else
        {
            Debug.LogWarning("GridManager is missing the Square Prefab or Grid Parent reference.");
        }
    }
}