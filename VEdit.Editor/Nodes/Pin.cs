using System;
using System.Collections.Generic;
using System.Linq;
using VEdit.Common;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class Pin : BaseViewModel, ISaveLoad
    {
        public bool IsExec { get; }
        public bool IsInput { get; }
        public bool IsArray { get; }
        public Type Type { get; }
        public string Color { get; }
        public Node Node { get; }
        public Graph Graph => Node.Graph;

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private object _defaultValue;
        public object DefaultValue
        {
            get => _defaultValue;
            set => SetProperty(ref _defaultValue, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            internal set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    Actions.RequeryCanExecute();
                }
            }
        }

        public IEnumerable<Link> Links => Graph.GetLinks(this);

        public bool IsRemovable { get; }
        public bool CanRename { get; }

        private bool _hasWatch;
        public bool HasWatch
        {
            get => _hasWatch;
            private set => SetProperty(ref _hasWatch, value);
        }

        public int Index => IsInput ? Node.Input.IndexOf(this) : Node.Output.IndexOf(this);

        private double _x;
        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private double _y;
        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        private string _error;

        public event Action Loaded;

        public string Error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        public Pin(Node parent, bool isInput, bool isExec, string name = null, Type dataType = null, string color = "#FFFFFF", bool isRemovable = false, bool canBeRenamed = false) : base(parent.ServiceProvider)
        {
            Color = color;
            Name = name;
            Type = dataType;
            Node = parent;

            IsExec = isExec;
            IsInput = isInput;
            IsArray = !IsExec && (Type?.IsArray ?? false);
            IsRemovable = isRemovable;
            CanRename = canBeRenamed;

            var commandProvider = parent.ServiceProvider.Get<ICommandProvider>();

            Actions.Add("Break link(s)", commandProvider.Create(BreakLinks, () => IsConnected));
            Actions.Add($"Select connected node(s)", commandProvider.Create(SelectAllNodes, () => IsConnected));
            Actions.Add($"Jump to connected node(s)", commandProvider.Create(JumpToNodes, () => IsConnected));

            if (!IsExec)
            {
                if (IsInput)
                {
                    Actions.Add("Toggle watch", commandProvider.Create(ToggleWatch));
                    Actions.Add("Promote to variable", commandProvider.Create(PromoteInput));
                }
                else
                {
                    Actions.Add("Promote to variable", commandProvider.Create(PromoteOutput));
                }
            }

            if (CanRename)
            {
                Actions.Add("Rename", commandProvider.Create(Rename));
            }

            if (IsRemovable)
            {
                Actions.Add("Remove", commandProvider.Create(Remove));
            }
        }

        #region Commands

        public void Remove()
        {
            Node.RemovePin(this);
        }

        public async void Rename()
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            Name = await dialog.ShowInputAsync("Rename", "Enter new name for pin");
        }

        public void BreakLinks()
        {
            Graph.BreakLinks(this);
        }

        public void SelectAllNodes()
        {
            var service = Graph.SelectionService;
            var links = Graph.GetLinks(this);
            var nodes = links.Select(l => IsInput ? l.Output.Node : l.Input.Node);

            service.UnselectAll();
            service.SelectRange(nodes);
        }

        private void JumpToNodes()
        {
            var links = Graph.GetLinks(this);
            var nodes = links.Select(l => IsInput ? l.Output.Node : l.Input.Node);

            Graph.Focus(nodes.First());
        }

        public void ToggleWatch()
        {
            if (IsInput && !IsExec)
            {
                HasWatch = !HasWatch;
            }
        }

        private void PromoteOutput()
        {
            var variable = Graph.AddVariable(Type);

            variable.Name = Name;
            variable.X = Node.X + 100;
            variable.Y = Node.Y + 100;

            var set = variable.CreateSetNode();

            Node.TryToConnectPin(set.GetInput());
            Node.TryToConnectPin(set.GetExecIn());
        }

        private void PromoteInput()
        {
            var variable = Graph.AddVariable(Type);

            variable.Name = Name;
            variable.X = Node.X - 100;
            variable.Y = Node.Y - 100;

            var get = variable.CreateGetNode();

            Node.TryToConnectPin(get.GetOutput());
        }

        #endregion

        #region Serialization

        public void Save(IArchive archive)
        {
            archive.Write(nameof(Node), Node.Id);
            archive.Write(nameof(Index), Index);
            archive.Write(nameof(Name), Name);

            if (!IsExec)
            {
                archive.Write(nameof(Type), Type);

                if (IsInput)
                {
                    archive.Write(nameof(DefaultValue), DefaultValue);
                    archive.Write(nameof(HasWatch), HasWatch);
                }
            }
        }

        public void Load(IArchive archive)
        {
            Name = archive.Read<string>(nameof(Name));

            if (!IsExec)
            {
                if (IsInput)
                {
                    HasWatch = archive.Read<bool>(nameof(HasWatch));
                    DefaultValue = archive.Read(nameof(DefaultValue), DefaultValue?.GetType());
                }
            }
        }

        #endregion
    }
}
