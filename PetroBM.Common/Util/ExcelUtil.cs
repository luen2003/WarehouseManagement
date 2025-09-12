using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Common.Util
{
  
    public class ExcelUtil
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ExcelUtil));


        public static DataTable ReadFromExcelfile(string path)
        {
            DataTable dt = new DataTable();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count > 0)
                {
                    var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                    foreach (var firstRowCell in workSheet.Cells[1, 1, 22, 4])
                    {
                        dt.Columns.Add(firstRowCell.Text);
                    }

                    for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, 3];
                        var newRow = dt.NewRow();

                        foreach (var cell in row)
                        {
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                        dt.Rows.Add(newRow);
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// Đọc file Excel By ThangNK CreateDate:26/11/2017
        /// </summary>
        /// <param name="path">Đường dẫn file </param>
        /// <param name="col">Số cột cần đọc</param>
        /// <returns></returns>
        public static DataTable CommonReadFromExcelfile(string path, int col)
        {
            DataTable dt = new DataTable();
            try
            {
              
                using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                        for (var rowNumber = 1; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                        {
                            var row = workSheet.Cells[rowNumber, 1, rowNumber, col];
                            if (rowNumber == 1)
                            {
                                foreach (var cell in row)
                                {
                                    dt.Columns.Add(cell.Text);
                                }
                            }
                            else
                            {
                                var newRow = dt.NewRow();
                                foreach (var cell in row)
                                {
                                    if (cell.Text != "")
                                    {
                                        newRow[cell.Start.Column - 1] = cell.Text;
                                    }
                                }
                                var c = newRow.ItemArray[0].ToString();
                                if (c.ToString() != "" )
                                {
                                    dt.Rows.Add(newRow);
                                }
                              
                            }

                        }
                    }
                    return dt;
                }
           
              
            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                return dt;
            }
           
        }
        public DataTable ReadFromExcelfile(string path, int col)
        {
            DataTable dt = new DataTable();
            try
            {
                log.Info("Start");
                using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                        for (var rowNumber = 1; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                        {
                            var row = workSheet.Cells[rowNumber, 1, rowNumber, col];
                            if (rowNumber == 1)
                            {
                                foreach (var cell in row)
                                {
                                    dt.Columns.Add(cell.Text);
                                }
                            }
                            else
                            {
                                var newRow = dt.NewRow();
                                foreach (var cell in row)
                                {
                                    newRow[cell.Start.Column - 1] = cell.Text;
                                }
                                dt.Rows.Add(newRow);
                            }

                        }
                    }
                    
                    
                }

                log.Info("End");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                //return dt;
            }
            return dt;
        }
    }
}
