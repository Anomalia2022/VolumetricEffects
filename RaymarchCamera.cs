using UnityEngine;

namespace VolumetricEffects
{
    
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class RaymarchCamera : MonoBehaviour
    {
        public Shader raymarchShader = ShaderLoader.shaderFinder("VolumetricEffects/RaymarchShader");
        private Material raymarchMat;
        private Camera targetCamera;
        public float _maxDistance = 4;
        public float _maxSteps = 164;
        public Vector4 _sphere1 = new Vector4(0, 0, 0, 2);
        public Transform _directionalLight;
        
        public Material raymarchMaterial
        {
            get
            {
                if (!raymarchMat && raymarchShader)
                {
                    raymarchMat = new Material(raymarchShader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }
                return raymarchMat;
            }
        }

        public Camera getCamera
        {
            get
            {
                if (!targetCamera)
                {
                    targetCamera = FlightCamera.fetch.mainCamera;
                }

                return targetCamera;
            }
        }

        public Transform getLightTransform
        {
            get
            {
                if (!_directionalLight)
                {
                    _directionalLight = Sun.Instance.GetComponent<Light>().transform;
                }

                return _directionalLight;
            }
        }
        

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            
            Debug.Log("[Volumetric Effects] Running OnRenderImage");
            
            if (!raymarchMaterial)
            {
                Graphics.Blit(source, destination);
                return;
            }
            
            raymarchMaterial.SetVector("_LightDirection", getLightTransform ? getLightTransform.forward : Vector3.down);
            raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(getCamera));
            raymarchMaterial.SetMatrix("_CamToWorld", getCamera.cameraToWorldMatrix);
            raymarchMaterial.SetFloat("_maxDistance", _maxDistance);
            raymarchMaterial.SetFloat("_maxSteps", _maxSteps);
            raymarchMaterial.SetVector("_sphere1", _sphere1);

            
            RenderTexture.active = destination;
            raymarchMaterial.SetTexture("_MainTex", source);
            
            GL.PushMatrix();
            GL.LoadOrtho();
            raymarchMaterial.SetPass(0);
            GL.Begin(GL.QUADS);
            
            // BL Quad
            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 3.0f);
            
            // BR Quad
            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 2.0f);

            
            // TR Quad
            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            
            
            // TL Quad
            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f);
            
            
            GL.End();
            GL.PopMatrix();
            
            Debug.Log("[Volumetric Effects] Matrix Popped!");
            
        }

        private Matrix4x4 CamFrustum(Camera camera)
        {
            Matrix4x4 frustum = Matrix4x4.identity;
            float fov = Mathf.Tan((camera.fieldOfView * 0.5f) * Mathf.Deg2Rad);

            Vector3 Up = Vector3.up * fov;
            Vector3 Right = Vector3.right * fov * camera.aspect;

            Vector3 TL = (-Vector3.forward - Right + Up);
            Vector3 TR = (-Vector3.forward + Right + Up);
            Vector3 BL = (-Vector3.forward - Right - Up);
            Vector3 BR = (-Vector3.forward + Right - Up);
            
            frustum.SetRow(0, TL);
            frustum.SetRow(1, TR);
            frustum.SetRow(2, BR);
            frustum.SetRow(3, BL);
            
            return frustum;

        }
        public void Start()
        {
            // Prepare targetCamera to the FlightCamera
            targetCamera = FlightCamera.fetch.mainCamera;
            if (targetCamera == null && targetCamera.activeTexture == null)
            {
                Debug.Log("[Volumetric Effects] Target Camera is null!");
            }
            else
            {
                Debug.Log("[Volumetric Effects] Target Camera has been located!");
            }
            _directionalLight = Sun.Instance.GetComponent<Light>().transform;
            if (_directionalLight == null)
            {
                Debug.Log("[Volumetric Effects] Sunlight transform could not be obtained!");
            }
            else
            {
                Debug.Log("[Volumetric Effects] Sunlight transform has been located!");

            }
            
            // Add the prepared script to the Flight Camera as long as all preparations are complete
            if (FlightCamera.fetch.mainCamera.gameObject.GetComponent<RaymarchCamera>() == null && _directionalLight != null && targetCamera != null)
            {
                FlightCamera.fetch.mainCamera.gameObject.AddComponent<RaymarchCamera>();
                Debug.Log($"[Volumetric Effects] Attached Raymarch Camera Script to Flight Camera of name {FlightCamera.fetch.mainCamera.name}");
                return;
            }
            Debug.Log("[Volumetric Effects] Raymarch Camera Script has already been applied to Flight Camera or Camera/Light could not be found! Ignoring!");
        }
    }
}