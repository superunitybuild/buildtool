using System.IO;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    public class FilePathAttribute : PropertyAttribute
    {
        public bool folder = false;
        public bool allowManualEdit = true;
        public string message = "";
        public string initialNameOrFilter = "";
        public string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..")) + Path.DirectorySeparatorChar;

        public FilePathAttribute(bool folder = true, bool allowManualEdit = true, string message = "", string initialFolderName = "")
        {
            this.folder = folder;
            this.allowManualEdit = allowManualEdit;
            this.message = message;
            this.initialNameOrFilter = initialFolderName;
        }
    }
}
