using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    [CustomPropertyDrawer(typeof(SerializedTimeSpanDetail))]
    public class SerializedTimeSpanDetailPropertyDrawer : SerializedValueDetailPropertyDrawer
    {
        protected override void DrawValue(Rect valuePos, SerializedProperty valueProp)
        {
            OJEditorUtility.DrawTimeSpanFromLong(valuePos, valueProp);
        }
    }
}