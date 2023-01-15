using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace K1_Parser_v1.Tool_Logic
{
    public  class K1_Processor
    {
        public struct Investor_Info
        {
            public string Name;

            public iTextSharp.text.pdf.PdfDocument K1;

            public int StartingPage;

            public int EndPage;
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
            int interval = 0;
            int pageNameSuffix = 0;

            StringBuilder sb = new StringBuilder();

            PdfReader reader1 = new PdfReader(filePath);


            for (int i = 1; i <= reader1.NumberOfPages; i++)
            {
                try
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

                            if (newLine.Contains(","))
                            {
                                newLine = newLine.Replace(",", "");
                            }

                            K1_List.Add(new Investor_Info() { Name = newLine, StartingPage = i }); ;

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // if any exception occurs, we will show the error and continue processing
                    MessageBox.Show(ex.Message);
                }
            }

            // create separate K-1 pdf documents for each investor 
            PdfReader reader = new PdfReader(filePath); // remove
            int currentInvestor = 0;
            for (int pageNumber = 1; pageNumber <= reader.NumberOfPages;)
            {
                //pageNameSuffix++;

                // make sure that there is at least one more investor in the list before processing
                if(K1_List.Count > currentInvestor + 1)
                {
                    interval = K1_List[currentInvestor + 1].StartingPage - K1_List[currentInvestor].StartingPage;

                    string newPdfFileName = System.IO.Path.Combine(OutputDirectory, CompanyName
                    + "_K1_" + K1_List[currentInvestor].Name + ".pdf");

                    if (File.Exists(newPdfFileName))
                    {
                        newPdfFileName = System.IO.Path.Combine(OutputDirectory, CompanyName
                        + "_K1_" + K1_List[currentInvestor].Name + " (2)" + ".pdf");
                    }

                    SplitAndSaveInterval(filePath, pageNumber, interval, newPdfFileName);
                    // get the page number where the next investor documents begin
                    currentInvestor++;
                    pageNumber = K1_List[currentInvestor].StartingPage;
                }
                else
                {
                    // if there are no more investors, process the current investor until the end of the document
                    interval = reader.NumberOfPages - K1_List[currentInvestor].StartingPage + 1;

                    string newPdfFileName = System.IO.Path.Combine(OutputDirectory, CompanyName
                    + "_K1_" + K1_List[currentInvestor].Name + ".pdf");

                    if (File.Exists(newPdfFileName))
                    {
                        newPdfFileName = System.IO.Path.Combine(OutputDirectory, CompanyName
                        + "_K1_" + K1_List[currentInvestor].Name + " (2)" + ".pdf");
                    }

                    SplitAndSaveInterval(filePath, pageNumber, interval, newPdfFileName);

                    // since we just processed the last investor, we should go ahead and break out of the for loop
                    break;
                }

              
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
