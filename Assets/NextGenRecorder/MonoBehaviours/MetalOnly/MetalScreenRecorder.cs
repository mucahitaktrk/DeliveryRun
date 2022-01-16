using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace pmjo.NextGenRecorder
{
    [AddComponentMenu("Next Gen Recorder/Metal Screen Recorder")]
    public class MetalScreenRecorder : Recorder.VideoRecorderBase
    {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private bool m_Supported;
        private bool m_IsQuitting;

        void Awake()
        {
            m_Supported = Recorder.IsSupported;

            if (!m_Supported)
            {
                Debug.LogWarning("Next Gen Recorder not supported on this platform");
                return;
            }

            if (!MetalRenderTargetCaptureSupported)
            {
                Debug.LogWarning("Metal render target capture not supported, Metal renderer and Unity 2017.3 or greater required");
                return;
            }

            Recorder.VerticalFlip = true;

            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            if (!m_Supported)
            {
                return;
            }

            StartCoroutine(Capture());
        }

        void OnDisable()
        {
            if (!m_Supported)
            {
                return;
            }

            StopAllCoroutines();
        }

        private IEnumerator Capture()
        {
            while (!m_IsQuitting)
            {
                yield return new WaitForEndOfFrame();

                CaptureMetalRenderTarget();
            }
        }

        void OnApplicationQuit()
        {
            m_IsQuitting = true;
        }

#endif
    }
}
