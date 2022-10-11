using System.Windows;
using System.Windows.Input;

namespace Flicker;

public class FlickerIconViewModel
{
    
    /// <summary>
    /// Shuts down the application.
    /// </summary>
    public ICommand ExitApplicationCommand
    {
        get
        {
            return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
        }
    }
}