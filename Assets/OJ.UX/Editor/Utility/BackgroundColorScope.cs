using System;
using UnityEngine;

namespace OJ.UX.Editor.Utility
{
    public class BackgroundColorScope : IDisposable
    {
        private readonly Color _previousColor;
        private bool _disposed;

        public BackgroundColorScope(Color color)
        {
            _previousColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                InternalDispose();
            }
        }

        private void InternalDispose()
        {
            GUI.backgroundColor = _previousColor;
        }
    }
}