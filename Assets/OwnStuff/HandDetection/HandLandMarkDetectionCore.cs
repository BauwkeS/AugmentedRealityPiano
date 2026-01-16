using System.Collections;
using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine;
using UnityEngine.Rendering;


public class HandLandMarkDetectionCore : TaskApiRunner<HandLandmarker>
{
    [SerializeField] private HandLandMarkResultAnnotation _handLandmarkerResultAnnotationController;

    private Mediapipe.Unity.Experimental.TextureFramePool _textureFramePool;

    public readonly DetectionConfig config = new DetectionConfig();

    public override void Stop()
    {
        base.Stop();
        _textureFramePool?.Dispose();
        _textureFramePool = null;
    }

    protected override IEnumerator Run()
    {
       // Debug.Log($"Delegate = {config.Delegate}");
       // Debug.Log($"Image Read Mode = {config.ImageReadMode}");
        //Debug.Log($"Running Mode = {config.RunningMode}");
     //   Debug.Log($"NumHands = {config.NumHands}");
       // Debug.Log($"MinHandDetectionConfidence = {config.MinHandDetectionConfidence}");
        //Debug.Log($"MinHandPresenceConfidence = {config.MinHandPresenceConfidence}");
        //Debug.Log($"MinTrackingConfidence = {config.MinTrackingConfidence}");

        yield return Mediapipe.Unity.Sample.AssetLoader.PrepareAssetAsync(config.ModelPath);

        var options = config.GetHandLandmarkerOptions(OnHandLandmarkDetectionOutput);
        taskApi = HandLandmarker.CreateFromOptions(options, Mediapipe.Unity.GpuManager.GpuResources);
        //taskApi = HandLandmarker.CreateFromOptions(options, Mediapipe.Unity.GpuManager.GpuResources);
        var imageSource = Mediapipe.Unity.Sample.ImageSourceProvider.ImageSource;

        yield return imageSource.Play();

        if (!imageSource.isPrepared)
        {
            Debug.LogError("Failed to start ImageSource, exiting...");
            yield break;
        }

        // Use RGBA32 as the input format.
        _textureFramePool = new Mediapipe.Unity.Experimental.TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);

        // NOTE: The screen will be resized later, keeping the aspect ratio.
        screen.Initialize(imageSource);

        SetupAnnotationController(_handLandmarkerResultAnnotationController, imageSource);

        var transformationOptions = imageSource.GetTransformationOptions();
        var flipHorizontally = transformationOptions.flipHorizontally;
        var flipVertically = transformationOptions.flipVertically;
        var imageProcessingOptions = new Mediapipe.Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: (int)transformationOptions.rotationAngle);


       // var testingOptions = imageSource.GetTestingConfig();

        AsyncGPUReadbackRequest req = default;
        var waitUntilReqDone = new WaitUntil(() => req.done);
        var waitForEndOfFrame = new WaitForEndOfFrame();
        var result = HandLandmarkerResult.Alloc(options.numHands);

        while (true)
        {
            if (isPaused)
            {
                yield return new WaitWhile(() => isPaused);
            }

            if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
            {
                yield return new WaitForEndOfFrame();
                continue;
            }

            // Build the input Image
            Mediapipe.Image image;

            req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
            

            if (req.hasError)
            {
                Debug.LogWarning($"Failed to read texture from the image source");
                continue;
            }
            image = textureFrame.BuildCPUImage();
            textureFrame.Release();

            yield return waitUntilReqDone;

            //running mode livetsream
            taskApi.DetectAsync(image, GetCurrentTimestampMillisec(), imageProcessingOptions);
            
        }
    }

    private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Mediapipe.Image image, long timestamp)
    {
        _handLandmarkerResultAnnotationController.DrawLater(result);
    }
}
