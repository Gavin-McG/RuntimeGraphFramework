using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace RuntimeGraphFramework.Editor
{
    public interface IEditorNodeOwner<TRuntimeNode>
        where TRuntimeNode : RuntimeNode
    {
        Hash128 ID { get; }

        IEnumerable<IPort> GetInputPorts();
        IEnumerable<IPort> GetOutputPorts();

        void InitializeRuntimeNode(DialogueImportContext context, TRuntimeNode node);
    }
}