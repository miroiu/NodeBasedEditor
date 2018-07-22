using System.Linq;
using VEdit.Common;
using VEdit.Editor;
using VEdit.Execution;

namespace VEdit.UI
{
    public class ImageFileEditor : ExecutableFileEditor
    {
        private int _scale;
        public int Scale
        {
            get => _scale;
            set
            {
                SetProperty(ref _scale, value);
                ScalePercent = (double)value / MaxScale;
            }
        }

        private double _scalePercent;
        public double ScalePercent
        {
            get => _scalePercent;
            set => SetProperty(ref _scalePercent, value * ScaleMultiplier);
        }

        public int ScaleInterval { get; }
        public int MaxScale { get; }
        public double ScaleMultiplier { get; }

        public ImageFileEditor(ProjectFile file, IServiceProvider serviceProvider) : base(file, serviceProvider)
        {
            MaxScale = 10;
            ScaleMultiplier = MaxScale / 2;
            ScaleInterval = 1;
            Scale = 2;

            Content = ServiceProvider.Get<IImage>();
        }

        private byte[] _bytes;
        public byte[] Bytes
        {
            get => _bytes;
            set => SetProperty(ref _bytes, value);
        }

        public IImage Content { get; }

        public override bool LoadContent()
        {
            Bytes = System.IO.File.ReadAllBytes(Path);
            return Content.TrySetBytes(Bytes);
        }

        public override void SaveContent()
        {
            System.IO.File.WriteAllBytes(Path, Bytes);
        }

        public override bool CanExecuteNode(GraphNodeEntry data)
        {
            var inputData = data.Input.Where(i => i.Type == PinType.Data).ToList();
            var outputData = data.Output.Where(i => i.Type == PinType.Data).ToList();

            return inputData.Count == 5 &&
                outputData.Count == 5 &&
                inputData[0].DataType == typeof(double[]) &&
                inputData[1].DataType == typeof(double[]) &&
                inputData[2].DataType == typeof(double[]) &&
                inputData[3].DataType == typeof(int) &&
                inputData[4].DataType == typeof(int) &&
                outputData[0].DataType == typeof(double[]) &&
                outputData[1].DataType == typeof(double[]) &&
                outputData[2].DataType == typeof(double[]) &&
                outputData[3].DataType == typeof(int) &&
                outputData[4].DataType == typeof(int);
        }

        public override void OnAfterExecute(IExecutionBlock block)
        {
            Content.Red = block.Out[0].Value as double[];
            Content.Green = block.Out[1].Value as double[];
            Content.Blue = block.Out[2].Value as double[];
            Content.Width = (int)block.Out[3].Value;
            Content.Height = (int)block.Out[4].Value;
            
            if(!Content.TryGetBytes(out byte[] bytes))
            {
                var output = ServiceProvider.Get<IOutputManager>();
                output.Write("Image bytes are not valid.", OutputType.Warning);
            }
            else
            {
                Bytes = bytes;
            }
        }

        public override void OnBeforeExecute(IExecutionBlock block)
        {
            // TODO: This should be enabled here and removed from LoadContent
            //Content.Load(Bytes);

            block.In[0].Value = Content.Red;
            block.In[1].Value = Content.Green;
            block.In[2].Value = Content.Blue;
            block.In[3].Value = Content.Width;
            block.In[4].Value = Content.Height;
        }
    }
}
