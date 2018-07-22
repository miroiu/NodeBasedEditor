using VEdit.Plugins;

namespace VEdit.StandardLibrary
{
    [ContainsNodes]
    public static class Image
    {
        [ExecutableNode(Category = "Image|Algorithm", Keywords = "grayscale algorithm image")]
        public static void GrayScale(double[] red, double[] green, double[] blue, out double[] Red, out double[] Green, out double[] Blue)
        {
            var len = red.Length;
            Red = new double[len];
            Green = new double[len];
            Blue = new double[len];

            for (int i = 0; i < len; i++)
            {
                var sum = red[i] * 0.21 + green[i] * 0.72 + blue[i] * 0.07;
                Red[i] = Green[i] = Blue[i] = sum;
            }
        }

        [ExecutableNode(Category = "Image|Algorithm", Keywords = "binary algorithm image")]
        public static void Binary(double[] red, double[] green, double[] blue, double threshold, out double[] Red, out double[] Green, out double[] Blue)
        {
            var len = red.Length;
            Red = new double[len];
            Green = new double[len];
            Blue = new double[len];

            for (int i = 0; i < len; i++)
            {
                Red[i] = Math.Clamp(red[i], 0, 1, threshold);
                Green[i] = Math.Clamp(green[i], 0, 1, threshold);
                Blue[i] = Math.Clamp(blue[i], 0, 1, threshold);
            }
        }

        [ExecutableNode(Category = "Image|Algorithm", Keywords = "negative algorithm image")]
        public static void Negative(double[] red, double[] green, double[] blue, out double[] Red, out double[] Green, out double[] Blue)
        {
            var len = red.Length;
            Red = new double[len];
            Green = new double[len];
            Blue = new double[len];

            for (int i = 0; i < len; i++)
            {
                Red[i] = 1 - red[i];
                Green[i] = 1 - green[i];
                Blue[i] = 1 - blue[i];
            }
        }
    }
}
