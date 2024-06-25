using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class PieceTrigger : MonoBehaviour
  {
    public event Action PiecePlaced;

    [Header("SFX")]
    [SerializeField]
    private AudioClip _placeClip;

    public bool IsOccupied { get; private set; } = false;
    public Sprite Sprite { get; private set; }

    private AudioSource _audioSource;

    private void Awake()
    {
      _audioSource = GetComponent<AudioSource>();
    }

    public void SetSprite(Sprite sprite)
    {
      Sprite = sprite;
    }

    public void Place(Transform transform, Vector3 position, Quaternion rotation)
    {
      transform.DORotateQuaternion(rotation, 0.2f);
      transform.DOMove(position, 0.2f).OnComplete(() =>
      {
        IsOccupied = true;

        _audioSource.pitch = Random.Range(0.95f, 1.05f);
        _audioSource.PlayOneShot(_placeClip);

        PiecePlaced?.Invoke();
      });
    }
  }
}
