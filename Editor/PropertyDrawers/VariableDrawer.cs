using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RuntimeGraphFramework.Editor
{
    [CustomPropertyDrawer(typeof(RuntimeVariable))]
    public class VariableDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var wrapperProp = property.FindPropertyRelative("valueWrapper");
            var valueProp = wrapperProp.FindPropertyRelative("_value");
            
            VisualElement root = new VisualElement();
            
            var sourceField = new PropertyField(valueProp);
            sourceField.BindProperty(valueProp);
            root.Add(sourceField);
            
            return root;
        }
    }
}