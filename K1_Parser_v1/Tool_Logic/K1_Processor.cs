using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using K1_Parser_v1.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public string Year { get; set; }

        public K1_Processor()
        {
            K1_List = new List<Investor_Info>();
        }

        private bool isYearSet = false;

        public async void LoadPDF(string filePath)
        {
            CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

            ProgressWindow progressWindow = new ProgressWindow();
            progressWindow.Show();

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

                                newLine = newLine.TrimEnd(newLine[newLine.Length - 1]);
                                newLine = string.Join("", newLine.Split(System.IO.Path.GetInvalidFileNameChars()));

                                CompanyName = newLine;

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

                            newLine = string.Join("", newLine.Split(System.IO.Path.GetInvalidFileNameChars()));

                            K1_List.Add(new Investor_Info() { Name = newLine, StartingPage = i }); ;

                            break;
                        }
                    }

                    // get the date
                    if (!isYearSet)
                    {
                        bool isFirstLine = true;

                        foreach (string line in lines)
                        {
                            // we do not want to process the first line since we know that does not contain the correct date
                            if (!isFirstLine)
                            {
                                string newLine = "";
                                string subYear = "";

                                // the line that we care about will always start with "Enclosed". We will first look for that.
                                if (line.Contains("Enclosed"))
                                {
                                    // check for year prefixes that would match the 2020's, 2010's, or 2000's
                                    if (line.Contains("202"))
                                    {
                                        subYear = "202";
                                    }
                                    else if (line.Contains("201"))
                                    {
                                        subYear = "201";
                                    }
                                    else if (line.Contains("200"))
                                    {
                                        subYear = "200";
                                    }
                                    else
                                    {
                                        // if none of the years exist, then we know something changed to the K1 format and we will throw an exception
                                        throw new Exception("Could not find the correct tax year. K1 file formatting has changed.");
                                    }

                                    int pos = line.IndexOf(subYear);

                                    if (pos >= 0)
                                    {
                                        // first get rid of all character BEFORE the year value
                                        newLine = line.Remove(0, pos);
                                        // then get rid of all characters AFTER the year value (starting at
                                        // position 3 since thats where the year would end)
                                        newLine = newLine.Remove(4, newLine.Length - 4);
                                    }

                                    Year = newLine;
                                    isYearSet = true;

                                    break;
                                }
                            }
                            else
                            {
                                isFirstLine = false;
                            }
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    // if any exception occurs, we will show the error and continue processing
                    MessageBox.Show(ex.Message);
                }
            }

            // reset the isYearSet value to false so that it does not stay true when someone tries to process 
            // another set of K1's within the same session
            isYearSet = false;

            // Create a new Progress<T> object to report progress
            IProgress<(string, double)> progress = new Progress<(string, double)>(value =>
            {
                progressWindow.SetProgress(value.Item1, value.Item2);
            });

            int totalFiles = K1_List.Count;
            int filesProcessed = 0;

            // create separate K-1 pdf documents for each investor 
            PdfReader reader = new PdfReader(filePath); // remove
            int currentInvestor = 0;

            try 
            {
                for (int pageNumber = 1; pageNumber <= reader.NumberOfPages;)
                {
                    // make sure that there is at least one more investor in the list before processing
                    if (K1_List.Count > currentInvestor + 1)
                    {
                        interval = K1_List[currentInvestor + 1].StartingPage - K1_List[currentInvestor].StartingPage;

                        string newPdfFileName = System.IO.Path.Combine(OutputDirectory, Year + "_" + CompanyName
                        + "_K1_" + K1_List[currentInvestor].Name + ".pdf");

                        if (File.Exists(newPdfFileName))
                        {
                            newPdfFileName = System.IO.Path.Combine(OutputDirectory, Year + "_" + CompanyName
                            + "_K1_" + K1_List[currentInvestor].Name + " (2)" + ".pdf");
                        }

                        SplitAndSaveInterval(filePath, pageNumber, interval, newPdfFileName);

                        filesProcessed++;

                        // Run the long-running task asynchronously
                        await Task.Run(() =>
                        {
                            progressWindow.SetProgress($"Processing file {filesProcessed} of {totalFiles}...", (double)filesProcessed / totalFiles);

                            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }, _cancellationTokenSource.Token);

                        // get the page number where the next investor documents begin
                        currentInvestor++;
                        pageNumber = K1_List[currentInvestor].StartingPage;
                    }
                    else
                    {
                        // if there are no more investors, process the current investor until the end of the document
                        interval = reader.NumberOfPages - K1_List[currentInvestor].StartingPage + 1;

                        string newPdfFileName = System.IO.Path.Combine(OutputDirectory, Year + "_" + CompanyName
                        + "_K1_" + K1_List[currentInvestor].Name + ".pdf");

                        if (File.Exists(newPdfFileName))
                        {
                            newPdfFileName = System.IO.Path.Combine(OutputDirectory, Year + "_" + CompanyName
                            + "_K1_" + K1_List[currentInvestor].Name + " (2)" + ".pdf");
                        }

                        SplitAndSaveInterval(filePath, pageNumber, interval, newPdfFileName);

                        filesProcessed++;


                        // Run the long-running task asynchronously
                        await Task.Run(() =>
                        {
                            progressWindow.SetProgress($"Processing file {filesProcessed} of {totalFiles}...", (double)filesProcessed / totalFiles);

                            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }, _cancellationTokenSource.Token);

                        // since we just processed the last investor, we should go ahead and break out of the for loop
                        break;
                    }
                }

                // Run the long-running task asynchronously
                await Task.Run(() =>
                {
                    MessageBox.Show("The following K1 files were successfully processed: " + filePath, "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            } 
            catch (Exception ex)
            {
                // if any exception occurs, we will show the error and continue processing
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _cancellationTokenSource.Cancel();

                // Close the progress window
                progressWindow.Close();
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
