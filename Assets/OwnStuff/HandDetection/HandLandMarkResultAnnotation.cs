using UnityEngine;

using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using Mediapipe.Tasks.Components.Containers;
using System.Collections.Generic;
using Mediapipe;

public class HandLandMarkResultAnnotation : MonoBehaviour
{
    [SerializeField] protected MultiHandsAnnotation annotation;
    //[SerializeField] protected MultiHandsAnnotation annotationForWorld; if worldannotation is wanted comment code back out
    List<List<NormalizedLandmarks>> landmarkBufferList = new List<List<NormalizedLandmarks>>();
    [SerializeField] int maxBufferListSize = 5;
    List<NormalizedLandmarks> bufferedLandmarkList = new List<NormalizedLandmarks>();

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
        //world input cleanup
        //if (annotationForWorld != null)
        //{
        //    Destroy(annotationForWorld);
        //    annotationForWorld = null;
        //}
        isStale = false;
    }

    protected void SyncNow()
    {
        UpdateBufferLandmark();


        lock (_currentTargetLock)
        {
            isStale = false;
            annotation.SetThoseColors();
            annotation.Draw(bufferedLandmarkList, _visualizeZ); //draw from buffered landmarks for smoothing
           // annotation.Draw(_currentTarget.handLandmarks, _visualizeZ); //normal draw from input


           // annotationForWorld.SetThoseColors(); //colors for world input
           // annotationForWorld.DrawWorldHand(_currentTarget.handWorldLandmarks,uiPrefab, _visualizeZ); //draw the input but in world coords


        }
    }


    private void UpdateBufferLandmark()
    {
        //add the handlandmarks to a buffer for smoothing
        lock (_currentTargetLock)
        {
            if (_currentTarget.handLandmarks != null)
            {
                //clone the landmarks to avoid reference issues
                List<NormalizedLandmarks> clonedLandmarks = new List<NormalizedLandmarks>();
                foreach (var landmarks in _currentTarget.handLandmarks)
                {
                    var cloned = new NormalizedLandmarks();
                    landmarks.CloneTo(ref cloned);
                    clonedLandmarks.Add(cloned);
                }
                landmarkBufferList.Add(clonedLandmarks);
                if (landmarkBufferList.Count > maxBufferListSize)
                {
                    landmarkBufferList.RemoveAt(0);
                }
                //--


                bufferedLandmarkList.Clear();

                int frameCount = landmarkBufferList.Count;
                int handCount = _currentTarget.handLandmarks.Count;   // usually 2
                int jointCount = landmarkBufferList[0][0].Count; // 21

                for (int handIndex = 0; handIndex < handCount; handIndex++)
                {
                    var averaged = default(NormalizedLandmarks);

                    if (handIndex >= landmarkBufferList[0].Count)
                    {
                        //no second hand detected -> add empty landmarks as comparison
                        bufferedLandmarkList.Add(averaged);
                        continue;
                    }

                    landmarkBufferList[0][handIndex].CloneTo(ref averaged);

                    for (int jointIndex = 0; jointIndex < jointCount; jointIndex++)
                    {
                        float sumX = 0f, sumY = 0f, sumZ = 0f;

                        for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                        {
                            var lm = landmarkBufferList[frameIndex][handIndex].landmarks[jointIndex];
                            sumX += lm.x;
                            sumY += lm.y;
                            sumZ += lm.z;
                        }

                        averaged.landmarks[jointIndex] =
                        Mediapipe.Tasks.Components.Containers.NormalizedLandmark.CreateFrom(new Mediapipe.NormalizedLandmark
                        {
                            X = sumX / frameCount,
                            Y = sumY / frameCount,
                            Z = sumZ / frameCount
                        });
                    }

                    bufferedLandmarkList.Add(averaged);
                }

                isStale = true;
            }
        }
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