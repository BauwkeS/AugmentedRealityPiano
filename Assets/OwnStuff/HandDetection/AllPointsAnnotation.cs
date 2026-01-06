using System.Collections.Generic;
using UnityEngine;

using mplt = Mediapipe.LocationData.Types;
using mptcc = Mediapipe.Tasks.Components.Containers;

#pragma warning disable IDE0065
using Color = UnityEngine.Color;
using Mediapipe.Unity;
using Mediapipe;
#pragma warning restore IDE0065

public class AllPointsAnnotation : ListAnnotation<OnePointAnnotation>
{
    [SerializeField] private float _radius = 15.0f;

    public void SetColor(Color color)
    {
        foreach (var point in children)
        {
            if (point != null) { point. SetColor(color); }
        }
    }

    public Vector3 GetPointPosition(int pointIndex)
    {
        return children[pointIndex].transform.localPosition;
    }
    public void SetColorOfOnePoint(Color color, int pointIndex)
    {
        children[pointIndex].SetColor(color);
    }

    public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> targets, bool visualizeZ = true)
    {
        if (ActivateFor(targets))
        {
            CallActionForAll(targets, (annotation, target) =>
            {
                if (annotation != null) { annotation.Draw(in target, visualizeZ); }
            });
        }
    }

    protected override OnePointAnnotation InstantiateChild(bool isActive = true)
    {
        var annotation = base.InstantiateChild(isActive);
        annotation.SetRadius(_radius);
        return annotation;
    }
}
