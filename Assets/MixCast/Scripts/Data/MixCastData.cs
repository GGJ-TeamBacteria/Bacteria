/*======= (c) Blueprint Reality Inc., 2017. All rights reserved =======*/
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    [System.Serializable]
    public class MixCastData
    {
        public enum OutputMode
        {
            Immediate, Buffered, Quadrant
        }

        public enum IsolationMode
        {
            None, Chromakey, StaticSubtraction
        }

        public enum LicenseType
        {
            Free, Arcade, Creator, Indie, Educator, Streamer, eSports, Film, Pro, Enterprise
        }

        static public event EventHandler OnWriteData;

        static public void WriteData()
        {
            if (OnWriteData != null) {
                OnWriteData(null, null);
            }
        }

        [System.Serializable]
        public class GlobalData
        {
            public int targetFramerate = 30;
            public int spareRendersPerFrame = 1;
            public string rootOutputPath = "";
            public bool startAutomatically = false;

            /*
             * Friendly reminder that the default value of the HasTrackingDeviceInCapture setting (in Resoureces -> Data -> Settings -> App)
             * needs to be set to true, prevent the badge on the tracking device dropdown from showing without first going into Capture with no 3rd controller
             * */
            public int hasTrackedDeviceInCapture; // 1 if yes, 2 if no, 0 default (used for reset)
        }

        [System.Serializable]
        public class SecureData
        {
            public LicenseType licenseType = LicenseType.Free;
            public string machineId;

            public bool IsFreeLicense
            {
                get { return licenseType == LicenseType.Free; }
            }
        }

        [System.Serializable]
        public class CameraCalibrationData
        {
            //Camera parameters
            public string displayName;
            public float deviceFoV = 45;

            //Imaging device data
            public string deviceName;
            public int deviceFeedWidth, deviceFeedHeight;

            //Placement data
            public Vector3 worldPosition = Vector3.zero;
            public Quaternion worldRotation = Quaternion.identity;

            public bool wasTracked;
            public string trackedByDevice;
            public string trackedByDeviceId;
            public string trackedByDeviceRole;
            public Vector3 trackedPosition = Vector3.zero;
            public Quaternion trackedRotation = Quaternion.identity;

            //Motion data
            public float positionSmoothTime;
            public float rotationSmoothTime;

            //Output data
            public int outputWidth, outputHeight;
            public OutputMode outputMode = OutputMode.Immediate;
            //For buffered camera mode
            public float bufferTime = 0f;

            //Background removal data
            public CroppingData croppingData = new CroppingData();
            public IsolationMode isolationMode = IsolationMode.None;
            public ChromakeyCalibrationData chromakeying = new ChromakeyCalibrationData();
            public StaticKeyCalibrationData staticSubtractionData = new StaticKeyCalibrationData();

            //InScene display data
            public SceneDisplayData displayData = new SceneDisplayData();

            public LightingData lightingData = new LightingData();

            public RecordingData recordingData = new RecordingData();
        }

        [System.Serializable]
        public class ChromakeyCalibrationData
        {
            public bool active = false;

            public Vector3 keyHsvMid = Vector3.one * 0.01f;
            public Vector3 keyHsvRange = Vector3.one * 0.01f;
            public Vector3 keyHsvFeathering = Vector3.one * 0.01f;

            public float keyDesaturationBandWidth = 0.1f;
            public float keyDesaturationFalloffWidth = 0.1f;
        }

        [System.Serializable]
        public class CroppingData
        {
            public bool active = false;

            public float headRadius = 0.25f;
            public float handRadius = 0.25f;
            public float baseRadius = 0.25f;
        }

        [System.Serializable]
        public class StaticKeyCalibrationData
        {
            [System.Serializable]
            public class FileBackedTexture
            {
                private const string FOLDER_NAME = "Blueprint Reality/MixCast VR";

                public FileBackedTexture(string filename)
                {
                    FileName = filename;
                }

                public string FileName { get; protected set; }

                [SerializeField]
                private bool valueSet;
                [SerializeField]
                private int width, height;

                private Texture2D tex;
                public Texture2D Tex
                {
                    //Load the texture from the saved file if available
                    get
                    {
                        if (tex == null && valueSet)
                        {
                            string programDataFolder = new DirectoryInfo(Application.persistentDataPath).Parent.Parent.FullName;
                            string folderPath = Path.Combine(programDataFolder, FOLDER_NAME);
                            string filePath = Path.Combine(folderPath, FileName);

                            if( !File.Exists(filePath) )
                            {
                                valueSet = false;
                                return null;
                            }

                            tex = new Texture2D(width, height, TextureFormat.RGB24, false, true);
                            byte[] mapBytes = File.ReadAllBytes(filePath);
                            tex.LoadRawTextureData(mapBytes);
                            tex.filterMode = FilterMode.Point;
                            tex.Apply();
                        }
                        return tex;
                    }
                    set
                    {
                        tex = value;

                        try {
                            string programDataFolder = new DirectoryInfo(Application.persistentDataPath).Parent.Parent.FullName;
                            string folderPath = Path.Combine(programDataFolder, FOLDER_NAME);
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);

                            string filePath = Path.Combine(folderPath, FileName);

                            if (tex != null)
                            {
                                File.WriteAllBytes(filePath, tex.GetRawTextureData());
                                width = tex.width;
                                height = tex.height;
                            }
                            else if (valueSet)
                            {
                                if (File.Exists(filePath))
                                    File.Delete(filePath);
                            }
                        }
                        catch(System.Exception ex)
                        {
                            Debug.LogException(ex);
                        }

                        valueSet = tex != null;
                    }
                }
            }

            public bool active = false;

            public FileBackedTexture midValueTexture = new FileBackedTexture("static_mid");
            public FileBackedTexture rangeValueTexture = new FileBackedTexture("static_range");

            public Vector3 keyHsvFeathering = Vector3.one * 0.01f;
        }

        [System.Serializable]
        public class SceneDisplayData
        {
            public enum PlacementMode
            {
                Camera, World, Headset
            }
            public PlacementMode mode = PlacementMode.Camera;

            public Vector3 position;
            public Quaternion rotation;

            public const float MAX_SCALE = 1.5f, MIN_SCALE = 0.25f;
            public float scale = 1f;
            public float alpha = 1f;
        }

        [System.Serializable]
        public class LightingData
        {
            public const float DEFAULT_EFFECT_AMOUNT = 0.75f;
            public const float DEFAULT_BASE_LIGHTING = 0.5f;
            public const float DEFAULT_POWER_MULTIPLIER = 1f;

            // Toogle to easily turn on/off ligting while preserving set values
            public bool isEnabled = DEFAULT_EFFECT_AMOUNT > 0;
            //Factor lerps from no lighting (0) to full lighting (1)
            public float effectAmount = DEFAULT_EFFECT_AMOUNT;
            //Adds a constant value to lighting to set a baseline amount of light
            public float baseLighting = DEFAULT_BASE_LIGHTING;
            //Multiplies the final lighting power
            public float powerMultiplier = DEFAULT_POWER_MULTIPLIER;
        }

        [System.Serializable]
        public class RecordingData
        {
            public bool autoStartTimelapse = false;
            public float timelapseInterval = 60;
        }

        [System.Serializable]
        public class OculusOrigin
        {
            public Vector3 position;
            public Vector3 rotation;

            public static bool operator ==(OculusOrigin a, OculusOrigin b)
            {
                if ((object)a == null || (object)b == null)
                {
                    return false;
                }

                return a.position == b.position
                        && a.rotation == b.rotation;
            }

            public static bool operator !=(OculusOrigin a, OculusOrigin b)
            {
                return !(a == b);
            }

            public override bool Equals(object obj)
            {
                return (OculusOrigin)obj == this;
            }

            public override int GetHashCode()
            {
                return position.GetHashCode() ^ rotation.GetHashCode();
            }
            
            public bool IsInitialized()
            {
                return position != Vector3.zero || rotation != Vector3.zero;
            }

            public override string ToString()
            {
                return string.Format("{0} - {1}", position, rotation);
            }
        }

        public GlobalData global = new GlobalData();
        public List<CameraCalibrationData> cameras = new List<CameraCalibrationData>();
        public SceneDisplayData sceneDisplay = new SceneDisplayData();
        public OculusOrigin oculusOrigin = new OculusOrigin();

        public Vector3 cameraStartPosition;
        public Quaternion cameraStartRotation;

        public string sourceVersion = "";

        public string language = "english";
        public string persistentDataPath;

    }
}