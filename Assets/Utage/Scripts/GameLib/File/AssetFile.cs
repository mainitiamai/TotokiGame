//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


using UnityEngine;

namespace Utage
{

	/// <summary>
	/// ファイルタイプ
	/// </summary>
	public enum AssetFileType
	{
		/// <summary>テキスト</summary>
		Text,
		/// <summary>バイナリ</summary>
		Bytes,
		/// <summary>テクスチャ</summary>
		Texture,
		/// <summary>サウンド</summary>
		Sound,
		/// <summary>CSVファイル（テキストファイルの拡張）</summary>
		Csv,
		/// <summary>アセットバンドル</summary>
		ASSET_BUNDLE,
	};

	/// <summary>
	/// ファイルのおき場所のタイプ
	/// </summary>
	public enum AssetFileStrageType
	{
		/// <summary>WEB（一度ＤＬしたものは、デバイスストレージにキャッシュする）</summary>
		Web,
		/// <summary>WEB（デバイスストレージにキャッシュしない）</summary>
		WebNocache,				//
		/// <summary>ストリーミングアセット</summary>
		StreamingAssets,
		/// <summary>リソース</summary>
		Resources,
	};

	/// <summary>
	/// 暗号化のタイプ
	/// </summary>
	public enum AssetFileCryptType
	{
		/// <summary>暗号化なし</summary>
		None,					//
		/// <summary>「宴」組み込みのものを使う</summary>
		Utage,
		/// <summary>独自カスタム</summary>
		Custom,
	};

	/// <summary>
	/// ロードする際のフラグ
	/// </summary>
	[System.Flags]
	public enum AssetFileLoadFlags
	{
		/// <summary>なにもなし</summary>
		None = 0x00,
		/// <summary>ストリーミングでロードする</summary>
		Streaming = 0x01,
		/// <summary>3Dサウンドとしてロードする</summary>
		Audio3D = 0x02,
		/// <summary>テクスチャにミップマップを使う</summary>
		TextureMipmap = 0x04,
		/// <summary>CSVをロードする際にTSV形式でロードする</summary>
		Csv = 0x08,
	};

	/// <summary>
	/// スプライト情報
	/// </summary>
	[System.Serializable]
	public class AssetFileSpriteInfo
	{
		public Vector2 pivot = new Vector2(0.5f, 0.5f);
		public float scale = 1.0f;
	};



	/// <summary>
	/// ファイルのインターフェース
	/// </summary>
	public interface AssetFile
	{
		/// <summary>ファイル名</summary>
		string FileName { get; }

		/// <summary>ファイルタイプ</summary>
		AssetFileType FileType { get; }

		/// <summary>ロード終了したか</summary>
		bool IsLoadEnd { get; }

		/// <summary>ロードエラーしたか</summary>
		bool IsLoadError { get; }

		/// <summary>ロードエラーメッセージ</summary>
		string LoadErrorMsg { get; }

		/// <summary>ストリーム再生ができるか</summary>
		bool IsReadyStreaming { get; }

		/// <summary>ロードしたバイナリ</summary>
		byte[] Bytes { get; }

		/// <summary>ロードしたテクスチャ</summary>
		Texture2D Texture { get; }

		/// <summary>ロードしたサウンド</summary>
		AudioClip Sound { get; }

		/// <summary>ロードしたCSV</summary>
		StringGrid Csv { get; }

		/// <summary>
		/// ロードしたテクスチャから作ったスプライトを取得
		/// </summary>
		/// <param name="pixelsToUnits"></param>
		/// <returns></returns>
		Sprite GetSprite(float pixelsToUnits);

		/// <summary>
		/// バージョン
		/// </summary>
		int Version { get; set; }

		/// <summary>
		/// キャッシュファイルのバージョン
		/// </summary>
		int CacheVersion { get; }

		/// <summary>
		/// ロードフラグ
		/// </summary>
		AssetFileLoadFlags LoadFlags { get; }

		/// <summary>
		/// ロードフラグを追加
		/// </summary>
		void AddLoadFlag(AssetFileLoadFlags flags);

		/// <summary>
		/// スプライト情報
		/// </summary>
		AssetFileSpriteInfo SpriteInfo { get; set; }

		/// <summary>
		/// オブジェクトがファイルを使用することを宣言（参照を設定する）
		/// </summary>
		/// <param name="obj">使用するオブジェクト</param>
		void Use(System.Object obj);

		/// <summary>
		/// オブジェクトがファイルを使用することをやめる（参照を解除する）
		/// </summary>
		/// <param name="obj">使用をやめるオブジェクト</param>
		void Unuse(System.Object obj);

		/// <summary>
		/// Gameオブジェクトに、このファイルの参照コンポーネントを追加
		/// これを使った後は、GameオブジェクトがDestoryされると自動的に、参照が解除される
		/// </summary>
		/// <param name="go">参照をするGameObject</param>
		void AddReferenceComponet(GameObject go);
	};
}