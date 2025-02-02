//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{

	/// <summary>
	/// キャラクタのテクスチャ設定（名前や表情と、テクスチャ名の対応）
	/// </summary>
	[System.Serializable]
	public partial class AdvCharacterSettingData : SerializableDictionaryFileReadKeyValue
	{
		/// <summary>
		/// 表示名のテキスト
		/// </summary>
		public string NameText { get { return this.nameText; } }
		[SerializeField]
		string nameText;

		/// <summary>
		/// ファイル名
		/// </summary>
		[SerializeField]
		string fileName;

		/// <summary>
		/// ファイルパス
		/// </summary>
		public string FilePath { get { return this.filePath; } }		
		string filePath;

		/// <summary>
		/// ファイルバージョン
		/// </summary>
		public int Version { get { return this.version; } }
		[SerializeField]
		int version;

		/// <summary>
		/// ピボット位置
		/// </summary>
		public Vector2 Pivot { get { return this.pivot; } }
		[SerializeField]
		Vector2 pivot;

		/// <summary>
		/// スケール
		/// </summary>
		public float Scale { get { return this.scale; } }
		[SerializeField]
		float scale;


		/// <summary>
		/// StringGridの一行からデータ初期化
		/// ただし、このクラスに限っては未使用
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public override bool InitFromStringGridRow(StringGridRow row)
		{
			Debug.LogError("Not Use");
			return false;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="key">キー(キャラ名を)</param>
		/// <param name="fileName">ファイルネーム</param>
		/// <param name="version">ファイルバージョン</param>
		public void Init(string key, string nameText, Vector2 pivot, float scale, string fileName, int version)
		{
			this.InitKey(key);
			this.nameText = nameText;
			this.pivot = pivot;
			this.scale = scale;
			this.fileName = fileName;
			this.version = version;
		}

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit(AdvBootSetting settingData)
		{
			filePath = settingData.CharacterDirInfo.FileNameToPath(fileName);
		}
	};

	/// <summary>
	/// キャラクタのテクスチャ設定（名前や表情と、テクスチャ名の対応）
	/// </summary>
	[System.Serializable]
	public partial class AdvCharacterSetting : SerializableDictionaryFileRead<AdvCharacterSettingData>
	{
		/// <summary>
		/// 各キャラのデフォルト表情のキーの一覧
		/// </summary>
		[SerializeField]
		DictionaryString defaultKey = new DictionaryString(); 

		/// <summary>
		/// StringGridのデータ解析
		/// </summary>
		/// <param name="grid">解析するデータ</param>
		protected override void ParseFromStringGrid(StringGrid grid)
		{
			defaultKey.Clear();
			string name = "";
			string nameText = "";
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
				if (row.IsEmpty) continue;								//データがない
				//名前は空白なら、直前のものと同じ
				name = AdvParser.ParseCellOptional<string>(row,AdvColumnName.CharacterName, name);
				string key = ToFileKey(name, AdvParser.ParseCellOptional<string>(row, AdvColumnName.Pattern, ""));

				//表示名は空白なら、直前のものと同じ
				nameText = AdvParser.ParseCellOptional<string>(row, AdvColumnName.NameText, nameText);

				Vector2 pivot = Vector2.one * 0.5f;
				try
				{
					pivot = PivotUtil.ParsePivotOptional(AdvParser.ParseCellOptional<string>(row, AdvColumnName.Pivot, ""), pivot);
				}
				catch( System.Exception e )
				{
					Debug.LogError(row.ToErrorString(e.Message));
				}
				//データ追加
				AdvCharacterSettingData data = new AdvCharacterSettingData();
				data.Init(key,
					string.IsNullOrEmpty(nameText) ? name : nameText,
					pivot,
					AdvParser.ParseCellOptional<float>(row,AdvColumnName.Scale,1.0f),
					AdvParser.ParseCell<string>(row,AdvColumnName.FileName),
					AdvParser.ParseCellOptional<int>(row,AdvColumnName.Version, 0));
				Add(data);
				if (!defaultKey.ContainsKey(name))
				{
					defaultKey.Add(name, key);
				}
			}
		}

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit(AdvBootSetting settingData)
		{
			foreach (AdvCharacterSettingData data in List)
			{
				data.BootInit(settingData);
				AssetFile file = AssetFileManager.GetFileCreateIfMissing(data.FilePath);
				file.Version = data.Version;
				file.SpriteInfo.pivot = data.Pivot;
				file.SpriteInfo.scale = data.Scale;
			}
		}

		/// <summary>
		/// 全てのリソースをダウンロード
		/// </summary>
		public void DownloadAll()
		{
			foreach (AdvCharacterSettingData data in List)
			{
				AssetFileManager.Download(data.FilePath);
			}
		}


		/// <summary>
		/// 指定のキャラ名の立ち絵があるか
		/// </summary>
		/// <param name="name">キャラ名</param>
		/// <returns>ファイルパス</returns>
		public bool Contains(string name)
		{
			return defaultKey.ContainsKey(name);
		}

		/// <summary>
		/// 指定のキャラ名の立ち絵があるか
		/// </summary>
		/// <param name="name">キャラ名</param>
		/// <param name="label">ラベル</param>
		/// <returns>ファイルパス</returns>
		public bool Contains(string name, string label)
		{
			if (!defaultKey.ContainsKey(name))
			{
				return false;
			}
			else
			{
				string key = ToFileKey(name, label);
				return this.ContainsKey(key);
			}
		}

		/// <summary>
		/// キャラのデフォルトファイルパスを取得
		/// </summary>
		/// <param name="name">キャラ名</param>
		/// <returns>ファイルパス</returns>
		public string GetDefaultPath(string name )
		{
			if (!defaultKey.ContainsKey(name))
			{
				Debug.LogError("Not found default texture :" + name );
				return "";
			}
			string key = defaultKey.Get(name);
			return FindData(key).FilePath;
		}


		/// <summary>
		/// ラベルからファイルパスを取得
		/// </summary>
		/// <param name="name">キャラ名</param>
		/// <param name="label">ラベル</param>
		/// <returns>ファイルパス</returns>
		public string LabelToPath(string name, string label)
		{
			//既に絶対URLならそのまま
			if (UtageToolKit.IsAbsoluteUri(label))
			{
				return label;
			}
			else
			{
				string key = ToFileKey(name, label);
				AdvCharacterSettingData data = FindData(key);
				if (data == null)
				{
					//ラベルをそのままファイル名扱いに
					return label;
				}
				else
				{
					return data.FilePath;
				}
			}
		}


		public void GetCharacterInfo(string name, string textureLabel, string textureLabelOff, out string path, out string nameText, out string errorMsg)
		{
			if (!Contains(name))
			{
				path = "";
				nameText = name;
				errorMsg = "";
				return;
			}
			else
			{
				//デフォルト設定
				string key = defaultKey.Get(name);
				AdvCharacterSettingData data = FindData(key);
				path = data.FilePath;
				nameText = string.IsNullOrEmpty(data.NameText) ? name : data.NameText;
				errorMsg = "";
			}

			if (textureLabel == textureLabelOff)
			{
			}
			else if (string.IsNullOrEmpty(textureLabel))
			{
			}
			else
			{
				//既に絶対URLならそのまま
				if (UtageToolKit.IsAbsoluteUri(textureLabel))
				{
					path = textureLabel;
				}
				else
				{
					string key = ToFileKey(name, textureLabel);
					AdvCharacterSettingData data = FindData(key);
					if (data == null)
					{
						errorMsg = name + ", " + textureLabel + " is not contained in file setting";
						//ラベルをそのままファイル名扱いに
						path = textureLabel;
					}
					else
					{
						path = data.FilePath;
						nameText = string.IsNullOrEmpty(data.NameText) ? name : data.NameText;
					}
				}
			}
		}


		//キーからファイルデータを取得
		AdvCharacterSettingData FindData(string key)
		{
			AdvCharacterSettingData data;
			if (!TryGetValue(key, out data))
			{
				return null;
			}
			else
			{
				return data;
			}
		}

		//キーの変更
		string ToFileKey(string name, string label)
		{
			//名前とラベルからキーを
			string key = string.Format(
				"{0},{1}",
				name,
				label
				);
			return key;
		}
	}
}