using System;
using System.Linq;
using System.Collections.Generic;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildVariant
    {
        public string variantName;
        public int selectedIndex;
        public List<string> values;
        public bool isFlag;

        public BuildVariant(string variantName, string[] values, int selectedIndex, bool isFlag = false)
        {
            this.variantName = variantName;
            this.values = new List<string>(values);
            this.selectedIndex = selectedIndex;
            this.isFlag = isFlag;
        }

        public string variantKey
        {
            get
            {
                return isFlag ? string.Join('+', flags) : values[selectedIndex];
            }
        }

        public List<string> flags
        {
            get
            {
                if (selectedIndex < 0)
                    return values;
                var indexList = ConvertFlagsToSelectedIndexList(selectedIndex);
                return indexList.Select(x => values[x]).ToList();
            }
        }
        
        /// <summary>
        /// Converts an integer to a list of selected values (indices). Each power of 2 is considered a flag value. For example, 3 returns [0,1].
        /// </summary>
        /// <param name="number">The flag value to convert.</param>
        /// <returns>A list of indices.</returns>
        List<int> ConvertFlagsToSelectedIndexList(int number)
        {
            List<int> powerOf2List = new List<int>();
            int power = 0;
            int powerOf2 = 1;
    
            while (powerOf2 <= number)
            {
                if ((number & powerOf2) == powerOf2)
                    powerOf2List.Add(power);

                power++;
                powerOf2 <<= 1;
            }
    
            return powerOf2List;
        }

        public override string ToString()
        {
            return isFlag ? string.Join('+', flags) : values[selectedIndex];
        }
    }
}
