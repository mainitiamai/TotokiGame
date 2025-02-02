//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// 拡張子に関する制御
	/// </summary>
	public static class ExtensionUtil
	{
//		public const string Utage = ".utage";
		
		public const string Ogg = ".ogg";
		public const string Mp3 = ".mp3";
		public const string Wav = ".wav";

		public const string CSV = ".csv";
		public const string TSV = ".tsv";

		public static bool IsTsv( string path )
		{
			return System.IO.Path.GetExtension(path).ToLower() == TSV;
		}

		public static bool IsCsv( string path )
		{
			return System.IO.Path.GetExtension(path).ToLower() == CSV;
		}

		/// <summary>
		/// 指定の拡張子かチェック
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="ext">拡張子</param>
		/// <returns>指定の拡張子ならtrue。違えばfalse</returns>
		public static bool CheckExtention( string path, string ext )
		{
			return System.IO.Path.GetExtension(path).ToLower() == ext;
		}

		/// <summary>
		/// オーディオのタイプを取得
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <returns>オーディオのタイプ</returns>
		public static AudioType GetAudioType(string path)
		{
			string ext = System.IO.Path.GetExtension(path).ToLower();
			switch (ext)
			{
				case Mp3:
					return AudioType.MPEG;
				case Ogg:
					return AudioType.OGGVORBIS;
				default:
					return AudioType.WAV;
			}
		}

		/// <summary>
		/// WebPlayer、StandAloneではOggが対応。MOBILEはMP3が非対応なので、拡張子を入れ替える
		/// http://docs-jp.unity3d.com/Documentation/ScriptReference/WWW.GetAudioClip.html
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <returns>対応するサウンドの拡張子を入れ替えたファイルパス</returns>
		public static string ChangeSoundExt(string path)
		{
			string ext = System.IO.Path.GetExtension(path).ToLower();
			switch (ext)
			{
				case Ogg:
					if (!IsSupportOggPlatform())
					{
						return System.IO.Path.ChangeExtension(path, Mp3);
					}
					break;
				case Mp3:
					if (IsSupportOggPlatform())
					{
						return System.IO.Path.ChangeExtension(path, Ogg);
					}
					break;
				default:
					break;
			}
			return path;
		}

		/// <summary>
		/// Oggをサポートしているプラットフォームかどうか
		/// WebPlayer、StandAloneではOggが対応。MOBILEはMP3が対応なので、拡張子を入れ替える
		/// http://docs-jp.unity3d.com/Documentation/ScriptReference/WWW.GetAudioClip.html
		/// </summary>
		/// <returns>サポートしていればtrue</returns>
		public static bool IsSupportOggPlatform()
		{
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			return true;
#else
			if( Application.isWebPlayer || UtageToolKit.IsPlatformStandAlone() )
			{	//無料版用にDLL化したときのため
				return true;
			}
			return false;
#endif
		}
	}
}