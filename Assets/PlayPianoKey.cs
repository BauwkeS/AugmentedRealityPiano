using UnityEngine;

public class PlayPianoKey : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Piano key pressed by: " + collision.gameObject.name);
    }
}
