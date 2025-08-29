using DebugToolkit.Interaction.Commands;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DebugToolkit.Sample
{
    public class DebugLight : MonoBehaviour
    {
        [Header("Command")]
        [SerializeField] BooleanCommand lightCommand;
        [SerializeField] BooleanCommand shadowCommand;

        private List<Light> _lights = new List<Light>();

        private void Awake()
        {
            lightCommand.OnIsValid += LightCommand_OnIsValid;
            shadowCommand.OnIsValid += ShadowCommand_OnIsValid;
        }

        private void OnDestroy()
        {
            lightCommand.OnIsValid -= LightCommand_OnIsValid;
            shadowCommand.OnIsValid -= ShadowCommand_OnIsValid;
        }

        private void LightCommand_OnIsValid(bool obj)
        {
            if (this == null) return;

            ToggleLights(obj);
        }

        private void ShadowCommand_OnIsValid(bool obj)
        {
            if (this == null) return;

            ToggleShadow(obj);
        }

        private void ToggleLights(bool obj)
        {
            foreach (Light light in GetLights())
            {
                light.enabled = obj;
            }
        }

        private void ToggleShadow(bool obj)
        {
            foreach (Light light in GetLights())
            {
                if (obj)
                {
                    light.shadows = LightShadows.Soft;
                }
                else
                {
                    light.shadows = LightShadows.None;
                }
            }
        }

        private List<Light> GetLights()
        {
            if (_lights.Count == 0)
            {
                _lights = FindObjectsByType<Light>(FindObjectsSortMode.None).ToList();
            }

            return _lights;
        }
    }
}

