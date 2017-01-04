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
    public bool enabled = false;
    public BuildDistributionList distributionList;

    #region Abstract

    /// <summary>
    /// Unity build target definition.
    /// </summary>
    public BuildArchitecture[] architectures;

    public BuildVariant[] variants;

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
    public virtual void Build()
    {
        BuildProject.PerformBuild(this);
    }

    #endregion

    #region Public Properties

    public bool atLeastOneArch
    {
        get
        {
            bool atLeastOneArch = false;
            for (int i = 0; i < architectures.Length && !atLeastOneArch; i++)
            {
                atLeastOneArch |= architectures[i].enabled;
            }

            return atLeastOneArch;
        }
    }

    public bool atLeastOneDistribution
    {
        get
        {
            bool atLeastOneDist = false;
            for (int i = 0; i < distributionList.distributions.Length && !atLeastOneDist; i++)
            {
                atLeastOneDist |= distributionList.distributions[i].enabled;
            }

            return atLeastOneDist;
        }
    }

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