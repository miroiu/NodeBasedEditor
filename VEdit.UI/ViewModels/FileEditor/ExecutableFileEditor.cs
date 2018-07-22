using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VEdit.Common;
using VEdit.Editor;
using VEdit.Execution;

namespace VEdit.UI
{
    public abstract class ExecutableFileEditor : FileEditor
    {
        private IActionsDatabase _actionsDb;
        private INodeDatabase _nodesDb;

        private IDialogManager _dialog;

        private ObservableCollection<ActionEntry> _actions;
        public ObservableCollection<ActionEntry> AvailableActions
        {
            get => _actions;
            private set => SetProperty(ref _actions, value);
        }

        private ActionEntry _action;
        public ActionEntry SelectedAction
        {
            get => _action;
            set => SetProperty(ref _action, value);
        }

        public ICommand ExecuteCommand { get; }

        public ExecutableFileEditor(ProjectFile file, Common.IServiceProvider serviceProvider) : base(file, serviceProvider)
        {
            _actionsDb = ServiceProvider.Get<IActionsDatabase>();
            _nodesDb = ServiceProvider.Get<INodeDatabase>();
            _actionsDb.DatabaseChanged += GetActions;

            _dialog = ServiceProvider.Get<IDialogManager>();

            var cmd = ServiceProvider.Get<ICommandProvider>();
            ExecuteCommand = cmd.Create(Execute, () => SelectedAction != null);

            GetActions();
        }

        private void GetActions()
        {
            var nodes = _nodesDb.GraphNodes.Where(n => CanExecuteNode(n));
            var ids = nodes.Select(n => n.TemplateId);

            var result = _actionsDb.ApplyFilter(a => a.TemplateId.In(ids), File.Project.Id);
            AvailableActions = new ObservableCollection<ActionEntry>(result);

            SelectedAction = AvailableActions.FirstOrDefault();

            ExecuteCommand.RaiseCanExecuteChanged();
        }

        public abstract bool CanExecuteNode(GraphNodeEntry data);

        public async void Execute()
        {
            var output = ServiceProvider.Get<IOutputManager>();
            var dialog = ServiceProvider.Get<IDialogManager>();

            var graphFactory = ServiceProvider.Get<IGraphFactory>();
            var graph = graphFactory.Create<FunctionGraph>();
            var node = new NodeBuilder(graph, new NodeBuilder.Configuration(ServiceProvider, SelectedAction.TemplateId)).Build();
            graph.AddNode(node);

            {
                var block = await Compile(node);

                OnBeforeExecute(block);

                var result = await Execute(block);

                if (result != null)
                {
                    output.Write(result.Message, OutputType.Error);
                }
                else
                {
                    OnAfterExecute(block);
                }
            }
        }

        private Task<IExecutionBlock> Compile(Node node) =>
            Task.Run(async () =>
            {
                var progress = await _dialog.ShowProgress("Task 1/2 (Compiling)", "Please wait...");

                var result = node.Compile(null);

                await progress.Close();
                return result;
            });

        private Task<Exception> Execute(IExecutionBlock block) =>
            Task.Run(async () =>
            {
                var progress = await _dialog.ShowProgress("Task 2/2 (Executing)", "Please wait...");

                Exception result = null;
                try
                {
                    block.Execute();
                }
                catch (Exception ex)
                {
                    result = ex;
                }

                await progress.Close();
                return result;
            });

        public abstract void OnAfterExecute(IExecutionBlock block);

        public abstract void OnBeforeExecute(IExecutionBlock block);
    }
}
