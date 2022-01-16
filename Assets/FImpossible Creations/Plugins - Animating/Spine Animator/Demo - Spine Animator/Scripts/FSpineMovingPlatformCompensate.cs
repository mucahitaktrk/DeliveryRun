using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FSpine
{
    public class FSpineMovingPlatformCompensate : MonoBehaviour
    {
        public FSpineAnimator Spine;
        public Transform Platform;
        Vector3 prePos;

        void Update()
        {
            Vector3 translate = Platform.position - prePos;

            for (int i = 0; i < Spine.SpineBones.Count; i++)
            {
                Spine.SpineBones[i].ProceduralPosition += translate;
            }

            prePos = Platform.position;
        }
    }
}
