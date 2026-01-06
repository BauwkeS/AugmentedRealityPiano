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
    [SerializeField] private PointListAnnotation _landmarkListAnnotation;
    [SerializeField] private ConnectionListAnnotation _connectionListAnnotation;
    [SerializeField] private Color _leftLandmarkColor = Color.green;
    [SerializeField] private Color _rightLandmarkColor = Color.green;
    [SerializeField] private Color _fingertipLandmarkColor = Color.red;



    public enum Hand
    {
        Left,
        Right,
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

    public void SetColorOfFingerTips()
    {
        if (_landmarkListAnnotation.count < 20) return;
        _landmarkListAnnotation.SetColorOfOnePoint(_fingertipLandmarkColor, 4);
        _landmarkListAnnotation.SetColorOfOnePoint(_fingertipLandmarkColor, 8);
        _landmarkListAnnotation.SetColorOfOnePoint(_fingertipLandmarkColor, 12);
        _landmarkListAnnotation.SetColorOfOnePoint(_fingertipLandmarkColor, 16);
        _landmarkListAnnotation.SetColorOfOnePoint(_fingertipLandmarkColor, 20);
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
            _connectionListAnnotation.isMirrored = value;
            base.isMirrored = value;
        }
    }

    public override RotationAngle rotationAngle
    {
        set
        {
            _landmarkListAnnotation.rotationAngle = value;
            _connectionListAnnotation.rotationAngle = value;
            base.rotationAngle = value;
        }
    }

    public PointAnnotation this[int index] => _landmarkListAnnotation[index];

    private void Start()
    {
        _landmarkListAnnotation.Fill(_LandmarkCount);
        _connectionListAnnotation.Fill(_connections, _landmarkListAnnotation);
    }

    public void SetLandmarkColor(Color rightLandmarkColor)
    {
        _rightLandmarkColor = rightLandmarkColor;
        _leftLandmarkColor = rightLandmarkColor;
    }


    public void SetLandmarkRadius(float landmarkRadius)
    {
        _landmarkListAnnotation.SetRadius(landmarkRadius);
    }

    public void SetConnectionColor(Color connectionColor)
    {
        _connectionListAnnotation.SetColor(connectionColor);
    }

    public void SetConnectionWidth(float connectionWidth)
    {
        _connectionListAnnotation.SetLineWidth(connectionWidth);
    }

    public void SetColorsHands()
    {
        ///this is the one that actually works
        _landmarkListAnnotation.SetColor(_rightLandmarkColor);
        SetColorOfFingerTips();
    }


    public void Draw(IReadOnlyList<NormalizedLandmark> target, bool visualizeZ = false)
    {
        if (ActivateFor(target))
        {
            _landmarkListAnnotation.Draw(target, visualizeZ);
            // Draw explicitly because connection annotation's targets remain the same.
            _connectionListAnnotation.Redraw();
        }
    }

    public void Draw(NormalizedLandmarkList target, bool visualizeZ = false)
    {
        Draw(target?.Landmark, visualizeZ);
    }

    public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> target, bool visualizeZ = false)
    {
        if (ActivateFor(target))
        {
            _landmarkListAnnotation.Draw(target, visualizeZ);
            // Draw explicitly because connection annotation's targets remain the same.
            _connectionListAnnotation.Redraw();
        }
    }

    public void Draw(mptcc.NormalizedLandmarks target, bool visualizeZ = false)
    {
        Draw(target.landmarks, visualizeZ);
    }
}

