using Mediapipe.Unity.CoordinateSystem;
using UnityEngine;

using mplt = Mediapipe.LocationData.Types;
using mptcc = Mediapipe.Tasks.Components.Containers;

#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
using Mediapipe.Unity;
using Mediapipe;
#pragma warning restore IDE0065

public class OnePointAnnotation : HierarchicalAnnotation
  {
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
}

