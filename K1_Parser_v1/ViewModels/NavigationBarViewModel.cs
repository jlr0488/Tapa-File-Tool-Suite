using K1_Parser_v1.Commands;
using K1_Parser_v1.Stores;
using System;
using System.Windows.Input;

namespace K1_Parser_v1.ViewModels
{
    public class NavigationBarViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }

        public ICommand NavigateK1ProcessorCommand { get; }

        public ICommand NavigateSupportCommand { get; }

        public NavigationBarViewModel(NavigationStore navigationStore, Func<K1ProcessorViewModel> createK1ProcessorViewModel, Func<ToolListViewModel> createToolListViewModel, Func<SupportViewModel> createSupportViewModel)
        {
            NavigateHomeCommand = new NavigateCommand(navigationStore, createToolListViewModel);
            NavigateK1ProcessorCommand = new NavigateCommand(navigationStore, createK1ProcessorViewModel);
            NavigateSupportCommand = new NavigateCommand(navigationStore, createSupportViewModel);
        }
    }
}
