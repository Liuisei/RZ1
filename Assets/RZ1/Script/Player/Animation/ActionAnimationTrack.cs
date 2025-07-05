using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[System.Serializable]
public class ActionAnimationTrack : AnimationTrackBase
{
    [SerializeField] private AvatarMask _mask;

    private AnimationClipPlayable _currentPlayable;

    public override void Initialize(PlayableGraph graph, AnimationLayerMixerPlayable layerMixer, int layerIndex)
    {
        base.Initialize(graph, layerMixer, layerIndex);
        _layerMixer.SetLayerAdditive((uint)_layerIndex, false);
        if (_mask)
        {
            _layerMixer.SetLayerMaskFromAvatarMask((uint)_layerIndex, _mask);
        }

        _layerMixer.SetInputWeight(_layerIndex, 0f);
    }

    public void PlayAction(AnimationClip clip)
    {
        _currentPlayable = AnimationClipPlayable.Create(_graph, clip);
        _currentPlayable.SetTime(0);
        _currentPlayable.SetSpeed(1f);
        _currentPlayable.SetApplyFootIK(false);

        _layerMixer.DisconnectInput(_layerIndex);
        _graph.Connect(_currentPlayable, 0, _layerMixer, _layerIndex);
        _layerMixer.SetInputWeight(_layerIndex, 1f);
    }

    public void StopAction()
    {
        _layerMixer.SetInputWeight(_layerIndex, 0f);
    }

    public bool IsPlaying()
    {
        return _currentPlayable.IsValid() && !_currentPlayable.IsDone();
    }

    public override void SetWeight(float weight)
    {
        _layerMixer.SetInputWeight(_layerIndex, weight);
    }
}