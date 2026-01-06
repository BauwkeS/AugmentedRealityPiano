// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
using UnityEngine;

using mptcc = Mediapipe.Tasks.Components.Containers;


#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
using Mediapipe.Unity;
using Mediapipe;
using System;
using System.Xml.Serialization;
#pragma warning restore IDE0065



public sealed class MultiHandsAnnotation : Mediapipe.Unity.ListAnnotation<HandsAnnotation>
{
    [SerializeField] private Color _handLandmarkColor = Color.blue;
    [SerializeField] private float _landmarkRadius = 15.0f;
    [SerializeField] private Color _connectionColor = Color.white;
    [SerializeField, Range(0, 1)] private float _connectionWidth = 1.0f;


    [SerializeField] private PointListAnnotation _landmarkListAnnotation;
    [SerializeField] private ConnectionListAnnotation _connectionListAnnotation;
    [SerializeField] private Color _fingertipLandmarkColor = Color.red;

    public List<Vector3> FingerTipPositions
    {
        get
        {
            List<Vector3> fingertipPositions = new List<Vector3>();
            foreach (var handLandmarkList in children)
            {
                fingertipPositions.AddRange(handLandmarkList.GetFingerTipPositions());
            }
            return fingertipPositions;
        }
    }

    private const int _LandmarkCount = 21;
    private readonly List<(int, int)> _connections = new List<(int, int)> {
      (0, 1),
      (1, 2),
      (2, 3),
      (3, 4),
      (0, 5),
      (5, 9),
      (9, 13),
      (13, 17),
      (0, 17),
      (5, 6),
      (6, 7),
      (7, 8),
      (9, 10),
      (10, 11),
      (11, 12),
      (13, 14),
      (14, 15),
      (15, 16),
      (17, 18),
      (18, 19),
      (19, 20),
    };

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this))
        {
            ApplyLandmarkColor(_handLandmarkColor);
            ApplyLandmarkRadius(_landmarkRadius);
            ApplyColorOfFingertips();
            ApplyConnectionColor(_connectionColor);
            ApplyConnectionWidth(_connectionWidth);
        }
    }
#endif

    public void SetLandmarkColor(Color leftLandmarkColor)
    {
        _handLandmarkColor = leftLandmarkColor;
        ApplyLandmarkColor(_handLandmarkColor);
    }

    public void SetLandmarkRadius(float landmarkRadius)
    {
        _landmarkRadius = landmarkRadius;
        ApplyLandmarkRadius(_landmarkRadius);
    }

    public void SetConnectionColor(Color connectionColor)
    {
        _connectionColor = connectionColor;
        ApplyConnectionColor(_connectionColor);
    }

    public void SetConnectionWidth(float connectionWidth)
    {
        _connectionWidth = connectionWidth;
        ApplyConnectionWidth(_connectionWidth);
    }
    public void SetThoseColors()
    {
        foreach (var hands in children)
        {
            hands.SetColorsHands();
        }
    }

    public void Draw(IReadOnlyList<Mediapipe.NormalizedLandmarkList> targets, bool visualizeZ = false)
    {
        if (ActivateFor(targets))
        {
            CallActionForAll(targets, (annotation, target) =>
            {
                if (annotation != null) { annotation.Draw(target, visualizeZ); }
            });
        }
    }

    public void Draw(IReadOnlyList<mptcc.NormalizedLandmarks> targets, bool visualizeZ = false)
    {
        if (ActivateFor(targets))
        {
            CallActionForAll(targets, (annotation, target) =>
            {
                if (annotation != null) { annotation.Draw(target, visualizeZ); }
            });
        }
    }

    protected override HandsAnnotation InstantiateChild(bool isActive = true)
    {
        var annotation = base.InstantiateChild(isActive);
        annotation.SetLandmarkColor(_handLandmarkColor);
        annotation.SetColorOfFingerTips();
        annotation.SetLandmarkRadius(_landmarkRadius);
        annotation.SetConnectionColor(_connectionColor);
        annotation.SetConnectionWidth(_connectionWidth);
        return annotation;
    }
    private void ApplyColorOfFingertips()
    {
        foreach (var handLandmarkList in children)
        {
            if (handLandmarkList != null) { handLandmarkList.SetColorOfFingerTips(); }
        }
    }

    private void ApplyLandmarkColor(Color LandmarkColor)
    {
        foreach (var handLandmarkList in children)
        {
            if (handLandmarkList != null) { handLandmarkList.SetLandmarkColor(LandmarkColor); }
        }
    }

    private void ApplyLandmarkRadius(float landmarkRadius)
    {
        foreach (var handLandmarkList in children)
        {
            if (handLandmarkList != null) { handLandmarkList.SetLandmarkRadius(landmarkRadius); }
        }
    }

    private void ApplyConnectionColor(Color connectionColor)
    {
        foreach (var handLandmarkList in children)
        {
            if (handLandmarkList != null) { handLandmarkList.SetConnectionColor(connectionColor); }
        }
    }

    private void ApplyConnectionWidth(float connectionWidth)
    {
        foreach (var handLandmarkList in children)
        {
            if (handLandmarkList != null) { handLandmarkList.SetConnectionWidth(connectionWidth); }
        }
    }
}


