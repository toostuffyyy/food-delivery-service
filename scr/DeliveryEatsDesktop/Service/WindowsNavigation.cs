using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using desktop.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using desktop.Views;
using Bundle = desktop.Models.Bundle;

namespace desktop.Service
{
    public class WindowsNavigation : IViewNavigation
    {
        private void ShowWindow<VM>(Bundle bundle) where VM : ViewModelBase
        {
            var name = $"{Assembly.GetEntryAssembly()?.GetName().Name}.Views.{typeof(VM).Name.Replace("ViewModel", "Window")}";
            var type = Type.GetType(name);
            if (type == null)
                throw new Exception("View не найдена");
            
            Window window = (Window)Activator.CreateInstance(type)!;
            window.Activated += (obj, e) => Locator.Current.GetService<INotificationService>()?
                .RegisterNotificationManager(new WindowNotificationManager(window));
            
            ViewModelBase viewModelBase = Locator.Current.GetService<VM>();
            viewModelBase.Bundle = bundle;
            window.DataContext = viewModelBase;
            
            window.Show();
            window.Focus();
        }

        public void GoTo<VM>(Bundle bundle = null) where VM : ViewModelBase
        {
            ShowWindow<VM>(bundle);
        }

        public void GoToAndCloseCurrent<VM>(ViewModelBase currentViewModel, Bundle bundle = null) where VM : ViewModelBase
        {
            ShowWindow<VM>(bundle);
            ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Windows
                .FirstOrDefault(a => a.DataContext == currentViewModel)?.Close();
        }

        public void GoToAndCloseOthers<VM>(Bundle bundle = null) where VM : ViewModelBase
        {
            ShowWindow<VM>(bundle);
            ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Windows
                .SkipLast(1).ToList().ForEach(a => a.Close());
        }
        
        public void Close(ViewModelBase viewModel)
        {
            ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Windows
                .FirstOrDefault(a => a.DataContext == viewModel)?.Close();
        }
    }
}