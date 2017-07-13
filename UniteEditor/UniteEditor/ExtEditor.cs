using System.Collections.Generic;
using System.Reflection;
using Unite;
using UnityEditor;
using UnityEngine;

namespace UniteEditor
{
    public static class ExtEditor
    {
        public static Component[] GetVisibleComponents()
        {
            Transform[] transforms = Selection.transforms;
            if (transforms.Length == 0)
                return new Component[] { };

            return transforms[0].GetComponents<Component>();
        }

        public static FieldInfo[] GetSelectionFields()
        {
            Transform[] transforms = Selection.transforms;
            if (transforms.Length == 0)
                return new FieldInfo[] { };

            List<FieldInfo> list = new List<FieldInfo>();
            Component[] components = transforms[0].GetComponents<Component>();
            foreach (Component comp in components)
            {
                foreach (FieldInfo field in InspectorReflect.GetAllFields(comp.GetType()))
                    list.Add(field);
            }
            return Ext.CreateArrayFromList(list);
        }
    }
}
