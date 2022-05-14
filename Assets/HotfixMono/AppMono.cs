using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HotfixMono
{
    public class AppMono : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void Start() {}

        private void Update()
        {
            
        }

        private void FixedUpdate()
        {
            
        }
    }
}