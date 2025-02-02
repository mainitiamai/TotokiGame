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
	/// 各コマンドの基底クラス
	/// </summary>
	public static class AdvCommandParser
	{
		static public Func<StringGridRow, AdvSettingDataManager, AdvCommand> CallBackCreateCustomCommnad;
		static public Func<AdvCommand, AdvCommand> CallBackCreateCustomContiunesEndCommand;

		/// <summary>
		/// コマンド生成
		/// </summary>
		/// <param name="row">行データ</param>
		/// <param name="dataManager">データマネージャー</param>
		/// <returns>生成されたコマンド</returns>
		static public AdvCommand CreateCommand(StringGridRow row, AdvSettingDataManager dataManager)
		{
			if (IsComment(row))
			{
				//コメント
				return null;
			}
			AdvCommand command = null;
			///独自のコマンド解析処理があるならそっちを先に
			if (CallBackCreateCustomCommnad != null) command = CallBackCreateCustomCommnad(row, dataManager);

			///基本のコマンド解析処理
			if (command == null) command = CreateCommandDefault(row, dataManager);


			if (command == null)
			{
				//記述ミス
				Debug.LogError(row.ToErrorString(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.CommnadParseNull)));
			}

			return command;
		}

		public const string IdError = "Error";						//構文エラー
		public const string IdComment = "Comment";					//コメント
		public const string IdCharacter = "Character";				//キャラクター＆台詞表示
		public const string IdCharacterOff = "CharacterOff";		//キャラクター表示オフ
		public const string IdText = "Text";						//テキスト表示（地の文）
		public const string IdBg = "Bg";							//背景表示・切り替え
		public const string IdBgOff = "BgOff";						//キャラクター表示オフ
		public const string IdBgEvent = "BgEvent";					//イベント絵表示・切り替え
		public const string IdBgEventOff = "BgEventOff";			//イベント絵表示
		public const string IdSprite = "Sprite";					//スプライト表示
		public const string IdSpriteOff = "SpriteOff";				//スプライト表示オフ

		public const string IdSe = "Se";							//SE再生
		public const string IdBgm = "Bgm";							//BGM再生
		public const string IdStopBgm = "StopBgm";					//BGM停止
		public const string IdAmbience = "Ambience";				//環境音再生
		public const string IdStopAmbience = "StopAmbience";		//環境音停止
		public const string IdStopSound = "StopSound";				//サウンドの停止

		public const string IdSelection = "Selection";				//選択肢表示
		public const string IdSelectionEnd = "SelectionEnd";		//選択肢追加終了
		public const string IdJump = "Jump";						//他シナリオにジャンプ
		public const string IdEffect = "Effect";					//エフェクト表示

		public const string IdWait = "Wait";						//一定時間のウェイト
		public const string IdTween = "Tween";						//Tweenアニメ
		public const string IdFadeIn = "FadeIn";					//フェードイン
		public const string IdFadeOut = "FadeOut";					//フェードアウト

		public const string IdParam = "Param";						//パラメーター代入
		public const string IdIf = "If";							//パラメーター代入
		public const string IdElseIf = "ElseIf";					//パラメーター代入
		public const string IdElse = "Else";						//パラメーター代入
		public const string IdEndIf = "EndIf";						//パラメーター代入

		public const string IdSendMessage = "SendMessage";			//SendMessage処理（各ゲームに固有の処理のために）

		public const string IdEndScenario = "EndScenario";			//シナリオ終了
		public const string IdScenarioLabel = "ScenarioLabel";		//シナリオラベル		
		public const string IdEndSceneGallery = "EndSceneGallery";	//シーン回想終了

		public const string IdStopScenario = "StopScenario";	//シナリオの停止
		public const string IdEraseScenario = "EraseScenario";	//シナリオの削除

		/// <summary>
		/// 基本のコマンド生成処理
		/// </summary>
		/// <param name="row">行データ</param>
		/// <param name="dataManager">データマネージャー</param>
		/// <returns>生成されたコマンド</returns>
		static public AdvCommand CreateCommandDefault( StringGridRow row, AdvSettingDataManager dataManager)
		{
			string id = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Command, "");
			switch (id)
			{
				case IdText:
					return new AdvCommandText(row);
				case IdCharacterOff:
					return new AdvCommandCharacterOff(row);
				case IdBg:
					return new AdvCommandBg(row, dataManager);
				case IdBgOff:
					return new AdvCommandBgOff(row);
				case IdBgEvent:
					return new AdvCommandBgEvent(row, dataManager);
				case IdBgEventOff:
					return new AdvCommandBgEventOff(row);
				case IdSprite:
					return new AdvCommandSprite(row, dataManager);
				case IdSpriteOff:
					return new AdvCommandSpriteOff(row);

				case IdTween:
					return new AdvCommandTween(row, dataManager);
				case IdFadeIn:
					return new AdvCommandFadeIn(row);
				case IdFadeOut:
					return new AdvCommandFadeOut(row);


				case IdSe:
					return new AdvCommandSe(row, dataManager);
				case IdBgm:
					return new AdvCommandBgm(row, dataManager);
				case IdStopBgm:
					return new AdvCommandStopBgm(row);
				case IdAmbience:
					return new AdvCommandAmbience(row, dataManager);
				case IdStopAmbience:
					return new AdvCommandStopAmbience(row);
				case IdStopSound:
					return new AdvCommandStopSound(row);

				case IdWait:
					return new AdvCommandWait(row);

				case IdParam:
					return new AdvCommandParam(row, dataManager);
				case IdSelection:
					return new AdvCommandSelection(row, dataManager);
				case IdJump:
					return new AdvCommandJump(row, dataManager);
				case IdIf:
					return new AdvCommandIf(row, dataManager);
				case IdElseIf:
					return new AdvCommandElseIf(row, dataManager);
				case IdElse:
					return new AdvCommandElse(row);
				case IdEndIf:
					return new AdvCommandEndIf(row);

				case IdSendMessage:
					return new AdvCommandSendMessage(row);
				case IdEndScenario:
					return new AdvCommandEndScenario(row);
				case IdEndSceneGallery:
					return new AdvCommandEndSceneGallery(row);

				case IdStopScenario:
					return new AdvCommandStopScenario(row);
				case IdEraseScenario:
					return new AdvCommandEraseScenario(row);


				default:
					return CreateCommandNoCommand(row, dataManager);
			}
		}

		/// <summary>
		/// コマンド名なしのデータからコマンド生成
		/// </summary>
		/// <param name="row">行データ</param>
		/// <param name="dataManager">データマネージャー</param>
		/// <returns>生成されたコマンド</returns>
		static AdvCommand CreateCommandNoCommand(StringGridRow row, AdvSettingDataManager dataManager)
		{

			string command = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Command, "");
			if (command == "")
			{
				//コマンドなしは、テキスト表示
				string arg1 = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg1, "");
				if (!string.IsNullOrEmpty(arg1))
				{
					//パラメーターつきなので、キャラの台詞
					return new AdvCommandCharacter(row, dataManager);
				}
				string text = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Text, "");
				if (text.Length > 0)
				{
					//地の文
					return new AdvCommandText(row);
				}
				else
				{	//なにもないので空データ
					return null;
				}
			}
			else
			{
				if (IsScenarioLabel(command))
				{
					//シナリオラベル
					return new AdvCommandScenarioLabel(row);
				}
				else
				{
					//記述ミス
					return null;
				}
			}
		}

		//連続コマンドの終了コマンドを取得
		static public AdvCommand CreateContiunesEndCommand(AdvCommand last)
		{
			AdvCommand command = null;
			///独自のコマンド解析処理があるならそっちを先に
			if (CallBackCreateCustomContiunesEndCommand != null) command = CallBackCreateCustomContiunesEndCommand(last);
			///基本のコマンド解析処理
			if (command == null) command = CreateDefaultContiunesEndCommand(last);

			return command;
		}

		//連続コマンドの終了コマンドを取得
		static public AdvCommand CreateDefaultContiunesEndCommand(AdvCommand last)
		{
			Type type = last.GetType();
			if( type == typeof(AdvCommandSelection) )
			{
				return new AdvCommandSelectionEnd();
			}
			else
			{
				return null;
			}
		}

		// シナリオラベルかチェック
		static bool IsScenarioLabel(string str)
		{
			return ( !string.IsNullOrEmpty(str) && str.Length > 2 && (str[0] == '*'));
		}

		// シナリオラベルを解析
		static public string ParseScenarioLabel(StringGridRow row, AdvColumnName columnName)
		{
			string label = AdvParser.ParseCell<string>(row, columnName);
			if (!IsScenarioLabel(label)) Debug.LogError(row.ToErrorString(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotScenarioLabel)));
			return label.Substring(1);
		}

		//コメントのコマンドかチェック
		static bool IsComment(StringGridRow row)
		{
			string command = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Command, "");
			if( string.IsNullOrEmpty(command) )
			{
				return false;
			}
			else if (command == IdComment)
			{
				return true;
			}
			else if (command.Substring(0, 2) == "//")
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}