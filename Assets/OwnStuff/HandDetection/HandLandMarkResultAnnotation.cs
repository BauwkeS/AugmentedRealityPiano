using UnityEngine;

using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;

//public class HandLandMarkResultAnnotation : AnnotationController<MultiHandLandmarkListAnnotation>
//{
//    [SerializeField] private bool _visualizeZ = false;

//    private readonly object _currentTargetLock = new object();
//    private HandLandmarkerResult _currentTarget;

//    public void DrawNow(HandLandmarkerResult target)
//    {
//        target.CloneTo(ref _currentTarget);
//        SyncNow();
//    }

//    public void DrawLater(HandLandmarkerResult target) => UpdateCurrentTarget(target);

//    protected void UpdateCurrentTarget(HandLandmarkerResult newTarget)
//    {
//        lock (_currentTargetLock)
//        {
//            newTarget.CloneTo(ref _currentTarget);
//            isStale = true;
//        }
//    }

//    protected override void SyncNow()
//    {
//        lock (_currentTargetLock)
//        {
//            isStale = false;
//            annotation.SetHandedness(_currentTarget.handedness);
//            annotation.Draw(_currentTarget.handLandmarks, _visualizeZ);
//        }
//    }
//}

public class HandLandMarkResultAnnotation : MonoBehaviour
{
    [SerializeField] protected MultiHandLandmarkListAnnotation annotation;
    protected bool isStale = false;

    [SerializeField] private bool _visualizeZ = false;

    private readonly object _currentTargetLock = new object();
    private HandLandmarkerResult _currentTarget;

    public bool isMirrored
    {
        get => annotation.isMirrored;
        set
        {
            if (annotation.isMirrored != value)
            {
                annotation.isMirrored = value;
            }
        }
    }

    public RotationAngle rotationAngle
    {
        get => annotation.rotationAngle;
        set
        {
            if (annotation.rotationAngle != value)
            {
                annotation.rotationAngle = value;
            }
        }
    }

    public Vector2Int imageSize { get; set; }

    protected virtual void Start()
    {
        if (!TryGetComponent<RectTransform>(out var _))
        {
            //Logger.LogVerbose(GetType().Name, $"Adding RectTransform to {gameObject.name}");
            var rectTransform = gameObject.AddComponent<RectTransform>();
            // stretch width and height by default
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }

    protected virtual void LateUpdate()
    {
        if (isStale)
        {
            SyncNow();
        }
    }

    protected virtual void OnDestroy()
    {
        if (annotation != null)
        {
            Destroy(annotation);
            annotation = null;
        }
        isStale = false;
    }

    protected void SyncNow()
    {
        lock (_currentTargetLock)
        {
            isStale = false;
            annotation.SetHandedness(_currentTarget.handedness);
            annotation.Draw(_currentTarget.handLandmarks, _visualizeZ);
        }
    }

    protected void UpdateCurrentTarget<TValue>(TValue newTarget, ref TValue currentTarget)
    {
        if (IsTargetChanged(newTarget, currentTarget))
        {
            currentTarget = newTarget;
            isStale = true;
        }
    }

    protected bool IsTargetChanged<TValue>(TValue newTarget, TValue currentTarget)
    {
        // It's assumed that target has not changed iff previous target and new target are both null.
        return currentTarget != null || newTarget != null;
    }
    public void DrawNow(HandLandmarkerResult target)
    {
        target.CloneTo(ref _currentTarget);
        SyncNow();
    }

    public void DrawLater(HandLandmarkerResult target) => UpdateCurrentTarget(target);

    protected void UpdateCurrentTarget(HandLandmarkerResult newTarget)
    {
        lock (_currentTargetLock)
        {
            newTarget.CloneTo(ref _currentTarget);
            isStale = true;
        }
    }
}