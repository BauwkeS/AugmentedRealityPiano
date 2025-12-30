using Mediapipe.Unity;
using System.Collections.Generic;
using UnityEngine;

//public class MediaPipeWorldMapper : MonoBehaviour
//{
//    [Header("Camera that renders the 3D world")]
//    public Camera mainCamera;

//    [Header("Object to spawn")]
//    public GameObject spawnPrefab;

//    [Header("Distance in meters from camera")]
//    public float spawnDistance = 0.5f;

//    /// <summary>
//    /// Spawns prefab in front of the camera based on MediaPipe normalized coords.
//    /// mpX and mpY must be 0–1.
//    /// </summary>
//    public void SpawnAtMediaPipePoint(float mpX, float mpY)
//    {
//        Vector3 worldPos = MediaPipeToWorld(mpX, mpY);
//        Instantiate(spawnPrefab, worldPos, Quaternion.identity);
//    }

//    /// <summary>
//    /// Moves a gameObject to the MediaPipe location (good for hand collider or cursor).
//    /// </summary>
//    public void MoveObjectToMediaPipePoint(GameObject obj, float mpX, float mpY)
//    {
//        Vector3 worldPos = MediaPipeToWorld(mpX, mpY);
//        obj.transform.position = worldPos;
//    }

//    /// <summary>
//    /// Converts MediaPipe normalized screen-space (0–1) into world coordinates.
//    /// </summary>
//    private Vector3 MediaPipeToWorld(float mpX, float mpY)
//    {
//        // Convert to screen pixel coordinates
//        Vector2 screenPoint = new Vector2(
//            mpX * Screen.width,
//            (1f - mpY) * Screen.height // MediaPipe Y is flipped
//        );

//        // Ray from the camera through the point
//        Ray ray = mainCamera.ScreenPointToRay(screenPoint);

//        // Position "spawnDistance" meters in front of camera
//        return ray.GetPoint(spawnDistance);
//    }
//}

//public class CanvasToWorldMapper : MonoBehaviour
//{
//    public Camera worldCamera;           // your 3D world camera
//    public RectTransform rawImageRect;   // your RawImage RectTransform
//    public GameObject prefab;
//    public float depth = 6f;             // how far into 3D space to spawn

//    public void SpawnAtMediaPipe(float mpX, float mpY)
//    {
//        // Convert normalized to RectTransform local coordinates
//        Vector2 localPos = NormalizedToRect(mpX, mpY);

//        // Convert RectTransform point -> screen point
//        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rawImageRect.TransformPoint(localPos));

//        // Convert screen -> ray -> world
//        Ray ray = worldCamera.ScreenPointToRay(screenPoint);
//        Vector3 worldPos = ray.GetPoint(depth);

//        Instantiate(prefab, worldPos, Quaternion.identity);
//    }

//    private Vector2 NormalizedToRect(float x, float y)
//    {
//        return new Vector2(
//            (x - 0.5f) * rawImageRect.sizeDelta.x,
//            (y - 0.5f) * rawImageRect.sizeDelta.y
//        );
//    }

//    private void Update()
//    {
//        // For testing: spawn object at center of screen on space key press
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            SpawnAtMediaPipe(depth, depth);
//        }
//    }
//}


public class SpawnUIOnRawImage : MonoBehaviour
{
    [Header("References")]
    public RectTransform rawImageRect;      // RawImage RectTransform that shows camera feed
    public RectTransform canvasRect;        // Root canvas RectTransform (or the Canvas' RectTransform)
    public RectTransform uiPrefab;          // prefab must be a RectTransform (UI element)

    public MultiHandLandmarkListAnnotation MultiHands;
   // private List<Vector3> _fingertips;

    private void Start()
    {
        MultiHands = FindAnyObjectByType<MultiHandLandmarkListAnnotation>();
        //_fingertips = MultiHands.FingerTipPositions;
    }

    // mpX/mpY are MediaPipe normalized coords (0..1)
    public void SpawnUIAtMediaPipe(float mpX, float mpY)
    {
        // Convert normalized (0..1) to local rect coords (centered)
        Vector2 localPos = new Vector2(
            (mpX - 0.5f) * rawImageRect.sizeDelta.x,
            (mpY - 0.5f) * rawImageRect.sizeDelta.y
        );

        // Instantiate as child of the canvas (keeps UI layering)
        RectTransform go = Instantiate(uiPrefab, canvasRect);
        // Position relative to rawImageRect (use TransformPoint -> inverse of canvas space)
        Vector3 worldPos = rawImageRect.TransformPoint(localPos);

        // Convert the world pos to the canvas local point
        Vector2 canvasLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(null, worldPos), null, out canvasLocal);

        go.anchoredPosition = canvasLocal;
    }


    public void SpawnUIAtMediaPipeVector(Vector3 pos)
    {
        // Instantiate as child of the canvas (keeps UI layering)
        RectTransform go = Instantiate(uiPrefab, canvasRect);
        // Position relative to rawImageRect (use TransformPoint -> inverse of canvas space)
        Vector3 worldPos = rawImageRect.TransformPoint(pos);

        // Convert the world pos to the canvas local point
        Vector2 canvasLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(null, worldPos), null, out canvasLocal);

        go.anchoredPosition = canvasLocal;
    }

    private void Update()
    {
        // For testing: spawn object at center of screen on space key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 pos = MultiHands.FingerTipPositions[1];
            SpawnUIAtMediaPipeVector(pos);
        }

        //Debug.Log("Fingertips found: " + MultiHands.FingerTipPositions.Count);

    }
}

