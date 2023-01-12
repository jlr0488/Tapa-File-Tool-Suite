using K1_Parser_v1.Commands;
using K1_Parser_v1.Models;
using K1_Parser_v1.Stores;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace K1_Parser_v1.ViewModels
{
    public class ToolListViewModel : ViewModelBase
    {
        private string? _companyName;

        private string? _imagesDirectoryPath;
        public string ImagesPath
        {
            get 
            { 
                return Path.Combine(_imagesDirectoryPath, "Images", "Tapa File - Icon - 400x400.png"); 
            }
            set
            {
                _imagesDirectoryPath = value;
                OnPropertyChanged(nameof(ImagesPath));
            }
        }

        public string CompanyName
        {
            get
            {
                return "WELCOME, " + _companyName + "!";
            }
            set
            {
                _companyName = value;
                OnPropertyChanged(nameof(CompanyName));
            }
        }

        private string? _toolName;

        public string ToolName
        {
            get
            {
                return _toolName;
            }
            set
            {
                _toolName = value;
            }
        }

        public ICommand SubmitCommand { get; }

        public ICommand NavigateHomeCommand { get; }

        public ICommand NavigateK1ProcessorCommand { get; }

        public ICommand NavigateSupportCommand { get; }

        public ToolListViewModel(ToolSuite toolSuite, NavigationStore navigationStore, Func<K1ProcessorViewModel> createK1ProcessorViewModel, Func<ToolListViewModel> createToolListViewModel, Func<SupportViewModel> createSupportViewModel)
        {
            CompanyName = toolSuite.ToolSuiteContext;
            ImagesPath = Directory.GetCurrentDirectory();

            var companyTools = toolSuite.GetToolsForCompany(toolSuite.ToolSuiteContext);
            var companyTool = companyTools.ToList().First<Tool>();
            ToolName = companyTool.Tool_Id;

            SubmitCommand = new NavigateCommand(navigationStore, createK1ProcessorViewModel);

            NavigateHomeCommand = new NavigateCommand(navigationStore, createToolListViewModel);
            NavigateK1ProcessorCommand = new NavigateCommand(navigationStore, createK1ProcessorViewModel);
            NavigateSupportCommand = new NavigateCommand(navigationStore, createSupportViewModel);
        }
    }
}
