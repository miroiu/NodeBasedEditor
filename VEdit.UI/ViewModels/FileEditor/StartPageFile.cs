using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VEdit.Common;

namespace VEdit.UI
{
    public class RecentProject
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string FullPath => System.IO.Path.Combine(Path, $"{Name}.{Extension.Project}");
    }

    public class StartPageFile : FileEditor
    {
        private EditorViewModel _editor;
        const string StartPagePath = "start_page.json";

        private bool _isLoaded;
        private ISerializer<string> _serializer;

        public ICommand NewProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand OpenRecentProjectCommand { get; }
        public ObservableCollection<RecentProject> RecentProjects { get; set; } = new ObservableCollection<RecentProject>();

        public StartPageFile(ProjectFile file, IServiceProvider serviceProvider) : base(file, serviceProvider)
        {
            _editor = serviceProvider.Get<EditorViewModel>();
            _serializer = serviceProvider.Get<ISerializer<string>>();

            var cmdProvider = serviceProvider.Get<ICommandProvider>();
            NewProjectCommand = _editor.NewProjectCommand;
            OpenProjectCommand = _editor.OpenProjectCommand;
            OpenRecentProjectCommand = cmdProvider.Create<string>(_editor.LoadProject);
        }

        public override bool LoadContent()
        {
            if (_isLoaded)
            {
                return true;
            }

            if (System.IO.File.Exists(StartPagePath))
            {
                var io = ServiceProvider.Get<IFileIO>();
                var json = io.Read(StartPagePath);
                var result = _serializer.Deserialize<List<RecentProject>>(json);

                List<RecentProject> toBeRemoved = new List<RecentProject>();
                foreach (var project in result)
                {
                    if (!System.IO.File.Exists(project.FullPath))
                    {
                        toBeRemoved.Add(project);
                    }
                }
                toBeRemoved.ForEach(p => result.Remove(p));

                RecentProjects.Clear();
                RecentProjects.AddRange(result);
                _isLoaded = true;
                return true;
            }

            SaveContent();
            return LoadContent();
        }

        public override void SaveContent()
        {
            var json = _serializer.Serialize(RecentProjects);
            var io = ServiceProvider.Get<IFileIO>();
            io.Write(StartPagePath, json);
        }

        public static StartPageFile Create(IServiceProvider serviceProvider)
        {
            var obj = new StartPageFile(new ProjectFile("Start Page"), serviceProvider);

            if (obj.LoadContent())
            {
                return obj;
            }
            else
            {
                var _output = serviceProvider.Get<IOutputManager>();
                _output.Write("Could not load Start Page.", OutputType.Error);
            }
            return null;
        }

        public void AddRecent(Project project)
        {
            var obj = RecentProjects.FirstOrDefault(p => p.Path == project.DirectoryPath);
            if (obj == null)
            {
                RecentProjects.Insert(0, new RecentProject
                {
                    Name = project.Name,
                    Path = project.DirectoryPath
                });
            }
            else
            {
                RecentProjects.Remove(obj);
                RecentProjects.Insert(0, obj);
            }
        }
    }
}
