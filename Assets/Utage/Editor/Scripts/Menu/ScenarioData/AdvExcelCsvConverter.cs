//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utage
{

	//「Utage」のシナリオデータ用のエクセルファイル解析して、CSVファイルを生成する
	public class AdvExcelCsvConverter
	{
		//TSV形式で出力する
		//const string extConvert = ".tsv";
		const string extConvert = ".csv";

		StringGrid bootSettingGrid;
		StringGrid scenarioSettingGrid;
		StringGridDictionary scenarioSheetDictionary = new StringGridDictionary();
		const string extXls = ".xls";
		const string extXlsx = ".xlsx";

		/// <summary>
		/// コンバートする
		/// </summary>
		/// <param name="folderPath">出力先パス</param>
		/// <param name="assetPathList">読み込むエクセルファイルのリスト</param>
		/// <returns>コンバートしたらtrue。失敗したらfalse</returns>
		public bool Convert(string folderPath, List<string> assetPathList, int version )
		{
			scenarioSheetDictionary.Clear();
			if (!string.IsNullOrEmpty(folderPath) && assetPathList.Count > 0)
			{
				//対象のエクセルファイルを全て読み込み
				StringGridDictionary readSheet = new StringGridDictionary();
				foreach (string assetPath in assetPathList)
				{
					if (!string.IsNullOrEmpty(assetPath))
					{
						StringGridDictionary dictionary = ExcelParser.Read(assetPath);
						foreach (var sheet in dictionary.List )
						{
							readSheet.Add(sheet);
						}
					}
				}

				if (readSheet.List.Count <= 0) return false;

				//各シートをコンバート
				foreach (var sheet in readSheet.List)
				{
					ConvertSheet(sheet, folderPath);
				}
				//シナリオ設定シートは個別にコンバート
				WriteScenarioSetting(folderPath, version);

				///起動用CSVをコンバート
				WriteBootSetting(folderPath, version);

				return true;
			}
			return false;
		}

		//起動用CSVをコンバート
		void WriteBootSetting(string folderPath, int version)
		{
			StringGrid grid = AdvBootSetting.CreateOnCsvOnvert(bootSettingGrid, version);
			string outPutPath = folderPath + "/" + AdvSettingDataManager.SheetNameBoot + extConvert;
			WriteFile(grid,outPutPath);
		}

		//シートをCSVにコンバート
		void ConvertSheet(StringGridDictionaryKeyValue sheet, string folderPath)
		{
			string outPutPath = folderPath + "/";
			if (AdvSettingDataManager.IsBootSheet(sheet.Name))
			{
				///起動用データは個別にコンバート
				bootSettingGrid = sheet.Grid;
				return;
			}
			else if (AdvSettingDataManager.IsScenarioSettingSheet(sheet.Name))
			{
				///シナリオ設定データは個別にコンバート
				scenarioSettingGrid = sheet.Grid;
				return;
			}
			else if (AdvSettingDataManager.IsSettingsSheet(sheet.Name))
			{
				outPutPath += "Settings";
			}
			else
			{
				scenarioSheetDictionary.Add(sheet);
				outPutPath += "Scenario";
			}
			outPutPath += "/" + sheet.Key + extConvert;
			WriteFile(sheet.Grid, outPutPath);
		}

		//シナリオ設定シートを個別にコンバート
		void WriteScenarioSetting(string folderPath, int version)
		{
			scenarioSettingGrid = AdvScenarioSetting.MargeScenarioData(scenarioSettingGrid, scenarioSheetDictionary, version);
			string path = folderPath + "/Settings/" + AdvSettingDataManager.SheetNameScenario + extConvert;
			WriteFile(scenarioSettingGrid, path);
		}

		//CSVを書き込み
		void WriteFile(StringGrid grid, string path)
		{
			WriteStringGrid(grid,path);

			string relativePath = FileUtil.GetProjectRelativePath(path);
			Object assetObject = AssetDatabase.LoadAssetAtPath(relativePath, typeof(Object));
			if (assetObject!=null)
			{
				EditorUtility.SetDirty(assetObject);
			}
		}

		//ファイルの書き込み
		void WriteStringGrid(StringGrid grid, string path)
		{
			string dir = System.IO.Path.GetDirectoryName(path);
			if (!System.IO.Directory.Exists(dir))
			{
				System.IO.Directory.CreateDirectory(dir);
			}

			// ファイルにテキストを書き出し。
			using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
			{
				char separator = grid.CsvSeparator;

				Debug.Log( path + ":" +separator );

				foreach (StringGridRow row in grid.Rows)
				{
					for (int i = 0; i < row.Strings.Length; ++i)
					{
						//CSVの書式にあわせる
						string line = row.Strings[i].Replace("\n", "\\n");
						writer.Write(line);
						if (i < row.Strings.Length - 1)
						{
							writer.Write(separator);
						}
					}
					writer.Write("\n");
				}
			}
		}
	}
}