using System.Collections.Generic;
using UnityEngine;

using mplt = Mediapipe.LocationData.Types;
using mptcc = Mediapipe.Tasks.Components.Containers;

#pragma warning disable IDE0065
using Color = UnityEngine.Color;
using Mediapipe.Unity;
using Mediapipe;
using System;
#pragma warning restore IDE0065

//public class AllPointsAnnotation : ListAnnotation<OnePointAnnotation>
//{

//    public void SetColor(Color color)
//    {
//        foreach (var point in children)
//        {
//            if (point != null) { point. SetColor(color); }
//        }
//    }

//    public Vector3 GetPointPosition(int pointIndex)
//    {
//        return children[pointIndex].transform.localPosition;
//    }
//    public void SetColorOfOnePoint(Color color, int pointIndex)
//    {
//        children[pointIndex].SetColor(color);
//    }

//    public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> targets, bool visualizeZ = true)
//    {
//        if (ActivateFor(targets))
//        {
//            CallActionForAll(targets, (annotation, target) =>
//            {
//                if (annotation != null) { annotation.Draw(in target, visualizeZ); }
//            });
//        }
//    }

//    protected override OnePointAnnotation InstantiateChild(bool isActive = true)
//    {
//        var annotation = base.InstantiateChild(isActive);
//        return annotation;
//    }
//}


//public class AllPointsAnnotation : HierarchicalAnnotation
//{
//    [SerializeField] private float _radius = 15.0f;

//    [SerializeField] GameObject _handPointPrefab;
//    private List<OnePointAnnotation> _handPoints;
//    public List<OnePointAnnotation> HandPoints => children;

//    private const int _LandmarkCount = 21;

//    private List<OnePointAnnotation> children
//    {
//        get
//        {
//            if (_handPoints.Count <= 0)
//            {
//                _handPoints = new List<OnePointAnnotation>();
//                while (_handPoints.Count < _LandmarkCount)
//                {
//                    OnePointAnnotation newPoint = new();
//                    newPoint.SetActive(false);
//                    newPoint.isMirrored = isMirrored;
//                    newPoint.rotationAngle = rotationAngle;
//                    _handPoints.Add(newPoint);
//                }
//            }
//            return _handPoints;
//        }
//    }

//    private void Start()
//    {
//        if (_handPoints.Count <= 0)
//        {
//            _handPoints = new List<OnePointAnnotation>();
//            while (_handPoints.Count < _LandmarkCount)
//            {
//                OnePointAnnotation newPoint = new();
//                newPoint.SetActive(false);
//                newPoint.isMirrored = isMirrored;
//                newPoint.rotationAngle = rotationAngle;
//                _handPoints.Add(newPoint);
//            }
//        }
//    }

//    public void SetColor(Color color)
//    {
//        foreach (var point in children)
//        {
//            if (point != null) { point.SetColor(color); }
//        }
//    }

//    public Vector3 GetPointPosition(int pointIndex)
//    {
//        return children[pointIndex].transform.localPosition;
//    }
//    public void SetColorOfOnePoint(Color color, int pointIndex)
//    {
//        children[pointIndex].SetColor(color);
//    }

//    public void Draw(IReadOnlyList<mptcc.NormalizedLandmark> targets, bool visualizeZ = true)
//    {
//        if (ActivateFor(targets))
//        {
//            CallActionForAll(targets, (annotation, target) =>
//            {
//                if (annotation != null) { annotation.Draw(in target, visualizeZ); }
//            });
//        }
//    }

//    //protected override OnePointAnnotation InstantiateChild(bool isActive = true)
//    //{
//    //    var annotation = base.InstantiateChild(isActive);
//    //    annotation.SetRadius(_radius);
//    //    return annotation;
//    //}

//    private void CallActionForAll<TArg>(IReadOnlyList<TArg> argumentList, Action<OnePointAnnotation, TArg> action)
//    {
//        for (var i = 0; i < Mathf.Max(children.Count, argumentList.Count); i++)
//        {
//            if (i >= argumentList.Count)
//            {
//                // children.Count > argumentList.Count
//                action(children[i], default);
//                continue;
//            }

//            // reset annotations
//            //if (i >= children.Count)
//            //{
//            //    // children.Count < argumentList.Count
//            //    children.Add(InstantiateChild());
//            //}
//            //else if (children[i] == null)
//            //{
//            //    // child is not initialized yet
//            //    children[i] = InstantiateChild();
//            //}
//            action(children[i], argumentList[i]);
//        }
//    }
//}

