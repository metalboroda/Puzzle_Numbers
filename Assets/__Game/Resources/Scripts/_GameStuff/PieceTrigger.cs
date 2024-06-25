using DG.Tweening;
using System;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class PieceTrigger : MonoBehaviour
  {
    public event Action PiecePlaced;

    public bool IsOccupied { get; private set; } = false;
    public Sprite Sprite { get; private set; }

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

        PiecePlaced?.Invoke();
      });
    }
  }
}
