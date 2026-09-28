using System;
using UnityEngine;

// Detects the Spider-Man "thwip": index + pinky straight, middle + ring curled.
public class ThwipGestureDetector : MonoBehaviour
{
    public HandLandmarkBridge hand;

    [Tooltip("Knuckle angle above this = finger straight")]
    public float straightAngle = 150f;
    [Tooltip("Knuckle angle below this = finger curled")]
    public float curledAngle = 110f;
    public int confirmFrames = 3;   // frames the pose must hold before firing
    public int releaseFrames = 5;   // frames without the pose before letting go

    public event Action<Vector2> OnThwip;   // screen position of the index fingertip
    public event Action OnRelease;
    public bool IsHolding { get; private set; }

    int onCount, offCount;

    void Update()
    {
        bool pose = hand.HasHand && IsThwipPose();
        if (pose) { onCount++; offCount = 0; }
        else      { offCount++; onCount = 0; }

        if (!IsHolding && onCount >= confirmFrames)
        {
            IsHolding = true;
            OnThwip?.Invoke(hand.ToScreen(8));
        }
        else if (IsHolding && offCount >= releaseFrames)
        {
            IsHolding = false;
            OnRelease?.Invoke();
        }
    }

    bool IsThwipPose()
    {
        return JointAngle(5, 6, 7)    > straightAngle   // index straight
            && JointAngle(17, 18, 19) > straightAngle   // pinky straight
            && JointAngle(9, 10, 11)  < curledAngle     // middle curled
            && JointAngle(13, 14, 15) < curledAngle;    // ring curled
    }

    // Angle at the middle knuckle (PIP joint): about 180 when straight, smaller when bent.
    float JointAngle(int a, int b, int c)
    {
        Vector3 pa = hand.Get(a), pb = hand.Get(b), pc = hand.Get(c);
        return Vector3.Angle(pa - pb, pc - pb);
    }
}
