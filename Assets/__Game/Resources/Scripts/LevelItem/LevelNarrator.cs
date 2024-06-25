using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Tools;
using System.Collections;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.LevelItem
{
  public class LevelNarrator : MonoBehaviour
  {
    [Header("Announcer")]
    [SerializeField] private AudioClip _questStartClip;
    [SerializeField] private AudioClip[] _questClips;
    [Space]
    [SerializeField] private float _delayBetweenClips = 0.25f;
    [Space]
    [SerializeField] private AudioClip[] _winAnnouncerClips;
    [SerializeField] private AudioClip[] _loseAnnouncerClips;
    [SerializeField] private AudioClip[] _stuporAnnouncerClips;

    private AudioSource _audioSource;
    private AudioTool _audioTool;
    private EventBinding<EventStructs.StateChanged> _stateEvent;
    private EventBinding<EventStructs.StuporEvent> _stuporEvent;
    private EventBinding<EventStructs.UiButtonEvent> _uiButtonEvent;
    private EventBinding<EventStructs.VariantAudioClickedEvent> _variantAudioClickedEvent;

    private void Awake()
    {
      _audioSource = GetComponent<AudioSource>();
      _audioTool = new AudioTool(_audioSource);
    }

    private void OnEnable()
    {
      _stateEvent = new EventBinding<EventStructs.StateChanged>(PlayScreenSound);
      _stuporEvent = new EventBinding<EventStructs.StuporEvent>(PlayStuporSound);
      //_uiButtonEvent = new EventBinding<EventStructs.UiButtonEvent>(PlayQuestClipsSequentially);
      _variantAudioClickedEvent = new EventBinding<EventStructs.VariantAudioClickedEvent>(PlayWordAudioCLip);
    }

    private void OnDisable()
    {
      _stateEvent.Remove(PlayScreenSound);
      _stuporEvent.Remove(PlayStuporSound);
      //_uiButtonEvent.Remove(PlayQuestClipsSequentially);
      _variantAudioClickedEvent.Remove(PlayWordAudioCLip);
    }

    private void Start()
    {
      if (_questStartClip != null)
        _audioSource.PlayOneShot(_questStartClip);
    }

    private void PlayScreenSound(EventStructs.StateChanged state)
    {
      switch (state.State)
      {
        case GameWinState:
          _audioSource.Stop();
          _audioSource.PlayOneShot(_audioTool.GetRandomCLip(_winAnnouncerClips));
          break;
        case GameLoseState:
          _audioSource.Stop();
          _audioSource.PlayOneShot(_audioTool.GetRandomCLip(_loseAnnouncerClips));
          break;
      }
    }

    private void PlayStuporSound(EventStructs.StuporEvent stuporEvent)
    {
      _audioSource.Stop();
      _audioSource.PlayOneShot(_audioTool.GetRandomCLip(_stuporAnnouncerClips));
    }

    public void PlayQuestClipsSequentially(EventStructs.UiButtonEvent uiButtonEvent)
    {
      if (uiButtonEvent.UiEnums == __Game.Scripts.Enums.UiEnums.QuestPlayButton)
        StartCoroutine(DoPlayClipsSequentially(_questClips));
    }

    private IEnumerator DoPlayClipsSequentially(AudioClip[] clips)
    {
      yield return new WaitForSecondsRealtime(0.1f);

      foreach (var clip in clips)
      {
        _audioSource.Stop();
        _audioSource.PlayOneShot(clip);

        yield return new WaitForSecondsRealtime(clip.length + _delayBetweenClips);
      }
    }

    private void PlayWordAudioCLip(EventStructs.VariantAudioClickedEvent variantAudioClickedEvent)
    {
      _audioSource.Stop();
      _audioSource.PlayOneShot(variantAudioClickedEvent.AudioClip);
    }
  }
}
