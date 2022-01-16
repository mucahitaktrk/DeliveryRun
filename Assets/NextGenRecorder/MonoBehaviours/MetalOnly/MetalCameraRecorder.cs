using UnityEngine;
using UnityEngine.Rendering;

namespace pmjo.NextGenRecorder
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Next Gen Recorder/Metal Camera Recorder")]
    public class MetalCameraRecorder : Recorder.VideoRecorderBase
    {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private CommandBuffer m_CommandBuffer;
        private Camera m_Camera;

        void Awake()
        {
            if (!Recorder.IsSupported)
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

            m_CommandBuffer = new CommandBuffer();
            CommandBufferCaptureMetalRenderTarget(m_CommandBuffer);

            m_Camera = GetComponent<Camera>();
            m_Camera.AddCommandBuffer(CameraEvent.AfterEverything, m_CommandBuffer);

            DontDestroyOnLoad(gameObject);
        }

#endif
    }
}
