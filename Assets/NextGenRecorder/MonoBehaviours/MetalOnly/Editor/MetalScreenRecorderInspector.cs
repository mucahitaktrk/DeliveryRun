using UnityEditor;

namespace pmjo.NextGenRecorder
{
    [CustomEditor(typeof(MetalScreenRecorder))]
    public class MetalScreenRecorderInspector : Editor
    {
#if !UNITY_2017_3_OR_NEWER
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This script is only supported with Metal renderer and Unity 2017.3 or newer", MessageType.Warning);
        }

#endif
    }
}
