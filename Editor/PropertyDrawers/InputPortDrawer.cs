using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RuntimeGraphFramework.Editor
{
    [CustomPropertyDrawer(typeof(InputPort<,>))]
    public class InputPortDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var typeProp = property.FindPropertyRelative("portType");
            
            InputPortType inputPortType = (InputPortType)typeProp.intValue;

            var sourceProp = inputPortType switch
            {
                InputPortType.Constant => property.FindPropertyRelative("constantValue"),
                InputPortType.Parameter => property.FindPropertyRelative("parameterName"),
                InputPortType.PortReference => property.FindPropertyRelative("portReference"),
                _ => null
            };
            
            Foldout root = new Foldout();
            root.text = property.displayName;

            var typeField = new PropertyField(typeProp);
            typeField.BindProperty(typeProp);
            root.Add(typeField);
            
            var portField = new PropertyField(sourceProp);
            portField.BindProperty(sourceProp);
            root.Add(portField);
            
            return root;
        }
    }
}