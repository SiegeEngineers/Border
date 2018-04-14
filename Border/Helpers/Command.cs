using Border.View;
using Border.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Border.Helpers
{
    public class SimpleCommand : System.Windows.Input.ICommand
    {
        private Action _action;
        public SimpleCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class SelectBuildOrderCommand : System.Windows.Input.ICommand
    {
        private MainWindow _window;
        public SelectBuildOrderCommand(MainWindow window)
        {
            _window = window;
        }

        public bool CanExecute(object parameter)
        {
            if(parameter is DynamicBuildOrder)
            {
                return _window.BuildOrders.List.Contains((DynamicBuildOrder)parameter);
            }
            return false;
            
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter is DynamicBuildOrder)
            {
                _window.BuildOrders.SetCurrentBuildOrder((DynamicBuildOrder)parameter);
            }
        }
    }
}
