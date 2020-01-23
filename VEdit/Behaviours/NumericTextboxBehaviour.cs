using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace VEdit
{
    public abstract class NumericTextboxBehaviour<T> : Behavior<TextBox>
    {
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register(nameof(Output),
               typeof(T), typeof(NumericTextboxBehaviour<T>), new PropertyMetadata(default(T)));

        public string Text
        {
            get => AssociatedObject.Text;
            set => AssociatedObject.Text = value;
        }

        public T Output
        {
            get => (T)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        protected abstract string CharacterRegex { get; }
        protected abstract string TextRegex { get; }
        protected abstract T Convert(string input);

        protected override void OnAttached()
        {
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            AssociatedObject.TextChanged += OnTextChanged;
            AssociatedObject.LostFocus += OnLostFocus;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            Output = Convert(Text);
            Text = Output.ToString();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = !Regex.IsMatch(Text, TextRegex);
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, CharacterRegex);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            AssociatedObject.TextChanged -= OnTextChanged;
        }
    }

    public class IntegerBehaviour : NumericTextboxBehaviour<int>
    {
        protected override string CharacterRegex => @"[0-9]|\-";
        protected override string TextRegex => "[0-9]";

        protected override int Convert(string input)
        {
            if (int.TryParse(input, out int value))
            {
                return value;
            }
            return default(int);
        }
    }

    public class DoubleBehaviour : NumericTextboxBehaviour<double>
    {
        protected override string CharacterRegex => @"[0-9]|\.|\-";
        protected override string TextRegex => @"-?[0-9]+(\.[0-9]+)?";

        protected override double Convert(string input)
        {
            if (double.TryParse(input, out double value))
            {
                return value;
            }
            return default(double);
        }
    }

    public class FloatBehaviour : NumericTextboxBehaviour<float>
    {
        protected override string CharacterRegex => @"[0-9]|\.|\-";
        protected override string TextRegex => @"-?[0-9]+(\.[0-9]+)?";

        protected override float Convert(string input)
        {
            if (float.TryParse(input, out float value))
            {
                return value;
            }
            return default(float);
        }
    }

    public class ByteBehaviour : NumericTextboxBehaviour<byte>
    {
        protected override string CharacterRegex => @"[0-9]|\-";
        protected override string TextRegex => @"[0-2]\d{0,2}";

        protected override byte Convert(string input)
        {
            if (byte.TryParse(input, out byte value))
            {
                return value;
            }
            return default(byte);
        }
    }
}
