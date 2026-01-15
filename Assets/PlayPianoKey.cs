using UnityEngine;

public class PlayPianoKey : MonoBehaviour
{
    //private bool isTriggered = false;
    [SerializeField] private AudioClip pianoNoteSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (pianoNoteSound == null)
        {
            Debug.LogWarning("Piano note sound is not assigned in the inspector.");
        }
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isTriggered)
    //    {
    //        isTriggered = true;
    //        if (pianoNoteSound != null && audioSource != null)
    //        {
    //            audioSource.PlayOneShot(pianoNoteSound);
    //        }
    //        Debug.Log("Piano key pressed by: " + other.gameObject.name);
    //        // Here you can add code to play a sound or trigger an animation
    //    }
    //}
    private void OnTriggerExit(Collider other)
    {
       // other.gameObject.
        //isTriggered = false;
        //Debug.Log(other.gameObject.name + " exited the piano key trigger.");
    }

    public void PlayNote()
    {
        if (pianoNoteSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pianoNoteSound);
        }
    }
}
