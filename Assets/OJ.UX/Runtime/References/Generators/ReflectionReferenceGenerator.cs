using System;
using System.Collections.Generic;
using System.Reflection;

namespace OJ.UX.Runtime.References.Generators
{
    public class ReflectionReferenceGenerator<T> : IReferenceGenerator<T> where T : class
    {
        public IReference ConstructReference(T value)
        {
            var details = new List<KeyValuePair<string, IDetail>>();
            var type = typeof(T);
            var detailType = typeof(Detail<>);
            foreach (var field in type.GetFields())
            {
                var generationAttribute = field.GetCustomAttribute<IncludeAsDetailAttribute>();
                if (generationAttribute != null)
                {
                    var genericType = detailType.MakeGenericType(field.FieldType);
                    var detail = (IDetail)Activator.CreateInstance(genericType, field.GetValue(value));
                    details.Add(new KeyValuePair<string, IDetail>(field.Name, detail));
                }
            }
            
            foreach (var property in type.GetProperties())
            {
                var generationAttribute = property.GetCustomAttribute<IncludeAsDetailAttribute>();
                if (generationAttribute != null)
                {
                    var genericType = detailType.MakeGenericType(property.PropertyType);
                    var detail = (IDetail)Activator.CreateInstance(genericType, property.GetValue(value));
                    details.Add(new KeyValuePair<string, IDetail>(property.Name, detail));
                }
            }

            return new Reference(details);
        }

        public void ApplyReference(T target, IReference reference)
        {
            var type = typeof(T);
            foreach (var detail in reference)
            {
                var field = type.GetField(detail.Key);
                if (field != null)
                {
                    field.SetValue(target, detail.Value.Value);
                    continue;
                }

                var property = type.GetProperty(detail.Key);
                if (property != null)
                {
                    property.SetValue(target, detail.Value.Value);
                }
            }
        }
    }
}