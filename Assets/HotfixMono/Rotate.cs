using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HotfixMono
{
    public class Rotate : MonoBehaviour
    {
        public float RotateSpeed = 1f;

        void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * RotateSpeed);
        }
    }
}