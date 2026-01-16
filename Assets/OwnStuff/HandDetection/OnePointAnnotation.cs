using Mediapipe.Unity.CoordinateSystem;
using UnityEngine;

using mplt = Mediapipe.LocationData.Types;
using mptcc = Mediapipe.Tasks.Components.Containers;

#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
using Mediapipe.Unity;
using Mediapipe;
using UnityEngine.Audio;
#pragma warning restore IDE0065

public class OnePointAnnotation : HierarchicalAnnotation
  {
    public bool inPianoKey = false;
    private Collider lastCollided;


    private void OnEnable()
    {
        transform.localScale = 15.0f * Vector3.one; //15.0f is the radius of the dot
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    public void Draw(in mptcc.NormalizedLandmark target, bool visualizeZ = true)
    {
      if (ActivateForFingerLandMark(target))
      {
            var oldpos = transform.localPosition;
        var position = GetScreenRect().GetPoint(in target, rotationAngle, isMirrored);
            position.x= Mathf.Round(position.x);
            position.y= Mathf.Round(position.y);
            position.z= Mathf.Round(position.z);

            if (!visualizeZ)
            {
                position.z = 0.0f;
            }

            if (!NewPositionFarEnough(oldpos,position,4.5f)) return;

            //Debug.Log("Moving with distance of: " + Vector3.Distance(oldpos, position));

            transform.localPosition = position;

            //Debug.Log("Drawing point at: " + target.ToString());
        }
    }
    
    public void DrawWorldHand(in mptcc.Landmark target, bool visualizeZ = true)
    {
      if (ActivateForFingerLandMark(target))
      {
            var oldpos = transform.position;
            float multiplyer = 1.0f;
            float roundingFactor = 100.0f;

            var position = GetScreenRect().GetPoint(in target, Vector3.one * multiplyer, rotationAngle, isMirrored);

            //if (!visualizeZ)
            //{
            //    position.z = 0.0f;
            //}

            position.x = Mathf.Round(position.x*  roundingFactor) / roundingFactor;
            position.y = Mathf.Round(position.y * roundingFactor) / roundingFactor;
            position.z = Mathf.Round(position.z * roundingFactor) / roundingFactor;


            if (!NewPositionFarEnough(oldpos, position, 1f)) return;
            //Debug.Log("Moving with distance of: " + Vector3.Distance(oldpos, position));

            transform.position = position;  
      }
    }

    private bool NewPositionFarEnough(Vector3 oldPos, Vector3 newPos, float threshold)
    {
        var distance = Vector3.Distance(oldPos, newPos);
        return distance >= threshold;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //you first check if you're already in a pianokey
        //due to collision detection being weird sometimes -> another collider added with the tag that is above the piano that you can hit (lift up your finger out the piano) to reset being able to hit the piano keys again
        if (!inPianoKey)
        {
            inPianoKey = true;
            collision.gameObject.GetComponent<PlayPianoKey>()?.PlayNote();

        }
        lastCollided = collision.collider;

        if (collision.gameObject.tag == "PianoCollider")
        {
            inPianoKey = false;
        }

    }
}

