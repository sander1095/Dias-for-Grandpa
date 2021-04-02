using DiasForGrandpa.WPF.Exceptions;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DiasForGrandpa.WPF.Helpers
{
    public static class ErrorDialog
    {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShowError(string message)
        {
            throw new ErrorDialogException(message);
        }
    }
}
