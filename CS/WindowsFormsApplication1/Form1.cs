using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public string FilePostfix { get; set; }
        public Form1()
        {
            InitializeComponent();

            FilePostfix = "Test";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XtraReport1 report = new XtraReport1();
            ReportPrintTool pt = new ReportPrintTool(report);

            pt.PrintingSystem.AddCommandHandler(new ExcelExportCommandHandler(FilePostfix));
            pt.ShowPreviewDialog();
        }
    }

    public class ExcelExportCommandHandler : ICommandHandler
    {
        public string FileName { get; set; }
        public ExcelExportCommandHandler()
        {

        }
        public ExcelExportCommandHandler(string fileName)
        {
            FileName = fileName;
        }
        public bool CanHandleCommand(PrintingSystemCommand command, IPrintControl printControl)
        {
            return command == PrintingSystemCommand.ExportXlsx;
        }

        public void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled)
        {
            if (!CanHandleCommand(command, printControl))
                return;

            XlsxExportOptions options = new XlsxExportOptions() { ExportMode = XlsxExportMode.DifferentFiles} ;

            DialogResult dr = ExportOptionsTool.EditExportOptions(options, printControl.PrintingSystem);
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "XtraReport.xlsx";

                sfd.Filter = "XLSX File|*.xlsx";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    printControl.PrintingSystem.ExportToXlsx(sfd.FileName, options);

                    string fileName = Path.GetFileNameWithoutExtension(sfd.FileName);
                    string path = Path.GetDirectoryName(sfd.FileName);

                    List<string> fileNames = new List<string>();
                    for (int i = 1; i <= printControl.PrintingSystem.PageCount; i++)
                    {
                        string genFileName = string.Format("{0}\\{1}{2}", path, fileName, i);
                        System.IO.File.Move(genFileName + ".xlsx", String.Format("{0}{1}.xlsx", genFileName, FileName));
                    }                    
                }
            }

            handled = true;
        }
    }
}
