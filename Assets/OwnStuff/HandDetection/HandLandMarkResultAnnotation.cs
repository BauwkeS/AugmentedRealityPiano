using UnityEngine;

using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using Mediapipe.Tasks.Components.Containers;

public class HandLandMarkResultAnnotation : MonoBehaviour
{
    [SerializeField] protected MultiHandsAnnotation annotation;
    [SerializeField] protected MultiHandsAnnotation annotationForWorld;

    protected bool isStale = false;

    [SerializeField] private bool _visualizeZ = false;
    public GameObject uiPrefab;      


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
        if (annotationForWorld != null)
        {
            Destroy(annotationForWorld);
            annotationForWorld = null;
        }
        isStale = false;
    }

    protected void SyncNow()
    {
        lock (_currentTargetLock)
        {
            isStale = false;
            annotation.SetThoseColors();
            annotationForWorld.SetThoseColors();
            annotation.Draw(_currentTarget.handLandmarks, _visualizeZ);
            annotationForWorld.DrawWorldHand(_currentTarget.handWorldLandmarks,uiPrefab, _visualizeZ);


        }
    }
    //private void Update()
    //{
    //    // For testing: spawn object at center of screen on space key press
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        foreach (var worldN in _currentTarget.handWorldLandmarks)
    //        {
    //            foreach (var point in worldN.landmarks)
    //            {
    //                if(point.)
    //                SpawnHandsWorld(point);
    //            }
    //        }
    //    }

    //    //Debug.Log("Fingertips found: " + MultiHands.FingerTipPositions.Count);

    //}

    public void SpawnHandsWorld(Landmark pos)
    {
       // RectTransform canvasRect = GetComponent<RectTransform>();
        // Instantiate as child of the canvas (keeps UI layering)
        // Position relative to rawImageRect (use TransformPoint -> inverse of canvas space)

        Vector3 landmarkPos = new Vector3(
            (float)pos.x,
            (float)pos.y,
            (float)pos.z
        );

        Debug.Log("Landmark: " + pos.ToString());

        // Convert the world pos to the canvas local point
        //Vector2 canvasLocal;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(null, worldPos), null, out canvasLocal);

        //go.position = landmarkPos;

        Instantiate(uiPrefab,landmarkPos*100,Quaternion.identity);

    }

    //protected void UpdateCurrentTarget<TValue>(TValue newTarget, ref TValue currentTarget)
    //{
    //    if (IsTargetChanged(newTarget, currentTarget))
    //    {
    //        currentTarget = newTarget;
    //        isStale = true;
    //    }
    //}

    //protected bool IsTargetChanged<TValue>(TValue newTarget, TValue currentTarget)
    //{
    //    // It's assumed that target has not changed iff previous target and new target are both null.
    //    return currentTarget != null || newTarget != null;
    //}
    //public void DrawNow(HandLandmarkerResult target)
    //{
    //    target.CloneTo(ref _currentTarget);
    //    SyncNow();
    //}

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