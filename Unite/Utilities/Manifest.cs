using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    class Manifest
    {
        private Dictionary<string, string> values = new Dictionary<string, string>();

        public Manifest(string str)
        {
            Parse(str);
        }

        public Manifest(AssetBundleManifest bundleManifest)
        {
            Parse(bundleManifest.ToString());
        }

        private void Parse(string str)
        {
            string[] lines = str.Split('\n');

            foreach (string line in lines)
            {
                string[] sides = line.Split(':');
                if (sides.Length == 2)
                    values.Add(sides[0].Trim(), sides[1].Trim());
            }
        }
    }
}
