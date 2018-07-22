using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VEdit.Common;
using VEdit.Editor;

namespace VEdit.UI
{
    public class EditorViewModel : BaseViewModel
    {
        #region Private Properties

        private readonly IEditorSettings _settings;
        private readonly INodeDatabase _nodeDatabase;
        private readonly ILogger _logger;
        private readonly IOutputManager _output;

        private readonly FileEditorFactory _fileEditorFactory;

        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();
        private ObservableCollection<FileEditor> _editors = new ObservableCollection<FileEditor>();

        #endregion

        #region Public Properties

        public ICommand NewProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand OpenStartPageCommand { get; }
        public ICommand ToggleOutputCommand { get; }

        public StartPageFile StartPage { get; private set; }

        public ReadOnlyObservableCollection<Project> Projects { get; }
        public ReadOnlyObservableCollection<FileEditor> OpenedFileEditors { get; }

        private bool _showProjectExplorer;
        public bool ShowProjectExplorer
        {
            get => _showProjectExplorer;
            set => SetProperty(ref _showProjectExplorer, value);
        }

        private bool _isInitialized;
        public bool IsInitialized
        {
            get => _isInitialized;
            private set => SetProperty(ref _isInitialized, value);
        }

        private bool _showOutput;
        public bool ShowOutput
        {
            get => _showOutput;
            set => SetProperty(ref _showOutput, value);
        }

        private FileEditor _selectedFileEditor;
        public FileEditor SelectedFileEditor
        {
            get => _selectedFileEditor;
            set => SetProperty(ref _selectedFileEditor, value);
        }

        public OutputWindow OutputWindow { get; }
        public BreakpointWindow BreakpointWindow { get; }

        #endregion

        public EditorViewModel(Common.IServiceProvider serviceProvider, ICommandProvider commandProvider, IEditorSettings settings, IOutputManager errorReporter) : base(serviceProvider)
        {
            _settings = settings;
            _logger = serviceProvider.Get<ILogger>();
            _output = serviceProvider.Get<IOutputManager>();
            _nodeDatabase = serviceProvider.Get<INodeDatabase>();
            _fileEditorFactory = serviceProvider.Get<FileEditorFactory>();

            NewProjectCommand = commandProvider.Create(CreateProject);
            OpenProjectCommand = commandProvider.Create(OpenProject);
            OpenStartPageCommand = commandProvider.Create(OpenStartPage, CanOpenStartPage);
            ToggleOutputCommand = commandProvider.Create(() => ShowOutput = !ShowOutput);

            Projects = new ReadOnlyObservableCollection<Project>(_projects);
            OpenedFileEditors = new ReadOnlyObservableCollection<FileEditor>(_editors);

            OutputWindow = new OutputWindow(serviceProvider);
            BreakpointWindow = new BreakpointWindow(serviceProvider);
        }

        #region Private

        private void AddEditor(FileEditor editor)
        {
            _editors.Add(editor);
        }

        private void RemoveEditor(FileEditor editor)
        {
            _editors.Remove(editor);
        }

        private void AddProject(Project project)
        {
            _projects.Add(project);
        }

        private void RemoveProject(Project project)
        {
            _projects.Remove(project);
        }

        #endregion

        #region Start Page

        private bool CanOpenStartPage()
        {
            return !OpenedFileEditors.Contains(StartPage);
        }

        private void OpenStartPage()
        {
            OpenFileEditor(StartPage, false);
        }

        private void SetupStartPage()
        {
            StartPage = StartPageFile.Create(ServiceProvider);
            OpenStartPage();
        }

        private void CloseStartPage()
        {
            StartPage.SaveContent();
            RemoveEditor(StartPage);
        }

        #endregion

        #region Project

        public Task CloseAllProjects()
        {
            var tasks = Projects.ToList().Select(p => CloseProject(p));
            return Task.WhenAll(tasks);
        }

        public async Task CloseProject(Project project)
        {
            RemoveProject(project);

            if (Projects.Count == 0)
            {
                ShowProjectExplorer = false;
            }

            foreach (var editor in GetActiveEditors(project).ToList())
            {
                await editor.Close();
            }

            await Task.Run(() => project.Save());
        }

        public IEnumerable<FileEditor> GetActiveEditors(Project project)
        {
            foreach (var editor in OpenedFileEditors)
            {
                if (editor.File.Project == project)
                {
                    yield return editor;
                }
            }
        }

        private void CreateProject()
        {
            var openFolder = ServiceProvider.Get<IOpenFolderDialog>();

            if (openFolder.Show())
            {
                var result = Project.Create(openFolder.FolderName, openFolder.FolderPath, ServiceProvider);
                LoadProject(result);
            }
        }

        private void OpenProject()
        {
            var openFile = ServiceProvider.Get<IOpenFileDialog>();
            openFile.Filter = new FileDialogFilter("Visual Editor Project File", Extension.Project);

            if (openFile.Show())
            {
                LoadProject(openFile.FilePath);
            }
        }

        public void LoadProject(string path)
        {
            Project result = Project.Load(path, ServiceProvider);

            if (!Projects.Any(p => p.DirectoryPath == result.DirectoryPath))
            {
                AddProject(result);
                StartPage.AddRecent(result);
                ShowProjectExplorer = true;
            }
        }

        #endregion

        #region Event Handlers

        public async void Initialize()
        {
            if (IsInitialized) return;

            try
            {
                SetupStartPage();
                await Task.Run(() =>
                {
                    _settings.Load();
                    _nodeDatabase.Load();
                });

                IsInitialized = true;
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString());
            }
        }

        public async Task Shutdown()
        {
            _settings.Save();
            CloseStartPage();

            await CloseAllEditors(true);
            await CloseAllProjects();

            TempFile.DeleteAll();
        }

        #endregion

        #region File Editor

        public bool IsOpen(FileEditor editor)
        {
            return OpenedFileEditors.Contains(editor);
        }

        public bool TryGetFileEditor(ProjectFile file, out FileEditor editor)
        {
            if (file.IsOpen)
            {
                editor = file.Editor;
                return true;
            }

            editor = _fileEditorFactory.Create(file.Type, file);

            return editor != null;
        }

        private async void OpenFileEditor(FileEditor editor, bool showProgress = true)
        {
            if (!IsOpen(editor))
            {
                bool loaded = false;

                if (showProgress)
                {
                    var dialog = ServiceProvider.Get<IDialogManager>();
                    var progress = await dialog.ShowProgress("Open File", $"Please wait... Opening file {editor.File.Name}");
                    loaded = editor.LoadContent();
                    await progress.Close();
                }
                else
                {
                    loaded = editor.LoadContent();
                }

                if (loaded)
                {
                    AddEditor(editor);
                }
                else
                {
                    _output.Write($"Could not load file contents: {editor.Path}", OutputType.Error);
                }
            }
            SelectedFileEditor = editor;
        }

        public bool TryOpenFileEditor(ProjectFile file)
        {
            if (TryGetFileEditor(file, out FileEditor editor))
            {
                OpenFileEditor(editor, true);
                return true;
            }
            return false;
        }

        public Task CloseAllEditors(bool save)
        {
            var tasks = OpenedFileEditors.ToList().Select(o => o.Close());
            return Task.WhenAll(tasks);
        }

        private async Task<bool> CloseFileEditor(FileEditor editor, bool askForSave)
        {
            bool removeEditor = !askForSave;
            if (askForSave)
            {
                var dialog = ServiceProvider.Get<IDialogManager>();
                var result = await dialog.ShowMessageAsync("Close file", $"{editor.Name} has unsaved changes. Save ?", DialogStyle.AffirmativeNegativeAndCancel);

                if (result == DialogResult.Affirmative)
                {
                    editor.SaveContent();
                }
                removeEditor = result != DialogResult.Canceled;
            }

            if (removeEditor)
            {
                RemoveEditor(editor);
            }

            return removeEditor;
        }

        public async Task<bool> TryCloseFileEditor(ProjectFile file, bool save)
        {
            if (file.IsOpen)
            {
                return await CloseFileEditor(file.Editor, save);
            }
            return false;
        }

        #endregion
    }
}
