using UnityEngine.Playables;
using UnityEngine.Animations;

[System.Serializable]
public abstract class AnimationTrackBase
{
    protected PlayableGraph _graph;
    protected AnimationLayerMixerPlayable _layerMixer;
    protected int _layerIndex;

    protected AnimationTrackBase() { }

    public virtual void Initialize(PlayableGraph graph, AnimationLayerMixerPlayable layerMixer, int layerIndex)
    {
        _graph = graph;
        _layerMixer = layerMixer;
        _layerIndex = layerIndex;
    }

    public abstract void SetWeight(float weight);
}