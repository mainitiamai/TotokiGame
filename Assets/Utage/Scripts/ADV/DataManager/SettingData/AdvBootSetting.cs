//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// ゲームの起動設定データ
	/// </summary>
	[System.Serializable]
	public partial class AdvBootSetting
	{
		//タグ　シナリオ設定ファイル
		const string TagScenarioSetting = "ScenarioSetting";
		//タグ　キャラクタ設定ファイル
		const string TagCharacterSetting = "CharacterSetting";
		//タグ　テクスチャ設定ファイル
		const string TagTextureSetting = "TextureSetting";
		//タグ　サウンド設定ファイル
		const string TagSoundSetting = "SoundSetting";
		//タグ　パラメーター設定ファイル
		const string TagParamSetting = "ParamSetting";
		//タグ　レイヤー設定ファイル
		const string TagLayerSetting = "LayerSetting";
		//タグ　シーン回想設定ファイル
		const string TagSceneGallerySetting = "SceneGallerySetting";

		[System.Serializable]
		public class DefaultDirInfo
		{
			public string defaultDir;		//デフォルトのディレクトリ
			public string defaultExt;		//デフォルトの拡張子

			public string FileNameToPath(string fileName)
			{
				if (string.IsNullOrEmpty(fileName)) return fileName;

				string path;
				//既に絶対URLならそのまま
				if (UtageToolKit.IsAbsoluteUri(fileName))
				{
					path = fileName;
				}
				else
				{
					try
					{
						//拡張子がなければデフォルト拡張子を追加
						if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fileName)))
						{
							fileName += defaultExt;
						}
						path = defaultDir + fileName;
					}
					catch (System.Exception e)
					{
						Debug.LogError(fileName + "  " + e.ToString());
						path = defaultDir + fileName;
					}
				}

				//プラットフォームが対応する拡張子にする(mp3とoggを入れ替え)
				return ExtensionUtil.ChangeSoundExt(path);
			}
		};

		/// <summary>
		/// シナリオディレクトリの情報
		/// </summary>
		public DefaultDirInfo ScenarioDirInfo { get { return scenarioDirInfo ?? (scenarioDirInfo = new DefaultDirInfo()); } }
		DefaultDirInfo scenarioDirInfo;

		/// <summary>
		/// キャラクターテクスチャのパス情報
		/// </summary>
		public DefaultDirInfo CharacterDirInfo { get { return characterDirInfo; } }
		DefaultDirInfo characterDirInfo;

		/// <summary>
		/// 背景テクスチャのパス情報
		/// </summary>
		public DefaultDirInfo BgDirInfo { get { return bgDirInfo; } }
		DefaultDirInfo bgDirInfo;

		/// <summary>
		/// イベントCGテクスチャのパス情報
		/// </summary>
		public DefaultDirInfo EventDirInfo { get { return eventDirInfo; } }
		DefaultDirInfo eventDirInfo;

		/// <summary>
		/// スプライトテクスチャのパス情報
		/// </summary>
		public DefaultDirInfo SpriteDirInfo { get { return spriteDirInfo; } }
		DefaultDirInfo spriteDirInfo;

		/// <summary>
		/// サムネイルテクスチャのパス情報
		/// </summary>
		public DefaultDirInfo ThumbnailDirInfo { get { return thumbnailDirInfo; } }
		DefaultDirInfo thumbnailDirInfo;

		/// <summary>
		/// BGMのパス情報
		/// </summary>
		public DefaultDirInfo BgmDirInfo { get { return bgmDirInfo; } }
		DefaultDirInfo bgmDirInfo;

		/// <summary>
		/// SEのパス情報
		/// </summary>
		public DefaultDirInfo SeDirInfo { get { return seDirInfo; } }
		DefaultDirInfo seDirInfo;

		/// <summary>
		/// 環境音のパス情報
		/// </summary>
		public DefaultDirInfo AmbienceDirInfo { get { return ambienceDirInfo; } }
		DefaultDirInfo ambienceDirInfo;

		/// <summary>
		/// ボイスのパス情報
		/// </summary>
		public DefaultDirInfo VoiceDirInfo { get { return voiceDirInfo; } }
		DefaultDirInfo voiceDirInfo;

		/// <summary>
		/// シナリオの設定ファイルのURLリスト
		/// </summary>
		public List<AssetFilePathInfo> ScenarioSettingUrlList { get { return this.scenarioSettingUrlList; } }
		List<AssetFilePathInfo> scenarioSettingUrlList = new List<AssetFilePathInfo>();

		/// <summary>
		/// キャラクターテクスチャの設定ファイルのURLリスト
		/// </summary>
		public List<AssetFilePathInfo> CharacterSettingUrlList { get { return this.characterSettingUrlList; } }
		List<AssetFilePathInfo> characterSettingUrlList = new List<AssetFilePathInfo>();

		/// <summary>
		/// テクスチャの設定ファイルのURLリスト
		/// </summary>
		public List<AssetFilePathInfo> TextureSettingUrlList { get { return this.textureSettingUrlList; } }
		List<AssetFilePathInfo> textureSettingUrlList = new List<AssetFilePathInfo>();

		/// <summary>
		/// サウンドの設定ファイルのURLリスト
		/// </summary>
		public List<AssetFilePathInfo> SoundSettingUrlList { get { return this.soundSettingUrlList; } }
		List<AssetFilePathInfo> soundSettingUrlList = new List<AssetFilePathInfo>();

		/// <summary>
		/// パラメーターの設定ファイルのURLリスト
		/// </summary>
		public List<AssetFilePathInfo> ParamSettingUrlList { get { return this.paramSettingUrlList; } }
		List<AssetFilePathInfo> paramSettingUrlList = new List<AssetFilePathInfo>();

		/// <summary>
		/// レイヤーの設定ファイルのURLリスト
		/// </summary>
		public List<AssetFilePathInfo> LayerSettingUrlList { get { return this.layerSettingUrlList; } }
		List<AssetFilePathInfo> layerSettingUrlList = new List<AssetFilePathInfo>();

		/// <summary>
		/// シーン回想の設定ファイルのURLリスト
		/// </summary>
		public List<AssetFilePathInfo> SceneGallerySettingUrlList { get { return this.sceneGallerySettingUrlList; } }
		List<AssetFilePathInfo> sceneGallerySettingUrlList = new List<AssetFilePathInfo>();
		
		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="resorceDir">リソースディレクトリ</param>
		public void BootInit( string resorceDir )
		{
			characterDirInfo = new DefaultDirInfo { defaultDir = @"Texture/Character", defaultExt = ".png" };
			bgDirInfo = new DefaultDirInfo { defaultDir = @"Texture/BG", defaultExt = ".jpg" };
			eventDirInfo = new DefaultDirInfo { defaultDir = @"Texture/Event", defaultExt = ".jpg" };
			spriteDirInfo = new DefaultDirInfo { defaultDir = @"Texture/Sprite", defaultExt = ".png" };
			thumbnailDirInfo = new DefaultDirInfo { defaultDir = @"Texture/Thumbnail", defaultExt = ".jpg" };
			bgmDirInfo = new DefaultDirInfo { defaultDir = @"Sound/BGM", defaultExt = ".wav" };
			seDirInfo = new DefaultDirInfo { defaultDir = @"Sound/SE", defaultExt = ".wav" };
			ambienceDirInfo = new DefaultDirInfo { defaultDir = @"Sound/Ambience", defaultExt = ".wav" };
			voiceDirInfo = new DefaultDirInfo { defaultDir = @"Sound/Voice", defaultExt = ".wav" };


			InitDefaultDirInfo(resorceDir, characterDirInfo);
			InitDefaultDirInfo(resorceDir, bgDirInfo);
			InitDefaultDirInfo(resorceDir, eventDirInfo);
			InitDefaultDirInfo(resorceDir, spriteDirInfo);
			InitDefaultDirInfo(resorceDir, thumbnailDirInfo);
			InitDefaultDirInfo(resorceDir, bgmDirInfo);
			InitDefaultDirInfo(resorceDir, seDirInfo);
			InitDefaultDirInfo(resorceDir, ambienceDirInfo);
			InitDefaultDirInfo(resorceDir, voiceDirInfo);
		}
		void InitDefaultDirInfo(string root, DefaultDirInfo info)
		{
			info.defaultDir = root + "/" + info.defaultDir + "/";
		}

		/// <summary>
		/// StringGridから基本的なデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public void InitFromStringGrid(StringGrid grid  )
		{
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
				if (row.IsEmpty) continue;								//データがない
			}
			//今のところ何も処置なし
		}

		/// <summary>
		/// CSVからデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public void InitFromCsv(StringGrid grid, string url )
		{
			string csvDir = url.Replace(System.IO.Path.GetFileName(url), "");
			scenarioDirInfo = new DefaultDirInfo { defaultDir = @"Scenario", defaultExt = ".csv" };

			InitDefaultDirInfo(csvDir, scenarioDirInfo);
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
				if (row.IsEmpty) continue;								//データがない
				ParseFromCsvStringGridRow(row, csvDir);
			}
		}

		void ParseFromCsvStringGridRow(StringGridRow row, string csvDir)
		{
			string tag = AdvParser.ParseCell<string>(row, AdvColumnName.Tag);
			switch (tag)
			{
				case TagScenarioSetting:
					AddUrltList(ScenarioSettingUrlList, row, csvDir);
					break;
				case TagCharacterSetting:
					AddUrltList(CharacterSettingUrlList, row, csvDir);
					break;
				case TagTextureSetting:
					AddUrltList(TextureSettingUrlList, row, csvDir);
					break;
				case TagSoundSetting:
					AddUrltList(SoundSettingUrlList, row, csvDir);
					break;
				case TagParamSetting:
					AddUrltList(ParamSettingUrlList, row, csvDir);
					break;
				case TagLayerSetting:
					AddUrltList(LayerSettingUrlList, row, csvDir);
					break;
				case TagSceneGallerySetting:
					AddUrltList(SceneGallerySettingUrlList, row, csvDir);
					break;
				default:
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.UnknownTag,tag));
					break;
			}
		}

		void AddUrltList(List<AssetFilePathInfo> urlList, StringGridRow row, string csvDir)
		{
			string path = AdvParser.ParseCell<string>(row, AdvColumnName.Param1);
			int version = AdvParser.ParseCell<int>(row, AdvColumnName.Version);
			urlList.Add( new AssetFilePathInfo( FileNameToPath(path, csvDir), version));
		}

		string FileNameToPath(string fileName, string csvDir )
		{
			//既に絶対URLならそのまま
			if (UtageToolKit.IsAbsoluteUri(fileName))
			{
				return fileName;
			}
			else
			{
				return csvDir + fileName;
			}
		}

		/// <summary>
		/// CSVにコンバートする際起動用のBootファイルを作成
		/// </summary>
		/// <param name="grid">エクセル側のデータ</param>
		public static StringGrid CreateOnCsvOnvert(StringGrid grid, int version)
		{
			if( grid == null )
			{
				grid = new StringGrid(AdvSettingDataManager.SheetNameBoot, CsvType.Csv);
				grid.AddRow(new List<string> { AdvParser.Localize(AdvColumnName.Tag), AdvParser.Localize(AdvColumnName.Param1), AdvParser.Localize(AdvColumnName.Version) });
			}
			///起動用データをコンバート
			AdvBootSetting.AddDefaultUrlSettingsOnCsvOnvert(grid, version);
			return grid;
		}


		/// <summary>
		/// CSVにコンバートする際に、デフォルトで使用するsettings系のURLリストを追加する
		/// </summary>
		/// <param name="grid"></param>
		static void AddDefaultUrlSettingsOnCsvOnvert( StringGrid grid, int version ){

			const string format = "Settings/{0}.csv";
			grid.AddRow(new List<string> { TagScenarioSetting, string.Format(format, AdvSettingDataManager.SheetNameScenario), ""+version });
			grid.AddRow(new List<string> { TagCharacterSetting, string.Format(format, AdvSettingDataManager.SheetNameCharacter), "" + version });
			grid.AddRow(new List<string> { TagTextureSetting, string.Format(format, AdvSettingDataManager.SheetNameTexture), "" + version });
			grid.AddRow(new List<string> { TagSoundSetting, string.Format(format, AdvSettingDataManager.SheetNameSound), "" + version });
			grid.AddRow(new List<string> { TagParamSetting, string.Format(format, AdvSettingDataManager.SheetNameParam), "" + version });
			grid.AddRow(new List<string> { TagLayerSetting, string.Format(format, AdvSettingDataManager.SheetNameLayer), "" + version });
			grid.AddRow(new List<string> { TagSceneGallerySetting, string.Format(format, AdvSettingDataManager.SheetNameSceneGallery), "" + version });
		}
	}
}