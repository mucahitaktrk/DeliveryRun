#if UNITY_2019_4_OR_NEWER
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

        [MenuItem("Assets/Utilities/Create Prefab and Add Collider", true)]
        private static bool CreatePrefabOutOfModelAssetCollCheck(MenuCommand menuCommand)
        { return IsAnyPrefabable(Selection.objects); }

        [MenuItem("Assets/Utilities/Create Prefab", true)]
        private static bool CreatePrefabOutOfModelAssetCheck(MenuCommand menuCommand)
        { return IsAnyPrefabable(Selection.objects); }


        [MenuItem("Assets/Utilities/Create Prefab and Add Collider")]
        private static void CreatePrefabOutOfModelAssetColl(MenuCommand menuCommand)
        {
            if (Selection.objects.Length == 0) return;

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                Object ob = Selection.objects[i];
                var type = PrefabUtility.GetPrefabAssetType(ob);
                if (type == PrefabAssetType.NotAPrefab || type == PrefabAssetType.MissingAsset) continue;
                
                string directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ob));
                GameObject toSave = GeneratePrePrefabObject(ob);

                if (toSave == null) return;

                toSave.AddComponent<BoxCollider>();
                directory = Path.Combine(directory, toSave.name + ".prefab");
                PrefabUtility.SaveAsPrefabAsset(toSave, directory);

                if (toSave) GameObject.DestroyImmediate(toSave);
            }
        }


        [MenuItem("Assets/Utilities/Create Prefab")]
        private static void CreatePrefabOutOfModelAsset(MenuCommand menuCommand)
        {
            if (Selection.objects.Length == 0) return;

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                Object ob = Selection.objects[i];
                var type = PrefabUtility.GetPrefabAssetType(ob);
                if (type == PrefabAssetType.NotAPrefab || type == PrefabAssetType.MissingAsset) continue;

                string directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ob));

                GameObject toSave = GeneratePrePrefabObject(ob);

                directory = Path.Combine(directory, toSave.name + ".prefab");
                PrefabUtility.SaveAsPrefabAsset(toSave, directory);

                if (toSave) GameObject.DestroyImmediate(toSave);
            }
        }

    }
}
#endif
