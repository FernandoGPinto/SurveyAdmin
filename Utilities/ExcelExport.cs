using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SurveyAdmin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Utilities
{
    public enum ExcelTemplate
    {
        Default
    }
    
    public class ExcelExport
    {
        string _sheetName;
        ExcelTemplate _template;

        public ExcelExport(string sheetName, ExcelTemplate template)
        {
            _sheetName = sheetName;
            _template = template;
        }

        /// <summary>
        /// Converts a List of type T into an Excel document. Returns a memory stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public NpoiMemoryStream PCDToExcel(List<ViewPCDResponse> data)
        {
            IWorkbook workbook;

            workbook = new XSSFWorkbook();

            ISheet excelSheet = workbook.CreateSheet(_sheetName);

            // Create headers.
            IRow questionRow = excelSheet.CreateRow(0);
            IRow responseRow = excelSheet.CreateRow(1);

            for (int i = 0; i < data.Count; i++)
            {
                ICell questionCell = questionRow.CreateCell(i);
                questionCell.SetCellValue(data[i].Question);
                ICell responseCell = responseRow.CreateCell(i);
                responseCell.SetCellValue(data[i].Response);
            }

            // Create an NpoiMemoryStream to store the workbook. As NPOI will close the stream before it can be converted to File we need to use a workaround extension class NpoiMemoryStream that inherits from MemoryStream and overrides its Close method. This enables us to avoid having to use multiple streams as a workaround.
            var memoryStream = new NpoiMemoryStream()
            {
                AllowClose = false
            };

            workbook.Write(memoryStream);

            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}
