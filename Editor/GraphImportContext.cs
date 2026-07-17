using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace RuntimeGraphFramework.Editor
{
    public class GraphImportContext
    {
        public Type graphType;
        public ISubgraphNode currentSubgraph = null;
        public HashSet<IVariable> validVariables;
    }
}