using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RuntimeGraphFramework.Editor
{
    [CustomPropertyDrawer(typeof(OutputPort<>))]
    public class OutputPortDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Foldout root = new Foldout();
            root.text = property.displayName;
            
            var nodeProp = property.FindPropertyRelative("node");
            var nodeField = new PropertyField(nodeProp);
            nodeField.BindProperty(nodeProp);
            root.Add(nodeField);
            
            return root;
        }
    }
}