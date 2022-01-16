using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FSpine
{
    public partial class FSpineAnimator
    {
        /// <summary>
        /// Rounding position used in calculating difference for straightening
        /// </summary>
        Vector3 RoundPosDiff(Vector3 pos, int digits = 1)
        {
            return new Vector3((float)System.Math.Round(pos.x, digits), (float)System.Math.Round(pos.y, digits), (float)System.Math.Round(pos.z, digits));
        }


        /// <summary>
        /// Rounding fix correction angles to nearest values, we calculate axes directions in precise way, but in most cases rounded are doing job much better
        /// </summary>
        Vector3 RoundToBiggestValue(Vector3 vec)
        {
            int biggest = 0;
            if (Mathf.Abs(vec.y) > Mathf.Abs(vec.x))
            {
                biggest = 1;
                if (Mathf.Abs(vec.z) > Mathf.Abs(vec.y)) biggest = 2;
            }
            else
                if (Mathf.Abs(vec.z) > Mathf.Abs(vec.x)) biggest = 2;

            if (biggest == 0) vec = new Vector3(Mathf.Round(vec.x), 0f, 0f);
            else
            if (biggest == 1) vec = new Vector3(0f, Mathf.Round(vec.y), 0f);
            else
                vec = new Vector3(0f, 0f, Mathf.Round(vec.z));

            return vec;
        }


        /// <summary>
        /// Helping calculating safe delta time
        /// </summary>
        float GetClampedSmoothDelta()
        {
            return Mathf.Clamp(Time.smoothDeltaTime, 0f, 0.1f);
        }


        /// <summary>
        /// Used for back compatibility
        /// </summary>
        public List<Transform> GetOldSpineTransforms()
        {
            return SpineTransforms;
        }


        /// <summary>
        /// Used for back compatibility
        /// </summary>
        public void ClearOldSpineTransforms()
        {
            if ( SpineTransforms != null ) SpineTransforms.Clear();
        }

    }
}