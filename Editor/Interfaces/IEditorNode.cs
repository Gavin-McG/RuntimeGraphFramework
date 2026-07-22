using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public interface IEditorNode<out T> 
    {
        bool IsCreated { get; }
        
        void ClearData();
        T GetRuntimeNode(GraphImportContext context);
        
        bool TryGetOutputPortIndex(IPort port, out int portIndex);
        bool TryGetInputPortIndex(IPort port, out int portIndex);
    }
}