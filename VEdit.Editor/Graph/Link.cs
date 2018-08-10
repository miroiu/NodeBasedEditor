using System;
using System.Collections.ObjectModel;
using VEdit.Common;

namespace VEdit.Editor
{
    public class Link : BlackboardElement, ISaveLoad
    {
        private readonly ObservableCollection<SplitNode> _splits;

        public event Action Loaded;

        public Link(Graph graph, Pin input, Pin output) : base(graph, graph.ServiceProvider)
        {
            _splits = new ObservableCollection<SplitNode>();
            Splits = new ReadOnlyObservableCollection<SplitNode>(_splits);

            Input = input;
            Output = output;

            Color = Input.Color;
        }

        public string Color { get; }

        public Pin Input { get; }

        public Pin Output { get; }

        public bool HasSplits => Splits.Count > 0;

        public ReadOnlyObservableCollection<SplitNode> Splits { get; }

        // Don't notify UI
        public override double X { get; set; }

        // Don't notify UI
        public override double Y { get; set; }

        public void Save(IArchive archive)
        {
            var inputArc = ServiceProvider.Get<IArchive>();
            archive.Write(nameof(Input), inputArc);
            Input.Save(inputArc);

            var outputArc = ServiceProvider.Get<IArchive>();
            archive.Write(nameof(Output), outputArc);
            Output.Save(outputArc);
        }

        public void Load(IArchive archive)
        {
            var input = archive.Read<Archive>(nameof(Input));
            Input.Load(input);

            var output = archive.Read<Archive>(nameof(Output));
            Output.Load(output);
        }
    }
}
