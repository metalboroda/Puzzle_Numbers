using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class Piece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
  {
    [Header("Tutorial Settings")]
    [SerializeField] private Color _brightColor;
    [SerializeField] private Color _darkColor;
    [field: Space]
    [field: SerializeField] public Transform TutorialPoint;

    private bool _canDrag = true;
    private bool _isDragging = false;
    private bool _placed = false;
    private Vector3 _offset;
    private Vector3 _initPosition;
    private Quaternion _initRotation;
    private Vector3 _scatteredPosition;
    private Quaternion _scatteredRotation;
    private Sprite _sprite;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider _boxCollider;

    private void Awake()
    {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _boxCollider = GetComponent<BoxCollider>();

      _initPosition = transform.position;
      _initRotation = transform.rotation;
    }

    private void Start()
    {
      _spriteRenderer.sortingOrder = 1;

      UpdateColor();
    }

    private void OnTriggerStay(Collider other)
    {
      if (other.TryGetComponent(out PieceTrigger pieceTrigger))
      {
        if (_isDragging == false) return;

        if (pieceTrigger.Sprite == _spriteRenderer.sprite)
        {
          pieceTrigger.Place(transform, _initPosition, _initRotation);

          _canDrag = false;
          _placed = true;
          _boxCollider.enabled = false;
          _spriteRenderer.sortingOrder = 0;

          UpdateColor();
        }
      }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      _isDragging = true;

      Vector3 worldPoint = GetWorldPoint(eventData.position);

      _offset = transform.position - worldPoint;
      _spriteRenderer.sortingOrder = 2;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if (_canDrag == false) return;

      _isDragging = false;

      transform.DOMove(_scatteredPosition, 0.2f);
      _spriteRenderer.sortingOrder = 1;
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (_canDrag == false) return;

      if (_isDragging == true)
      {
        Vector3 worldPoint = GetWorldPoint(eventData.position);
        Vector3 newPosition = worldPoint + _offset;

        newPosition.z = 0;
        transform.position = newPosition;
      }
    }

    public void SetCanDrag(bool canDrag)
    {
      _canDrag = canDrag;

      UpdateColor();
    }

    public void SetScatteredPositionAndRotation(Vector3 position, Quaternion rotation)
    {
      _scatteredPosition = position;
      _scatteredRotation = rotation;
    }

    private void UpdateColor()
    {
      _spriteRenderer.color = _placed ? _brightColor : (_canDrag ? _brightColor : _darkColor);
    }

    private Vector3 GetWorldPoint(Vector3 screenPoint)
    {
      screenPoint.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

      Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);

      worldPoint.z = 0;

      return worldPoint;
    }
  }
}