using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

// Shoots the web, snaps it to a building corner and keeps it alive as a spring spline.
public class WebShooter : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public HandLandmarkBridge hand;
    public ThwipGestureDetector gesture;
    public SwingController swing;
    public SplineContainer web;
    public SplineExtrude webMesh;
    public SplineAnimate webTip;

    [Header("Aiming")]
    public float maxDistance = 80f;
    public LayerMask swingableLayers = ~0;
    public float cornerSnapRadius = 4f;
    [Tooltip("If the ray misses, look for a target this close to it (thin targets are hard to hit exactly)")]
    public float aimAssistRadius = 0.5f;
    public float handDepth = 0.6f;      // how far in front of the camera the web starts

    [Header("Web shape")]
    public float shootTime = 0.15f;
    public float startSag = 3f;
    public float startCurve = 2f;
    public float tightenTime = 0.35f;

    [Header("Spring")]
    public float stiffness = 120f;
    public float damping = 12f;

    [Header("Aim point")]
    public RectTransform canvasRect;    // canvas the reticle lives on
    public Camera uiCamera;             // camera the canvas renders with (Tracking Camera)
    public Image reticle;               // follows the index fingertip
    public Image targetMarker;          // sits on the point the web will attach to
    public Color idleColor = new Color(1f, 1f, 1f, 0.8f);
    public Color lockedColor = new Color(0.2f, 1f, 0.3f, 1f);

    const int KnotCount = 4;
    readonly Vector3[] knotPos = new Vector3[KnotCount];
    readonly Vector3[] knotVel = new Vector3[KnotCount];
    Vector3 anchor;
    bool attached;
    float attachTime;

    void OnEnable()  { gesture.OnThwip += Shoot; gesture.OnRelease += Release; }
    void OnDisable() { gesture.OnThwip -= Shoot; gesture.OnRelease -= Release; }
    void Start()     { web.gameObject.SetActive(false); }

    // The web leaves from the wrist (landmark 0), like a real web-shooter.
    Vector3 WristPoint()
    {
        Vector2 s = hand.ToScreen(0);
        return cam.ScreenToWorldPoint(new Vector3(s.x, s.y, handDepth));
    }

    // Where the web would attach if fired at this screen position.
    bool TryGetTarget(Vector2 screenPos, out Vector3 point)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, swingableLayers)
            || (aimAssistRadius > 0f && Physics.SphereCast(ray, aimAssistRadius, out hit, maxDistance, swingableLayers)))
        {
            point = SnapToCorner(hit);
            return true;
        }
        point = default;
        return false;
    }

    void Shoot(Vector2 aimScreenPos)
    {
        Release();
        if (!TryGetTarget(aimScreenPos, out anchor)) return;

        Vector3 start = WristPoint();
        for (int i = 0; i < KnotCount; i++)
        {
            knotPos[i] = Vector3.Lerp(start, anchor, i / (KnotCount - 1f));
            knotVel[i] = Vector3.zero;
        }

        // Kick the middle knots sideways so the web curves as it flies.
        Vector3 side = Vector3.Cross(anchor - start, Vector3.up).normalized;
        knotVel[1] = side * startCurve * 10f;
        knotVel[2] = side * startCurve * 5f;

        attached = true;
        attachTime = Time.time;
        RebuildSpline();
        web.gameObject.SetActive(true);
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        // Grow the mesh from the hand to the anchor, with a glob riding the tip.
        webTip.Duration = shootTime;
        webTip.Restart(true);
        for (float t = 0f; t < shootTime; t += Time.deltaTime)
        {
            webMesh.Range = new Vector2(0f, Mathf.Max(0.01f, t / shootTime));
            yield return null;
        }
        webMesh.Range = new Vector2(0f, 1f);
        swing.Attach(anchor);
    }

    void Update()
    {
        if (!attached) return;

        // Tension goes 0 -> 1 after attaching: the sag and curve fade and the web tightens.
        float tension = Mathf.Clamp01((Time.time - attachTime) / tightenTime);
        float sag = Mathf.Lerp(startSag, 0f, tension);
        float k = stiffness * (1f + 2f * tension);

        Vector3 start = WristPoint();
        knotPos[0] = start;
        knotPos[KnotCount - 1] = anchor;

        for (int i = 1; i < KnotCount - 1; i++)
        {
            float t = i / (KnotCount - 1f);
            Vector3 target = Vector3.Lerp(start, anchor, t) + Vector3.down * sag * Mathf.Sin(t * Mathf.PI);
            Vector3 accel = k * (target - knotPos[i]) - damping * knotVel[i];
            knotVel[i] += accel * Time.deltaTime;
            knotPos[i] += knotVel[i] * Time.deltaTime;
        }

        RebuildSpline();
    }

    // Aim point: reticle on the index fingertip, marker on the point the web will attach to.
    void LateUpdate()
    {
        if (reticle == null || targetMarker == null) return;

        if (!hand.HasHand)
        {
            reticle.enabled = false;
            targetMarker.enabled = false;
            return;
        }

        // Same fingertip point (landmark 8) the web is fired at.
        Vector2 aim = hand.ToScreen(8);
        reticle.enabled = true;
        PlaceOnCanvas(reticle.rectTransform, aim);

        bool locked = TryGetTarget(aim, out Vector3 target);
        reticle.color = locked ? lockedColor : idleColor;

        Vector3 screen = locked ? cam.WorldToScreenPoint(target) : Vector3.zero;
        targetMarker.enabled = locked && screen.z > 0f;
        if (targetMarker.enabled) PlaceOnCanvas(targetMarker.rectTransform, screen);
    }

    void PlaceOnCanvas(RectTransform rt, Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out Vector2 local);
        rt.localPosition = new Vector3(local.x, local.y, 0f);
    }

    void RebuildSpline()
    {
        Spline s = web.Spline;
        s.Clear();
        for (int i = 0; i < KnotCount; i++)
        {
            float3 local = web.transform.InverseTransformPoint(knotPos[i]);
            s.Add(new BezierKnot(local), TangentMode.AutoSmooth);
        }
        webMesh.Rebuild();
    }

    // Snap to the nearest vertical edge (corner) of the building, if one is close.
    Vector3 SnapToCorner(RaycastHit hit)
    {
        Bounds b = hit.collider.bounds;
        Vector3 best = hit.point;
        float bestDist = cornerSnapRadius;
        foreach (float x in new[] { b.min.x, b.max.x })
        foreach (float z in new[] { b.min.z, b.max.z })
        {
            Vector3 edge = new Vector3(x, Mathf.Clamp(hit.point.y, b.min.y, b.max.y), z);
            float d = Vector3.Distance(hit.point, edge);
            if (d < bestDist) { bestDist = d; best = edge; }
        }
        return best;
    }

    void Release()
    {
        StopAllCoroutines();
        attached = false;
        swing.Detach();
        web.gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        if (!attached) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(anchor, 0.3f);
    }
}
