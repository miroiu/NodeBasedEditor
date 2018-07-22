using System;
using System.Linq;
using VEdit.Common;
using VEdit.Editor;

namespace VEdit.UI
{
    public class GraphProvider : IGraphProvider
    {
        private EditorViewModel _editor;
        private INodeDatabase _database;
        private IOutputManager _output;

        public GraphProvider(Common.IServiceProvider provider)
        {
            _editor = provider.Get<EditorViewModel>();
            _database = provider.Get<INodeDatabase>();
            _output = provider.Get<IOutputManager>();
        }

        public Graph Get(Guid instanceId)
        {
            var file = GetGraphFile(instanceId);
            if (_editor.TryGetFileEditor(file, out FileEditor editor))
            {
                if (editor is GraphFileEditor gr)
                {
                    if (!_editor.IsOpen(editor))
                    {
                        editor.LoadContent();
                    }
                    return gr.Graph;
                }
            }
            return null;
        }

        private ProjectFile GetGraphFile(Guid instanceId)
        {
            if (_database.Get(instanceId) is GraphNodeEntry data)
            {
                var projectId = data.ProjectId;
                var fileId = data.FileId;

                var project = _editor.Projects.First(p => p.Id == projectId);
                var file = project.GetFiles(project.Root).First(f => f.Id == fileId);

                if (_editor.TryGetFileEditor(file, out FileEditor fileEditor))
                {
                    return file;
                }
            }

            return null;
        }

        public void OpenInEditor(Guid instanceId)
        {
            var graph = GetGraphFile(instanceId);
            if (graph != null)
            {
                _editor.TryOpenFileEditor(graph);
            }
            else
            {
                _output.Write($"Could not open graph {instanceId}. Maybe it's not compiled.", OutputType.Info);
            }
        }
    }
}
