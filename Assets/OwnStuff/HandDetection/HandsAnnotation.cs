using Mediapipe.Unity;
using Mediapipe;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using mptcc = Mediapipe.Tasks.Components.Containers;

#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
using static UnityEngine.GraphicsBuffer;
#pragma warning restore IDE0065



public sealed class HandsAnnotation : HierarchicalAnnotation
{
    [SerializeField] private GameObject _pointAllAnnotationPrefab;
    private AllPointsAnnotation _landmarkListAnnotation;

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

    public List<Vector3> GetFingerTipPositions()
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


    private void Start()
    {
        Instantiate(_pointAllAnnotationPrefab, transform);

        _landmarkListAnnotation = GetComponentInChildren<AllPointsAnnotation>();
        _landmarkListAnnotation.Fill(_LandmarkCount);
    }


    public void SetColorsHands(Color landmarkColor, Color fingertipColor)
    {
        _landmarkListAnnotation.SetColor(landmarkColor);
        SetColorOfFingerTips(fingertipColor);
    }

    public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
    {
        if (ActivateFor(target))
        {
            _landmarkListAnnotation.Draw(target, visualizeZ);
        }
    }

    public void Draw(mptcc.NormalizedLandmarks target, bool visualizeZ = false)
    {
        Draw(target.landmarks, visualizeZ);
    }
}






//public sealed class HandsAnnotation : HierarchicalAnnotation
//{
//    //[SerializeField] private AllPointsAnnotation _landmarkListAnnotation;

//    [SerializeField] private ListAnnotation<OnePointAnnotation> _handPoints;
//    //private List<OnePointAnnotation> _handPoints;

//    [SerializeField] private GameObject _annotationPrefabOnePoint;

//    private const int _LandmarkCount = 21;



//    public void SetColorOfFingerTips(Color fingertipColor)
//    {
//        if (_handPoints.count < 20) return;
//        SetColorOfOnePoint(fingertipColor, 4);
//        SetColorOfOnePoint(fingertipColor, 8);
//        SetColorOfOnePoint(fingertipColor, 12);
//        SetColorOfOnePoint(fingertipColor, 16);
//        SetColorOfOnePoint(fingertipColor, 20);
//    }

//    //public List<Vector3> GetFingerTipPositions()
//    //{
//    //    List<Vector3> fingertipPositions = new List<Vector3>();
//    //    if (_landmarkListAnnotation.count < 20) return fingertipPositions;
//    //    fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(4));
//    //    fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(8));
//    //    fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(12));
//    //    fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(16));
//    //    fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(20));
//    //    return fingertipPositions;
//    //}

//    public override bool isMirrored
//    {
//        set
//        {
//            _handPoints.isMirrored = value;
//            base.isMirrored = value;
//        }
//    }

//    public override RotationAngle rotationAngle
//    {
//        set
//        {
//            _handPoints.rotationAngle = value;
//            base.rotationAngle = value;
//        }
//    }


//    private void Start()
//    {
//        // Replace direct instantiation of ListAnnotation with a factory method or derived class instantiation
//        // _handPoints = new ListAnnotation<OnePointAnnotation>();
//        _handPoints.Fill(_LandmarkCount);
//    }

//    public void SetColorsHands(Color landmarkColor, Color fingertipColor)
//    {
//        //_handPoints.SetColor(landmarkColor);
//        foreach (var point in _handPoints.GetChildren())
//        {
//            if (point != null) { point.SetColor(landmarkColor); }
//        }
//        SetColorOfFingerTips(fingertipColor);
//    }

//    //public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
//    //{
//    //    if (ActivateFor(target))
//    //    {
//    //        _handPoints.Draw(target, visualizeZ);
//    //    }
//    //}

//    //public void Draw(mptcc.NormalizedLandmarks target, bool visualizeZ = false)
//    //{
//    //    Draw(target.landmarks, visualizeZ);
//    //}




//    public void SetColor(Color color)
//    {
//        foreach (var point in _handPoints.GetChildren())
//        {
//            if (point != null) { point.SetColor(color); }
//        }
//    }

//    public Vector3 GetPointPosition(int pointIndex)
//    {
//        return _handPoints[pointIndex].transform.localPosition;
//    }
//    public void SetColorOfOnePoint(Color color, int pointIndex)
//    {
//        _handPoints[pointIndex].SetColor(color);
//    }

//    //public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> targets, bool visualizeZ = true)
//    //{
//    //    if (ActivateFor(targets))
//    //    {
//    //        _handPoints.CallActionForAll(targets, (annotation, target) =>
//    //        {
//    //            if (annotation != null) { annotation.Draw(in target, visualizeZ); }
//    //        });
//    //    }
//    //}
//    public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
//    {
//        if (ActivateFor(target))
//        {
//            //_landmarkListAnnotation.Draw(target, visualizeZ);
//            _handPoints.CallActionForAll(target, (annotation, target) =>
//            {
//                if (annotation != null) { annotation.Draw(in target, visualizeZ); }
//            });
//        }
//    }

//    public void Draw(mptcc.NormalizedLandmarks target, bool visualizeZ = false)
//    {
//        Draw(target.landmarks, visualizeZ);
//    }

//    //protected override OnePointAnnotation InstantiateChild(bool isActive = true)
//    //{
//    //    var annotation = base.InstantiateChild(isActive);
//    //    return annotation;
//    //}
//}
