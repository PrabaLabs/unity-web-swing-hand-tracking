# Web Swinging with Hand Tracking in Unity

Make a Spider-Man-style "thwip" with your real hand in front of a webcam. A web shoots out,
curves through the air, sticks to a target, tightens like a real rope and swings you forward.
Open your hand to let go.

Built with **Unity Splines** and the **MediaPipe Unity Plugin**.

[![Watch the tutorial](https://img.shields.io/badge/YouTube-Watch%20the%20tutorial-red?logo=youtube)](VIDEO_LINK_HERE)

> 📺 **Full step-by-step tutorial:** VIDEO_LINK_HERE
> New to MediaPipe in Unity? Start with the setup tutorial: SETUP_VIDEO_LINK_HERE

<!-- Add a GIF of the swing here, e.g. ![Web swing demo](Docs/demo.gif) -->

---

## Versions used

| Tool | Version |
| --- | --- |
| Unity | 6000.0.66f2 (Unity 6) |
| Universal Render Pipeline | 17.0.4 |
| Splines | 2.8.4 |
| MediaPipe Unity Plugin | 0.16.3 |

Other Unity 6 versions will probably work. On Unity 2022.3, change `linearVelocity` to
`velocity` in `SwingController.cs`.

## Setup

1. **Download this project.** Click **Code > Download ZIP** (or `git clone`) and open the
   folder in Unity Hub with Unity 6.
2. **Install the MediaPipe Unity Plugin 0.16.3.** Download `com.github.homuler.mediapipe-0.16.3.tgz`
   from the [plugin's Releases page](https://github.com/homuler/MediaPipeUnityPlugin/releases),
   then in Unity go to **Window > Package Manager > + > Install package from tarball** and
   pick the file.
3. **Import the city.** Get the free
   [Low Poly FPS Map Lite](https://assetstore.unity.com/packages/3d/environments/low-poly-fps-map-lite-258453)
   from the Unity Asset Store and import it (**Window > Package Manager > My Assets**).
   It isn't included here because Asset Store content can't be redistributed.
4. **Open the scene** `Assets/Scenes/Spider Man Web Shooter.unity` and press **Play**.

Missing-prefab warnings before step 3 are expected. They go away once the city is imported.

**Only want the scripts?** Download the `.unitypackage` from the
[Releases](../../releases) page and import it into your own MediaPipe project.

## How to play

| Hand | What happens |
| --- | --- |
| Point your index finger | The ring follows your finger and turns green over something you can swing from |
| **Thwip**: index + pinky out, middle + ring curled | The web shoots, tightens and pulls you forward |
| Open your hand | The web lets go and your momentum carries you |

## What's inside

| Script | What it does |
| --- | --- |
| `HandLandmarkBridge` | Stores the latest 21 hand landmarks from MediaPipe (thread-safe) and converts them to screen positions |
| `ThwipGestureDetector` | Detects the thwip from finger knuckle angles |
| `WebShooter` | Aims from your fingertip, builds the web as a live spline with spring physics, and shows the aim reticle |
| `SwingController` | Pulls the player toward the web anchor with a rope constraint |
| `KeepChildrenOnLayer` | Keeps MediaPipe's landmark drawings on the UI layer so they show in the Game view |

`HandLandmarkerRunner.cs` (MediaPipe sample) was changed to send landmarks to `HandLandmarkBridge`.

## Troubleshooting

| Problem | Fix |
| --- | --- |
| Thwip never fires | Lower **Straight Angle** to 140 or raise **Curled Angle** to 120 on Thwip Gesture Detector |
| Aim is mirrored | Toggle **Mirror X** on Hand Landmark Bridge |
| Ring never turns green | Targets need the **Swingable** layer and a collider; raise **Aim Assist Radius** |
| Landmarks only in Scene view | Annotation Layer needs **Keep Children On Layer** (UI) |
| Webcam view missing in Game view | Player Camera > Stack must contain Tracking Camera |
| `linearVelocity` not found | You're on Unity 2022; use `velocity` instead |

## Support

If this helped you, a ⭐ on the repo and a subscribe on
[YouTube](VIDEO_LINK_HERE) really help the channel.
You can also support the work through the **Sponsor** button at the top of this page.

Questions or ideas for the next feature? Leave a comment on the video.

## License

The PrabaLabs code in this repository is under the [MIT License](LICENSE).
Third-party parts are listed in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).

Spider-Man is a trademark of Marvel Characters, Inc. This is a fan-made tutorial and is not
affiliated with or endorsed by Marvel, Disney, Sony, Google or Unity.
