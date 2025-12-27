using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace QLDiemSV_GUI
{
    public static class PdfHelper
    {
        public static void XuatRaPdf(DataTable dt, string title)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF Files (*.pdf)|*.pdf";
            sfd.FileName = title + "_" + DateTime.Now.ToString("ddMMyyyy_HHmm");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Document doc = new Document(PageSize.A4, 20f, 20f, 30f, 30f);
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    // Font tiếng Việt (Lấy Arial từ Windows)
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                    BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    Font fTitle = new Font(bf, 16, Font.BOLD, BaseColor.BLUE);
                    Font fHeader = new Font(bf, 10, Font.BOLD, BaseColor.WHITE);
                    Font fRow = new Font(bf, 10, Font.NORMAL, BaseColor.BLACK);

                    // Tiêu đề
                    Paragraph pTitle = new Paragraph(title.ToUpper(), fTitle);
                    pTitle.Alignment = Element.ALIGN_CENTER;
                    pTitle.SpacingAfter = 20f;
                    doc.Add(pTitle);

                    // Bảng dữ liệu
                    PdfPTable table = new PdfPTable(dt.Columns.Count);
                    table.WidthPercentage = 100;

                    // Header
                    foreach (DataColumn col in dt.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName, fHeader));
                        cell.BackgroundColor = new BaseColor(12, 59, 124);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }

                    // Dữ liệu
                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (object item in row.ItemArray)
                        {
                            string text = item.ToString();
                            if (item is DateTime d) text = d.ToString("dd/MM/yyyy");

                            PdfPCell cell = new PdfPCell(new Phrase(text, fRow));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.Padding = 5;
                            table.AddCell(cell);
                        }
                    }

                    doc.Add(table);
                    doc.Close();

                    MessageBox.Show("Xuất PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất PDF: " + ex.Message);
                }
            }
        }
    }
}