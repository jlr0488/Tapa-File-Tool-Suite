using System.Collections.Generic;
using System.Linq;

namespace K1_Parser_v1.Models
{
    public class Tools
    {
        public List<Tool> ToolList { get; }

        public Tools()
        {
            ToolList = new List<Tool>();
        }

        public IEnumerable<Tool> GetToolsForCompany(string companyName)
        {
            return ToolList.Where(TL => TL.CompanyName == companyName);
        }

        public void AddCompanyTool(Tool tool)
        {
            ToolList.Add(tool);
        }
    }
}
