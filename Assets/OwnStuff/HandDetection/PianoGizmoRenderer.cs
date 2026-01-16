using UnityEngine;
using System.Collections.Generic;

//thank you ChatGPT for some help

public interface IBoundsRenderer
{
    void DrawBox(BoxCollider collider);
    void Clear();
}

public class PianoGizmoRenderer : MonoBehaviour, IBoundsRenderer
{
    public Color color = Color.yellow;
    public float width = 0.3f;

    private readonly Dictionary<BoxCollider, LineRenderer> lines =
        new Dictionary<BoxCollider, LineRenderer>();

    public void DrawBox(BoxCollider box)
    {
        if (!lines.TryGetValue(box, out var lr))
        {
            var go = new GameObject($"Bounds_{box.name}");
            go.transform.SetParent(transform, false);

            lr = go.AddComponent<LineRenderer>();
            lr.gameObject.layer = LayerMask.NameToLayer("Debug");
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.positionCount = 16;
            lr.widthMultiplier = width;
            lr.startColor = color;
            lr.endColor = color;
            lr.useWorldSpace = true;

            lines.Add(box, lr);
        }

        UpdateBox(lr, box);
    }

    public void Clear()
    {
        foreach (var lr in lines.Values)
            if (lr) Destroy(lr.gameObject);

        lines.Clear();
    }

    private void UpdateBox(LineRenderer lr, BoxCollider box)
    {
        Vector3 c = box.center;
        Vector3 e = box.size * 0.5f;
        Transform t = box.transform;

        Vector3[] p =
        {
            t.TransformPoint(c + new Vector3(-e.x,-e.y,-e.z)),
            t.TransformPoint(c + new Vector3( e.x,-e.y,-e.z)),
            t.TransformPoint(c + new Vector3( e.x,-e.y, e.z)),
            t.TransformPoint(c + new Vector3(-e.x,-e.y, e.z)),
            t.TransformPoint(c + new Vector3(-e.x,-e.y,-e.z)),

            t.TransformPoint(c + new Vector3(-e.x, e.y,-e.z)),
            t.TransformPoint(c + new Vector3( e.x, e.y,-e.z)),
            t.TransformPoint(c + new Vector3( e.x, e.y, e.z)),
            t.TransformPoint(c + new Vector3(-e.x, e.y, e.z)),
            t.TransformPoint(c + new Vector3(-e.x, e.y,-e.z)),

            t.TransformPoint(c + new Vector3(-e.x, e.y, e.z)),
            t.TransformPoint(c + new Vector3(-e.x,-e.y, e.z)),
            t.TransformPoint(c + new Vector3( e.x,-e.y, e.z)),
            t.TransformPoint(c + new Vector3( e.x, e.y, e.z)),
            t.TransformPoint(c + new Vector3( e.x, e.y,-e.z)),
            t.TransformPoint(c + new Vector3( e.x,-e.y,-e.z)),
        };

        lr.SetPositions(p);
    }
}


