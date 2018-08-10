using System.Collections.Generic;
using VEdit.Common;

namespace VEdit.Editor
{
    public interface IBreakpointManager
    {
        Breakpoints Breakpoints { get; }

        void ToggleBreakpoint(Node node);
        void DeleteAll();
        void Delete(Breakpoint brk);
    }

    public class BreakpointManager : IBreakpointManager
    {
        public Breakpoints Breakpoints { get; }

        private Dictionary<Node, Breakpoint> _breakpoints = new Dictionary<Node, Breakpoint>();
        private HashSet<System.Guid> _added = new HashSet<System.Guid>();
        private ICommandProvider _cmdProvider;
        private IServiceProvider _provider;

        public BreakpointManager(IServiceProvider service)
        {
            Breakpoints = new Breakpoints();

            _cmdProvider = service.Get<ICommandProvider>();
            _provider = service;
        }

        public void ToggleBreakpoint(Node node)
        {
            if (node.HasBreakpoint)
            {
                if (!_breakpoints.ContainsKey(node) && _added.Add(node.Id))
                {
                    var brk = new Breakpoint(node, _cmdProvider.Create(() =>
                    {
                        var graphProvider = _provider.Get<IGraphProvider>();

                        graphProvider.OpenInEditor(node.Graph.Id);
                        node.Graph.Focus(node);
                    }));

                    Breakpoints.Add(brk);
                    _breakpoints.Add(node, brk);
                }
            }
            else
            {
                if(_breakpoints.TryGetValue(node, out Breakpoint brk))
                {
                    Breakpoints.Remove(brk);
                    _breakpoints.Remove(node);
                }
            }
        }

        public void DeleteAll()
        {
            foreach(var node in _breakpoints.Keys)
            {
                node.HasBreakpoint = false;
            }
        }

        public void Delete(Breakpoint brk)
        {
            brk.Node.HasBreakpoint = false;
        }
    }
}
