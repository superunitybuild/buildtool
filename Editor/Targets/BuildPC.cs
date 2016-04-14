using UnityEngine;
using UnityEditor;

namespace UnityBuild
{

[InitializeOnLoad]
public class BuildPC : BuildPlatform
{
    #region Constants (SET VALUES)

    private const BuildTarget _target = BuildTarget.StandaloneWindows;
    private const string _name = "PC";
    private const string _binaryNameFormat = "{0}.exe";
    private const string _dataDirNameFormat = "{0}_Data";

    #endregion

    #region Constructor (SET CLASS NAME)

    static BuildPC()
    {
        BuildProject.RegisterPlatform(new BuildPC());
    }

    #endregion

    #region Methods & Properties (DO NOT EDIT)

    public override void Build()
    {
        Build(target, name, binaryNameFormat, dataDirNameFormat);
    }

    public override BuildTarget target
    {
        get { return _target; }
    }

    public override string name
    {
        get { return _name; }
    }

    public override string binaryNameFormat
    {
        get { return _binaryNameFormat; }
    }

    public override string dataDirNameFormat
    {
        get { return _dataDirNameFormat; }
    }

    [MenuItem("Build/" + _name)]
    private static void Toggle()
    {
        Toggle(_name);
    }
    [MenuItem("Build/" + _name, true)]
    private static bool ToggleValidate()
    {
        return ToggleValidate(_name);
    }

    #endregion
}

}