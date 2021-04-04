using System;

namespace DiasForGrandpa.WPF.Exceptions
{
    /// <summary>
    /// Used to show error dialogs and to avoid that a global exception handler 
    /// would swallow the exception and thus not display the error dialog
    /// </summary>
    public class ErrorDialogException : ApplicationException
    {
        public ErrorDialogException()
        {
        }

        public ErrorDialogException(string message) : base(message)
        {
        }
    }
}
