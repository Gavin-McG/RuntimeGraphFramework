using RuntimeGraphFramework;

public interface IRuntimeBlockNode : IRuntimeNode
{
    public IRuntimeContextNode ContextNode { get; }
    public int Index { get; }
}