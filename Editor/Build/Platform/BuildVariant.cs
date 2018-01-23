using System.Collections.Generic;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildVariant
{
    public string variantName;
    public int selectedIndex;
    public List<string> values;

    public BuildVariant(string variantName, string[] values, int selectedIndex)
    {
        this.variantName = variantName;
        this.values = new List<string>(values);
        this.selectedIndex = selectedIndex;
    }

    public string variantKey
    {
        get
        {
            return values[selectedIndex];
        }
    }
}

}