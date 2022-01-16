using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pmjo.NextGenRecorder;
// using pmjo.NextGenRecorder.Sharing;

public class RecordingTest : MonoBehaviour
{
    public float startDelay = 2.0f;
    public float recordingDuration = 5.0f;

    void Start()
    {
        if (Recorder.IsSupported)
        {
            StartCoroutine(RecordForSeconds(startDelay, recordingDuration));
        }
    }

    void OnEnable()
    {
        Recorder.RecordingStarted += RecordingStarted;
        Recorder.RecordingStopped += RecordingStopped;
        Recorder.RecordingExported += RecordingExported;
    }

    void OnDisable()
    {
        Recorder.RecordingStarted -= RecordingStarted;
        Recorder.RecordingStopped -= RecordingStopped;
        Recorder.RecordingExported -= RecordingExported;
    }

    IEnumerator RecordForSeconds(float startDelaySeconds, float recordingSeconds)
    {
        Recorder.PrepareRecording();

        yield return new WaitForSeconds(startDelaySeconds);

        Recorder.StartRecording();

        yield return new WaitForSeconds(recordingSeconds);

        Recorder.StopRecording();
    }

    private void RecordingStarted(long sessionId)
    {
        Debug.Log("Recording " + sessionId + " was started.");
    }

    private void RecordingStopped(long sessionId)
    {
        Debug.Log("Recording " + sessionId + " was stopped.");

        Recorder.ExportRecordingSession(sessionId);
    }

    void RecordingExported(long sessionId, string path, Recorder.ErrorCode errorCode)
    {
        if (errorCode == Recorder.ErrorCode.NoError)
        {
            Debug.Log("Recording exported to " + path + ", session id " + sessionId);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            CopyFileToDesktop(path, "MyAwesomeRecording.mp4");
#elif UNITY_IOS || UNITY_TVOS
            PlayVideo(path);
#endif

            // Sharing.SaveToPhotos(path);
        }
        else
        {
            Debug.Log("Failed to export recording, error code " + errorCode + ", session id " + sessionId);
        }
    }

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    private static void CopyFileToDesktop(string path, string fileName)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string dstPath = Path.Combine(desktopPath, fileName);

        File.Copy(path, dstPath, true);

        Debug.Log("Recording " + fileName + " copied to the desktop");
    }

#elif UNITY_IOS || UNITY_TVOS
    private static void PlayVideo(string path)
    {
        if (!path.Contains("file://"))
        {
            path = "file://" + path;
        }

        Handheld.PlayFullScreenMovie(path);
    }

#endif
}
