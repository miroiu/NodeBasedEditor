using System;

namespace VEdit.Editor
{
    public delegate void CloseEventHandler();

    public interface IActionList : IElement
    {
        event CloseEventHandler CloseEvent;
        void Close();
        void SpawnNode(Guid pluginId);
    }
}
