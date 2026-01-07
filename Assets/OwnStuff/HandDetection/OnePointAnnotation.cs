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
        transform.localScale = 25.0f * Vector3.one; //15.0f is the radius of the dot
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
        var position = GetScreenRect().GetPoint(in target, rotationAngle, isMirrored);
        if (!visualizeZ)
        {
          position.z = 0.0f;
        }
        transform.localPosition = position;
      }
    }
  }

