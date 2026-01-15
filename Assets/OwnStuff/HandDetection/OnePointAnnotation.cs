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
            //position.x= Mathf.Round(position.x * 1000f) / 1000f;
            //position.y= Mathf.Round(position.y * 1000f) / 1000f;
            //position.z= Mathf.Round(position.z * 1000f) / 1000f;
            position.x= Mathf.Round(position.x);
            position.y= Mathf.Round(position.y);
            position.z= Mathf.Round(position.z);

            if (!visualizeZ)
            {
                position.z = 0.0f;
            }
        //    var dis = Vector3.Distance(oldpos, position);
           // Debug.Log("Moving point by distance: " + dis.ToString());


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

            //var position = GetScreenRect().GetPoint(target, Vector3.one ,rotationAngle, isMirrored);


            //var position = RealWorldCoordinate.RealWorldToLocalPoint(
            //target.x*multiplyer,target.y* multiplyer, target.z* multiplyer, Vector3.one ,rotationAngle, isMirrored);

            var position = GetScreenRect().GetPoint(in target, Vector3.one * multiplyer, rotationAngle, isMirrored);

            //Yreversed = true


            //if (!visualizeZ)
            //{
            //    position.z = 0.0f;
            //}


            position.x = Mathf.Round(position.x*  roundingFactor) / roundingFactor;
            position.y = Mathf.Round(position.y * roundingFactor) / roundingFactor;
            position.z = Mathf.Round(position.z * roundingFactor) / roundingFactor;


            if (!NewPositionFarEnough(oldpos, position, 1f)) return;
            //Debug.Log("Moving with distance of: " + Vector3.Distance(oldpos, position));


            //float newX = Mathf.Abs(oldpos.x-position.x) > 5f ? position.x : oldpos.x;
            //float newY = Mathf.Abs(oldpos.y-position.y) > 5f ? position.y : oldpos.y;
            //float newZ = Mathf.Abs(oldpos.z-position.z) > 5f ? position.z : oldpos.z;
            //position = new Vector3(newX, newY, newZ);


            transform.position = position;  
      }
    }

    private bool NewPositionFarEnough(Vector3 oldPos, Vector3 newPos, float threshold)
    {
        var distance = Vector3.Distance(oldPos, newPos);
        return distance >= threshold;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //  Debug.Log("Piano key pressed by: " + other.gameObject.name);
    //    // Here you can add code to play a sound or trigger an animation
    //    if (!inPianoKey)
    //    {
    //        other.gameObject.GetComponent<PlayPianoKey>()?.PlayNote();
    //        lastCollided = other;
    //    }
    //    //else
    //    //{
    //    //    if (other.gameObject.name == lastCollided.gameObject.name)
    //    //    {
    //    //       // Debug.Log("Exiting the same piano key we entered.");
    //    //        Debug.Log(other.gameObject.name + " entering same key.");
    //    //        // inPianoKey = false;
    //    //    }
    //    //}
    //        inPianoKey = true;

    //    //other.gameObject.GetComponent<PlayPianoKey>()?.PlayNote();

    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    //if (other.gameObject.name == lastCollided.gameObject.name)
    //    //{
    //    //    Debug.Log("Exiting the same piano key we entered.");
    //    //    Debug.Log(other.gameObject.name + " exited the piano key trigger.");
    //    //    // inPianoKey = false;
    //    //}
    //    //else
    //    //{
    //    //     Debug.Log("Exiting a different piano key than we entered.");
    //    //}


    //    //inPianoKey = false;
    //    // Debug.Log(other.gameObject.name + " exited the piano key trigger.");
    //    //chekc if the piano key you are exit colliding with is happening weither while enter another one or maybe exit if that is
    //    //this one or another one? check the enter  exit and if you know the last one or maybe you should save it




    //}

    private void OnCollisionEnter(Collision collision)
    {
        //you first collide again before exiting the other one
        //you can check if you are still collided with something first before exiting
        if (!inPianoKey)
        {
          //  Debug.Log("Collided with: " + collision.gameObject.name);
            inPianoKey = true;
            collision.gameObject.GetComponent<PlayPianoKey>()?.PlayNote();

        }
        lastCollided = collision.collider;

        if (collision.gameObject.tag == "PianoCollider")
        {
         //   Debug.Log("Collided with piano key collider: " + collision.gameObject.name);
            inPianoKey = false;
        }

    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    bool trig = collision.gameObject.GetComponent<PlayPianoKey>().isTriggered;

    //    if (collision.collider == lastCollided && !trig)
    //    {
    //        Debug.Log("Last collided with: " + lastCollided.gameObject.name);
    //        Debug.Log("Stopped colliding with in pointAnnotation: " + collision.gameObject.name);
    //        inPianoKey = false;
    //    }
    //}

}

