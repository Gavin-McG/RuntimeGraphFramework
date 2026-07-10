using System;
using System.Collections.Generic;
using System.Linq;

namespace BehavioralDialogue
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PortTypeCastAttribute : Attribute
    {
        public Type outputType;
        public Type inputType;
        public List<Type> graphTypes;

        public PortTypeCastAttribute(Type outputType, Type inputType, params Type[] graphTypes)
        {
            this.outputType = outputType;
            this.inputType = inputType;
            this.graphTypes = graphTypes.ToList();
        }
    }
}