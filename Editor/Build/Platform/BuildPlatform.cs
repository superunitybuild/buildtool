using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildPlatform
{
    public class BuildArchitecture
    {
        public BuildTarget target;
        public string name;
        public bool enabled;

        public BuildArchitecture(BuildTarget target, string name, bool enabled)
        {
            this.target = target;
            this.name = name;
            this.enabled = enabled;
        }
    }

    public bool enabled = false;

    #region Abstract

    /// <summary>
    /// Unity build target definition.
    /// </summary>
    //public abstract BuildTarget target { get; }
    public virtual BuildArchitecture[] architectures { get { return null; } }

    /// <summary>
    /// Platform name.
    /// </summary>
    public string platformName;

    /// <summary>
    /// The format of the binary executable name (e.g. {0}.exe). {0} = Executable name specified in BuildSettings.basicSettings.
    /// </summary>
    public virtual string binaryNameFormat { get { return ""; } }

    /// <summary>
    /// The format of the data directory (e.g. {0}_Data). {0} = Executable name specified in BuildSettings.basicSettings.
    /// </summary>
    public virtual string dataDirNameFormat { get { return ""; } }

    #endregion

    #region Public Methods

    /// <summary>
    /// Perform build for platform.
    /// </summary>
    public void Build()
    {
        BuildProject.PerformBuild(this);
    }

    #endregion

    #region Public Properties

    public string buildPath
    {
        get
        {
            return BuildSettings.basicSettings.buildPath + Path.DirectorySeparatorChar + platformName + Path.DirectorySeparatorChar;
        }
    }

    public string dataDirectory
    {
        get
        {
            return buildPath + string.Format(dataDirNameFormat, BuildSettings.basicSettings.executableName) + Path.DirectorySeparatorChar;
        }
    }

    public string exeName
    {
        get
        {
            return string.Format(binaryNameFormat, BuildSettings.basicSettings.executableName);
        }
    }

    #endregion
}

}