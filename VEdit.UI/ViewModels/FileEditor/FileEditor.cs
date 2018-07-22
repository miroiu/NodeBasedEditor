using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VEdit.Common;

namespace VEdit.UI
{
    public abstract class FileEditor : BaseViewModel
    {
        private EditorViewModel _editor;

        public ICommand CloseCommand { get; }

        public FileEditor(ProjectFile file, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _editor = serviceProvider.Get<EditorViewModel>();
            IsDirty = true;

            File = file;
            File.Editor = this;
            File.Renamed += OnFileRenamed;
            Name = File.Name;

            var cmdProvider = serviceProvider.Get<ICommandProvider>();

            CloseCommand = cmdProvider.CreateAsync(Close);

            Actions.Add("Save", cmdProvider.Create(SaveContent));
            Actions.Add("Save As", cmdProvider.Create(SaveAs));
            Actions.Add("Close", CloseCommand);
            Actions.Add("Close All", cmdProvider.Create(CloseAll));
        }

        private void CloseAll()
        {
            _editor.CloseAllEditors(true);
        }

        public async Task Close()
        {
            if(await _editor.TryCloseFileEditor(File, IsDirty))
            {
                File.Editor = null;
                File.Renamed -= OnFileRenamed;
                OnClosing();
            }
        }

        private void OnFileRenamed()
        {
            Name = File.Name;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Path => File.AbsolutePath;
        public ProjectFile File { get; }
        public virtual bool IsDirty { get; protected set; }

        public abstract bool LoadContent();
        public abstract void SaveContent();

        public async void SaveAs()
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            var input = await dialog.ShowInputAsync($"Save {File.Name}", "Enter new name");

            var oldName = File.Name;
            if (!string.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, @"[aA-zZ0-9]|\s"))
            {
                File.Name = input;

                SaveContent();
                var file = new ProjectFile(File.Project, File.Root, File.FullName);

                File.Name = oldName;

                File.Project.OpenFile(file);
                //File.Editor.SaveContent();
            }
        }

        public virtual void OnClosing()
        {
        }
    }
}
