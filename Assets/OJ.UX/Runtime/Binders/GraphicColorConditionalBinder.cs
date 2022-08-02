using System.Collections.Generic;
using OJ.UX.Runtime.Binders.Conditions;
using OJ.UX.Runtime.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace OJ.UX.Runtime.Binders
{
    public class GraphicColorConditionalBinder : ConditionalBinder<GraphicColorConditionalBinder.ColorAction>
    {
        [System.Serializable]
        public struct GraphicColorState
        {
            [SerializeField]
            public Graphic Graphic;
            
            [SerializeField]
            public Color Color;
        }
        
        [System.Serializable]
        public class ColorAction : IConditionalActionDetail
        {
            [SerializeField, ComplexInlineProperty("Graphic", "Color")]
            private List<GraphicColorState> _colors;

            public void PerformAction()
            {
                foreach (var color in _colors)
                {
                    if (color.Graphic != null)
                        color.Graphic.color = color.Color;
                }
            }
        }
    }
}