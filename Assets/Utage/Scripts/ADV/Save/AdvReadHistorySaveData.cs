//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.IO;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// ゲームで共通して使う既読データ
	/// </summary>
	public class AdvReadHistorySaveData
	{

		DictionaryInt data = new DictionaryInt();
		const int VERSION = 0;

		/// <summary>
		/// 既読ページ追加
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <param name="pageNo">ページ番号</param>
		public void AddReadPage(string scenarioLabel, int page)
		{
			DictionaryKeyValueInt pageNo;
			if (data.TryGetValue(scenarioLabel, out pageNo))
			{
				if (pageNo.value < page)
				{
					pageNo.value = page;
				}
				else
				{
					//既読なのでそのまま
				}
			}
			else
			{
				data.Add(new DictionaryKeyValueInt(scenarioLabel, page));
			}
		}

		/// <summary>
		/// 既読チェック
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <param name="pageNo">ページ番号</param>
		/// <returns>既読ならtrue。そうでないならfalse</returns>
		public bool CheckReadPage(string scenarioLabel, int pageNo)
		{
			DictionaryKeyValueInt page;
			if (data.TryGetValue(scenarioLabel, out page))
			{
				if (pageNo <= page.value)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// バイナリ読み込み
		/// </summary>
		/// <param name="reader">バイナリリーダー</param>
		public void Read(BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version == VERSION)
			{
				data.Read(reader);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}

		/// <summary>
		/// バイナリ書き込み
		/// </summary>
		/// <param name="writer">バイナリライター</param>
		public void Write(BinaryWriter writer)
		{
			writer.Write(VERSION);
			data.Write(writer);
		}
	}
}