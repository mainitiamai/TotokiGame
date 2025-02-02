//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// 便利クラス
	/// </summary>
	public class UtageToolKit
	{
		/// <summary>
		/// SendMessageをする。ただし、targetやfuncがnullだった場合何もしない
		/// </summary>
		/// <param name="target">メッセージの送り先のGameObject</param>
		/// <param name="functionName">送信するメッセージ</param>
		/// <param name="isForceActive">送り先のGameObjectを強制的にactiveにしてからSendMessageするか</param>
		public static void SafeSendMessage(GameObject target, string functionName, bool isForceActive )
		{
			if (null != target && !string.IsNullOrEmpty(functionName))
			{
				if (isForceActive) target.SetActive(true);
				target.SendMessage(functionName, SendMessageOptions.DontRequireReceiver);
			}
		}
		public static void SafeSendMessage(GameObject target, string functionName )
		{
			SafeSendMessage(target, functionName, false);
		}


		/// <summary>
		/// SendMessageをする。ただし、targetやfuncがnullだった場合何もしない
		/// </summary>
		/// <param name="obj">メッセージ送信の際に引数として持たせるオブジェクト</param>
		/// <param name="target">メッセージの送り先のGameObject</param>
		/// <param name="functionName">送信するメッセージ</param>
		/// <param name="isForceActive">送り先のGameObjectを強制的にactiveにしてからSendMessageするか</param>
		public static void SafeSendMessage(System.Object obj, GameObject target, string functionName, bool isForceActive )
		{
			if (null != target && !string.IsNullOrEmpty(functionName))
			{
				if (isForceActive) target.SetActive(true);
				target.SendMessage(functionName, obj, SendMessageOptions.DontRequireReceiver);
			}
		}
		public static void SafeSendMessage(System.Object obj, GameObject target, string functionName)
		{
			SafeSendMessage(obj, target, functionName, false);
		}


		/// <summary>
		/// 子要素の全削除
		/// </summary>
		/// <param name="parent">親要素</param>
		public static void DestroyChildren(Transform parent)
		{
			while (parent.childCount > 0)
			{
				Transform child = parent.GetChild(0);
				child.parent = null;
				GameObject.Destroy(child.gameObject);
			}
		}

		/// <summary>
		/// nullチェックした上でコンポーネントのGameObjectを破棄
		/// </summary>
		/// <param name="obj">破棄するGameObjectを持つコンポーネント</param>
		public static void SafeDestroy(MonoBehaviour obj)
		{
			if (null != obj)
			{
				GameObject.Destroy(obj.gameObject);
				obj = null;
			}
		}

		/// <summary>
		/// nullチェックした上でGameObjectをDestory
		/// </summary>
		/// <param name="go">破棄するGameObject</param>
		public static void SafeDestroy(GameObject go)
		{
			if (null != go)
			{
				GameObject.Destroy(go);
				go = null;
			}
		}

		/// <summary>
		/// 子の追加
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="go">子</param>
		/// <returns>追加済みの子</returns>
		public static GameObject AddChild(Transform parent, GameObject go)
		{
			return UtageToolKit.AddChild(parent, go, Vector3.zero, Vector3.one);
		}
		/// <summary>
		/// 子の追加
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="go">子</param>
		/// <param name="localPosition">子に設定するローカル座標</param>
		/// <returns>追加済みの子</returns>
		public static GameObject AddChild(Transform parent, GameObject go, Vector3 localPosition)
		{
			return UtageToolKit.AddChild(parent, go, localPosition, Vector3.one);
		}
		/// <summary>
		/// 子の追加
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="go">子</param>
		/// <param name="localPosition">子に設定するローカル座標</param>
		/// <param name="localScale">子に設定するローカルスケール</param>
		/// <returns>追加済みの子</returns>
		public static GameObject AddChild(Transform parent, GameObject go, Vector3 localPosition, Vector3 localScale)
		{
			go.transform.parent = parent;
			go.transform.localScale = localScale;
			go.transform.localPosition = localPosition;
			go.transform.localRotation = Quaternion.identity;
			go.layer = parent.gameObject.layer;
			return go;
		}

		/// <summary>
		/// プレハブからGameObjectを作成して子として追加する
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="prefab">子を作成するためのプレハブ</param>
		/// <returns>追加済みの子</returns>
		public static GameObject AddChildPrefab(Transform parent, GameObject prefab )
		{
			return UtageToolKit.AddChildPrefab(parent, prefab, Vector3.zero, Vector3.one);
		}
		/// <summary>
		/// プレハブからGameObjectを作成して子として追加する
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="prefab">子を作成するためのプレハブ</param>
		/// <param name="localPosition">子に設定するローカル座標</param>
		/// <returns>追加済みの子</returns>
		public static GameObject AddChildPrefab(Transform parent, GameObject prefab, Vector3 localPosition)
		{
			return UtageToolKit.AddChildPrefab(parent, prefab, localPosition, Vector3.one);
		}
		/// <summary>
		/// プレハブからGameObjectを作成して子として追加する
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="prefab">子を作成するためのプレハブ</param>
		/// <param name="localPosition">子に設定するローカル座標</param>
		/// <param name="localScale">子に設定するローカルスケール</param>
		/// <returns>追加済みの子</returns>
		public static GameObject AddChildPrefab(Transform parent, GameObject prefab, Vector3 localPosition, Vector3 localScale)
		{
			GameObject go = GameObject.Instantiate(prefab) as GameObject;
			if (go != null && parent != null) AddChild(parent, go, localPosition, localScale);
			ChangeLayerAllChildren(go.transform, parent.gameObject.layer);
			return go;
		}

		/// <summary>
		/// 子を含む全てのレイヤーを変更する
		/// </summary>
		/// <param name="trans">レイヤーを変更する対象</param>
		/// <param name="layer">設定するレイヤー</param>
		public static void ChangeLayerAllChildren(Transform trans, int layer)
		{
			foreach (Transform child in trans)
			{
				child.gameObject.layer = layer;
				ChangeLayerAllChildren(child, layer);
			}
		}

		/// <summary>
		/// 指定のコンポーネントつきのGameObjectを作成して子として追加
		/// </summary>
		/// <typeparam name="T">コンポーネントの型</typeparam>
		/// <param name="parent">親</param>
		/// <param name="name">作成する子の名前</param>
		/// <returns>追加済みの子</returns>
		public static T AddChildGameObjectComponent<T>(Transform parent, string name) where T : Component
		{
			return UtageToolKit.AddChildGameObjectComponent<T>(parent, name, Vector3.zero, Vector3.one);
		}

		/// <summary>
		/// 指定のコンポーネントつきのGameObjectを作成して子として追加
		/// </summary>
		/// <typeparam name="T">コンポーネントの型</typeparam>
		/// <param name="parent">親</param>
		/// <param name="name">作成する子の名前</param>
		/// <param name="localPosition">子に設定するローカル座標</param>
		/// <returns>追加済みの子</returns>
		public static T AddChildGameObjectComponent<T>(Transform parent, string name, Vector3 localPosition) where T : Component
		{
			return UtageToolKit.AddChildGameObjectComponent<T>(parent, name, localPosition, Vector3.one);
		}
		/// <summary>
		/// 指定のコンポーネントつきのGameObjectを作成して子として追加
		/// </summary>
		/// <typeparam name="T">コンポーネントの型</typeparam>
		/// <param name="parent">親</param>
		/// <param name="name">作成する子の名前</param>
		/// <param name="localPosition">子に設定するローカル座標</param>
		/// <param name="localScale">子に設定するローカルスケール</param>
		/// <returns>追加済みの子</returns>
		public static T AddChildGameObjectComponent<T>(Transform parent, string name, Vector3 localPosition, Vector3 localScale) where T : Component
		{
			GameObject go = AddChildGameObject(parent, name, localPosition, localScale);
			return go.AddComponent<T>();
		}

		/// <summary>
		/// GameObjectを作成し、子として追加
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="name">作成する子の名前</param>
		/// <returns>追加済みの子</returns>
		public static GameObject AddChildGameObject(Transform parent, string name)
		{
			return AddChildGameObject(parent, name, Vector3.zero, Vector3.one);
		}

		/// <summary>
		/// GameObjectを作成し、子として追加
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="name">作成する子の名前</param>
		/// <param name="localPosition">子に設定するローカル座標</param>
		/// <returns>追加済みの子</returns>
		public static GameObject AddChildGameObject(Transform parent, string name, Vector3 localPosition)
		{
			return AddChildGameObject(parent, name, localPosition, Vector3.one);
		}

		/// <summary>
		/// GameObjectを作成し、子として追加
		/// </summary>
		/// <param name="parent">親</param>
		/// <param name="name">作成する子の名前</param>
		/// <param name="localPosition">子に設定するローカル座標</param>
		/// <returns>追加済みの子</returns>
		/// <param name="localScale">子に設定するローカルスケール</param>
		public static GameObject AddChildGameObject(Transform parent, string name, Vector3 localPosition, Vector3 localScale)
		{
			GameObject go = new GameObject(name);
			AddChild(parent, go, localPosition, localScale);
			return go;
		}


		/// <summary>
		/// 親オブジェクトやさらにその上位の親から、指定のコンポーネントを持つオブジェクトを検索
		/// </summary>
		/// <typeparam name="T">検索するコンポーネントの型</typeparam>
		/// <param name="transform">自分自身(親ではないので注意)</param>
		/// <returns>最初に見つかった指定のコンポーネントを持つオブジェクト。見つからなかったらnull</returns>
		public static T FindParentComponent<T>(Transform transform) where T : Component
		{
			Transform parent = transform.parent;
			while (null != parent)
			{
				T component = parent.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
				parent = parent.parent;
			}
			return null;
		}

		/// <summary>
		/// 子オブジェクトやさらにその子から、指定の名前のGameObjecctのTrasnfromを検索
		/// </summary>
		/// <param name="trasnform">自分自身</param>
		/// <param name="name">検索する名前</param>
		/// <returns>最初にみつかった指定の名前をもつTrasform。見つからなかったらnull</returns>
		public static Transform FindInChirdlen( Transform trasnform, string name)
		{
			foreach( Transform child in trasnform )
			{
				if (name == child.name)
				{
					return child;
				}
				else
				{
					Transform ret = FindInChirdlen(child, name);
					if (ret != null)
					{
						return ret;
					}
				}
			}
			return null;
		}


		/// <summary>
		/// パスが絶対URLかどうか（ホスト名やドライブ名がついているか）
		/// </summary>
		/// <param name="path">パス</param>
		/// <returns>絶対パスの場合はtrue。そうでない場合はfalse</returns>
		public static bool IsAbsoluteUri(string path)
		{
			if (path.Length <= 1) return false;

			try
			{
				System.Uri uri = new System.Uri(path, System.UriKind.RelativeOrAbsolute);
				return uri.IsAbsoluteUri;
			}
			catch( System.Exception e )
			{
				Debug.LogError( path +":" +e.Message);
				return false;
			}
		}

		/// <summary>
		/// 二バイト文字を含むURLをエンコード
		/// </summary>
		/// <param name="url">url</param>
		/// <returns>エンコードしたURL</returns>
		public static string EncodeUrl(string url)
		{
			try
			{
				System.Uri uri = new System.Uri(url);
				char[] separator = { '/', '\\' };

				string[] strings = uri.AbsolutePath.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
				string path = "";
				for (int i = 0; i < strings.Length; ++i )
				{
					path += "/" + WWW.EscapeURL(strings[i]);
				}

				url = uri.GetLeftPart(System.UriPartial.Authority) +path + uri.Query;
				return url;
			}
			catch (System.Exception e)
			{
				Debug.LogError(url + ":" + e.Message);
				return url;
			}
		}


		/// <summary>
		/// 日付を日本式表記のテキストで取得
		/// </summary>
		/// <param name="date">日付</param>
		/// <returns>日付の日本式表記テキスト</returns>
		static public string DateToStringJp(System.DateTime date)
		{
			return date.ToString(cultureInfJp);
		}
		static System.Globalization.CultureInfo cultureInfJp = new System.Globalization.CultureInfo("ja-JP");


		/// <summary>
		/// サイズ変更したテクスチャを作成する
		/// </summary>
		/// <param name="tex">リサイズするテクスチャ</param>
		/// <param name="captureW">リサイズ後のテクスチャの横幅(pix)</param>
		/// <param name="captureH">リサイズ後のテクスチャの縦幅(pix)</param>
		/// <returns>キャプチャ画像のテクスチャバイナリ</returns>
		public static Texture2D CreateResizeTexture(Texture2D tex, int width, int height)
		{
			return CreateResizeTexture(tex, width, height, tex.format, tex.mipmapCount > 1 );
		}

		/// <summary>
		/// サイズ変更したテクスチャを作成する
		/// </summary>
		/// <param name="tex">リサイズするテクスチャ</param>
		/// <param name="width">リサイズ後のテクスチャの横幅(pix)</param>
		/// <param name="height">リサイズ後のテクスチャの縦幅(pix)</param>
		/// <param name="format">リサイズ後のテクスチャフォーマット</param>
		/// <param name="isMipmap">ミップマップを有効にするか</param>
		/// <returns>リサイズして作成したテクスチャ</returns>
		public static Texture2D CreateResizeTexture(Texture2D tex, int width, int height, TextureFormat format, bool isMipmap )
		{
			Color[] colors = new Color[width * height];
			int index = 0;
			for (int y = 0; y < height; y++)
			{
				float v = 1.0f * y / (height - 1);
				for (int x = 0; x < width; x++)
				{
					float u = 1.0f * x / (width - 1);
					colors[index] = tex.GetPixelBilinear(u, v);
					++index;
				}
			}
			Texture2D resizedTex = new Texture2D(width, height, format, isMipmap);
			resizedTex.SetPixels(colors);
			resizedTex.Apply();
			return resizedTex;
		}
		public static Texture2D CreateResizeTexture(Texture2D tex, int width, int height, TextureFormat format )
		{
			return CreateResizeTexture( tex, width, height, format, false );
		}

		/// <summary>
		/// テクスチャから基本的なスプライト作成
		/// </summary>
		/// <param name="tex">テクスチャ</param>
		/// <param name="pixelsToUnits">スプライトを作成する際の、座標1.0単位辺りのピクセル数</param>
		/// <returns></returns>
		public static Sprite CreateSprite( Texture2D tex, float pixelsToUnits )
		{
			return CreateSprite(tex, pixelsToUnits, new Vector2(0.5f, 0.5f));
		}
		/// <summary>
		/// テクスチャから基本的なスプライト作成
		/// </summary>
		/// <param name="tex">テクスチャ</param>
		/// <param name="pixelsToUnits">スプライトを作成する際の、座標1.0単位辺りのピクセル数</param>
		/// <returns></returns>
		public static Sprite CreateSprite(Texture2D tex, float pixelsToUnits, Vector2 pivot)
		{
			if (tex.mipmapCount > 1) Debug.LogWarning(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.SpriteMimMap, tex.name));
			Rect rect = new Rect(0, 0, tex.width, tex.height);
			return Sprite.Create(tex, rect, pivot, pixelsToUnits);
		}

		/// <summary>
		/// Enum型を文字列から解析
		/// </summary>
		/// <typeparam name="T">enum型</typeparam>
		/// <param name="str">enum値の文字列</param>
		/// <param name="val">結果のenum値</param>
		/// <returns>成否</returns>
		public static bool TryParaseEnum<T>(string str, out T val )
		{
			try
			{
				val = (T)System.Enum.Parse(typeof(T), str);
				return true;
			}
			catch (System.Exception)
			{
				val = default(T);
				return false;
			}
		}

		/// <summary>
		/// Transformのローカル情報をバイナリ書き込み
		/// </summary>
		/// <param name="transform">書き込むTransform</param>
		/// <param name="writer">バイナリライター</param>
		public static void WriteLocalTransform( Transform transform, System.IO.BinaryWriter writer)
		{
			writer.Write(transform.localPosition.x);
			writer.Write(transform.localPosition.y);
			writer.Write(transform.localPosition.z);

			writer.Write(transform.localEulerAngles.x);
			writer.Write(transform.localEulerAngles.y);
			writer.Write(transform.localEulerAngles.z);

			writer.Write(transform.localScale.x);
			writer.Write(transform.localScale.y);
			writer.Write(transform.localScale.z);
		}

		/// <summary>
		/// Colorをバイナリ書き込み
		/// </summary>
		/// <param name="color">書き込むカラー</param>
		/// <param name="writer">バイナリライター</param>
		public static void WriteColor( Color color, System.IO.BinaryWriter writer)
		{
			writer.Write(color.r);
			writer.Write(color.g);
			writer.Write(color.b);
			writer.Write(color.a);
		}

		/// <summary>
		/// Transformのローカル情報をバイナリ読み込み
		/// </summary>
		/// <param name="transform">読み込むTransform</param>
		/// <param name="reader">バイナリリーダー/param>
		public static void ReadLocalTransform(Transform transform, System.IO.BinaryReader reader)
		{
			Vector3 pos = new Vector3();
			pos.x = reader.ReadSingle();
			pos.y = reader.ReadSingle();
			pos.z = reader.ReadSingle();

			Vector3 euler = new Vector3();
			euler.x = reader.ReadSingle();
			euler.y = reader.ReadSingle();
			euler.z = reader.ReadSingle();

			Vector3 scale = new Vector3();
			scale.x = reader.ReadSingle();
			scale.y = reader.ReadSingle();
			scale.z = reader.ReadSingle();

			transform.localPosition = pos;
			transform.localEulerAngles = euler;
			transform.localScale = scale;
		}


		/// <summary>
		/// Colorをバイナリ書き込み読み込み
		/// </summary>
		/// <param name="transform">読み込むカラー</param>
		/// <param name="reader">バイナリリーダー</param>
		/// <returns>読み込んだカラー値</returns>
		public static Color ReadColor(System.IO.BinaryReader reader)
		{
			Color color;
			color.r = reader.ReadSingle();
			color.g = reader.ReadSingle();
			color.b = reader.ReadSingle();
			color.a = reader.ReadSingle();
			return color;
		}

		public static bool IsHankaku(char c)
		{
			if ((c <= '\u007e') || // 英数字
				(c == '\u00a5') || // \記号
				(c == '\u203e') || // ~記号
				(c >= '\uff61' && c <= '\uff9f') // 半角カナ
			)
				return true;
			else
				return false;
		}

		public static bool IsPlatformStandAloneOrEditor()
		{
			return Application.isEditor || IsPlatformStandAlone();
		}

		public static bool IsPlatformStandAlone()
		{
			switch (Application.platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.OSXPlayer:
				case RuntimePlatform.LinuxPlayer:
					return true;
				default:
					return false;
			}
		}
	}
}