using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VEdit.Common
{
    public interface IFileIO
    {
        void Write(string file, string content);
        void Append(string file, string content);
        string Read(string file);

        Task WriteAsync(string file, string content);
        Task<string> ReadAsync(string file);
    }

    public class FileIO : IFileIO
    {
        public string Read(string file)
        {
            byte[] result;
            using (var stream = File.Open(file, FileMode.Open))
            {
                result = new byte[stream.Length];
                stream.Read(result, 0, (int)stream.Length);
            }
            return Encoding.UTF8.GetString(result);
        }

        public async Task<string> ReadAsync(string file)
        {
            byte[] result;
            using (var stream = File.Open(file, FileMode.Open))
            {
                result = new byte[stream.Length];
                await stream.ReadAsync(result, 0, (int)stream.Length);
            }
            return Encoding.UTF8.GetString(result);
        }

        public void Write(string file, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using (var stream = File.Open(file, FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public async Task WriteAsync(string file, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using (var stream = File.Open(file, FileMode.Create))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public void Append(string file, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using (var stream = File.Open(file, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
