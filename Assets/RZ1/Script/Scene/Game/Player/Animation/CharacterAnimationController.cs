using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    [SerializeReference, SubclassSelector]
    private List<AnimationTrackBase> _tracks = new();

    private PlayableGraph _graph;
    private AnimationLayerMixerPlayable _layerMixer;
    private Dictionary<Type, AnimationTrackBase> _trackMap = new();

    private void Awake()
    {
        var animator = GetComponent<Animator>();
        _graph = PlayableGraph.Create("CharacterGraph");
        var output = AnimationPlayableOutput.Create(_graph, "Output", animator);

        _layerMixer = AnimationLayerMixerPlayable.Create(_graph, _tracks.Count);
        output.SetSourcePlayable(_layerMixer);
        _graph.Play();

        for (int i = 0; i < _tracks.Count; i++)
        {
            var track = _tracks[i];
            track.Initialize(_graph, _layerMixer, i);
            _trackMap[track.GetType()] = track;
        }
    }

    public T GetTrack<T>() where T : AnimationTrackBase
    {
        return _trackMap.TryGetValue(typeof(T), out var track) ? track as T : null;
    }

    private void OnDestroy()
    {
        if (_graph.IsValid()) _graph.Destroy();
    }
}