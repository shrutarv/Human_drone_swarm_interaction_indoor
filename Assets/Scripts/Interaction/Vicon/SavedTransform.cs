using System;
using UnityEngine;

namespace ClassicConsoleApp1
{
    public class SavedTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public SavedTransform(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
            scale = transform.localScale;
        }

        public void Apply(Transform transform) {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;
        }
    }
}
