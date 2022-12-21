namespace OJ.UX.Runtime.References.Generators
{
    /// <summary>
    /// Allows you to define relationship between a reference and an object that is not a reference.
    /// </summary>
    public interface IReferenceGenerator<in T>
    {
        IReference ConstructReference(T value);

        void ApplyReference(T target, IReference reference);
    }
}