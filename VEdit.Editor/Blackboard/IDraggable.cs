using System;

namespace VEdit.Editor
{
    public interface IDraggable
    {
        void Drag(double x, double y);
        void StartDrag(double x, double y);
        void EndDrag(double x, double y);
        bool IsDragging { get; }
        event Action Dragged;
    }
}
