using System;

namespace VEdit.Common
{
    public interface ISaveLoad
    {
        void Save(IArchive archive);
        void Load(IArchive archive);

        event Action Loaded;
    }
}
