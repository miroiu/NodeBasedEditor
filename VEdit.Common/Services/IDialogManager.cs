using System;
using System.Threading.Tasks;

namespace VEdit.Common
{
    public enum DialogResult
    {
        Canceled = -1,
        Negative = 0,
        Affirmative = 1,
    }

    public enum DialogStyle
    {
        Affirmative = 0,
        AffirmativeAndNegative = 1,
        AffirmativeNegativeAndCancel = 2,
    }

    public class DialogSettings
    {
        public string AffirmativeButtonText { get; set; }
        public string NegativeButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public DialogResult DefaultButtonFocus { get; set; }
    }

    public interface IDialogManager
    {
        Task<DialogResult> ShowMessageAsync(string title, string message, DialogStyle style = DialogStyle.Affirmative, DialogSettings dialogSettings = null);
        Task<string> ShowInputAsync(string title, string message);
        Task<IProgressController> ShowProgress(string title, string message, bool isCancelable = false, bool indeterminate = true);
    }

    public interface IProgressController
    {
        bool IsCanceled { get; }
        bool IsOpen { get; }
        Task Close();
        event EventHandler Closed;
        event EventHandler Canceled;
        void SetMessage(string message);
        void SetProgress(double value);
    }
}
