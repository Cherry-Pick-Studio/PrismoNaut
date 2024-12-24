using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPath : MonoBehaviour
{
    public bool isLoop;

    private List<Transform> points = new List<Transform>();
    public List<Transform> Points { get => points; }

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
            points.Add(transform.GetChild(i));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        int count = transform.childCount;
        for (int i = 1; i < count; i++)
            Gizmos.DrawLine(transform.GetChild(i - 1).position, transform.GetChild(i).position);

        if (isLoop)
            Gizmos.DrawLine(transform.GetChild(count - 1).position, transform.GetChild(0).position);
    }
}
