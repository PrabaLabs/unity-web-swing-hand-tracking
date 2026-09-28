using UnityEngine;

// Pulls the player (and the camera on it) toward the web anchor like a pendulum.
[RequireComponent(typeof(Rigidbody))]
public class SwingController : MonoBehaviour
{
    public float pullForce = 35f;       // how hard the web pulls you forward
    public float reelSpeed = 4f;        // how fast the rope shortens
    public float minRopeLength = 3f;

    Rigidbody body;
    Vector3 anchor;
    float ropeLength;
    bool attached;

    void Awake() => body = GetComponent<Rigidbody>();

    public void Attach(Vector3 point)
    {
        anchor = point;
        ropeLength = Vector3.Distance(body.position, point);
        attached = true;
    }

    public void Detach() => attached = false;

    void FixedUpdate()
    {
        if (!attached) return;

        Vector3 toAnchor = anchor - body.position;
        float dist = toAnchor.magnitude;
        Vector3 dir = toAnchor / dist;

        body.AddForce(dir * pullForce, ForceMode.Acceleration);
        ropeLength = Mathf.Max(minRopeLength, ropeLength - reelSpeed * Time.fixedDeltaTime);

        // Rope constraint: never further than ropeLength, and cancel outward velocity.
        // Unity 6 uses linearVelocity; on Unity 2022 use body.velocity instead.
        if (dist > ropeLength)
        {
            body.position = anchor - dir * ropeLength;
            float outward = Vector3.Dot(body.linearVelocity, -dir);
            if (outward > 0f) body.linearVelocity += dir * outward;
        }
    }
}
