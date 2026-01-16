using Mediapipe.Unity;
using Mediapipe;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using mptcc = Mediapipe.Tasks.Components.Containers;

#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
using static UnityEngine.GraphicsBuffer;
using Mediapipe.Tasks.Components.Containers;
#pragma warning restore IDE0065



public sealed class HandsAnnotation : HierarchicalAnnotation
{
    [SerializeField] private GameObject _pointAllAnnotationPrefab;
    private AllPointsAnnotation _landmarkListAnnotation;
    public List<Vector3> FingertipPositions
    {
        get
        {
            List<Vector3> fingertipPositions = new List<Vector3>();
            if (_landmarkListAnnotation.count < 20) return fingertipPositions;
            fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(4));
            fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(8));
            fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(12));
            fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(16));
            fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(20));
            return fingertipPositions;
        }
    }

    private const int _LandmarkCount = 21;

    public void SetColorOfFingerTips(Color fingertipColor)
    {
        if (_landmarkListAnnotation.count < 20) return;
        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 4);
        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 8);
        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 12);
        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 16);
        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 20);
    }

    public override bool isMirrored
    {
        set
        {
            _landmarkListAnnotation.isMirrored = value;
            base.isMirrored = value;
        }
    }

    public override RotationAngle rotationAngle
    {
        set
        {
            _landmarkListAnnotation.rotationAngle = value;
            base.rotationAngle = value;
        }
    }
    private void Awake()
    {
        if (_pointAllAnnotationPrefab == null)
        {
            Debug.LogError("Point All Annotation Prefab is not assigned in the inspector.");
        }
        Instantiate(_pointAllAnnotationPrefab, transform);

        _landmarkListAnnotation = GetComponentInChildren<AllPointsAnnotation>();
        _landmarkListAnnotation.Fill(_LandmarkCount);
        OnlyActivateFingertips();
    }

    public void SetColorsHands(Color landmarkColor, Color fingertipColor)
    {
        _landmarkListAnnotation.SetColor(landmarkColor);
        SetColorOfFingerTips(fingertipColor);
    }

    public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false, bool EnableDistanceFiltering = true)
    {
        if (ActivateFor(target))
        {
            _landmarkListAnnotation.Draw(target, visualizeZ, EnableDistanceFiltering);
        }
    }
    public void DrawWorldHand(IReadOnlyList<mptcc.Landmark> target, bool visualizeZ = false)
    {
        if (ActivateFor(target))
        {
            _landmarkListAnnotation.DrawWorldHand(target, visualizeZ);
        }
    }

    public void Draw(mptcc.NormalizedLandmarks target, bool visualizeZ = false, bool EnableDistanceFiltering = true)
    {
        Draw(target.landmarks, visualizeZ,EnableDistanceFiltering);
    }

    public void DrawWorldHand(Landmarks target, bool visualizeZ = false)
    {
        DrawWorldHand(target.landmarks, visualizeZ);
    }

    private void OnlyActivateFingertips()
    {
        for (int i = 0; i < _landmarkListAnnotation.count; i++)
        {
            if (i == 4 || i == 8 || i == 12 || i == 16 || i == 20)
            {
                _landmarkListAnnotation[i].SetActive(true);
            }
            else
            {
                _landmarkListAnnotation[i].SetActive(false);

            }
        }
    }
}
