using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    internal interface IEditorNode<out T> 
    {
        T RuntimeNode { get; }
        
        void ClearData();
        
        internal void CreateRuntimeNode(GraphImportContext context);
        internal void ConnectRuntimeNode(GraphImportContext context);
        internal void InitializeRuntimeNode(GraphImportContext context);
        bool TryGetOutputPortIndex(IPort port, out int portIndex);
        bool TryGetInputPortIndex(IPort port, out int portIndex);
    }
}