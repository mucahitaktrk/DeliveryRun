Next Gen Recorder Free v0.9.14.1 beta
=====================================

Next Gen Recorder is a video recording library that takes the gameplay recording to the next level. It is available for iOS, tvOS and macOS platforms when using Metal or OpenGL renderer.

Documentation
=============

Documentation is provided as a PDF document inside the NextGenRecorder/Documentation folder.

Website
=======

See online documentation and more about the Next Gen Recorder at http://www.pmjo.org/nextgenrecorder

Support
=======

In case you are having problems with Next Gen Recorder, don't hesitate to contact support@pmjo.org

Version history
===============

ALWAYS REMEMBER TO RESTART EDITOR AFTER UPDATING!

0.9.14.1:

- Apple M1 support was added
- Fixed value for frame skipping in RecommendedSettings example script
- Virtual Screen is now deprecated and will be removed in next release
- Breaking change: Automatic integration is now only supported when using Metal renderer and it will always record the whole screen.
- Dropped support for older Unity versions. Last supported version is 2017.4.40f1.
- Dropped support for iOS 8
- Experimental: multichannel audio recording support for up to 8 channels was added

0.9.13.0:

- Watermark positioning support was added. Feature does not exist in Free version.
- Fixes an issue where you might not be able to set the WatermarkTexture in Awake

0.9.12.0:

- Earlier video dimensions were always forced to nearest divisible by 16. For now on width and height are only forced to even values.
- Added Recorder.VideoDimensionsDivisibleBy property, defaults to 2
- Fixed a bug where part of the recorded video goes missing when a user goes to the background and comes back while recording. This bug was caused by changes in 0.9.8.3.

0.9.11.0:

- Added RecommendedSettings.cs to SimpleRecorder prefab to avoid configuration errors
- Removed WebCamRecorder example to make the app not to require NSCameraUsageDescription when Next Gen Recorder is imported
- Added better support for Next Gen Recorder audio extensions
- Added *EXPERIMENTAL* Audio Mixer recording support capable of recording any Audio Mixer bus instead of Audio Source or Audio Listener. This will for example allow you to mix game audio and the microphone. Audio Mixer Recorder extension is available as a separate package.

0.9.10.1:

- Fixed MetalScreenRecorder and MetalCameraRecoder video orientation by adding VerticalFlip = true to both components

0.9.10.0:

- For now on VerticalFlip, Scale and ColorSpace also affect to the CaptureMetalRenderTarget and CommandBufferCaptureMetalRenderTarget
- For now on the plugin gets automatically initialized when RecordingTexture is set to avoid possible custom integration issues
- Moved SimpleRecorder UI so that it will fully visible on newer devices also

0.9.9.1:

- Added MetalScreenRecorder component for recording everything seen on the screen including the UI overlay. MetalScreenRecorder works with Built-In Render Pipeline and also with Scriptable Render Pipeline (SRP), High Definition Render Pipeline (HDRP) and Universal Render Pipeline (URP). MetalScreenRecorder is only available for the Metal renderer and Unity 2017.3 or greater.
- Added MetalCameraRecorder component for recording a camera without the UI overlay. MetalCameraRecorder is only available for the Metal renderer (Built-In Render Pipeline only) and Unity 2017.3 or greater.
- Added VerticalFlip property to the Recorder for flipping the RecordingTexture vertically without any performance loss.

0.9.8.3:

- Removed incorrectly configured background task handler that might have caused app to be killed earlier while on background

0.9.8.2:

- Experimental Linear colorspace support, not enabled by default. Use ColorSpace property to the change the colorspace.
- Fix export for videos that are recorded with audio enabled but no audio frames are received
- Stability fixes

0.9.7.11:

- Metal support was fixed on macOS
- SaveToSelectedFolder function was added to the Sharing API on macOS. It can be used to trigger a Save to file dialog with a preferred file name.
- A possible fix for a rare and random NSInternalInconsistencyException on tvOS and iOS

0.9.7.10:

- Compatibility with Vuforia and with some older graphics plugins was fixed

0.9.7.9:

- Added one more parameter to ExportRecordingSessionHighlights function so you can now select what highlights or highlight groups are exported into the highlight video! With this you could for example export all head shots, boss kills, goals etc. to separate videos.
- Fixed a null pointer exception when Virtual Screen is used on a platform that is not supported
- Added RealtimeRecording property to the Recorder. By default it is enabled but when it is disabled the frames will get written with a constant delta time depending on the current Recorder TargetFrameRate, e.g. delta time = 1.0 / Recorder.TargetFrameRate.
- Fixed boolean marshaling from native to managed on macOS. Fixes false positives for IsRecording and for some other properties and functions.

0.9.7.8:

- Improved audio sync when pause/resume functionality used
- RecordingStopped event is no longer triggered when entering background (internal stop)
- RecordingStarted event is no longer triggered when returning from the background (internal start)
- Fixed audio sync of the exported video when the user has entered background during the recording
- Always round scaled resolution to nearest divisible of 16 to avoid green lines in the exported video
- Updated WebCamRecorder example, will now check recording permission before starting

0.9.7.7:

- Added support for changing Virtual Screen properties in runtime (BlitToScreen and BlitToRecorder)
- Added RecordAudio property to the Recorder to allow disabling audio recording. RecordAudio is automatically set to false if no AudioListeners or custom audio recorders are found.

0.9.7.6:

- Fixed a possible crash when returning from the background and exporting the video, update highly recommended

0.9.7.5:

- SimpleRecorder example scene and prefab was added. Just import the package and drag the SimpleRecorder prefab to your hierarchy root to test the recording!
- WebCamRecorder example scene was added
- Ifdeffed ImageEffectRecorder for iOS, tvOS and macOS only

0.9.7.4:

- Fixed black video issue with OpenGL when UI Mask or stencil buffers are used in the project
- Virtual Screen blit camera is now automatically removed when last Virtual Screen is removed
- Added SavedToPhotos event to Sharing API that gets triggered when saving of a video has completed (or failed)
- Added a possibility to define an album name when saving to the photos (instead of just saving to Camera Roll)

0.9.7.3:

- Fixed a regression with Virtual Screen that caused empty videos when Virtual Screen was added before Next Gen Recorder is initialized. Also fixes similar issue with command buffers.

0.9.7.2:

- First public release
