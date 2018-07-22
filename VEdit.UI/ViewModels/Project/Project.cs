using System.Collections.Generic;
using System.IO;
using System.Linq;
using VEdit.Common;
using VEdit.Editor;

namespace VEdit.UI
{
    public class Project : BaseViewModel
    {
        private IOutputManager _errorReporter;
        private EditorViewModel _editor;

        public ProjectDirectory Root { get; }
        public string DirectoryPath { get; }
        public string Name => Root.FullName;
        public string ProjectFilePath => $"{DirectoryPath}{Name}.{Extension.Project}";
        public System.Guid Id { get; }

        private Project(ProjectData project, string projectFolder, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _editor = ServiceProvider.Get<EditorViewModel>();
            _errorReporter = ServiceProvider.Get<IOutputManager>();

            Id = project.Id;
            DirectoryPath = projectFolder;
            Root = CreateRoot(project);

            var commandProvider = ServiceProvider.Get<ICommandProvider>();
            AddExistingItemCommand = commandProvider.Create<ProjectDirectory>(AddExistingItem);
            AddFolderCommand = commandProvider.Create<ProjectDirectory>(AddFolder);
            AddImageGraphCommand = commandProvider.Create<ProjectDirectory>(AddImageGraph);
            AddTextGraphCommand = commandProvider.Create<ProjectDirectory>(AddTextGraph);
            AddFunctionGraphCommand = commandProvider.Create<ProjectDirectory>(AddFunctionGraph);
            AddTextFileCommand = commandProvider.Create<ProjectDirectory>(AddTextFile);
            CloseProjectCommand = commandProvider.CreateAsync<Project>(_editor.CloseProject);
            OpenFileCommand = commandProvider.Create<ProjectFile>(OpenFile);
            DeleteCommand = commandProvider.Create<ProjectEntry>(Delete);
            RenameCommand = commandProvider.Create<ProjectEntry>(Rename);
        }

        public ICommand AddExistingItemCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CloseProjectCommand { get; set; }
        public ICommand AddImageGraphCommand { get; set; }
        public ICommand AddTextGraphCommand { get; set; }
        public ICommand AddFunctionGraphCommand { get; set; }
        public ICommand AddTextFileCommand { get; set; }
        public ICommand RenameCommand { get; set; }

        #region Commands

        #region Create Graphs

        private async void AddTextFile(ProjectDirectory root)
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            var name = await dialog.ShowInputAsync("New text file", "Enter file name");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var unique = GetUniqueFileName(root, name, Extension.Text);
                var newFile = new ProjectFile(this, root, unique);

                var io = ServiceProvider.Get<IFileIO>();
                io.Write(newFile.AbsolutePath, string.Empty);

                OpenFile(newFile);
            }
        }

        private void AddTextGraph(ProjectDirectory root)
        {
            CreateAndOpenGraphFile<TextGraph>(root);
        }

        private void AddFunctionGraph(ProjectDirectory root)
        {
            CreateAndOpenGraphFile<FunctionGraph>(root);
        }

        private void AddImageGraph(ProjectDirectory root)
        {
            CreateAndOpenGraphFile<ImageGraph>(root);
        }

        private async void CreateAndOpenGraphFile<T>(ProjectDirectory root) where T : Graph
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            var name = await dialog.ShowInputAsync("New graph", "Enter file name");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var factory = ServiceProvider.Get<IGraphFactory>();
                var archive = ServiceProvider.Get<IArchive>();
                var graph = factory.Create<T>();
                graph.Name = name;

                archive.Write(nameof(System.Type), typeof(T));
                graph.Save(archive);

                var unique = GetUniqueFileName(root, graph.Name, Extension.Graph);
                var newFile = new ProjectFile(this, root, unique);

                archive.Save(newFile.AbsolutePath);

                OpenFile(newFile);
            }
        }

        private string GetUniqueFileName(ProjectDirectory root, string fileName, string extension)
        {
            var names = root.Children.Select(n => n.FullName.Replace($".{extension}", string.Empty));
            var unique = $"{fileName.Unique(names)}.{extension}";

            return unique;
        }

        #endregion

        private async void AddFolder(ProjectDirectory root)
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            var name = await dialog.ShowInputAsync("New folder", "Enter folder name");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var names = root.Children.Select(n => n.FullName);
                var unique = name.Unique(names);
                var newDir = new ProjectDirectory(this, root, unique);

                Directory.CreateDirectory(newDir.AbsolutePath);
            }
        }

        private void AddExistingItem(ProjectDirectory root)
        {
            var dialog = ServiceProvider.Get<IOpenFileDialog>();
            dialog.AllowMultiple = false;
            dialog.Filter = new FileDialogFilter("All files", "*");

            if (dialog.Show())
            {
                var newPath = Path.Combine(root.RelativePath, dialog.FileName);
                var absPath = Path.Combine(DirectoryPath, newPath);

                if (dialog.FilePath != absPath)
                {
                    File.Copy(dialog.FilePath, absPath, true);
                }

                if (!root.Children.Any(f => f.FullName == dialog.FileName))
                {
                    new ProjectFile(this, root, dialog.FileName);
                }
            }
        }

        private async void Delete(ProjectEntry item)
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            var result = await dialog.ShowMessageAsync($"Delete {(item is ProjectDirectory ? "directory" : "file")}",
                $"Delete {item.FullName} and all its contents?",
                DialogStyle.AffirmativeAndNegative);

            if (result == DialogResult.Affirmative)
            {
                item.Delete();

                if (item is ProjectDirectory)
                {
                    Directory.Delete(item.AbsolutePath, true);
                }
                else if (item is ProjectFile file)
                {
                    File.Delete(item.AbsolutePath);

                    await _editor.TryCloseFileEditor(file, false);

                    var db = ServiceProvider.Get<INodeDatabase>();
                    var data = db.GetByFile(file.Id);

                    if (data != null)
                    {
                        db.Remove(data);
                    }
                }
            }
        }

        private async void Rename(ProjectEntry item)
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            var newName = await dialog.ShowInputAsync($"Rename {item.Name}", "Enter new name");

            if (!string.IsNullOrWhiteSpace(newName))
            {
                var absPath = Path.Combine(item.FullDirectoryPath, newName);

                var isDirectory = item is ProjectDirectory;
                bool valid = false;

                if (isDirectory)
                {
                    valid = !Directory.Exists(absPath);
                }
                else if (item is ProjectFile fv)
                {
                    valid = !File.Exists($"{absPath}.{fv.Extension}");
                }

                // path before rename
                var oldPath = item.AbsolutePath;

                if (valid && item.Rename(newName))
                {
                    if (isDirectory)
                    {
                        Directory.Move(oldPath, item.AbsolutePath);
                    }
                    else if (item is ProjectFile fv)
                    {
                        File.Move(oldPath, item.AbsolutePath);

                        // TODO: Refactor this
                        var db = ServiceProvider.Get<INodeDatabase>();
                        var data = db.GetByFile(fv.Id);

                        if(data != null)
                        {
                            data.DisplayName = item.Name;
                            db.Update(data);
                        }
                    }
                }
                else
                {
                    var result = await dialog.ShowMessageAsync("Invalid file name", newName, DialogStyle.AffirmativeAndNegative);
                    if (result == DialogResult.Affirmative)
                    {
                        Rename(item);
                    }
                }
            }
        }

        public void OpenFile(ProjectFile file)
        {
            if(!_editor.TryOpenFileEditor(file))
            {
                var dialog = ServiceProvider.Get<IDialogManager>();
                dialog.ShowMessageAsync($"Open {file.FullName}", $"Unknown file type {file.Extension}");
            }
        }

        #endregion

        #region Methods

        private ProjectDirectory CreateRoot(ProjectData project)
        {
            var folders = new Dictionary<string, ProjectDirectory>();
            var root = new ProjectDirectory(this, null, project.Name);

            var directories = project.Files.Where(f => f.IsDirectory);
            var files = project.Files.Where(f => !f.IsDirectory);

            foreach (var dir in directories)
            {
                folders.Add(dir.RelativePath, new ProjectDirectory(this, null, Path.GetFileName(dir.RelativePath)));
            }

            foreach (var kvp in folders)
            {
                var path = kvp.Key.Split(Path.DirectorySeparatorChar);
                var currDir = kvp.Value;
                if (path.Length > 1)
                {
                    var parentDirName = string.Join(Path.DirectorySeparatorChar.ToString(), path.Take(path.Length - 1));
                    var parentDir = folders[parentDirName];
                    currDir.Root = parentDir;
                }
                else
                {
                    currDir.Root = root;
                }
            }

            foreach (var file in files)
            {
                var dirName = Path.GetDirectoryName(file.RelativePath);
                var fileName = Path.GetFileName(file.RelativePath);

                new ProjectFile(this, string.IsNullOrWhiteSpace(dirName) ? root : folders[dirName], fileName)
                {
                    Id = file.Id
                };
            }

            return root;
        }

        public static Project Load(string projectFilePath, IServiceProvider serviceProvider)
        {
            var io = serviceProvider.Get<IFileIO>();

            var folderPath = Path.GetDirectoryName(projectFilePath);
            var serializer = serviceProvider.Get<ISerializer<string>>();

            var projectFile = io.Read(projectFilePath);
            var project = serializer.Deserialize<ProjectData>(projectFile);

            if (project == null)
            {
                throw new System.Exception($"Could not load project file: {projectFilePath}");
            }

            var database = serviceProvider.Get<INodeDatabase>();
            database.Load(project.Nodes);

            ValidateProjectFiles(folderPath, project);

            return new Project(project, folderPath, serviceProvider);

            void ValidateProjectFiles(string path, ProjectData p)
            {
                var toBeRemoved = new List<FileEntry>();
                foreach (var file in p.Files)
                {
                    var absolutePath = Path.Combine(path, file.RelativePath);
                    if (file.IsDirectory)
                    {
                        if (!Directory.Exists(absolutePath))
                        {
                            toBeRemoved.Add(file);
                        }
                    }
                    else
                    {
                        if (!File.Exists(absolutePath))
                        {
                            toBeRemoved.Add(file);
                        }
                    }
                }

                toBeRemoved.ForEach(f => p.Files.Remove(f));
            }
        }

        public static string Create(string projectName, string projectFolder, IServiceProvider serviceProvider)
        {
            ProjectData project = new ProjectData
            {
                Id = System.Guid.NewGuid(),
                Name = projectName
            };

            return SaveInternal(projectFolder, project, serviceProvider);
        }

        private static string SaveInternal(string path, ProjectData project, IServiceProvider serviceProvider)
        {
            var serializer = serviceProvider.Get<ISerializer<string>>();
            var io = serviceProvider.Get<IFileIO>();

            var projectPath = Path.Combine(path, $"{project.Name}.{Extension.Project}");
            var json = serializer.Serialize(project);
            io.Write(projectPath, json);

            return projectPath;
        }

        public void Save()
        {
            SaveInternal(DirectoryPath, GetData(), ServiceProvider);

            ProjectData GetData()
            {
                ProjectData project = new ProjectData
                {
                    Id = Id,
                    Name = Root.FullName
                };

                AddNodes(project.Nodes);
                AddFiles(Root, project.Files);

                return project;

                void AddFiles(ProjectDirectory dir, List<FileEntry> files)
                {
                    foreach (var child in dir.Children)
                    {
                        files.Add(new FileEntry
                        {
                            Id = child is ProjectFile fw ? fw.Id : System.Guid.Empty,
                            IsDirectory = child is ProjectDirectory,
                            RelativePath = child.RelativePath
                        });

                        if (child is ProjectDirectory root)
                        {
                            AddFiles(root, files);
                        }
                    }
                }

                void AddNodes(List<GraphNodeEntry> nodes)
                {
                    var database = ServiceProvider.Get<INodeDatabase>();
                    var data = database.GetByProject(Id);

                    foreach (var node in data)
                    {
                        nodes.Add(node);
                    }
                }
            }
        }

        public IEnumerable<ProjectFile> GetFiles(ProjectDirectory root)
        {
            foreach (var file in root.Children)
            {
                if (file is ProjectDirectory dw)
                {
                    foreach (var fw in GetFiles(dw))
                    {
                        yield return fw;
                    }
                }
                else
                {
                    yield return file as ProjectFile;
                }
            }
        }

        #endregion
    }
}
