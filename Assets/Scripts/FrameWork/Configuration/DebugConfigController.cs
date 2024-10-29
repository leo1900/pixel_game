using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FreamWork
{
    public class DebugConfigController : ScriptableObject
    {
        public static string DebugConfigControllerPath = "Settings/DebugConfigController";

        private static DebugConfigController _instance = null;

        public static DebugConfigController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<DebugConfigController>(DebugConfigControllerPath);
                }
                return _instance;
            }
        }

        [Space(10)]
        public int _maxEnergy = 5;
    }
}