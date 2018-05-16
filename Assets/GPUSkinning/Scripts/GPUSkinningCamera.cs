namespace chengkehan.GPUSkinning
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class GPUSkinningCamera : MonoBehaviour
    {
        public static Camera globalCamera { get; private set; }

        private void Awake()
        {
            if (globalCamera == null)
                globalCamera = GetComponent<Camera>();
        }
    }
}