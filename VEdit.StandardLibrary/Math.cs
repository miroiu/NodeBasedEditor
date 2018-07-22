using System.Linq;
using VEdit.Plugins;

namespace VEdit.StandardLibrary
{
    [ContainsNodes]
    public static class Math
    {
        #region Integer

        [PureNode(Category = "Math/Integer", Keywords = "add + plus", DisplayName = "int + int", CompactNodeTitle = "+")]
        public static int AddInt([GenerateArray(2)] params int[] numbers) => numbers.Sum();

        [PureNode(Category = "Math/Integer", Keywords = "substract - minus", DisplayName = "int - int", CompactNodeTitle = "-")]
        public static int SubstractInt(int _x, int _y) => _x - _y;

        [PureNode(Category = "Math/Integer", Keywords = "divide", DisplayName = "int / int", CompactNodeTitle = "/")]
        public static int DivideInt(int _x, int _y) => _x / _y;

        [PureNode(Category = "Math/Integer", Keywords = "multiply *", DisplayName = "int * int", CompactNodeTitle = "*")]
        public static int MultiplyInt([GenerateArray(2)] params int[] numbers) => numbers.Aggregate((a, b) => a * b);

        [PureNode(Category = "Math/Integer", Keywords = "modulo", DisplayName = "int % int", CompactNodeTitle = "%")]
        public static int ModuloInt(int _x, int _y) => _x % _y;

        [PureNode(Category = "Math/Integer", Keywords = "sqrt square root", DisplayName = "sqrt int", CompactNodeTitle = "√n")]
        public static double SqrtInt(int _x) => System.Math.Sqrt(_x);

        #endregion

        #region Double

        [PureNode(Category = "Math/Double", Keywords = "sqrt square root", DisplayName = "sqrt double", CompactNodeTitle = "√n")]
        public static double SqrtDouble(double _x) => System.Math.Sqrt(_x);

        [PureNode(Category = "Math/Double", Keywords = "add + plus", DisplayName = "double + double", CompactNodeTitle = "+")]
        public static double AddDouble([GenerateArray(2)] params double[] numbers) => numbers.Sum();

        [PureNode(Category = "Math/Double", Keywords = "substract - minus", DisplayName = "double - double", CompactNodeTitle = "-")]
        public static double SubstractDouble(double _x, double _y) => _x - _y;

        [PureNode(Category = "Math/Double", Keywords = "multiply *", DisplayName = "double * double", CompactNodeTitle = "*")]
        public static double MultiplyDouble([GenerateArray(2)] params double[] numbers) => numbers.Aggregate((a, b) => a * b);

        [PureNode(Category = "Math/Double", Keywords = "divide", DisplayName = "double / double", CompactNodeTitle = "/")]
        public static double DivideDouble(double _x, double _y) => _x / _y;

        [PureNode(Category = "Math/Double", Keywords = "clamp")]
        public static double Clamp(double value, double min, double max) => value > max ? max : value < min ? min : value;

        [PureNode(Category = "Math/Double", Keywords = "clamp", DisplayName = "Clamp (threshold)")]
        public static double Clamp(double value, double min, double max, double threshold) => value > threshold ? max : min;

        #endregion

        #region Boolean

        #region Equality

        [PureNode(Category = "Math/Boolean/Equality", Keywords = "equals == bool", DisplayName = "bool == bool", CompactNodeTitle = "==")]
        public static bool Equals(bool a, bool b) => a == b;

        [PureNode(Category = "Math/Boolean/Equality", Keywords = "equals == int", DisplayName = "int == int", CompactNodeTitle = "==")]
        public static bool Equals(int a, int b) => a == b;

        [PureNode(Category = "Math/Boolean/Equality", Keywords = "equals == double", DisplayName = "double == double", CompactNodeTitle = "==")]
        public static bool Equals(double a, double b) => a == b;

        [PureNode(Category = "Math/Boolean/Equality", Keywords = "not equals != bool", DisplayName = "bool != bool", CompactNodeTitle = "!=")]
        public static bool NotEquals(bool a, bool b) => a != b;

        [PureNode(Category = "Math/Boolean/Equality", Keywords = "not equals != int", DisplayName = "int != int", CompactNodeTitle = "!=")]
        public static bool NotEquals(int a, int b) => a != b;

        [PureNode(Category = "Math/Boolean/Equality", Keywords = "not equals != double", DisplayName = "double != double", CompactNodeTitle = "!=")]
        public static bool NotEquals(double a, double b) => a != b;

        #endregion

        #region Inequality

        [PureNode(Category = "Math/Boolean/Inequality", Keywords = "less < int", DisplayName = "int < int", CompactNodeTitle = "<")]
        public static bool Less(int a, int b) => a < b;

        [PureNode(Category = "Math/Boolean/Inequality", Keywords = "less < double", DisplayName = "double < double", CompactNodeTitle = "<")]
        public static bool Less(double a, double b) => a < b;

        #endregion

        #endregion
    }
}
