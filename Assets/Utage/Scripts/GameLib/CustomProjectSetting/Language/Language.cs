//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// 表示言語切り替え用のクラス
	/// </summary>
	public class Language
	{
		/// <summary>
		/// 現在の言語
		/// </summary>
		public string CurrentLanguage { get { return currentLanguage; } }
		string currentLanguage;

		/// <summary>
		/// 対応する言語リスト
		/// </summary>
		public List<string> Languages { get { return languages; } }
		List<string> languages = new List<string>();

		//言語による表示テキストデータ
		StringGrid data;
		Dictionary<string, StringGridRow> dataTbl = new Dictionary<string, StringGridRow>();

		public Language( TextAsset csv )
		{
			//データ解析
			ParseData(csv);
		}

		/// <summary>
		/// 指定の言語に変更
		/// </summary>
		/// <param name="language">指定する言語</param>
		/// <returns>変更できればture。対応していなければfalse</returns>
		public bool TryChangeCurrentLanguage(string language)
		{
			if (languages.Contains(language))
			{
				currentLanguage = language;
				return true;
			}
			return false;
		}

		/// <summary>
		/// キーがあるか
		/// </summary>
		/// <param name="key">テキストのキー</param>
		/// <returns>あればtrue。なければfalse</returns>
		public bool ContainsKey(string key)
		{
			return dataTbl.ContainsKey(key);
		}
		
		/// <summary>
		/// キーから設定言語のテキストを取得
		/// </summary>
		/// <param name="key">テキストのキー</param>
		/// <returns>対応言語のテキスト</returns>
		public string LocalizeText(string key)
		{
			StringGridRow val;
			if (!dataTbl.TryGetValue(key, out val))
			{
				Debug.LogError(key + ": is not found in Language data");
				return key;
			}
			else
			{
				if (CurrentLanguage == null)
				{
					return key;
				}
				else
				{
					return val.ParseCell<string>(CurrentLanguage);
				}
			}
		}

		void ParseData(TextAsset csv)
		{
			dataTbl.Clear();
			//Debug.Log(csv.name);
			// ここのデータはTSV形式で読み込ませる・・・ってか拡張子でわけろよ・・・
			StringGrid data = new StringGrid(csv.name, CsvType.Tsv, csv.text);
			if (data.Rows.Count <= 0) return;

			foreach (StringGridRow row in data.Rows)
			{
				if (row.IsEmpty) continue;
				dataTbl.Add(row.ParseCell<string>("Key"), row);
			}

			StringGridRow header = data.Rows[0];
			for (int i = 0; i < header.Length; ++i)
			{
				if (i == 0) continue;
				if (string.IsNullOrEmpty(header.Strings[i])) continue;
				languages.Add(header.Strings[i]);
			}
		}
	}
}