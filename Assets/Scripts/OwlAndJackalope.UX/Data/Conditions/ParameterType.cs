using UnityEngine;

namespace OwlAndJackalope.UX.Data.Conditions
{
    public enum ParameterType
    {
        Value = 0,
        Detail = 1,
        Argument = 2,
        DetailComponent = 3,
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