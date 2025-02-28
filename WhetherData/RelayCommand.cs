﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WhetherData
{
    public class RelayCommand : ICommand
    {
        private Action action;
        public event EventHandler CanExecuteChanged = (sender,e) => {};

        public RelayCommand(Action action)
        {
            this.action = action;
        }
        public bool CanExecute(object parameter) => true;
       
        public void Execute(object parameter)
        {
            action.Invoke();
        }
    }
}
