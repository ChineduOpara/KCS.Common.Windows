using System;
using System.Collections.Generic;
using KCS.Common.Shared;
using System.Data;
//using Microsoft.Office.Interop.Excel;

namespace KCS.Common.Controls.ComboBoxes
{
	/// <summary>
	/// Displays a list of Excel Worksheets taken from a workbook.
	/// </summary>
	public class ExcelWorksheetComboBox : ExtendedComboBox
	{
		public void BindData(string excelDocumentPath)
		{
			MSExcel excel = new MSExcel(false);
			DataTable dt = new DataTable();
			dt.EnsureColumns<string>("Id", "Name");

			//foreach (Excel.Worksheet ws in excel.Worksheets)
			//{
			//    //dt.Rows.Add(ws.s
			//}
			excel.CloseFile();
		}
	}
}
