using System.Collections.Generic;

namespace K1_Parser_v1.Models
{
    public class ToolSuite
    {
        private readonly Tools _toolSet;

        /// <summary>
        /// Company that is using this tool
        /// </summary>
        public string ToolSuiteContext { get; }

        public ToolSuite(string comapnyName)
        {
            ToolSuiteContext = comapnyName;

            _toolSet = new Tools();
        }
        public IEnumerable<Tool> GetToolsForCompany(string companyName)
        {
            return _toolSet.GetToolsForCompany(companyName);
        }

        public void AddTool(Tool tool)
        {
            _toolSet.AddCompanyTool(tool);
        }
    }
}
