using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using pmjo.NextGenRecorder;
// using pmjo.NextGenRecorder.Sharing;

namespace pmjo.Examples
{
    public class SimpleRecorder : MonoBehaviour
    {
        public Button startRecordingButton;
        public Button stopRecordingButton;
        public Button saveRecordingButton;
        public Button viewRecordingButton;

        private long mLastSessionId;

        void OnEnable()
        {
            Recorder.RecordingStarted += RecordingStarted;
            Recorder.RecordingPaused += RecordingPaused;
            Recorder.RecordingResumed += RecordingResumed;
            Recorder.RecordingStopped += RecordingStopped;
            Recorder.RecordingExported += RecordingExported;
        }

        void OnDisable()
        {
            Recorder.RecordingStarted -= RecordingStarted;
            Recorder.RecordingPaused -= RecordingPaused;
            Recorder.RecordingResumed -= RecordingResumed;
            Recorder.RecordingStopped -= RecordingStopped;
            Recorder.RecordingExported -= RecordingExported;
        }

        void Awake()
        {
            mLastSessionId = Recorder.GetLastRecordingSession();

            UpdateStartAndStopRecordingButton();
            UpdateSaveOrViewRecordingButton();
        }

        void Start()
        {
            CreateEventSystemIfItDoesNotExist();
        }

        void RecordingStarted(long sessionId)
        {
            Debug.Log("Recording started, session id " + sessionId);

            UpdateStartAndStopRecordingButton();
            UpdateSaveOrViewRecordingButton();
        }

        void RecordingPaused(long sessionId)
        {
            Debug.Log("Recording paused, session id " + sessionId);
        }

        void RecordingResumed(long sessionId)
        {
            Debug.Log("Recording resumed, session id " + sessionId);
        }

        void RecordingStopped(long sessionId)
        {
            Debug.Log("Recording stopped, session id " + sessionId);

            mLastSessionId = sessionId;

            UpdateStartAndStopRecordingButton();
            UpdateSaveOrViewRecordingButton();
        }

        public void StartRecording()
        {
            Recorder.StartRecording();
        }

        public void StopRecording()
        {
            Recorder.StopRecording();
        }

        public  void ExportLastRecording()
        {
            if (mLastSessionId > 0)
            {
                Recorder.ExportRecordingSession(mLastSessionId);
            }
        }

        void RecordingExported(long sessionId, string path, Recorder.ErrorCode errorCode)
        {
            if (errorCode == Recorder.ErrorCode.NoError)
            {
                Debug.Log("Recording exported to " + path + ", session id " + sessionId);

    #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                CopyFileToDesktop(path, "MyAwesomeRecording.mp4");
    #elif UNITY_IOS ||  UNITY_TVOS
                PlayVideo(path);
    #endif

                // Or save to photos using the Sharing API (triggers save to file dialog on macOS)
                // Remember to uncomment using pmjo.NextGenRecorder.Sharing at the top of the file
                // Sharing.SaveToPhotos(path, "My Awesome Album");

                // Or share using the Sharing API (only available on iOS)
                // Sharing.ShowShareSheet(path, true);
            }
            else
            {
                Debug.Log("Failed to export recording, error code " + errorCode + ", session id " + sessionId);
            }
        }

        private void UpdateStartAndStopRecordingButton()
        {
            if (Recorder.IsSupported)
            {
                startRecordingButton.interactable = true;
                stopRecordingButton.interactable = true;

                startRecordingButton.gameObject.SetActive(!Recorder.IsRecording);
                stopRecordingButton.gameObject.SetActive(Recorder.IsRecording);
            }
            else
            {
                startRecordingButton.gameObject.SetActive(true);
                startRecordingButton.interactable = false;
                stopRecordingButton.gameObject.SetActive(false);
            }
        }

        private void UpdateSaveOrViewRecordingButton()
        {
    #if UNITY_EDITOR ||  UNITY_STANDALONE
            saveRecordingButton.gameObject.SetActive(true);
            saveRecordingButton.interactable = (mLastSessionId > 0) && !Recorder.IsRecording;
            viewRecordingButton.gameObject.SetActive(false);
    #else
            viewRecordingButton.gameObject.SetActive(true);
            viewRecordingButton.interactable = (mLastSessionId > 0) && !Recorder.IsRecording;
            saveRecordingButton.gameObject.SetActive(false);
    #endif
        }

    #if UNITY_EDITOR_OSX ||  UNITY_STANDALONE_OSX
        private static void CopyFileToDesktop(string path, string fileName)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dstPath = Path.Combine(desktopPath, fileName);

            File.Copy(path, dstPath, true);

            Debug.Log("Recording " + fileName + " copied to the desktop");
        }

    #elif UNITY_IOS ||  UNITY_TVOS
        private static void PlayVideo(string path)
        {
            if (!path.Contains("file://"))
            {
                path = "file://" + path;
            }

            Handheld.PlayFullScreenMovie(path);
        }

    #endif

        private static void CreateEventSystemIfItDoesNotExist()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            StandaloneInputModule inputModule = FindObjectOfType<StandaloneInputModule>();

            // Create EventSystem if it does not exist
            if (eventSystem == null && inputModule == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
    }
}
