using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;

namespace RuntimeGraphFramework.Editor
{
    public class GraphImportContext
    {
        public AssetImportContext assetContext;
        public RuntimeGraph runtimeGraph;
        public ISubgraphNode currentSubgraph = null;
        public HashSet<IVariable> validVariables;
        
        public Type GraphType => runtimeGraph.GetType();
    }
}