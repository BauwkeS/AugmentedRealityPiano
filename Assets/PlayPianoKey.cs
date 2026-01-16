using UnityEngine;

public class PlayPianoKey : MonoBehaviour
{
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

    public void PlayNote()
    {
        if (pianoNoteSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pianoNoteSound);
        }
    }
}
