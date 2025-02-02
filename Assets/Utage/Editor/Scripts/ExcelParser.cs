//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Utage
{

	/// <summary>
	/// エクセルの解析用クラス
	/// </summary>
	public static class ExcelParser
	{
		public const string ExtXls = ".xls";
		public const string ExtXlsx = ".xlsx";

		//エクセルファイルか判定
		public static bool IsExcelFile(string path)
		{
			string ext = Path.GetExtension(path);
			return ((ext == ExtXls || ext == ExtXlsx) && File.Exists(path));
		}

		//ファイルの読み込み
		public static StringGridDictionary Read( string path )
		{
			StringGridDictionary gridDictionary = new StringGridDictionary();
			if ( IsExcelFile(path) )
			{
				string ext = Path.GetExtension(path);
				using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					if (ext == ExtXls)
					{
						ReadBook(new HSSFWorkbook(fs), path, gridDictionary);
					}
					else if (ext == ExtXlsx)
					{
						ReadBook(new XSSFWorkbook(fs), path, gridDictionary);
					}
				}
			}
			return gridDictionary;
		}

		//ブックの読み込み
		static void ReadBook(IWorkbook book, string path, StringGridDictionary gridDictionary)
		{
			for (int i = 0; i < book.NumberOfSheets; ++i)
			{
				ISheet sheet = book.GetSheetAt(i);
				StringGrid grid = ReadSheet(sheet, path);
				gridDictionary.Add(new StringGridDictionaryKeyValue(sheet.SheetName, grid));
			}
		}

		//シートの読み込み
		static StringGrid ReadSheet(ISheet sheet, string path)
		{
			int lastRowNum = sheet.LastRowNum;

			CsvType use_csv_type = CsvType.Csv;

			/*
			if( 0<=path.IndexOf(".csv")){
				use_csv_type = CsvType.Csv;
				Debug.Log( "csv path="+path);
			}
			else {
				Debug.Log( "tsv path="+path);
			}
			*/

			StringGrid grid = new StringGrid(path + ":" + sheet.SheetName, use_csv_type);
			for (int rowIndex = sheet.FirstRowNum; rowIndex <= lastRowNum; ++rowIndex)
			{
				IRow row = sheet.GetRow(rowIndex);

				List<string> stringList = new List<string>();
				if (row != null)
				{
					foreach (var cell in row.Cells)
					{
						for (int i = stringList.Count; i < cell.ColumnIndex; ++i)
						{
							stringList.Add("");
						}
						stringList.Add(cell.ToString());
					}
				}
				grid.AddRow(stringList);
			}
			grid.ParseHeader();
			return grid;
		}
	}
}