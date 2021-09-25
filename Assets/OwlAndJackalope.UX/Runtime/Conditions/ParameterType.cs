using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Conditions
{
    public enum ParameterType
    {
        Value = 0, //Value
        Detail = 1, //Detail
        Argument = 2, //Argument
        DetailComponent = 3, //Detail Component
    }

    public enum ComponentType
    {
        Size = 0,
        X = 1,
        Y = 2,
        Z = 3,
        W = 4
    }
    
    [System.Serializable]
    public class Parameter
    {
        [SerializeField]
        public string Name;
        
        [SerializeField]
        public ParameterType Type;

        [SerializeField]
        public ComponentType ComponentType;
    }
}