using System.IO;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    /// <summary>
    /// FM: Class with basic tools for working in Unity Editor level
    /// </summary>
    public static partial class FEditor_MenuAddOptions
    {

        [MenuItem("Assets/Utilities/Copy Full Path To Directory")]
        private static void CopyWholePathToDir(MenuCommand menuCommand)
        {
            if (Selection.objects.Length == 0) return;
            string assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
            GUIUtility.systemCopyBuffer = Path.GetDirectoryName(fullPath);
        }



        [MenuItem("CONTEXT/MonoBehaviour/Go To Script's Directory")]
        private static void GoToBehaviourDirectory(MenuCommand menuCommand)
        {
            MonoBehaviour targetComponent = (MonoBehaviour)menuCommand.context;

            if (targetComponent)
            {
                MonoScript script = MonoScript.FromMonoBehaviour(targetComponent);
                if (script) EditorGUIUtility.PingObject(script);
            }
        }

#if UNITY_2019_4_OR_NEWER
        [MenuItem("Assets/Utilities/Add name prefixes for selection", true)]
        static bool SetPrefixesCheck(MenuCommand menuCommand)
        { return Selection.objects.Length > 0; }

        [MenuItem("Assets/Utilities/Add name prefixes for selection", false)]
        private static void SetPrefixes(MenuCommand menuCommand)
        {
            if (Selection.objects.Length == 0) return;

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i] == null) continue;

                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[i]);
                if (string.IsNullOrEmpty(assetPath)) continue;

                string prefix = GetPrefix(Selection.objects[i], assetPath);
                if (string.IsNullOrEmpty(prefix)) continue;
                {
                    AssetDatabase.RenameAsset(assetPath, prefix + Selection.objects[i].name);
                }
            }
        }
#endif

    }
}
