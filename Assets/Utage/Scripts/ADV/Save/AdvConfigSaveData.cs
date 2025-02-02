//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.IO;
using UnityEngine;

namespace Utage
{
	/// <summary>
	/// 音声の止め方の
	/// </summary>
	public enum VoiceStopType
	{
		/// <summary>次の音声まで再生を続ける</summary>
		OnNextVoice,
		/// <summary>クリックで停止</summary>
		OnClick,
	};

	/// <summary>
	/// コンフィグ用のセーブデータ
	/// </summary>
	[System.Serializable]
	public class AdvConfigSaveData
	{
		/// <summary>フルスクリーン切り替え</summary>
		public bool isFullScreen;
		/// <summary>マウスホイールでメッセージ送り切り替えるか</summary>
		public bool isMouseWheelSendMessage = true;
		/// <summary>エフェクトON・OFF切り替え</summary>
		public bool isEffect = true;
		/// <summary>未読スキップON・OFF切り替え</summary>
		public bool isSkipUnread;
		/// <summary>選択肢でスキップ解除ON・OFF切り替え</summary>
		public bool isStopSkipInSelection = true;
		/// <summary>文字送り速度</summary>
		public float messageSpeed = 0.5f;
		/// <summary>オート改ページ速度</summary>
		public float autoBrPageSpeed = 0.5f;
		/// <summary>メッセージウィンドウの透過色</summary>
		public float messageWindowTransparency = 0.3f;
		/// <summary>音量設定 サウンド全体</summary>
		public float soundMasterVolume = 1.0f;
		/// <summary>音量設定 BGM</summary>
		public float bgmVolume = 0.5f;
		/// <summary>音量設定 SE</summary>
		public float seVolume = 0.5f;
		/// <summary>音量設定 環境音</summary>
		public float ambienceVolume = 0.5f;
		/// <summary>音量設定 ボイス</summary>
		public float voiceVolume = 0.75f;
		/// <summary>音声設定</summary>
		public VoiceStopType voiceStopType;
		/// <summary>ボイスON・OFF個別に切り替え</summary>
		public bool[] isVoiceActiveArray;
		/// <summary>オート改ページ</summary>
		public bool isAutoBrPage;

		const int VERSION = 0;

		/// <summary>
		/// バイナリ読み込み
		/// </summary>
		/// <param name="reader">バイナリリーダー</param>
		public void Read(BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version == VERSION)
			{
				isFullScreen = reader.ReadBoolean();
				isMouseWheelSendMessage = reader.ReadBoolean();
				isEffect = reader.ReadBoolean();
				isSkipUnread = reader.ReadBoolean();
				isStopSkipInSelection = reader.ReadBoolean();
				messageSpeed = reader.ReadSingle();
				autoBrPageSpeed = reader.ReadSingle();
				messageWindowTransparency = reader.ReadSingle();
				soundMasterVolume = reader.ReadSingle();
				bgmVolume = reader.ReadSingle();
				seVolume = reader.ReadSingle();
				ambienceVolume = reader.ReadSingle();
				voiceVolume = reader.ReadSingle();
				voiceStopType = (VoiceStopType)(reader.ReadInt32());
				int num = reader.ReadInt32();
				isVoiceActiveArray = new bool[num];
				for (int i = 0; i < num; i++)
				{
					isVoiceActiveArray[i] = reader.ReadBoolean();
				}
				isAutoBrPage = reader.ReadBoolean();
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

			writer.Write(isFullScreen);
			writer.Write(isMouseWheelSendMessage);
			writer.Write(isEffect);
			writer.Write(isSkipUnread);
			writer.Write(isStopSkipInSelection);
			writer.Write(messageSpeed);
			writer.Write(autoBrPageSpeed);
			writer.Write(messageWindowTransparency);
			writer.Write(soundMasterVolume);
			writer.Write(bgmVolume);
			writer.Write(seVolume);
			writer.Write(ambienceVolume);
			writer.Write(voiceVolume);
			writer.Write((int)voiceStopType);
			writer.Write(isVoiceActiveArray.Length);
			foreach (bool isVoiceActive in isVoiceActiveArray)
			{
				writer.Write(isVoiceActive);
			}
			writer.Write(isAutoBrPage);
		}

	}
}