using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace FIMSpace.FEditor
{
    /// <summary>
    /// FM: Class with basic tools for working in Unity Editor level
    /// </summary>
    public static partial class FEditor_MenuAddOptions
    {

        [MenuItem("CONTEXT/Collider/Generate NavMesh Obstacle")]
        private static void GenerateNavMeshObstacle(MenuCommand menuCommand)
        {
            Collider targetComponent = (Collider)menuCommand.context;

            if (targetComponent)
            {
                NavMeshObstacle obstacle = targetComponent.gameObject.GetComponent<NavMeshObstacle>();
                if (obstacle == null) obstacle = targetComponent.gameObject.AddComponent<NavMeshObstacle>();
                obstacle.center = targetComponent.bounds.center;
                obstacle.size = targetComponent.bounds.size;
                obstacle.carving = true;

                EditorUtility.SetDirty(targetComponent.gameObject);
            }
        }


        [MenuItem("CONTEXT/AudioReverbZone/Fit To Collider")]
        private static void AudioReverbZoneFit(MenuCommand menuCommand)
        {
            AudioReverbZone targetComponent = (AudioReverbZone)menuCommand.context;

            if (targetComponent)
            {
                Collider c = targetComponent.gameObject.GetComponent<Collider>();

                if (c)
                {
                    targetComponent.minDistance = Vector3.Distance(c.bounds.min, c.bounds.max) * 0.45f;
                    targetComponent.maxDistance = targetComponent.minDistance * 1.35f;
                }

                EditorUtility.SetDirty(targetComponent.gameObject);
            }
        }


        [MenuItem("CONTEXT/ReflectionProbe/Fit To Collider")]
        private static void ReflectionProbeFit(MenuCommand menuCommand)
        {
            ReflectionProbe targetComponent = (ReflectionProbe)menuCommand.context;

            if (targetComponent)
            {
                Collider c = targetComponent.gameObject.GetComponent<Collider>();
                BoxCollider bc = c as BoxCollider;

                if (c)
                {
                    if (bc)
                    {
                        targetComponent.center = bc.center;
                        targetComponent.size = bc.size;
                    }
                    else
                    {
                        targetComponent.center = c.bounds.center;
                        targetComponent.size = c.bounds.size;
                    }
                }

                EditorUtility.SetDirty(targetComponent.gameObject);
            }
        }

    }
}
