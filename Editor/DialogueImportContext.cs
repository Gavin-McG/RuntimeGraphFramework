using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public class DialogueImportContext
    {
        public ISubgraphNode currentSubgraph = null;
        public HashSet<IVariable> validVariables;
    }
}