using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QLDiemSV_GUI
{
    public static class ExcelHelper
    {
        // Cấu hình License cho EPPlus 5+
        static ExcelHelper()
        {
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public static void XuatRaExcel(DataTable dt, string sheetName, string title)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
            sfd.FileName = title + "_" + DateTime.Now.ToString("ddMMyyyy_HHmm");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileInfo file = new FileInfo(sfd.FileName);
                    if (file.Exists) file.Delete();

                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                        // 1. Tiêu đề lớn
                        int colCount = dt.Columns.Count;
                        worksheet.Cells[1, 1, 1, colCount].Merge = true;
                        worksheet.Cells[1, 1].Value = title.ToUpper();
                        worksheet.Cells[1, 1].Style.Font.Size = 16;
                        worksheet.Cells[1, 1].Style.Font.Bold = true;
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Blue);

                        // 2. Header bảng
                        int rowStart = 3;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var cell = worksheet.Cells[rowStart, i + 1];
                            cell.Value = dt.Columns[i].ColumnName;
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        // 3. Dữ liệu
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                var cell = worksheet.Cells[rowStart + i + 1, j + 1];
                                cell.Value = dt.Rows[i][j];
                                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                if (dt.Columns[j].DataType == typeof(DateTime))
                                {
                                    cell.Style.Numberformat.Format = "dd/MM/yyyy";
                                }
                            }
                        }

                        worksheet.Cells.AutoFitColumns();
                        package.Save();
                    }

                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message);
                }
            }
        }
    }
}