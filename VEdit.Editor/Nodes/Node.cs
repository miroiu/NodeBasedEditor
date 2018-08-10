using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VEdit.Common;
using VEdit.Execution;

namespace VEdit.Editor
{
    public abstract class Node : BlackboardElement, ISaveLoad
    {
        public ReadOnlyObservableCollection<Pin> Input { get; }
        public ReadOnlyObservableCollection<Pin> Output { get; }

        private ObservableCollection<Pin> _input;
        private ObservableCollection<Pin> _output;

        protected readonly IPinFactory PinFactory;

        public bool IsFactory { get; private set; }
        public Pin FactoryPin
        {
            set
            {
                if (value != null)
                {
                    AddPinCommand = ServiceProvider.Get<ICommandProvider>().Create(() =>
                    {
                        var dir = value.IsInput ? Direction.Input : Direction.Output;
                        if (value.IsExec)
                        {
                            AddExecPin(dir, isRemovable: true);
                        }
                        else
                        {
                            AddDataPin(dir, value.Type, isRemovable: true);
                        }
                    });
                    Actions.Add("Add pin", AddPinCommand);
                }
            }
        }

        private ICommand AddPinCommand { get; set; }

        // Instance id used for serialization
        public Guid Id { get; internal set; }
        public Guid TemplateId { get; }

        private bool _hasBreakpoint;
        public bool HasBreakpoint
        {
            get => _hasBreakpoint;
            set
            {
                if (SetProperty(ref _hasBreakpoint, value))
                {
                    _breakpointManager.ToggleBreakpoint(this);
                }
            }
        }

        private bool _isBreakpointHit;
        public bool IsBreakpointHit
        {
            get => _isBreakpointHit;
            set => SetProperty(ref _isBreakpointHit, value);
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public IEnumerable<Link> Links => Graph.GetLinks(this);

        public string FullName => $"{Graph.Name}.{Name}";

        public Graph Graph { get; }
        public bool IsExec => Input.Any(p => p.IsExec);
        public bool IsCompact { get; protected set; }
        public bool CanCopy { get; protected set; }

        private IBreakpointManager _breakpointManager;

        public event Action Loaded;

        public Node(Graph root, Guid templateId) : base(root, root.ServiceProvider)
        {
            Id = Guid.NewGuid();
            TemplateId = templateId;
            Graph = root;
            CanCopy = true;

            _input = new ObservableCollection<Pin>();
            _output = new ObservableCollection<Pin>();

            Input = new ReadOnlyObservableCollection<Pin>(_input);
            Output = new ReadOnlyObservableCollection<Pin>(_output);

            PinFactory = ServiceProvider.Get<IPinFactory>();
            _breakpointManager = ServiceProvider.Get<IBreakpointManager>();

            var commandProvider = ServiceProvider.Get<ICommandProvider>();

            Actions.Add("Break links", commandProvider.Create(BreakLinks));
            Actions.Add("Toggle breakpoint", commandProvider.Create(ToggleBreakpoint));
            Actions.Add("Copy (selection)", commandProvider.Create(CopyToClipboard, CanBeDuplicatedOrDeleted));
            Actions.Add("Cut (selection)", commandProvider.Create(CutToClipboard, CanBeDuplicatedOrDeleted));
            Actions.Add("Delete (selection)", commandProvider.Create(Delete, CanBeDuplicatedOrDeleted));
            //Actions.Add("Collapse to function (selection)", commandProvider.Create(CollapseToFunction, CanBeDuplicatedOrDeleted));
            //Actions.Add("Collapse to macro (selection)", commandProvider.Create(CollapseToFunction, CanBeDuplicatedOrDeleted));
            Actions.Add("Create comment (selection)", commandProvider.Create(CreateComment));
        }

        #region Commands

        public void ToggleBreakpoint()
        {
            HasBreakpoint = !HasBreakpoint;
        }

        internal void CreateComment()
        {
            Graph.CommentSelection();
        }

        internal void CollapseToFunction()
        {
            Graph.CollapseSelectionToFunction();
        }

        internal void CollapseToMacro()
        {
            Graph.CollapseSelectionToFunction(true);
        }

        internal void CopyToClipboard()
        {
            Graph.CopySelectionToClipboard();
        }

        internal void CutToClipboard()
        {
            Graph.CopySelectionToClipboard(true);
        }

        private bool CanBeDuplicatedOrDeleted()
        {
            if (Graph.SelectionService.Selection.Count() == 1)
            {
                return !this.IsEntryOrExit();
            }
            return true;
        }

        public virtual void Delete()
        {
            IsSelected = true;
            Graph.DeleteSelection();
        }

        public void BreakLinks()
        {
            Graph.BreakLinks(this);
        }

        #endregion

        public virtual bool TryToConnectPin(Pin pin)
        {
            if (pin.IsInput)
            {
                foreach (var p in Output)
                {
                    if (Graph.TryToConnectPins(p, pin))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var p in Input)
                {
                    if (Graph.TryToConnectPins(p, pin))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region Pin Methods

        public Pin AddExecPin(Direction dir, string name = null, bool isRemovable = false, bool canBeRenamed = false)
        {
            if (dir == Direction.Input)
            {
                var pin = PinFactory.CreateExec(this, dir, name, isRemovable, canBeRenamed);
                _input.Add(pin);
                return pin;
            }
            else
            {
                var pin = PinFactory.CreateExec(this, dir, name, isRemovable, canBeRenamed);
                _output.Add(pin);
                return pin;
            }
        }

        public Pin AddDataPin<T>(Direction dir, string name = null, bool isRemovable = false, bool canBeRenamed = false)
        {
            return AddDataPin(dir, typeof(T), name, isRemovable, canBeRenamed);
        }

        public Pin AddDataPin(Direction dir, Type dataType, string name = null, bool isRemovable = false, bool canBeRenamed = false)
        {
            if (dir == Direction.Input)
            {
                var pin = PinFactory.CreateData(this, dir, dataType, name, isRemovable, canBeRenamed);
                _input.Add(pin);
                return pin;
            }
            else
            {
                var pin = PinFactory.CreateData(this, dir, dataType, name, isRemovable, canBeRenamed);
                _output.Add(pin);
                return pin;
            }
        }

        public virtual Pin GetPassThroughPin() => Output.Where(p => p.IsExec).ToList().Count == 1 ? Output[0] : null;

        public bool RemovePin(Pin pin)
        {
            if (pin.IsRemovable)
            {
                if (_output.Remove(pin) || _input.Remove(pin))
                {
                    pin.BreakLinks();
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Serialization

        public virtual void Save(IArchive archive)
        {
            archive.Write(nameof(X), X);
            archive.Write(nameof(Y), Y);

            archive.Write(nameof(Width), Width);
            archive.Write(nameof(Height), Height);

            archive.Write(nameof(Name), Name);
            archive.Write(nameof(Description), Description);

            archive.Write(nameof(Id), Id);
            archive.Write(nameof(TemplateId), TemplateId);
            archive.Write(nameof(HasBreakpoint), HasBreakpoint);

            archive.Write(nameof(Input), SavePins(Input));
            archive.Write(nameof(Output), SavePins(Output));
        }

        public virtual void Load(IArchive archive)
        {
            X = archive.Read<double>(nameof(X));
            Y = archive.Read<double>(nameof(Y));

            Width = archive.Read<double>(nameof(Width));
            Height = archive.Read<double>(nameof(Height));

            Name = archive.Read<string>(nameof(Name));
            Description = archive.Read<string>(nameof(Description));

            Id = archive.Read<Guid>(nameof(Id));
            HasBreakpoint = archive.Read<bool>(nameof(HasBreakpoint));

            LoadPins(archive.Read<Archive>(nameof(Input)), true);
            LoadPins(archive.Read<Archive>(nameof(Output)), false);
        }

        public virtual IArchive SavePins(IEnumerable<Pin> pins)
        {
            var pinArchive = ServiceProvider.Get<IArchive>();
            foreach (var pin in pins)
            {
                var arch = ServiceProvider.Get<IArchive>();
                pin.Save(arch);
                pinArchive.Write(pin.GetHashCode().ToString(), arch);
            }
            return pinArchive;
        }

        public virtual void LoadPins(IArchive pinsArchive, bool input)
        {
            foreach (var pin in pinsArchive.ReadAll<Archive>())
            {
                var type = pin.Read<Type>(nameof(Pin.Type));
                var index = pin.Read<int>(nameof(Pin.Index));
                if (input)
                {
                    if (Input.Count > index)
                    {
                        Input[index].Load(pin);
                    }
                    else
                    {
                        if(type != null)
                        {
                            var result = AddDataPin(Direction.Input, type, isRemovable: true);
                            result.Load(pin);
                        }
                        else
                        {
                            var result = AddExecPin(Direction.Input, isRemovable: true);
                            result.Load(pin);
                        }
                    }
                }
                else
                {
                    if (Output.Count > index)
                    {
                        Output[index].Load(pin);
                    }
                    else
                    {
                        if (type != null)
                        {
                            var result = AddDataPin(Direction.Output, type, isRemovable: true);
                            result.Load(pin);
                        }
                        else
                        {
                            var result = AddExecPin(Direction.Output, isRemovable: true);
                            result.Load(pin);
                        }
                    }
                }
            }
        }

        #endregion

        public abstract IExecutionBlock Compile(IExecutionContext context);
    }
}
