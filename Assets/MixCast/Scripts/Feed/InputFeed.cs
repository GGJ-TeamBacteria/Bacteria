/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BlueprintReality.MixCast
{
    public class InputFeed : CameraComponent
    {
        public class FramePlayerData
        {
            public float playerDist;

            public Vector3 playerHeadPos;
            public Vector3 playerBasePos;
            public Vector3 playerLeftHandPos;
            public Vector3 playerRightHandPos;
        }

        private bool loggedTrackingError = false; // used to avoid filling up the log with the same errors every frame
        private bool loggedTrackingErrorForParent = false;

        private const string KEYWORD_CROP_PLAYER = "CROP_PLAYER";
        private const string KEYWORD_WRITE_DEPTH = "DEPTH_ON";

        public const string COLOR_SPACE_EXP_PROP = "_ColorExponent";

        public static List<Material> activeMaterials = new List<Material>();

        public MixCastCamera cam;

        public Vector3 playerHeadsetOffset = new Vector3(0, 0, -0.05f);     //HMD tracked point about 5cm in front of center of skull

        public Material blitMaterial;

        public virtual Texture Texture
        {
            get
            {
                return null;
            }
        }


        public event Action OnRenderStarted;
        public event Action OnRenderEnded;

        private Material setMaterial;
        private FrameDelayQueue<FramePlayerData> frames;

        protected override void OnEnable()
        {
            if (cam == null)
                cam = GetComponentInParent<MixCastCamera>();

            frames = new FrameDelayQueue<FramePlayerData>();

            base.OnEnable();
            Invoke("HandleDataChanged", 0.01f);
        }

        protected override void OnDisable()
        {
            frames = null;

            ClearTexture();
            base.OnDisable();
        }

        private void OnApplicationQuit()
        {
            frames = null;
            ClearTexture();
        }

        protected override void HandleDataChanged()
        {
            base.HandleDataChanged();
            ClearTexture();

            if (context.Data != null && !string.IsNullOrEmpty(context.Data.deviceName))
                SetTexture();
        }

        protected virtual void LateUpdate()
        {
            if (context.Data != null)
            {
                if ((blitMaterial != setMaterial) && cam.ActiveFeeds.Contains(this))
                {
                    UnregisterFromCamera();
                    RegisterWithCamera();
                }
            }
        }

        public virtual void StartRender()
        {
            if (context.Data == null || frames == null)
                return;

            frames.delayDuration = context.Data.outputMode == MixCastData.OutputMode.Buffered ? context.Data.bufferTime : 0;
            frames.Update();

            if (blitMaterial != null)
            {
                blitMaterial.mainTexture = Texture;

                //Tell the material if linear color space needs to be corrected
                blitMaterial.SetFloat(COLOR_SPACE_EXP_PROP, QualitySettings.activeColorSpace == ColorSpace.Linear ? 2.2f : 1);

                if (Texture != null && cam.Output != null)
                {
                    //set transform to correct for different aspect ratios between screen and camera texture
                    float ratioRatio = ((float)Texture.width / Texture.height) / ((float)cam.Output.width / cam.Output.height);
                    blitMaterial.SetVector("_TextureTransform", new Vector4(1f / ratioRatio, 1, 0.5f * (1f - 1f / ratioRatio), 0));
                }

                //update the player's depth for the material
                FrameDelayQueue<FramePlayerData>.Frame<FramePlayerData> nextFrame = frames.GetNewFrame();
                if (nextFrame.data == null)
                    nextFrame.data = new FramePlayerData();

                if (cam.gameCamera != null)
                    nextFrame.data.playerDist = cam.gameCamera.transform.TransformVector(Vector3.forward).magnitude * CalculatePlayerDistance(); //Scale distance by camera scale

                FillTrackingData(nextFrame);

                WriteCroppingDataToMaterial(blitMaterial);

                if (cam is ImmediateMixCastCamera)
                {
                    blitMaterial.EnableKeyword(KEYWORD_WRITE_DEPTH);
                }
                else
                {
                    blitMaterial.DisableKeyword(KEYWORD_WRITE_DEPTH);
                }
            }

            if (OnRenderStarted != null)
                OnRenderStarted();
        }
        public virtual void StopRender()
        {
            if (blitMaterial != null)
            {
                blitMaterial.DisableKeyword(KEYWORD_CROP_PLAYER);
                blitMaterial.DisableKeyword(KEYWORD_WRITE_DEPTH);
            }
            if (OnRenderEnded != null)
                OnRenderEnded();
        }

        protected void RegisterWithCamera()
        {
            cam.RegisterFeed(this);
            activeMaterials.Add(blitMaterial);
            setMaterial = blitMaterial;
        }
        protected void UnregisterFromCamera()
        {
            cam.UnregisterFeed(this);
            activeMaterials.Remove(setMaterial);
        }

        protected virtual void SetTexture()
        {
            RegisterWithCamera();
        }

        protected virtual void ClearTexture()
        {
            UnregisterFromCamera();
        }

        //Returns the forward distance from the camera to the player plane in camera-space
        public float CalculatePlayerDistance()
        {
            Vector3 devicePos;
            Quaternion deviceRot;
            if (!TrackedDeviceManager.Instance.GetDeviceTransformByRole(TrackedDeviceManager.DeviceRole.Head, out devicePos, out deviceRot))
                return 0;

            Vector3 playerPositionWorld = MixCastCameras.Instance.transform.TransformPoint(devicePos + deviceRot * playerHeadsetOffset);
            Vector3 playerPositionLocal = cam.gameCamera.transform.InverseTransformPoint(playerPositionWorld);
            return playerPositionLocal.z;
        }
        protected void WriteCroppingDataToMaterial(Material mat)
        {
            FramePlayerData oldFrameData = frames.OldestFrameData;

            if (frames.OldestFrameData != null)
            {
                mat.SetFloat("_PlayerDist", oldFrameData.playerDist);
                mat.SetVector("_PlayerHeadPos", oldFrameData.playerHeadPos);
                mat.SetVector("_PlayerLeftHandPos", oldFrameData.playerLeftHandPos);
                mat.SetVector("_PlayerRightHandPos", oldFrameData.playerRightHandPos);
                mat.SetVector("_PlayerBasePos", oldFrameData.playerBasePos);
            }

            bool shouldWrite = false;

            if (Camera.main != null && Camera.main.transform.parent == null)
            {
                if (!loggedTrackingErrorForParent)
                {
                    Debug.Log("Parent object of Camera is null; do you have a VR camera set up correctly in your scene?");
                    loggedTrackingErrorForParent = true;
                    return;
                }
            }
            else
            {
                shouldWrite = true;
            }

            if (shouldWrite)
            {

                float scale = Camera.main != null ? Camera.main.transform.parent.TransformVector(Vector3.forward).magnitude : 1;
                mat.SetFloat("_PlayerScale", scale);

                mat.SetFloat("_PlayerHeadCropRadius", context.Data.croppingData.headRadius);
                mat.SetFloat("_PlayerHandCropRadius", context.Data.croppingData.handRadius);
                mat.SetFloat("_PlayerFootCropRadius", context.Data.croppingData.baseRadius);

                if (context.Data.croppingData.active)
                    mat.EnableKeyword(KEYWORD_CROP_PLAYER);
                else
                    mat.DisableKeyword(KEYWORD_CROP_PLAYER);
            }
        }

        void FillTrackingData(FrameDelayQueue<FramePlayerData>.Frame<FramePlayerData> frame)
        {
            frame.data.playerHeadPos = GetTrackingPosition(TrackedDeviceManager.DeviceRole.Head);
            frame.data.playerBasePos = new Vector3(frame.data.playerHeadPos.x, 0, frame.data.playerHeadPos.z);
            frame.data.playerLeftHandPos = GetTrackingPosition(TrackedDeviceManager.DeviceRole.LeftHand);
            frame.data.playerRightHandPos = GetTrackingPosition(TrackedDeviceManager.DeviceRole.RightHand);

            
            if (Camera.main == null)
            {
                if (!loggedTrackingError)
                {
                    Debug.Log("Main camera is null; is there a CameraRig in the scene?");
                    loggedTrackingError = true;
                }
                return;
            }
            

            Transform roomTransform = Camera.main.transform.parent;

            if (roomTransform == null)
            {
                if (!loggedTrackingError)
                {
                    Debug.Log("Room transform is null; do you have a CameraRig in your scene?");
                    loggedTrackingError = true;
                }
                return;
            }
            frame.data.playerHeadPos = roomTransform.TransformPoint(frame.data.playerHeadPos);
            frame.data.playerBasePos = roomTransform.TransformPoint(frame.data.playerBasePos);
            frame.data.playerLeftHandPos = roomTransform.TransformPoint(frame.data.playerLeftHandPos);
            frame.data.playerRightHandPos = roomTransform.TransformPoint(frame.data.playerRightHandPos);
        }
        Vector3 GetTrackingPosition(TrackedDeviceManager.DeviceRole role)
        {
            Vector3 pos;
            Quaternion rot;
            if (!TrackedDeviceManager.Instance.GetDeviceTransformByRole(role, out pos, out rot))
                TrackedDeviceManager.Instance.GetDeviceTransformByRole(TrackedDeviceManager.DeviceRole.Head, out pos, out rot);
            return pos;
        }
    }
}