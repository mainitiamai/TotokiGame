//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// 文字列グリッドデータの行
	/// </summary>
	[System.Serializable]
	public class StringGridRow
	{
		/// <summary>
		/// 元になるグリッド
		/// </summary>
		public StringGrid Grid { get { return callBackGetGrid(); } }

		/// <summary>
		/// 行番号
		/// </summary>
		public int RowIndex { get { return this.rowIndex; } }
		[SerializeField]
		int rowIndex;

		/// <summary>
		/// 文字列データ
		/// </summary>
		public string[] Strings { get { return this.strings; } }
		[SerializeField]
		string[] strings;

		/// <summary>
		/// 文字列データの長さ
		/// </summary>
		public int Length { get { return strings.Length; } }

		/// <summary>
		/// データが空かどうか
		/// </summary>
		public bool IsEmpty { get { return isEmpty; } }
		[SerializeField]
		bool isEmpty;

		Func<StringGrid> callBackGetGrid;


		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="grid">元になる文字列グリッド</param>
		/// <param name="rowIndex">行番号</param>
		public StringGridRow(Func<StringGrid> callBackGetGrid, int rowIndex )
		{
			this.rowIndex = rowIndex;
			InitLink(callBackGetGrid);
		}

		/// <summary>
		/// 親とのリンクを初期化
		/// ScriptableObjectなどで読み込んだ場合、参照が切れているのでそれを再設定するために
		/// </summary>
		/// <param name="grid">元になる文字列グリッド</param>
		public void InitLink(Func<StringGrid> callBackGetGrid)
		{
			this.callBackGetGrid = callBackGetGrid;
		}

		/// <summary>
		/// CSVテキストから初期化
		/// </summary>
		/// <param name="type">CSVタイプ</param>
		/// <param name="text">CSVテキスト</param>
		public void InitFromCsvText(CsvType type, string text )
		{
			const string conmmaSeparatePattern  = @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)";
			const string tabSeparatePattern = @"(((?<x>(?=[\t\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^\t\r\n]+))\t?)";

			string pattern;
			switch(type)
			{
				case CsvType.Tsv:
					pattern = tabSeparatePattern;
					break;
				case CsvType.Csv:
				default:
					pattern = conmmaSeparatePattern;
					break;
			}
			strings = (
				from System.Text.RegularExpressions.Match m 
					in System.Text.RegularExpressions.Regex.Matches(text,
					pattern,
					System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
				select m.Groups[1].Value).ToArray();

			this.isEmpty = CheckEmpty();
		}

		/// <summary>
		/// 文字列リストから初期化
		/// </summary>
		/// <param name="stringList">文字列リスト</param>
		public void InitFromStringList(List<string> stringList)
		{
			strings = stringList.ToArray();
			this.isEmpty = CheckEmpty();
		}

		//空データかチェック
		bool CheckEmpty()
		{
			foreach (var str in strings)
			{
				if (!string.IsNullOrEmpty(str))
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>
		/// 指定した列名のセルが空かどうか
		/// </summary>
		/// <param name="columnName">列の名前</param>
		/// <returns>空ならture、データがあればfalse</returns>
		public bool IsEmptyCell(string columnName)
		{
			int index;
			if (Grid.TryGetColumnIndex(columnName, out index))
			{
				return IsEmptyCell(index);
			}
			else
			{
				return false;
			}
		}

		//指定した列インデックスのセルが空かどうか
		bool IsEmptyCell(int index)
		{
			return !(index < Length && !string.IsNullOrEmpty(strings[index]));
		}

		/// <summary>
		/// 指定した列名のセルを値に変換
		/// </summary>
		/// <typeparam name="T">値の型</typeparam>
		/// <param name="columnName">列の名前</param>
		/// <returns>変換後の値</returns>
		public T ParseCell<T>(string columnName)
		{
			T ret;
			if (!TryParseCell(columnName, out ret))
			{
				Debug.LogError(ToErrorStringWithPraseColumnName(columnName));
			}
			return ret;
		}

		/// <summary>
		/// 指定した列名のセルを値に変換
		/// 要素が空だった場合は、デフォルト値を返す
		/// </summary>
		/// <typeparam name="T">値の型</typeparam>
		/// <param name="columnName">列の名前</param>
		/// <param name="defaultVal">デフォルト値</param>
		/// <returns>変換後の結果</returns>
		public T ParseCellOptional<T>(string columnName, T defaultVal)
		{
			T ret;
			return TryParseCell(columnName, out ret) ? ret : defaultVal;
		}

		/// <summary>
		/// 指定した列名のセルを値に変換を試みる。
		/// </summary>
		/// <typeparam name="T">値の型</typeparam>
		/// <param name="columnName">列の名前</param>
		/// <param name="val">変換後の結果</param>
		/// <returns>成功したらtrue。失敗したらfalse</returns>
		public bool TryParseCell<T>(string columnName, out T val)
		{
			int index;
			if (Grid.TryGetColumnIndex(columnName, out index))
			{
				return TryParseCellSub(index, out val);
			}
			else
			{
				val = default(T);
				return false;
			}
		}

		//指定した列インデックスのセルを値に変換
		bool TryParseCellSub<T>(int index, out T val)
		{
			if (!IsEmptyCell(index))
			{
				if (TryParse<T>(strings[index], out val))
				{
					return true;
				}
				else
				{
					Debug.LogError(ToErrorStringWithPrase(strings[index], index));
					return false;
				}
			}
			else
			{
				val = default(T);
				return false;
			}
		}

		/// <summary>
		/// 文字列を値に変換
		/// </summary>
		/// <typeparam name="T">値の型</typeparam>
		/// <param name="str">文字列</param>
		/// <param name="val">値</param>
		/// <returns>変換に成功したらtrue、書式違いなどで変換できなかったらfalse</returns>
		public static bool TryParse<T>(string str, out T val)
		{
			try
			{
				System.Type type = typeof(T);
				if (type == typeof(string))
				{
					val = (T)(object)str;
				}
				else if (type.IsEnum)
				{
					val = (T)System.Enum.Parse(typeof(T), str);
				}
				else if (type == typeof(Color))
				{
					Color color = Color.white;
					bool ret = ColorUtil.TryParseColor(str, ref color);
					val = ret ? (T)(object)color : default(T);
					return ret;
				}
				else if( type == typeof(int) )
				{
					val = (T)(object)int.Parse(str);
				}
				else if (type == typeof(float))
				{
					val = (T)(object)float.Parse(str);
				}
				else if (type == typeof(double))
				{
					val = (T)(object)double.Parse(str);
				}
				else if (type == typeof(bool))
				{
					val = (T)(object)bool.Parse(str);
				}
				else
				{
					System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
					val = (T)converter.ConvertFromString(str);
				}
				return true;
			}
			catch
			{
				val = default(T);
				return false;
			}
		}

		
		/// <summary>
		/// デバッグ文字列に変換
		/// </summary>
		/// <returns>デバッグ文字列</returns>
		public string ToDebugString()
		{
			char separator = Grid.CsvSeparator;

			string textOutput = "" + (RowIndex+1) + ":";
			foreach (string str in strings)
			{
				textOutput += " " + str + separator;
			}
			return textOutput;
		}

		/// <summary>
		/// エラー用の文字列を取得
		/// </summary>
		/// <param name="msg">エラーメッセージ</param>
		/// <returns>エラー用のテキスト</returns>
		public string ToErrorString(string msg)
		{
			return msg + "\n" + ToDebugString() + " " + Grid.Name + " :" + (rowIndex + 1);
		}

		//列名指定パースエラー出力
		string ToErrorStringWithPraseColumnName(string columnName)
		{
			return ToErrorString( LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridRowPraseColumnName, columnName ) );
		}
		//列インデックス指定パースエラー出力
		string ToErrorStringWithPraseColumnIndex(int index)
		{
			return ToErrorString( LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridRowPraseColumnIndex, index ) );
		}
		//パースエラー出力
		string ToErrorStringWithPrase(string column, int index)
		{
			return ToErrorString( LanguageErrorMsg.LocalizeTextFormat( ErrorMsg.StringGridRowPrase, index,column));
		}
	}
}