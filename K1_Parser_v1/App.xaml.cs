using K1_Parser_v1.Models;
using K1_Parser_v1.Stores;
using K1_Parser_v1.ViewModels;
using System.Threading;
using System.Windows;

namespace K1_Parser_v1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ToolSuite _toolSuite;
        private readonly NavigationStore _navigationStore;
        public NavigationBarViewModel _navigationBarViewModel;

        public App()
        {
            Tool tool = new Tool("K1 Processor", "Swimlar LLC.");
            _toolSuite = new ToolSuite("Swimlar LLC.");
            _toolSuite.AddTool(tool);

            _navigationStore = new NavigationStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.Sleep(1500);

            _navigationStore.CurrentViewModel = CreatToolListViewModel();

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };

            MainWindow.Show();

            base.OnStartup(e);
        }

        private ToolListViewModel CreatToolListViewModel()
        {
            return new ToolListViewModel(_toolSuite, _navigationStore, CreateK1ProcessorViewModel, CreatToolListViewModel, CreateSupportViewModel);
        }

        private K1ProcessorViewModel CreateK1ProcessorViewModel()
        {
            return new K1ProcessorViewModel(_navigationStore, CreateK1ProcessorViewModel, CreatToolListViewModel, CreateSupportViewModel);
        }

        private SupportViewModel CreateSupportViewModel()
        {
            return new SupportViewModel(_navigationStore, CreateK1ProcessorViewModel, CreatToolListViewModel, CreateSupportViewModel);
        }
    }
}
