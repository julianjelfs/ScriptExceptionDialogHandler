using System.Windows.Automation;
using NUnit.Framework;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Native.Windows;

namespace WatiNExtensions
{
    /// <summary>
    /// This is a general purpose dialog handler for unhandled javascript exceptions
    /// It will extract the error message and throw an exception
    /// </summary>
    public class ScriptExceptionDialogHandler : BaseDialogHandler
    {
        readonly AndCondition _documentCondition = new AndCondition(new PropertyCondition(AutomationElement.IsEnabledProperty, true),
                                                                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));

        readonly AndCondition _buttonConditions = new AndCondition(new PropertyCondition(AutomationElement.IsEnabledProperty, true),
                                                                   new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));


        public override bool HandleDialog(Window window)
        {
            if (CanHandleDialog(window))
            {
                var win = AutomationElement.FromHandle(window.Hwnd);
                var documents = win.FindAll(TreeScope.Children, _documentCondition);
                var buttons = win.FindAll(TreeScope.Children, _buttonConditions);

                foreach (AutomationElement document in documents)
                {
                    var textPattern = document.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                    var text = textPattern.DocumentRange.GetText(-1);
                    FailureTracker.Log(()=>Assert.Fail("Unhandled javascript exception: {0}", text));
                }

                foreach (AutomationElement button in buttons)
                {
                    if(button.Current.AutomationId == "7")
                    {
                        var invokePattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        invokePattern.Invoke();
                        break;
                    }
                }
                return true;
            }
            return false;
        }

        public override bool CanHandleDialog(Window window)
        {
            return window.StyleInHex == "94C808CC";
        }
    }
}