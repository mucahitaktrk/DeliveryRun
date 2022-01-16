using UnityEngine;
using System;

namespace pmjo.NextGenRecorder
{
    [AddComponentMenu("Next Gen Recorder/Image Effect Recorder")]
    public class ImageEffectRecorder : Recorder.VideoRecorderBase
    {
#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (RecordingTexture != src)
            {
                RecordingTexture = src;
            }

            BlitRecordingTexture();

            Graphics.Blit(src, dest);
        }

#endif
    }
}
