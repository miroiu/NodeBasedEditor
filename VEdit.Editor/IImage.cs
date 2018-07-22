namespace VEdit.Editor
{
    public interface IImage
    {
        double[] Red { get; set; }
        double[] Green { get; set; }
        double[] Blue { get; set; }
        double[] Alpha { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        bool TrySetBytes(byte[] bytes);
        bool TryGetBytes(out byte[] bytes);
    }
}
