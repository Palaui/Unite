using UnityEngine;

namespace Unite
{
    public class FlagAttribute : PropertyAttribute
    {
        // Variables
        #region Variables

        public string flag;
        public string value;
        public bool isvalueAvailable;

        #endregion

        // Override
        #region Override

        public FlagAttribute(string flag)
        {
            this.flag = flag;
            isvalueAvailable = false;
        }

        public FlagAttribute(string flag, string value)
        {
            this.flag = flag;
            this.value = value;
            isvalueAvailable = true;
        }

        #endregion

    }
}
