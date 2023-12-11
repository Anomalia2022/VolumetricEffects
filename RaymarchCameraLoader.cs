using System;
using UnityEngine;

namespace VolumetricEffects
{
    [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
    public class RaymarchCameraLoade : MonoBehaviour
    {
        private void Start()
        {
            FlightCamera.fetch.mainCamera.gameObject.AddComponent<RaymarchCamera>();
            Debug.Log($"[Volumetric Effects] Attached Raymarch Camera Script to Flight Camera of name {FlightCamera.fetch.mainCamera.name}");
        }
    }
}