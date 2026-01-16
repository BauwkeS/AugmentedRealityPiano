using System.Collections.Generic;
using UnityEngine;

using mptcc = Mediapipe.Tasks.Components.Containers;


#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
using Mediapipe.Unity;
using Mediapipe;
using System;
using System.Xml.Serialization;
using Mediapipe.Tasks.Components.Containers;
using static UnityEngine.GraphicsBuffer;
#pragma warning restore IDE0065



public sealed class MultiHandsAnnotation : HierarchicalAnnotation
{
    //[SerializeField] private GameObject _annotationPrefab; for the world hand

    [SerializeField] private GameObject _handsAnnotationPrefab;
    [SerializeField] private bool _activePianoOverlay = false;
    [SerializeField] private Color _handLandmarkColor = Color.red;
    [SerializeField] private Color _fingertipLandmarkColor = Color.blue;
    
    public List<Vector3> FingerTipPositions
    {
        get
        {
            var result = new List<Vector3>();
            foreach (var hands in children)
            {
                result.AddRange(hands.FingertipPositions);
            }
            return result;
        }
    }    


    private List<HandsAnnotation> _children;
        private List<HandsAnnotation> children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<HandsAnnotation>();
                }
                return _children;
            }
        }


    private void Awake()
    {
        GameObject hand1 = Instantiate(_handsAnnotationPrefab, transform);
        GameObject hand2 = Instantiate(_handsAnnotationPrefab, transform);
        _children = new List<HandsAnnotation>();
        _children.AddRange(GetComponentsInChildren<HandsAnnotation>());
    }

    private void Start()
    {
        if (_activePianoOverlay)
        {
            var handObjects = GetComponentsInChildren<HandsAnnotation>();
            GameObject hand1 = handObjects[0].gameObject;
            GameObject hand2 = handObjects[1].gameObject;

            LayerMask pMask = LayerMask.NameToLayer("Piano");

            SetChildrenToLayer(hand1.transform,pMask);
            SetChildrenToLayer(hand2.transform,pMask);
        }
    }

    private void SetChildrenToLayer(Transform child, LayerMask layer)
    {
        child.gameObject.layer = layer;
        if (child.childCount > 0)
        {
            foreach (Transform grandChild in child)
            {
                SetChildrenToLayer(grandChild, layer);
            }
        }
    }

    private void OnEnable()
    {
        SetThoseColors();
    }

    public void SetThoseColors()
    {
        foreach (var hands in children)
        {
            hands.SetColorsHands(_handLandmarkColor, _fingertipLandmarkColor);
        }
    }

    public void Draw(IReadOnlyList<mptcc.NormalizedLandmarks> targets, bool visualizeZ = false, bool EnableDistanceFiltering = true)
    {
        if (ActivateFor(targets))
        {
            CallActionForAll(targets, (annotation, target) =>
            {
                if (annotation != null) { annotation.Draw(target, visualizeZ,EnableDistanceFiltering); }
            });
        }
    }

    public void DrawWorldHand(IReadOnlyList<Landmarks> landmarks, GameObject prefabToSpawn, bool visualizeZ = false)
    {
        if (ActivateFor(landmarks))
        {
            CallActionForAll(landmarks, (annotation, target) =>
            {
                if (annotation != null) { annotation.DrawWorldHand(target, visualizeZ); }
            });
        }
    }

    private void CallActionForAll<TArg>(IReadOnlyList<TArg> argumentList, Action<HandsAnnotation, TArg> action)
    {
        for (var i = 0; i < Mathf.Max(children.Count, argumentList.Count); i++)
        {
            if (i >= argumentList.Count)
            {
                action(children[i], default);
                continue;
            }
            action(children[i], argumentList[i]);
        }
    }
}


