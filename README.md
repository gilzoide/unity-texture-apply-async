# Texture Apply Async
Alternative to [Texture2D.Apply()](https://docs.unity3d.com/ScriptReference/Texture2D.Apply.html) that doesn't require synchronizing with the render thread, avoiding stalls in the main thread.


# Features
- Asynchronous texture data update in CPU in the render thread, avoiding stalls in the main thread
- Supports registering for updates every frame or for a single frame