using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VEdit.Editor;

namespace VEdit
{
    public class ImageAdapter : IImage
    {
        public double[] Red { get; set; }
        public double[] Green { get; set; }
        public double[] Blue { get; set; }
        public double[] Alpha { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private int _numChannels = 4;

        public bool TrySetBytes(byte[] data)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(data);
                image.EndInit();

                Width = image.PixelWidth;
                Height = image.PixelHeight;

                int stride = Width * _numChannels;          // pixels per row (image width * RGBA (4 bytes used for a pixel) )
                int size = Height * stride;
                var bytes = new byte[size];
                image.CopyPixels(bytes, stride, 0);

                Red = new double[bytes.Length / _numChannels];
                Green = new double[bytes.Length / _numChannels];
                Blue = new double[bytes.Length / _numChannels];
                Alpha = new double[bytes.Length / _numChannels];

                for (int i = 0, j = 0; i < bytes.Length; i += _numChannels, j++)
                {
                    Blue[j] = bytes[i] / 255.0;
                    Green[j] = bytes[i + 1] / 255.0;
                    Red[j] = bytes[i + 2] / 255.0;
                    Alpha[j] = bytes[i + 3] / 255.0;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryGetBytes(out byte[] bytes)
        {
            bytes = new byte[Width * Height * _numChannels];

            for (int i = 0, j = 0; i < bytes.Length - (_numChannels - 1); i += _numChannels, j++)
            {
                bytes[i] = Clamp(Blue[j] * 255, 0, 255);
                bytes[i + 1] = Clamp(Green[j] * 255, 0, 255);
                bytes[i + 2] = Clamp(Red[j] * 255, 0, 255);
                bytes[i + 3] = Clamp(Alpha[j] * 255, 0, 255);
            }

            try
            {
                var bs = BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgra32, null, bytes, Width * _numChannels);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bs));

                using (var mem = new MemoryStream())
                {
                    encoder.Save(mem);
                    bytes = mem.GetBuffer();
                }

                return true;
            }
            catch
            {
                return false;
            }

            byte Clamp(double value, byte min, byte max)
            {
                return value > max ? max : value < min ? min : (byte)value;
            }
        }
    }
}
