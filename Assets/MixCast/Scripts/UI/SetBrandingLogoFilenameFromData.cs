using System.IO;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    [RequireComponent(typeof(SetRawImageFromFile))]
    public class SetBrandingLogoFilenameFromData : MonoBehaviour
    {
        private const string filename = "branding.png";

        void Start()
        {
            var rawImage = GetComponent<SetRawImageFromFile>();

            rawImage.filepath = Path.Combine(MixCast.Settings.persistentDataPath, filename);
            rawImage.LoadImage();
        }
    }
}