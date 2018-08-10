using System;

namespace VEdit.Editor
{
    public delegate void CloseEventHandler();

    public interface IActionList : IBlackboardElement
    {
        event CloseEventHandler CloseEvent;
        void Close();
        void SpawnNode(Guid pluginId);
    }
}
