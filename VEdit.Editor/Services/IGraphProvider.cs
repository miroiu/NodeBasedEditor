using System;

namespace VEdit.Editor
{
    public interface IGraphProvider
    {
        Graph Get(Guid graphNodeInstanceId);
        void OpenInEditor(Guid graphNodeInstanceId);
    }
}
