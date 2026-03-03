using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Vis.Modules
{
    internal static class DialogManagerSyncExtensions
    {
        public static void MsgBox(this IDialogManager dialogManager, string title, string promptText)
        {
            if (dialogManager == null) return;
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
