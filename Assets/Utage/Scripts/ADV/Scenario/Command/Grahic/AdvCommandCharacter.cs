//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：キャラクター＆台詞表示
	/// </summary>
	internal class AdvCommandCharacter : AdvCommand
	{

		public AdvCommandCharacter(StringGridRow row, AdvSettingDataManager dataManager)
		{
			this.name = AdvParser.ParseCell<string>(row, AdvColumnName.Arg1);
			//表示テクスチャ
			this.textureLabel = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg2, "");
			//表示レイヤー
			this.layerName = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName, AdvLayerSettingData.LayerType.Character ))
			{
				Debug.LogError(row.ToErrorString(name + ", " + layerName + " is not contained in layer setting"));
			}

			//表示位置
			float x;
			if (AdvParser.TryParseCell<float>(row, AdvColumnName.Arg4, out x))
			{
				this.x = x;
			}
			float y;
			if (AdvParser.TryParseCell<float>(row, AdvColumnName.Arg5, out y))
			{
				this.y = y;
			}
			//フェード時間
			this.fadeTime = AdvParser.ParseCellOptional<float>(row, AdvColumnName.Arg6, 0.2f);


			//テキスト関連
			this.text = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Text, "");
			if (AdvCommand.IsEditorErrorCheck)
			{
				TextData textData = new TextData(text);
				if (!string.IsNullOrEmpty(textData.ErrorMsg))
				{
					Debug.LogError( row.ToErrorString(textData.ErrorMsg) );
				}
			}
			//ボイス
			string voice = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Voice, "");
			int voiceVersion = AdvParser.ParseCellOptional<int>(row, AdvColumnName.VoiceVersion, 0);

			string path, errorMsg;
			dataManager.CharacterSetting.GetCharacterInfo(name, textureLabel, textureLabelOff, out path, out nameText, out errorMsg);

			if (!string.IsNullOrEmpty(errorMsg))
			{
				Debug.LogError(row.ToErrorString(errorMsg));

			}
			if (!string.IsNullOrEmpty(path))
			{
				textureFile = AddLoadFile(path);
			}


			//サウンドファイル設定
			if (!string.IsNullOrEmpty(voice))
			{
				if (AdvCommand.IsEditorErrorCheck)
				{
				}
				else
				{
					voiceFile = AddLoadFile(dataManager.SettingData.VoiceDirInfo.FileNameToPath(voice));
					if (voiceFile != null) voiceFile.Version = voiceVersion;
					//ストリーミング再生にバグがある模様。途中で無音が混じると飛ばされる？
					//				voiceFile.LoadFlags = AssetFileLoadFlags.Streaming;
				}
			}
		}
		public override void DoCommand(AdvEngine engine)
		{
			if (engine.LayerManager.IsEventMode)
			{
			}
			else
			{
				if (textureLabel == textureLabelOff)
				{
					engine.LayerManager.CharacterFadeOut(name, fadeTime);
				}
				else
				{
					if( string.IsNullOrEmpty(textureLabel) )
					{
						if (!engine.LayerManager.SetCharacterSprite(layerName, name, null, x, y, fadeTime) )
						{
							engine.LayerManager.SetCharacterSprite(layerName, name, textureFile, x, y, fadeTime);
						}
					}
					else
					{
						engine.LayerManager.SetCharacterSprite(layerName, name, textureFile, x, y, fadeTime);
					}
				}
			}

			if (null != voiceFile)
			{
				engine.SoundManager.Play(SoundManager.StreamType.Voice, voiceFile, false, true);
			}
			if (text.Length > 0)
			{
				engine.Page.SetCharacterText(text, nameText);
				engine.BacklogManager.Add(text, nameText, voiceFile);
			}
		}
		public override bool Wait(AdvEngine engine)
		{
			bool isWait = engine.Page.IsShowingText;
			if (!isWait)
			{
				if (null != voiceFile && engine.Config.VoiceStopType == VoiceStopType.OnClick)
				{
					engine.SoundManager.Stop(SoundManager.StreamType.Voice);
				}
			}
			return isWait;
		}

		//ページ区切りのコマンドか
		public override bool IsTypePageEnd() { return (text.Length > 0); }

		string name;
		string nameText;
		string layerName;
		string textureLabel;
		string text;
		AssetFile textureFile;
		AssetFile voiceFile;
		object x = null;
		object y = null;
		float fadeTime;

		const string textureLabelOff = "<Off>";
	}

}