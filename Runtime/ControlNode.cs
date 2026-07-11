
namespace RuntimeGraphFramework
{
    public abstract class ControlNode : RuntimeNode
    {
        public override bool IsConstantNode() => false;

        public abstract ControlNode GetNextNode(IQueryContext context);
    }
}