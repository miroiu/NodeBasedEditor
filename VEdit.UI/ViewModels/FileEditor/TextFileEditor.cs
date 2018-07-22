using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VEdit.Common;
using VEdit.Editor;
using VEdit.Execution;

namespace VEdit.UI
{
    public class TextFileEditor : ExecutableFileEditor
    {
        private string _content;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public List<double> PossibleFonts { get; } = new List<double> { 10, 12, 14, 16, 18, 20, 24 };

        private double _fontSize;
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        public TextFileEditor(ProjectFile file, IServiceProvider serviceProvider) : base(file, serviceProvider)
        {
            FontSize = PossibleFonts[2];

            var commandProvider = serviceProvider.Get<ICommandProvider>();
            SaveCommand = commandProvider.CreateAsync(SaveContentAsync, CanSave);
        }

        public ICommand SaveCommand { get; }

        private async Task SaveContentAsync()
        {
            await Task.Run(() => SaveContent());
        }

        private bool CanSave()
        {
            return System.IO.File.Exists(Path);
        }

        public override bool LoadContent()
        {
            var io = ServiceProvider.Get<IFileIO>();
            Content = io.Read(Path);
            return true;
        }

        public override void SaveContent()
        {
            var io = ServiceProvider.Get<IFileIO>();
            io.Write(Path, Content ?? string.Empty);
        }

        public override bool CanExecuteNode(GraphNodeEntry data)
        {
            var inputData = data.Input.Where(i => i.Type == PinType.Data).ToList();
            var outputData = data.Output.Where(i => i.Type == PinType.Data).ToList();

            return inputData.Count == 1 && inputData[0].DataType == typeof(string) &&
                outputData.Count == 1 && outputData[0].DataType == typeof(string);
        }

        public override void OnAfterExecute(IExecutionBlock block)
        {
            Content = block.Out[0].Value as string;
        }

        public override void OnBeforeExecute(IExecutionBlock block)
        {
            block.In[0].Value = Content;
        }
    }
}
