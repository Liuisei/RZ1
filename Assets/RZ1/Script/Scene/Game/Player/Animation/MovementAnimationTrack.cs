using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[System.Serializable]
public class MovementAnimationTrack : AnimationTrackBase
{
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _walkClip;
    [SerializeField] private AnimationClip _runClip;
    [SerializeField] private float _walkThreshold = 2f;
    [SerializeField] private float _runThreshold = 5f;

    private AnimationMixerPlayable _mixer;
    private AnimationClipPlayable _idlePlayable;
    private AnimationClipPlayable _walkPlayable;
    private AnimationClipPlayable _runPlayable;

    public override void Initialize(PlayableGraph graph, AnimationLayerMixerPlayable layerMixer, int layerIndex)
    {
        base.Initialize(graph, layerMixer, layerIndex);

        _idlePlayable = AnimationClipPlayable.Create(graph, _idleClip);
        _walkPlayable = AnimationClipPlayable.Create(graph, _walkClip);
        _runPlayable = AnimationClipPlayable.Create(graph, _runClip);

        _mixer = AnimationMixerPlayable.Create(graph, 3);
        graph.Connect(_idlePlayable, 0, _mixer, 0);
        graph.Connect(_walkPlayable, 0, _mixer, 1);
        graph.Connect(_runPlayable, 0, _mixer, 2);

        _layerMixer.ConnectInput(_layerIndex, _mixer, 0);
        _layerMixer.SetInputWeight(_layerIndex, 1f);
    }

    public void UpdateMoveSpeed(float speed)
    {
        float idleWeight = 0f;
        float walkWeight = 0f;
        float runWeight = 0f;

        if (speed < _walkThreshold)
        {
            float t = Mathf.InverseLerp(0f, _walkThreshold, speed);
            idleWeight = 1f - t;
            walkWeight = t;
        }
        else if (speed < _runThreshold)
        {
            float t = Mathf.InverseLerp(_walkThreshold, _runThreshold, speed);
            walkWeight = 1f - t;
            runWeight = t;
        }
        else
        {
            runWeight = 1f;
        }

        _mixer.SetInputWeight(0, idleWeight);
        _mixer.SetInputWeight(1, walkWeight);
        _mixer.SetInputWeight(2, runWeight);
    }

    public override void SetWeight(float weight)
    {
        _layerMixer.SetInputWeight(_layerIndex, weight);
    }
}