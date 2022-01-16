using UnityEngine;

namespace pmjo.Examples
{
    public class RotateObj : MonoBehaviour
    {
        private Vector3 mRandomDir;
        public float mRandomSpeedX;
        private float mRandomSpeedY;

        void Awake()
        {
            mRandomDir = Random.onUnitSphere;
            mRandomSpeedX = Random.Range(16.0f, 28.0f);
            mRandomSpeedY = Random.Range(14.0f, 26.0f);
        }

        void FixedUpdate()
        {
            transform.Rotate(mRandomSpeedX * mRandomDir.x * Time.deltaTime, 0, mRandomSpeedY * mRandomDir.z * Time.deltaTime);
        }
    }
}
