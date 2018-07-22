using System;

namespace VEdit.Editor
{
    public interface IDraggable : ISelectable
    {
        void Drag(double x, double y);
        event Action Dragged;
    }
}
