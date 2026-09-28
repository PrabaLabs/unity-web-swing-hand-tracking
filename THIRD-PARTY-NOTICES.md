# Third-party notices

This project uses the following third-party software. Only the files listed as
"included" are in this repository. Everything else must be installed separately.

## MediaPipe Unity Plugin (included in part)

- Source: https://github.com/homuler/MediaPipeUnityPlugin
- Included: the sample files under `Assets/MediaPipeUnity/`.
  `Assets/MediaPipeUnity/Samples/Scenes/Hand Landmark Detection/HandLandmarkerRunner.cs`
  was modified by PrabaLabs to send hand landmarks to `HandLandmarkBridge`.
- Not included: the plugin package itself (`com.github.homuler.mediapipe`).
- License: MIT, reproduced below.

```
MIT License

Copyright (c) 2021 homuler

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## Google MediaPipe (not included)

Used by the MediaPipe Unity Plugin. Apache License 2.0, Copyright The MediaPipe Authors.
https://github.com/google-ai-edge/mediapipe

## Unity Splines (not included)

`com.unity.splines`, installed through the Unity Package Manager.
Licensed under the Unity Companion License: https://unity.com/legal/licenses/unity-companion-license

## Low Poly FPS Map Lite (not included)

The city and wood plank models in the demo scene come from a free Unity Asset Store
package. Asset Store content cannot be redistributed, so download it yourself from the
Asset Store (see the README).

## Trademarks

Spider-Man is a trademark of Marvel Characters, Inc. This project is a fan-made
tutorial and is not affiliated with or endorsed by Marvel, Disney, Sony, Google or Unity.
