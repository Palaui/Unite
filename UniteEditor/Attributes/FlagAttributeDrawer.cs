using System.Reflection;
using UnityEditor;
using UnityEngine;
using Unite;

namespace UniteEditor
{
    [CustomPropertyDrawer(typeof(FlagAttribute))]
    public class FlagAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FlagAttribute flagAttribute = attribute as FlagAttribute;
            Component[] components = ExtEditor.GetVisibleComponents();
            foreach (Component comp in components)
            {
                FieldInfo[] fields = Reflect.GetAllFields(comp.GetType());
                foreach (FieldInfo field in fields)
                {
                    if (field.Name == flagAttribute.flag)
                    {
                        if (flagAttribute.isvalueAvailable)
                        {
                            if (field.GetValue(comp).ToString() == flagAttribute.value)
                                EditorGUI.PropertyField(position, property);
                        }
                        else
                        {
                            if (field.GetValue(comp).ToString() == "True")
                                EditorGUI.PropertyField(position, property);
                        }
                    }
                }
            }
        }
    }
}
