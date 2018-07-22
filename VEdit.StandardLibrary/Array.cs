using VEdit.Plugins;

namespace VEdit.StandardLibrary
{
    [ContainsNodes]
    public static class Array
    {
        #region Integer

        [PureNode(Category = "Array|Integer", DisplayName ="Get Element", CompactNodeTitle = "GET", Keywords = "get array")]
        public static int Get(int[] array, int index) => array[index];

        [PureNode(Category = "Array|Integer", DisplayName = "Length", CompactNodeTitle = "LEN", Keywords = "length array")]
        public static int Length(int[] array) => array.Length;

        #endregion

        #region Double

        [PureNode(Category = "Array|Double", DisplayName = "Length", CompactNodeTitle = "LEN", Keywords = "length array")]
        public static int Length(double[] array) => array.Length;

        [PureNode(Category = "Array|Double", DisplayName = "Get Element", CompactNodeTitle = "GET", Keywords = "get array")]
        public static double GetElement(double[] array, int index) => array[index];

        [ExecutableNode(Category = "Array|Double", DisplayName ="Set Element", Keywords = "set array", ReturnName = "Value")]
        public static double SetElement(double[] array, int index, double value) => array[index] = value;

        #endregion
    }
}
