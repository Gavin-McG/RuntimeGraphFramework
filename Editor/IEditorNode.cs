using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public interface IEditorNode<out T> 
        where T : RuntimeNode
    {
        void ClearData();
        T GetRuntimeNode(DialogueImportContext context);
        
        bool TryGetOutputPortIndex(IPort port, out int portIndex);
        bool TryGetInputPortIndex(IPort port, out int portIndex);
        
        IEnumerable<T> GetNodes();
    }
}