using Mediapipe.Unity;
using Mediapipe;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using mptcc = Mediapipe.Tasks.Components.Containers;

#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
#pragma warning restore IDE0065

    public sealed class HandsAnnotation : HierarchicalAnnotation
{
    [SerializeField] private AllPointsAnnotation _landmarkListAnnotation;

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

    public OnePointAnnotation this[int index] => _landmarkListAnnotation[index];

    private void Start()
    {
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


//   public sealed class HandsAnnotation : HierarchicalAnnotation
//{
//    [SerializeField] private PointListAnnotation _landmarkListAnnotation;

//    private const int _LandmarkCount = 21;

//    public void SetColorOfFingerTips(Color fingertipColor)
//    {
//        if (_landmarkListAnnotation.count < 20) return;
//        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 4);
//        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 8);
//        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 12);
//        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 16);
//        _landmarkListAnnotation.SetColorOfOnePoint(fingertipColor, 20);
//    }

//    public List<Vector3> GetFingerTipPositions()
//    {
//        List<Vector3> fingertipPositions = new List<Vector3>();
//        if (_landmarkListAnnotation.count < 20) return fingertipPositions;
//        fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(4));
//        fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(8));
//        fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(12));
//        fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(16));
//        fingertipPositions.Add(_landmarkListAnnotation.GetPointPosition(20));
//        return fingertipPositions;
//    }

//    public override bool isMirrored
//    {
//        set
//        {
//            _landmarkListAnnotation.isMirrored = value;
//            base.isMirrored = value;
//        }
//    }

//    public override RotationAngle rotationAngle
//    {
//        set
//        {
//            _landmarkListAnnotation.rotationAngle = value;
//            base.rotationAngle = value;
//        }
//    }

//    public PointAnnotation this[int index] => _landmarkListAnnotation[index];

//    private void Start()
//    {
//        _landmarkListAnnotation.Fill(_LandmarkCount);
//    }


//    public void SetColorsHands(Color landmarkColor, Color fingertipColor)
//    {
//        _landmarkListAnnotation.SetColor(landmarkColor);
//        SetColorOfFingerTips(fingertipColor);
//    }

//    public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
//    {
//        if (ActivateFor(target))
//        {
//            _landmarkListAnnotation.Draw(target, visualizeZ);
//        }
//    }

//    public void Draw(mptcc.NormalizedLandmarks target, bool visualizeZ = false)
//    {
//        Draw(target.landmarks, visualizeZ);
//    }
//}

