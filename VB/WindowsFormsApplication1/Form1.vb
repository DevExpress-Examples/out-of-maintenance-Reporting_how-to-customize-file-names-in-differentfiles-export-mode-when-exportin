Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms

Namespace WindowsFormsApplication1
    Partial Public Class Form1
        Inherits Form

        Public Property FilePostfix() As String
        Public Sub New()
            InitializeComponent()

            FilePostfix = "Test"
        End Sub

        Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button1.Click
            Dim report As New XtraReport1()
            Dim pt As New ReportPrintTool(report)

            pt.PrintingSystem.AddCommandHandler(New ExcelExportCommandHandler(FilePostfix))
            pt.ShowPreviewDialog()
        End Sub
    End Class

    Public Class ExcelExportCommandHandler
        Implements ICommandHandler

        Public Property FileName() As String
        Public Sub New()

        End Sub
        Public Sub New(ByVal fileName As String)
            Me.FileName = fileName
        End Sub
        Public Function CanHandleCommand(ByVal command As PrintingSystemCommand, ByVal printControl As IPrintControl) As Boolean Implements ICommandHandler.CanHandleCommand
            Return command = PrintingSystemCommand.ExportXlsx
        End Function

        Public Sub HandleCommand(ByVal command As PrintingSystemCommand, ByVal args() As Object, ByVal printControl As IPrintControl, ByRef handled As Boolean) Implements ICommandHandler.HandleCommand
            If Not CanHandleCommand(command, printControl) Then
                Return
            End If

            Dim options As New XlsxExportOptions() With {.ExportMode = XlsxExportMode.DifferentFiles}

            Dim dr As DialogResult = ExportOptionsTool.EditExportOptions(options, printControl.PrintingSystem)
            If dr = System.Windows.Forms.DialogResult.OK Then
                Dim sfd As New SaveFileDialog()
                sfd.FileName = "XtraReport.xlsx"

                sfd.Filter = "XLSX File|*.xlsx"
                If sfd.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    printControl.PrintingSystem.ExportToXlsx(sfd.FileName, options)


                    Dim fileName_Renamed As String = System.IO.Path.GetFileNameWithoutExtension(sfd.FileName)
                    Dim path As String = System.IO.Path.GetDirectoryName(sfd.FileName)

                    Dim fileNames As New List(Of String)()
                    For i As Integer = 1 To printControl.PrintingSystem.PageCount
                        Dim genFileName As String = String.Format("{0}\{1}{2}", path, fileName_Renamed, i)
                        System.IO.File.Move(genFileName & ".xlsx", String.Format("{0}{1}.xlsx", genFileName, FileName))
                    Next i
                End If
            End If

            handled = True
        End Sub
    End Class
End Namespace
