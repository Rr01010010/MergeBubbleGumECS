using System;
using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "InputSettingsSO", menuName = "ScriptableObjects/InputSettingsSO")]
    public class InputSettingsSO : ScriptableObject
    {
        public enum InputType { WASD, JoystickNotImplemented, Gyroscope }
        public InputType type = InputType.WASD;


        public WASDSettings SettingsWASD;
        public GyroscopeSettings SettingsGyroscope;

        [Serializable]
        public class WASDSettings 
        {
            public float sens = 1.0f;
            public float ForceMovement = 0.05f;
        }
        [Serializable]
        public class GyroscopeSettings
        {
            public float angleBorder = 360.0f;
            public float ForceMovement = 0.05f*4.0f;
        }
    }
}
