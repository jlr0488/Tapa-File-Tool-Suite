using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace K1_Parser_v1.Tool_Logic
{
    public  class K1_Processor
    {
        public struct Investor_Info
        {
            public string Name;

            public iTextSharp.text.pdf.PdfDocument K1;
        }

        public string OutputDirectory;

        public List<Investor_Info> K1_List { get; set; }

        public string CompanyName { get; set; }

        public K1_Processor()
        {
            K1_List = new List<Investor_Info>();
        }

        public void LoadPDF(string filePath)
        {
            int interval = 4;
            int pageNameSuffix = 0;

            StringBuilder sb = new StringBuilder();

            PdfReader reader1 = new PdfReader(filePath);


            for (int i = 1; i <= reader1.NumberOfPages; i = i + 4)
            {
                string page = "";

                page = PdfTextExtractor.GetTextFromPage(reader1, i);
                string[] lines = page.Split('\n');

                // get the company name from the first page
                if (i == 1)
                {
                    foreach (string line in lines)
                    {
                        if (line.Contains("Income of"))
                        {
                            var newLine = line.Replace("Income of ", "");

                            int index = newLine.IndexOf(" LLC");
                            string result = newLine.Substring(0, index);

                            CompanyName = result;

                            break;
                        }
                    }
                }

                // get each investor's name
                foreach (string line in lines)
                {
                    if (line.Contains("Dear"))
                    {
                        var newLine = line.Replace("Dear ", "");

                        K1_List.Add(new Investor_Info() { Name = newLine }); ;

                        break;
                    }
                }
            }

            // create separate K-1 pdf documents for each investor 
            PdfReader reader = new PdfReader(filePath); // remove
            int currentInvestor = 0;
            for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += interval)
            {
                pageNameSuffix++;
                string newPdfFileName = System.IO.Path.Combine(OutputDirectory, CompanyName
                    + "_K1_" + K1_List[currentInvestor].Name + ".pdf");

                if (File.Exists(newPdfFileName))
                {
                    newPdfFileName = System.IO.Path.Combine(OutputDirectory, CompanyName
                    + "_K1_" + K1_List[currentInvestor].Name + " (2)" + ".pdf");
                }

                SplitAndSaveInterval(filePath, pageNumber, interval, newPdfFileName);
                currentInvestor++;
            }
        }

        private void SplitAndSaveInterval(string pdfFilePath, int startPage, int interval, string pdfFileName)
        {
            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                Document document = new Document();
                PdfCopy copy = new PdfCopy(document, new FileStream(pdfFileName, FileMode.Create));
                document.Open();

                for (int pagenumber = startPage; pagenumber < (startPage + interval); pagenumber++)
                {
                    if (reader.NumberOfPages >= pagenumber)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, pagenumber));
                    }
                    else
                    {
                        break;
                    }

                }

                document.Close();
            }
        }
    }
}
