using Metro = MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using VEdit.UI;
using VEdit.Common;
using System;

namespace VEdit.Dialogs
{
    public class DialogManager : IDialogManager
    {
        private readonly Metro.IDialogCoordinator _dialog;
        private readonly EditorViewModel _context;

        public DialogManager(Common.IServiceProvider provider)
        {
            _dialog = Metro.DialogCoordinator.Instance;
            _context = provider.Get<EditorViewModel>();
        }

        public Task<string> ShowInputAsync(string title, string message)
        {
            return _dialog.ShowInputAsync(_context, title, message);
        }

        public async Task<DialogResult> ShowMessageAsync(string title, string message, DialogStyle style = DialogStyle.Affirmative, DialogSettings settings = null)
        {
            var metroStyle = Metro.MessageDialogStyle.Affirmative;

            if (style == DialogStyle.AffirmativeAndNegative)
            {
                metroStyle = Metro.MessageDialogStyle.AffirmativeAndNegative;
            }
            else
            {
                metroStyle = Metro.MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary;
            }

            var defaultButtonFocus = settings?.DefaultButtonFocus.ToMetro() ?? Metro.MessageDialogResult.Affirmative;
            defaultButtonFocus = defaultButtonFocus == Metro.MessageDialogResult.Canceled ? Metro.MessageDialogResult.FirstAuxiliary : defaultButtonFocus;

            var result = await _dialog.ShowMessageAsync(_context, title, message, metroStyle, new Metro.MetroDialogSettings
            {
                FirstAuxiliaryButtonText = settings?.CancelButtonText ?? "Cancel",
                AffirmativeButtonText= settings?.AffirmativeButtonText ?? "Ok",
                NegativeButtonText = settings?.NegativeButtonText ?? "No",
                DefaultButtonFocus = defaultButtonFocus,
                AnimateHide = false
            });

            return result.ToAdapter();
        }

        public async Task<IProgressController> ShowProgress(string title, string message, bool isCancelable = false, bool indeterminate = true)
        {
            var result = await _dialog.ShowProgressAsync(_context, title, message, isCancelable, new Metro.MetroDialogSettings
            {
                AnimateShow = false,
                AnimateHide = false
            });

            if (indeterminate)
            {
                result.SetIndeterminate();
            }
            return new ProgressControllerAdapter(result);
        }
    }

    public class ProgressControllerAdapter : IProgressController
    {
        private Metro.ProgressDialogController _controller;

        public ProgressControllerAdapter(Metro.ProgressDialogController ctrler)
        {
            _controller = ctrler;
            _controller.Canceled += Canceled;
            _controller.Closed += Closed;
        }

        public bool IsCanceled { get; }

        public bool IsOpen { get; }

        public event EventHandler Closed;
        public event EventHandler Canceled;

        public Task Close()
        {
            return _controller.CloseAsync();
        }

        public void SetMessage(string message)
        {
            _controller.SetMessage(message);
        }

        public void SetProgress(double value)
        {
            _controller.SetProgress(value);
        }
    }

    public static class DialogManagerExtensions
    {
        public static Metro.MessageDialogResult ToMetro(this DialogResult result)
        {
            switch (result)
            {
                case DialogResult.Canceled:
                    return Metro.MessageDialogResult.Canceled;
                case DialogResult.Negative:
                    return Metro.MessageDialogResult.Negative;
                case DialogResult.Affirmative:
                    return Metro.MessageDialogResult.Affirmative;
                default:
                    return Metro.MessageDialogResult.Affirmative;
            }
        }

        public static DialogResult ToAdapter(this Metro.MessageDialogResult result)
        {
            switch (result)
            {
                case Metro.MessageDialogResult.Canceled:
                    return DialogResult.Canceled;
                case Metro.MessageDialogResult.Negative:
                    return DialogResult.Negative;
                case Metro.MessageDialogResult.Affirmative:
                    return DialogResult.Affirmative;
                case Metro.MessageDialogResult.FirstAuxiliary:
                    return DialogResult.Canceled;
                default:
                    return DialogResult.Canceled;
            }
        }
    }
}