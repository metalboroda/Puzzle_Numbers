using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class PuzzlesContainer : MonoBehaviour
  {
    [SerializeField] private GameObject _canvas;
    [Header("")]
    [SerializeField] private Button[] _buttons;
    [field: Space]
    [field: SerializeField] public bool SinglePuzzleToComplete { get; private set; }
    [Space]
    [SerializeField] private Puzzle[] _puzzles;

    private Button _lastClickedButton;

    private GameBootstrapper _gameBootstrapper;

    private void Awake()
    {
      _gameBootstrapper = GameBootstrapper.Instance;
    }

    private void OnEnable()
    {
      for (int i = 0; i < _buttons.Length; i++)
      {
        int index = i;
        _buttons[index].onClick.AddListener(() => ActivatePuzzle(index));
      }
    }

    private void Start()
    {
      foreach (var puzzle in _puzzles)
        puzzle.gameObject.SetActive(false);
    }

    private void ActivatePuzzle(int index)
    {
      if (index >= 0 && index < _puzzles.Length)
      {
        foreach (var puzzle in _puzzles)
        {
          puzzle.PuzzleCompleted -= HandlePuzzleCompleted;
          puzzle.gameObject.SetActive(false);
        }

        _puzzles[index].gameObject.SetActive(true);
        _puzzles[index].PuzzleCompleted += HandlePuzzleCompleted;

        _canvas.SetActive(false);

        _lastClickedButton = _buttons[index];
        _lastClickedButton.interactable = false;
      }
      else
      {
        Debug.LogWarning("Puzzle index out of range: " + index);
      }
    }

    private void HandlePuzzleCompleted()
    {
      if (SinglePuzzleToComplete == false)
        _canvas.SetActive(true);

      if (SinglePuzzleToComplete)
      {
        Debug.Log("All puzzles completed!");

        return;
      }

      bool allPuzzlesCompleted = true;

      foreach (var puzzle in _puzzles)
      {
        if (puzzle.Completed == false)
        {
          allPuzzlesCompleted = false;

          break;
        }
      }

      if (allPuzzlesCompleted == true)
      {
        _gameBootstrapper.StateMachine.ChangeState(new GameWinState(_gameBootstrapper));
      }
    }
  }
}