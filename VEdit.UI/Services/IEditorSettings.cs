using System.IO;
using VEdit.Common;

namespace VEdit.UI
{
    public interface IEditorSettings
    {
        void Load();
        void Save();
    }

    public class EditorSettings : IEditorSettings
    {
        const string AppSettings = "app.settings";
        private ISerializer<string> _serializer;
        private IArchive _archive;

        public string Test { get; set; }

        public EditorSettings(IServiceProvider provider)
        {
            Test = "test";

            _serializer = provider.Get<ISerializer<string>>();
            _archive = provider.Get<IArchive>();
            _archive.AllowOverwrite = true;
        }

        public void Load()
        {
            if (File.Exists(AppSettings))
            {
                _archive.Load(AppSettings);
                Test = _archive.Read<string>(nameof(Test));
            }
        }

        public void Save()
        {
            _archive.Write(nameof(Test), Test);
            _archive.Save(AppSettings);
        }
    }
}
