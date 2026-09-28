using UnityEngine;

/// <summary>
/// Keeps this object and all of its children (including ones spawned at runtime,
/// such as MediaPipe landmark annotations) on a single layer, so they are rendered
/// by the camera that culls that layer.
/// </summary>
public class KeepChildrenOnLayer : MonoBehaviour
{
    [SerializeField] private string layerName = "UI";

    private int _layer;
    private int _lastChildCount = -1;

    private void Awake()
    {
        _layer = LayerMask.NameToLayer(layerName);
    }

    private void LateUpdate()
    {
        if (_layer < 0) return;

        // Only walk the hierarchy when it has changed size.
        int count = GetComponentsInChildren<Transform>(true).Length;
        if (count == _lastChildCount) return;
        _lastChildCount = count;

        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = _layer;
        }
    }
}
