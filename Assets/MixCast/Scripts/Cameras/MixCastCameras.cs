/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using BlueprintReality.Performance;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class MixCastCameras : MonoBehaviour
    {
        public static MixCastCameras Instance { get; protected set; }

        public CameraConfigContext cameraPrefab;

        public List<CameraConfigContext> CameraInstances { get; protected set; }
        public PerformanceTracker Performance { get; protected set; }

        public event System.Action OnBeforeRender;

        private float nextRenderTime;
        private static BlitTexture brandingBlit;
        private static BlitTexture logoBlit;

        void Awake()
        {
            Performance = GetComponent<PerformanceTracker>();
            if (Performance == null)
            {
                Performance = gameObject.AddComponent<PerformanceTracker>();
            }
        }

        void Start()
        {
            InitWatermarks();
        }
        
        private void OnEnable()
        {
            if (Instance != null)
            {
                UnityEngine.Debug.LogError("Should only have one MixCastCameras in the game");
                return;
            }

            Instance = this;

            MixCast.MixCastEnabled += HandleMixCastEnabled;
            MixCast.MixCastDisabled += HandleMixCastDisabled;

            GenerateCameras();

            StartCoroutine(RenderUsedCameras());
            StartCoroutine(RenderSpareCameras());

            nextRenderTime = Time.unscaledTime + 1f / MixCast.Settings.global.targetFramerate;
        }

        private void OnDisable()
        {
            StopCoroutine("RenderUsedCameras");
            StopCoroutine("RenderSpareCameras");

            DestroyCameras();

            MixCast.MixCastEnabled -= HandleMixCastEnabled;
            MixCast.MixCastDisabled -= HandleMixCastDisabled;

            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            List<MixCastData.CameraCalibrationData> createCams = new List<MixCastData.CameraCalibrationData>();
            List<CameraConfigContext> destroyCams = new List<CameraConfigContext>();

            createCams.AddRange(MixCast.Settings.cameras);
            destroyCams.AddRange(CameraInstances);
            for (int i = 0; i < CameraInstances.Count; i++)
                createCams.RemoveAll(c => c == CameraInstances[i].Data);
            for (int i = 0; i < MixCast.Settings.cameras.Count; i++)
                destroyCams.RemoveAll(c => c.Data == MixCast.Settings.cameras[i]);

            for (int i = 0; i < destroyCams.Count; i++)
            {
                CameraInstances.Remove(destroyCams[i]);
                Destroy(destroyCams[i].gameObject);
            }

            for (int i = 0; i < createCams.Count; i++)
            {
                bool wasPrefabActive = cameraPrefab.gameObject.activeSelf;
                cameraPrefab.gameObject.SetActive(false);

                CameraConfigContext instance = Instantiate(cameraPrefab, transform, false);

                instance.Data = createCams[i];

                CameraInstances.Add(instance);

                cameraPrefab.gameObject.SetActive(wasPrefabActive);

                instance.gameObject.SetActive(MixCast.Active);
            }
        }


        IEnumerator RenderUsedCameras()
        {
            while (isActiveAndEnabled)
            {
                if (Time.unscaledTime >= nextRenderTime)
                {
                    if (OnBeforeRender != null)
                        OnBeforeRender();

                    foreach (MixCastCamera cam in MixCastCamera.ActiveCameras)
                        if (cam.IsInUse)
                            cam.RenderScene();
                    
                    nextRenderTime += 1f / MixCast.Settings.global.targetFramerate;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator RenderSpareCameras()
        {
            int lastSpareRenderedIndex = 0;
            while (isActiveAndEnabled)
            {
                if (MixCastCamera.ActiveCameras.Count > 0)
                {
                    for (int i = 0; i < MixCast.Settings.global.spareRendersPerFrame; i++)
                    {
                        int startIndex = lastSpareRenderedIndex;
                        lastSpareRenderedIndex++;
                        while (MixCastCamera.ActiveCameras[lastSpareRenderedIndex % MixCastCamera.ActiveCameras.Count].IsInUse && (lastSpareRenderedIndex - startIndex) <= MixCastCamera.ActiveCameras.Count)
                            lastSpareRenderedIndex++;

                        if (lastSpareRenderedIndex - startIndex <= MixCastCamera.ActiveCameras.Count)
                            MixCastCamera.ActiveCameras[lastSpareRenderedIndex % MixCastCamera.ActiveCameras.Count].RenderScene();
                    }
                }

                yield return null;
            }
        }

        void GenerateCameras()
        {
            CameraInstances = new List<CameraConfigContext>();

            bool wasPrefabActive = cameraPrefab.gameObject.activeSelf;
            cameraPrefab.gameObject.SetActive(false);
            for (int i = 0; i < MixCast.Settings.cameras.Count; i++)
            {
                CameraConfigContext instance = Instantiate(cameraPrefab, transform, false);

                instance.Data = MixCast.Settings.cameras[i];

                CameraInstances.Add(instance);
            }
            cameraPrefab.gameObject.SetActive(wasPrefabActive);

            SetCamerasActive(MixCast.Active);
        }
        void DestroyCameras()
        {
            for (int i = 0; i < CameraInstances.Count; i++)
                Destroy(CameraInstances[i].gameObject);

            CameraInstances.Clear();
            CameraInstances = null;
        }
        private void HandleMixCastEnabled()
        {
            SetCamerasActive(true);
        }
        private void HandleMixCastDisabled()
        {
            SetCamerasActive(false);
        }

        void SetCamerasActive(bool active)
        {
            if (CameraInstances == null)
                return;

            for (int i = 0; i < CameraInstances.Count; i++)
                CameraInstances[i].gameObject.SetActive(active);
        }

        public static BlitTexture GetLogoBlit()
        {
            return logoBlit;
        }

        public static BlitTexture GetBrandingBlit()
        {
            return brandingBlit;
        }

        private void InitWatermarks()
        {

            // catch the first time Studio is loaded, when MixCastManageApp has not yet assigned the path causing the watermark to fail to show
            if (string.IsNullOrEmpty(MixCast.Settings.persistentDataPath))
                MixCast.Settings.persistentDataPath = Application.persistentDataPath;

            var textureLoader = gameObject.AddComponent<FileTextureLoader>();

            // only show arcade branding if not free license type
            if (!MixCast.SecureSettings.IsFreeLicense)
            {
                brandingBlit = new BlitTexture();
                brandingBlit.SetTexturePosition(BlitTexture.Position.BottomRight);

                // arcade branding logo
                string brandingFilepath = Path.Combine(MixCast.Settings.persistentDataPath, "branding.png");
                textureLoader.AddJob(brandingFilepath, (texture) =>
                {
                    brandingBlit.Texture = texture;
                    brandingBlit.Enable();
                });
            }
            
            // show our MixCast watermark
            logoBlit = new BlitTexture();
            logoBlit.SetTexturePosition(BlitTexture.Position.BottomLeft);
            Texture2D logoTexture = Resources.Load<Texture2D>("MixCast_Logo");
            logoBlit.Texture = logoTexture;
            logoBlit.Enable();
        }
    }
}