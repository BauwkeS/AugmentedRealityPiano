using UnityEngine;

using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using Mediapipe.Tasks.Components.Containers;
using System.Collections.Generic;
using Mediapipe;

public class HandLandMarkResultAnnotation : MonoBehaviour
{
    [SerializeField] protected MultiHandsAnnotation annotation;
    //[SerializeField] protected MultiHandsAnnotation annotationForWorld;
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
           // annotationForWorld.SetThoseColors();
            annotation.Draw(bufferedLandmarkList, _visualizeZ);
           // annotation.Draw(_currentTarget.handLandmarks, _visualizeZ);
           // annotationForWorld.DrawWorldHand(_currentTarget.handWorldLandmarks,uiPrefab, _visualizeZ);


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
                //int handCount = landmarkBufferList[0].Count;   // usually 2
                int handCount = _currentTarget.handLandmarks.Count;   // usually 2
                int jointCount = landmarkBufferList[0][0].Count; // 21

                for (int handIndex = 0; handIndex < handCount; handIndex++)
                {
                    var averaged = default(NormalizedLandmarks);

                    if (handIndex >= landmarkBufferList[0].Count)
                    {
                        // No hand detected in this frame for this hand index.
                        // Just add an empty NormalizedLandmarks
                        bufferedLandmarkList.Add(averaged);
                        continue;
                    }

                    landmarkBufferList[0][handIndex].CloneTo(ref averaged);

                   // var averagedJoints = new List<Mediapipe.Tasks.Components.Containers.NormalizedLandmark>(jointCount);

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


                //need to average the xyz position from all the collected points and set that as the averaged position to take

                //all the collected landmarks to calculate

                //List<NormalizedLandmarks> averagedLandmarks = new List<NormalizedLandmarks>();


                // bufferedLandmarkList.Clear();

                //foreach (List<NormalizedLandmarks> landmarkLists in landmarkBufferList) //the 5 sets of everything
                //{
                //    foreach (NormalizedLandmarks landmarks in landmarkLists) //the 2 hands per set
                //    {
                //        //you should check per hand and get the average of each hand separately
                //        var NormalizedLandmarks = new NormalizedLandmarks();

                //        foreach (NormalizedLandmark landmark in landmarks.landmarks) //the 21 marks in one hand
                //        {
                //            landmark.z;
                //        }
                //    }
                //}



                //average the landmarks
                //bufferedLandmarkList.Clear();
                //for (int i = 0; i < 21; i++) //for 21 landmarks
                //{
                //    var sumLandmarks = new NormalizedLandmarks();

                //    //you need to fill these landmarksin the sum because they result to 0 right now? or somethimg because line 158 is missing

                //    int count = 0;
                //    foreach (var landmarkList in landmarkBufferList)
                //    {
                //        if (i < landmarkList.Count)
                //        {
                //            var landmarks = landmarkList[i];
                //            for (int j = 0; j < landmarks.Count; j++)
                //            {
                //                var point = landmarks.GetLandmark(j);
                //                var sumPoint = sumLandmarks.GetLandmark(j);
                //                sumPoint.SetPoint(
                //                    sumPoint.x + point.x,
                //                    sumPoint.y + point.y,
                //                    sumPoint.z + point.z
                //                );
                //                sumLandmarks.SetLandmark(j, sumPoint);
                //            }
                //            count++;
                //        }
                //    }
                //    //average
                //    for (int j = 0; j < sumLandmarks.Count; j++)
                //    {
                //        var sumPoint = sumLandmarks.GetLandmark(j);
                //        sumPoint.SetPoint(
                //            sumPoint.x / count,
                //            sumPoint.y / count,
                //            sumPoint.z / count
                //        );

                //        sumLandmarks.SetLandmark(j, sumPoint);
                //    }
                //    //add to the buffered list the end result
                //    bufferedLandmarkList.Add(sumLandmarks);
                //}

                //okay so:
                //you have 2 hands
                //each hand holds 21 landmarks
                // you need to average those 21 positions
                //so for each hand you need to create a new NormalizedLandmarks


                //foreach(NormalizedLandmarks landmarksPerHand in landmarkBufferList[0])
                //{
                //    var sumLandmarks = NormalizedLandmarks.CreateFrom(landmarksPerHand);
                //    for (int j = 0; j < landmarksPerHand.Count; j++)
                //    {
                //        var sumPoint = new NormalizedLandmark();
                //        sumLandmarks.landmarks.Add(sumPoint);
                //    }
                //    bufferedLandmarkList.Add(sumLandmarks);
                //}








                //average the landmarks
                //bufferedLandmarkList.Clear();
                //for (int i = 0; i < clonedLandmarks.Count; i++)
                //{
                //    var sumLandmarks = new NormalizedLandmarks();

                //    //you need to fill these landmarksin the sum because they result to 0 right now? or somethimg because line 158 is missing

                //    int count = 0;
                //    foreach (var landmarkList in landmarkBufferList)
                //    {
                //        if (i < landmarkList.Count)
                //        {
                //            var landmarks = landmarkList[i];
                //            for (int j = 0; j < landmarks.Count; j++)
                //            {
                //                var point = landmarks.GetLandmark(j);
                //                var sumPoint = sumLandmarks.GetLandmark(j);
                //                sumPoint.SetPoint(
                //                    sumPoint.x + point.x,
                //                    sumPoint.y + point.y,
                //                    sumPoint.z + point.z
                //                );
                //                sumLandmarks.SetLandmark(j, sumPoint);
                //            }
                //            count++;
                //        }
                //    }
                //    //average
                //    for (int j = 0; j < sumLandmarks.Count; j++)
                //    {
                //        var sumPoint = sumLandmarks.GetLandmark(j);
                //        sumPoint.SetPoint(
                //            sumPoint.x / count,
                //            sumPoint.y / count,
                //            sumPoint.z / count
                //        );

                //        sumLandmarks.SetLandmark(j, sumPoint);
                //    }
                //    //add to the buffered list the end result
                //    bufferedLandmarkList.Add(sumLandmarks);
                //}
                isStale = true;
            }
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

    //public void SpawnHandsWorld(Landmark pos)
    //{
    //   // RectTransform canvasRect = GetComponent<RectTransform>();
    //    // Instantiate as child of the canvas (keeps UI layering)
    //    // Position relative to rawImageRect (use TransformPoint -> inverse of canvas space)

    //    Vector3 landmarkPos = new Vector3(
    //        (float)pos.x,
    //        (float)pos.y,
    //        (float)pos.z
    //    );

    //    Debug.Log("Landmark: " + pos.ToString());

    //    // Convert the world pos to the canvas local point
    //    //Vector2 canvasLocal;
    //    //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(null, worldPos), null, out canvasLocal);

    //    //go.position = landmarkPos;

    //    Instantiate(uiPrefab,landmarkPos*100,Quaternion.identity);

    //}

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