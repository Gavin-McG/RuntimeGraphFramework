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
            var typeProp = property.FindPropertyRelative("portSource");
            
            InputPortSource inputPortSource = (InputPortSource)typeProp.intValue;

            var sourceProp = inputPortSource switch
            {
                InputPortSource.Constant => property.FindPropertyRelative("constantValue"),
                InputPortSource.Variable => property.FindPropertyRelative("variableName"),
                InputPortSource.PortReference => property.FindPropertyRelative("portReference"),
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