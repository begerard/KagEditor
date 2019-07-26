using System.Windows;
using System.Windows.Input;

namespace KagEditor.WpfCore.Utils
{
    public static class DragOverBehavior
    {
        #region Property
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
            typeof(ICommand), typeof(DragOverBehavior),
            new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback(DragOverBehavior.CommandChanged)));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, string value)
        {
            obj.SetValue(CommandProperty, value);
        }
        #endregion

        private static void CommandChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            UIElement element = obj as UIElement;

            if (element != null)
            {
                // If we're putting in a new command and there wasn't one already hook the event
                if ((e.NewValue != null) && (e.OldValue == null))
                    element.PreviewDragOver += element_PreviewDragOver;

                // If we're clearing the command and it wasn't already null unhook the event
                else if ((e.NewValue == null) && (e.OldValue != null))
                    element.PreviewDragOver -= element_PreviewDragOver;
            }
        }

        static void element_PreviewDragOver(object s, DragEventArgs e)
        {
            UIElement element = s as UIElement;
            ICommand command = (ICommand)element.GetValue(
                DragOverBehavior.CommandProperty);
            command.Execute(e);
        }
    }
}
