using K1_Parser_v1.Models;
using K1_Parser_v1.ViewModels;
using System.Windows;

namespace K1_Parser_v1.Commands
{
    public class GoToToolListCommand : CommandBase
    {
        private readonly ToolSuite _toolSuite;
        private readonly ToolListViewModel _toolListViewModel;

        public GoToToolListCommand(ToolListViewModel toolListViewModel, ToolSuite toolSuite) 
        {
            _toolListViewModel = toolListViewModel;
            _toolSuite = toolSuite;
        }

        public override void Execute(object? parameter)
        {
            //Tool tool = new Tool(_toolListViewModel.ToolName, _toolSuite.ToolSuiteContext);

            MessageBox.Show($"Congrats {_toolSuite.ToolSuiteContext}! You are now inside the {_toolListViewModel.ToolName} view!", "Success!",
                MessageBoxButton.OK, MessageBoxImage.Information);


        }
    }
}
