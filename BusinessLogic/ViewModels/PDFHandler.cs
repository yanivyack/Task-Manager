using BusinessLogic.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace BusinessLogic.ViewModels
{
    public static class PDFHandler
    {
        /// <summary>
        /// a class to handle the PDF export
        /// </summary>
        public static void ExportToPDF(string filePath, List<ManagableTask> tasks)
        {
            string HTMLstring = TasksToHTML(tasks);

            PdfGenerateConfig conf = new PdfGenerateConfig();
            conf.PageSize = PdfSharp.PageSize.A4;
            conf.PageOrientation = PageOrientation.Portrait;
            conf.ManualPageSize = PdfSharp.Drawing.XSize.Empty;

            PdfSharp.Pdf.PdfDocument pdf;
            try
            {
                pdf = PdfGenerator.GeneratePdf(HTMLstring, conf);
            }
            catch (Exception)
            {
                pdf = PdfGenerator.GeneratePdf(HTMLstring, PdfSharp.PageSize.A4);
            }
            pdf.Save(filePath);
        }

        /// <summary>
        /// converts a list of tasks into an HTML document
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        private static string TasksToHTML(List<ManagableTask> tasks)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html><body>");
            sb.AppendLine("<h1>Task List:</h1><br />");
            sb.AppendLine("<table border=0>");

            foreach (ManagableTask task in tasks.OrderBy(task => task.Priority))
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine(TaskToHTML(task));
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }
        /// <summary>
        /// converts a single task into an HTML representation
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static string TaskToHTML(ManagableTask task)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<table>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td>");
            sb.AppendLine("<h2>" + task.Title + "</h2>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td>");
            sb.AppendLine(task.Description);
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td>");
            sb.AppendLine(task.Done ? "Done" : "&nbsp;");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }
    }
}