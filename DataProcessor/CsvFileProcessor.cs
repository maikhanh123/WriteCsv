using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataProcessor
{
    class CsvFileProcessor
    {
        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public CsvFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public void Process()
        {
            using (StreamReader input = File.OpenText(InputFilePath))
            using (CsvReader csvReader = new CsvReader(input))
            using (StreamWriter output = File.CreateText(OutputFilePath))
            using (var csvWriter = new CsvWriter(output))
            {
                csvReader.Configuration.TrimOptions = TrimOptions.Trim; //trimm all white space around
                csvReader.Configuration.Comment = '@'; // Default is '#'
                csvReader.Configuration.AllowComments = true;
                //csvReader.Configuration.IgnoreBlankLines = true;  //not check Blank line
                //csvReader.Configuration.Delimiter = ";";          // field seperator by ";"
                //csvReader.Configuration.HasHeaderRecord = false;  //file no have header 
                //On the csv will no have header so they will not know .OrderNumber, must be change to .Field1, .Field2
                //csvReader.Configuration.HeaderValidated = null;
                //csvReader.Configuration.MissingFieldFound = null;
                csvReader.Configuration.RegisterClassMap<ProcessedOrderMap>();

                //IEnumerable<dynamic> records = csvReader.GetRecords<dynamic>();
                IEnumerable<ProcessedOrder> records = csvReader.GetRecords<ProcessedOrder>();

                //csvWriter.WriteRecords(records);

                csvWriter.WriteHeader<ProcessedOrder>();
                csvWriter.NextRecord();

                var recordsArray = records.ToArray();
                for (int i = 0; i < recordsArray.Length; i++)
                {

                    csvWriter.WriteField(recordsArray[i].OrderNumber);
                    csvWriter.WriteField(recordsArray[i].Customer);
                    csvWriter.WriteField(recordsArray[i].Amount);

                    bool isLastRecord = i == recordsArray.Length - 1;

                    if (!isLastRecord)
                    {
                        csvWriter.NextRecord();
                    }
                }
            }
        }
    }
}
