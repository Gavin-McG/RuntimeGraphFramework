using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public interface IEditorNode<out T> 
        where T : RuntimeNode
    {
        void ClearData();
        T GetRuntimeNode(DialogueImportContext context);
        
        OutputPort GetRuntimeOutputPort(DialogueImportContext context, IPort port);
        OutputPortReference GetRuntimeOutputPortReference(DialogueImportContext context, IPort port);
        
        InputPort GetRuntimeInputPort(DialogueImportContext context, IPort port);
        InputPortReference GetRuntimeInputPortReference(DialogueImportContext context, IPort port);
        
        IEnumerable<T> GetNodes();
    }
}