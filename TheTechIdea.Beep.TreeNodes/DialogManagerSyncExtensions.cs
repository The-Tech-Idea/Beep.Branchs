using System.Collections.Generic;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Vis.Modules
{
    /// <summary>
    /// Synchronous convenience wrappers around <see cref="IDialogManager"/>.
    /// 
    /// These are safe to call from the UI thread because every underlying
    /// async method returns an already-completed <see cref="Task"/> when
    /// invoked on the UI thread (no SynchronizationContext continuation
    /// is needed).
    /// </summary>
    internal static class DialogManagerSyncExtensions
    {
        public static void MsgBox(this IDialogManager dialogManager, string title, string promptText)
        {
            if (dialogManager == null) return;
            // Task is already completed when called on UI thread → GetResult() returns immediately.
            dialogManager.MsgBoxAsync(title, promptText).GetAwaiter().GetResult();
        }

        public static DialogReturn InputBox(this IDialogManager dialogManager, string title, string promptText)
        {
            if (dialogManager == null) return new DialogReturn { Result = BeepDialogResult.Cancel };
            return dialogManager.InputBoxAsync(title, promptText).GetAwaiter().GetResult();
        }

        public static DialogReturn InputBox(this IDialogManager dialogManager, string title, string promptText, string initialValue)
        {
            // Current async contract has no initial-value overload; fallback to standard input box.
            return dialogManager.InputBox(title, promptText);
        }

        public static DialogReturn InputBoxYesNo(this IDialogManager dialogManager, string title, string promptText)
        {
            if (dialogManager == null) return new DialogReturn { Result = BeepDialogResult.Cancel };
            return dialogManager.InputBoxYesNoAsync(title, promptText).GetAwaiter().GetResult();
        }

        public static DialogReturn LoadFileDialog(this IDialogManager dialogManager, string exts, string dir, string filter)
        {
            if (dialogManager == null) return new DialogReturn { Result = BeepDialogResult.Cancel };
            return dialogManager.LoadFileDialogAsync(exts, dir, filter).GetAwaiter().GetResult();
        }

        public static List<string> LoadFilesDialog(this IDialogManager dialogManager, string exts, string dir, string filter)
        {
            if (dialogManager == null) return new List<string>();
            return dialogManager.LoadFilesDialogAsync(exts, dir, filter).GetAwaiter().GetResult() ?? new List<string>();
        }

        public static DialogReturn SelectFolderDialog(this IDialogManager dialogManager)
        {
            if (dialogManager == null) return new DialogReturn { Result = BeepDialogResult.Cancel };
            return dialogManager.SelectFolderDialogAsync().GetAwaiter().GetResult();
        }
    }
}
