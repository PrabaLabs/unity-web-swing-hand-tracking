using System.Collections.Generic;
using UnityEngine;

// Holds the latest 21 hand landmarks from MediaPipe so any script can read them.
[DefaultExecutionOrder(-100)]
public class HandLandmarkBridge : MonoBehaviour
{
    [Tooltip("Flip X when the webcam image is mirrored")]
    public bool mirrorX = true;

    public bool HasHand { get; private set; }

    readonly Vector3[] latest = new Vector3[21];
    readonly Vector3[] points = new Vector3[21];
    readonly object gate = new object();
    bool hasNew, handVisible;

    // Call this from your MediaPipe result callback (it can run off the main thread).
    public void SetLandmarks(IList<Vector3> normalized)
    {
        lock (gate)
        {
            for (int i = 0; i < 21; i++) latest[i] = normalized[i];
            handVisible = true;
            hasNew = true;
        }
    }

    public void ClearHand()
    {
        lock (gate) { handVisible = false; hasNew = true; }
    }

    void Update()
    {
        lock (gate)
        {
            if (!hasNew) return;
            latest.CopyTo(points, 0);
            HasHand = handVisible;
            hasNew = false;
        }
    }

    public Vector3 Get(int index) => points[index];

    // MediaPipe: (0,0) is the top-left of the image. Unity screen: (0,0) is bottom-left.
    public Vector2 ToScreen(int index)
    {
        Vector3 p = points[index];
        float x = mirrorX ? 1f - p.x : p.x;
        return new Vector2(x * Screen.width, (1f - p.y) * Screen.height);
    }
}
