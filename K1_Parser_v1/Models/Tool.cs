namespace K1_Parser_v1.Models
{
    public class Tool
    {
        public string Tool_Id { get; }

        public string CompanyName { get; }

        public Tool(string toolId, string companyName)
        {
            Tool_Id = toolId;

            CompanyName = companyName;
        }
    }
}
