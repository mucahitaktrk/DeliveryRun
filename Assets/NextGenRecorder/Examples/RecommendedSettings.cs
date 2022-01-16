using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pmjo.NextGenRecorder;

public class RecommendedSettings : MonoBehaviour
{
    public int applicationTargetFrameRate = 60;
    public int recordingTargetFrameRate = 60;

    void Awake()
    {
        if (!Recorder.IsSupported)
        {
            Debug.Log("Next Gen Recorder not supported, settings not applied");
            return;
        }

        // Set application target framerate
        if (Application.targetFrameRate != applicationTargetFrameRate)
        {
            Application.targetFrameRate = applicationTargetFrameRate;
        }

        // Set recording target framerate
        Recorder.TargetFrameRate = recordingTargetFrameRate;
        Recorder.FrameSkipping = (Application.targetFrameRate > recordingTargetFrameRate) ? true : false;

        // Downscale huge resolutions 50%
        if (Mathf.Max(Screen.width, Screen.height) >= 1920)
        {
            Recorder.VideoScale = 0.5f;
        }
        else
        {
            Recorder.VideoScale = 1.0f;
        }

        // Enable to use a custom bitrate for audio and video
        // Recorder.CustomVideoBitrate = CustomVideoBitrate;
        // Recorder.CustomAudioBitrate = CustomAudioBitrate;
    }

    private int CustomVideoBitrate(long sessionId, int width, int height, int frameRate)
    {
        // float motionFactor = 0.5f; // Low
        float motionFactor = 1.0f; // Medium (default)
        // float motionFactor = 2.0f; // High
        // float motionFactor = 4.0f; // Super
        return Mathf.RoundToInt((frameRate * width * height * motionFactor * 0.07f) * 0.001f) * 1000;
    }

    private int CustomAudioBitrate(long sessionId, int sampleRate, int channelCount)
    {
        return 32000 * channelCount;
    }
}
