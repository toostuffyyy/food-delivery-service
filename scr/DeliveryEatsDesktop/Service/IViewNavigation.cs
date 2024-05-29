using desktop.ViewModels;
using System.Web.Optimization;
using Avalonia.Controls;
using Bundle = desktop.Models.Bundle;

namespace desktop.Service
{
    public interface IViewNavigation
    {
        public void GoTo<VM>(Bundle bundle = null) where VM : ViewModelBase;
        public void GoToAndCloseCurrent<VM>(ViewModelBase currentViewModel, Bundle bundle = null) where VM : ViewModelBase;
        public void GoToAndCloseOthers<VM>(Bundle bundle = null) where VM : ViewModelBase;
        public void Close(ViewModelBase viewModel);
    }
}
