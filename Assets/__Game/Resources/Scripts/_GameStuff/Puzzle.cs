using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class Puzzle : MonoBehaviour
  {
    public event Action PuzzleCompleted;

    [Header("Pieces Sprites")]
    [SerializeField] private Sprite[] _piecesSprites;
    [Header("Pieces References")]
    [SerializeField] private SpriteRenderer[] _piecesRenderers;
    [Space]
    [SerializeField] private PieceTrigger[] _piecesTriggers;
    [Header("Settings")]
    [SerializeField] private float offsetMargin = 1f;
    [SerializeField] private float minDistanceBetweenPieces = 1f;
    [Header("Tutorial")]
    [SerializeField] private bool _tutorial = false;
    [SerializeField] private GameObject _finger;

    public bool Completed { get; private set; } = false;

    private List<Vector3> _placedPositions = new List<Vector3>();
    private int _currentTutorialIndex = 0;

    private PuzzlesContainer _puzzlesContainer;

    private void Awake()
    {
      _puzzlesContainer = GetComponentInParent<PuzzlesContainer>();

      ClearSpriteRenderers();
    }

    private void OnEnable()
    {
      foreach (var trigger in _piecesTriggers)
        trigger.PiecePlaced += CheckAllPiecesPlaced;
    }

    private void OnDisable()
    {
      foreach (var trigger in _piecesTriggers)
        trigger.PiecePlaced -= CheckAllPiecesPlaced;
    }

    private void Start()
    {
      SetSprites();

      if (_tutorial == true)
        StartTutorial();
    }

    private void ClearSpriteRenderers()
    {
      foreach (var renderer in _piecesRenderers)
        renderer.sprite = null;
    }

    private void SetSprites()
    {
      for (int i = 0; i < _piecesRenderers.Length; i++)
      {
        _piecesRenderers[i].sprite = _piecesSprites[i];

        _piecesTriggers[i].SetSprite(_piecesSprites[i]);

        Vector3 randomPosition = GetRandomPosition(_piecesRenderers[i]);
        Quaternion randomRotation = GetRandomRotation();

        _piecesRenderers[i].transform.position = randomPosition;
        _piecesRenderers[i].transform.rotation = randomRotation;

        Piece piece = _piecesRenderers[i].GetComponent<Piece>();

        piece.SetScatteredPositionAndRotation(randomPosition, randomRotation);

        _placedPositions.Add(randomPosition);
      }
    }

    private Vector3 GetRandomPosition(SpriteRenderer renderer)
    {
      float screenWidth = Screen.width;
      float screenHeight = Screen.height;

      Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(screenWidth, screenHeight, Camera.main.nearClipPlane));
      Vector3 minBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

      float spriteWidth = renderer.bounds.size.x;
      float spriteHeight = renderer.bounds.size.y;

      float minX = minBounds.x + spriteWidth / 2 + offsetMargin;
      float maxX = screenBounds.x - spriteWidth / 2 - offsetMargin;
      float minY = minBounds.y + spriteHeight / 2 + offsetMargin;
      float maxY = screenBounds.y - spriteHeight / 2 - offsetMargin;

      Vector3 randomPosition;

      do
      {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        randomPosition = new Vector3(randomX, randomY, 0);
      }
      while (IsPositionValid(randomPosition) == false);

      return randomPosition;
    }

    private bool IsPositionValid(Vector3 position)
    {
      foreach (var placedPosition in _placedPositions)
        if (Vector3.Distance(position, placedPosition) < minDistanceBetweenPieces) return false;

      return true;
    }

    private Quaternion GetRandomRotation()
    {
      float randomZ = Random.Range(-22.5f, 22.5f);

      return Quaternion.Euler(0, 0, randomZ);
    }

    private void CheckAllPiecesPlaced()
    {
      foreach (var trigger in _piecesTriggers)
        if (trigger.IsOccupied == false) return;

      Completed = true;

      StartCoroutine(DoOnPuzzleCompleted());
    }

    private IEnumerator DoOnPuzzleCompleted()
    {
      yield return new WaitForSeconds(0.5f);

      if (_puzzlesContainer.SinglePuzzleToComplete == false)
        gameObject.SetActive(false);

      PuzzleCompleted?.Invoke();
    }

    private void StartTutorial()
    {
      SetAllPiecesDraggable(false);
      StartCoroutine(PlayTutorial());
    }

    private IEnumerator PlayTutorial()
    {
      while (_currentTutorialIndex < _piecesRenderers.Length)
      {
        Piece currentPiece = _piecesRenderers[_currentTutorialIndex].GetComponent<Piece>();

        currentPiece.SetCanDrag(true);

        _finger.SetActive(true);
        _finger.transform.position = currentPiece.TutorialPoint.position;

        _finger.transform.DOMove(_piecesTriggers[_currentTutorialIndex].transform.position, 1.5f)
            .SetLoops(-1, LoopType.Restart);

        while (_piecesTriggers[_currentTutorialIndex].IsOccupied == false)
        {
          yield return null;
        }

        _finger.transform.DOKill();
        currentPiece.SetCanDrag(false);

        _currentTutorialIndex++;
      }

      _finger.SetActive(false);
    }

    private void SetAllPiecesDraggable(bool canDrag)
    {
      foreach (var renderer in _piecesRenderers)
      {
        renderer.GetComponent<Piece>().SetCanDrag(canDrag);
      }
    }
  }
}