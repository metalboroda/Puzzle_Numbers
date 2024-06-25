using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Character
{
  public class CharacterAnimationHandler : MonoBehaviour
  {
    [SerializeField] private AnimationClip _idleAnimation;
    [SerializeField] private AnimationClip _pushMovementAnimation;

    private const float CrossFadeDuration = 0.15f;

    private Animator _animator;

    //private EventBinding<EventStructs.TrainMovementEvent> _trainMovementEvent;

    private void Awake()
    {
      _animator = GetComponent<Animator>();

      //_trainMovementEvent = new EventBinding<EventStructs.TrainMovementEvent>(PlayIdleAnimation);
      //_trainMovementEvent = new EventBinding<EventStructs.TrainMovementEvent>(PlayPushMovementAnimation);
    }

    private void OnDestroy()
    {
      //_trainMovementEvent.Remove(PlayIdleAnimation);
      //_trainMovementEvent.Remove(PlayPushMovementAnimation);
    }

    public void PlayIdleAnimation()
    {
      CrossFadeFixedTime(_idleAnimation);
    }

    public void PlayPushMovementAnimation()
    {
      CrossFadeFixedTime(_pushMovementAnimation);
    }

    private void CrossFadeFixedTime(AnimationClip clip)
    {
      _animator.CrossFadeInFixedTime(clip.name, CrossFadeDuration);
    }
  }
}