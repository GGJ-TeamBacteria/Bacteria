using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public class OutputMixCastToScreenshotAuto : OutputMixCastToScreenshot
    {
        public const string FILE_EXT = ".jpg";

        public override void Run()
        {
            string folderPath = MixCastFiles.GetOutputDirectory();
            folderPath = Path.Combine(folderPath, GetApplicationName());
            MixCastFiles.TryCreateFolderPath(folderPath);

            filename = Path.Combine(folderPath, MixCastFiles.GenerateProceduralFilename() + FILE_EXT).Replace("\\", "/");

            base.Run();
        }
        protected virtual string GetApplicationName()
        {
            return MixCastFiles.GetApplicationFolderName();
        }
    }
}