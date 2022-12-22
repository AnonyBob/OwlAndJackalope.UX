using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    [CustomPropertyDrawer(typeof(SerializedDateTimeDetail))]
    public class SerializedDateTimeDetailPropertyDrawer : SerializedValueDetailPropertyDrawer
    {
        protected override void DrawValue(Rect valuePos, SerializedProperty valueProp)
        {
            OJEditorUtility.DrawDateTimeFromLong(valuePos, valueProp);
        }
    }
}