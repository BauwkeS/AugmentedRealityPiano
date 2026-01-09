using Mediapipe.Unity.Sample;
using Mediapipe.Unity;
using UnityEngine;
using System.Collections;

public abstract class TaskApiRunner<TTask> : BaseRunner where TTask : Mediapipe.Tasks.Vision.Core.BaseVisionTaskApi
{
    [SerializeField] protected ScreenMain screen;

    private Coroutine _coroutine;
    protected TTask taskApi;

    public RunningMode runningMode;

    public override void Play()
    {
        if (_coroutine != null)
        {
            Stop();
        }
        base.Play();
        _coroutine = StartCoroutine(Run());
    }

    public override void Pause()
    {
        base.Pause();
        ImageSourceProvider.ImageSource.Pause();
    }

    public override void Resume()
    {
        base.Resume();
        var _ = StartCoroutine(ImageSourceProvider.ImageSource.Resume());
    }

    public override void Stop()
    {
        base.Stop();
        StopCoroutine(_coroutine);
        ImageSourceProvider.ImageSource.Stop();
        taskApi?.Close();
        taskApi = null;
    }

    protected abstract IEnumerator Run();

    protected static void SetupAnnotationController(HandLandMarkResultAnnotation annotationController, ImageSource imageSource, bool expectedToBeMirrored = false)
    {
        annotationController.isMirrored = expectedToBeMirrored;
        annotationController.imageSize = new Vector2Int(imageSource.textureWidth, imageSource.textureHeight);
    }
}
