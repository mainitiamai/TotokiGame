using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// カメラ制御。デバイスの解像度やアスペクト比の変更に対応できるようにしている。
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/Camera/CameraManager")]
	public class CameraManager : MonoBehaviour
	{
		/// <summary>
		/// シングルトンなインスタンスを取得
		/// </summary>
		/// <returns></returns>
		public static CameraManager GetInstance()
		{
			if (null == instance)
			{
				instance = (CameraManager)FindObjectOfType(typeof(CameraManager));
			}
			return instance;
		}
		static CameraManager instance;

		void Awake()
		{
			if (null == instance)
			{
				instance = this;
			}
			Refresh();
		}

		//2D用カメラ
		[SerializeField]
		List<Camera> cameras2D;
		public void AddCamera2D(Camera camera)
		{
			if (cameras2D == null)
			{
				cameras2D = new List<Camera>();
			}
			cameras2D.Add(camera);
			Refresh();
		}

		/// <summary>
		/// 2Dカメラの1単位あたりのピクセル数
		/// </summary>
		public int PixelsToUnits { get { return pixelsToUnits; } }
		[SerializeField]
		int pixelsToUnits = 100;

		//3D用カメラ
		[SerializeField]
		List<Camera> cameras3D;
		public void AddCamera3D(Camera camera)
		{
			if (cameras3D == null)
			{
				cameras3D = new List<Camera>();
			}
			cameras3D.Add(camera);
			Refresh();
		}


		//レターボックスを使う際に、ゲーム画面を画面中央ではなく、下にくっつける形にする。広告表示などのレイアウトに合わせるために
		[SerializeField]
		bool isAnchorBottom = false;


		//アスペクト比
		public enum ASPECT
		{
			_1x2,		// 縦持ち 1:2
			_9x16,		// 縦持ち iPhone4inch (iPhone5)
			_2x3,		// 縦持ち iPhone3.5inch (iPhone4s以前)
			_3x4,		// 縦持ち iPad
			_1x1,		// 正方形
			_4x3,		// 横持ち iPad
			_3x2,		// 横持ち iPhone3.5inch (iPhone4s以前)
			_16x9,		// 横持ち iPhone4inch (iPhone5)
			_2x1,		// 横持ち 2:1
			Custom,		// カスタム解像度
		};

		/// <summary>
		/// 基本の画面サイズ：高さ(px)
		/// この値と設定されたアスペクト比によって表示領域が決まる
		/// </summary>
		public int DefaultHeight { get { return defaultHeight; } }
		[SerializeField]
		int defaultHeight = 600;

		/// <summary>
		/// 最も縦長になった場合のアスペクト比
		/// </summary>
		public ASPECT NallowAspect
		{
			get { return nallowAspect; }
		}
		[SerializeField]
		ASPECT nallowAspect = ASPECT._4x3;

		public float CustomNallowAspect
		{
			get { return customNallowAspect; }
		}
		[SerializeField]
		float customNallowAspect = 1;

		/// <summary>
		/// 基本のアスペクト比
		/// </summary>
		public ASPECT DefaultAspect
		{
			get { return defaultAspect; }
		}
		[SerializeField]
		ASPECT defaultAspect = ASPECT._4x3;

		public float CustomDefaultAspect
		{
			get { return customDefaultAspect; }
		}
		[SerializeField]
		float customDefaultAspect = 1;


		/// <summary>
		/// 最も横長になった場合のアスペクト比
		/// </summary>
		public ASPECT WideAspect
		{
			get { return wideAspect; }
		}
		[SerializeField]
		ASPECT wideAspect = ASPECT._4x3;

		public float CustomWideAspect
		{
			get { return customWideAspect; }
		}
		[SerializeField]
		float customWideAspect = 1;


		/// <summary>
		/// 基本の画面サイズ：幅(px)
		/// </summary>
		public int DefaultWidth { get { return (int)(defaultHeight * CalcAspectRatio(DefaultAspect, CustomDefaultAspect)); } }

		/// <summary>
		/// 現在の画面サイズ：高さ(px)
		/// </summary>
		public int CurrentHeight { get { return currentHeight; } }
		int currentHeight;

		/// <summary>
		/// 現在の画面サイズ：幅(px)
		/// </summary>
		public int CurrentWidth { get { return currentWidth; } }
		int currentWidth;

		float screenAspectRatio;		//デバイススクリーンのアスペクト比
		float currentAspectRatio;		//現在のアスペクト比
		Rect currentRect;				//現在の表示領域矩形


		/// <summary>
		/// 初期化。スクリプトから動的に生成する場合に
		/// </summary>
		public void InitOnCreate(int widthPx, int heightPx)
		{
			ASPECT aspect = ASPECT.Custom;
			defaultHeight = heightPx;
			float aspectValue = 1.0f*widthPx / heightPx;
			foreach (ASPECT item in System.Enum.GetValues(typeof(ASPECT)))
			{
				if (Mathf.Approximately(aspectValue, CalcAspectRatio(item, aspectValue)))
				{
					aspect = item;
					break;
				}
			}
			defaultAspect = nallowAspect = wideAspect = aspect;
			customDefaultAspect = customNallowAspect = customWideAspect = aspectValue;
		}

		void Update()
		{
			if (!Mathf.Approximately(screenAspectRatio, 1.0f * Screen.width / Screen.height))
			{
				Refresh();
			}
		}

		void Refresh()
		{
			screenAspectRatio = 1.0f * Screen.width / Screen.height;

			float defaultAspectRatio = CalcAspectRatio(DefaultAspect, CustomDefaultAspect);
			float wideAspectRatio = CalcAspectRatio(WideAspect, CustomWideAspect);
			float nallowAspectRatio = CalcAspectRatio(NallowAspect, CustomNallowAspect);

			//スクリーンのアスペクト比から、ゲームのアスペクト比を決める
			if (screenAspectRatio > wideAspectRatio)
			{
				//アスペクト比が設定よりも横長
				currentAspectRatio = wideAspectRatio;
			}
			else if (screenAspectRatio < nallowAspectRatio)
			{
				//アスペクト比が設定よりも縦長
				currentAspectRatio = nallowAspectRatio;
			}
			else
			{
				//アスペクト比が設定の範囲内ならそのままスクリーンのアスペクト比を使う
				currentAspectRatio = screenAspectRatio;
			}

			//ゲームの画面サイズを決める
			if (currentAspectRatio < 1)
			{	//縦に短くする
				currentHeight = (int)(defaultHeight * defaultAspectRatio / currentAspectRatio);
			}
			else
			{	//デフォルトの値を使う
				currentHeight = defaultHeight;
			}
			currentWidth = (int)(currentHeight * currentAspectRatio);

			float marginX = 0;
			float marginY = 0;
			//描画領域をクリップする
			if (currentAspectRatio != screenAspectRatio)
			{
				if (screenAspectRatio > currentAspectRatio)
				{
					//スクリーンのほうが横長なので、左右をクリップ.
					marginX = (1.0f - currentAspectRatio / screenAspectRatio) / 2;
					marginY = 0;
				}
				else if (screenAspectRatio < currentAspectRatio)
				{
					//スクリーンのほうが縦長なので、上下をクリップ.
					marginX = 0;
					marginY = (1.0f - screenAspectRatio / currentAspectRatio) / 2;
				}
			}

			if (isAnchorBottom)
			{
				currentRect = new Rect(marginX, 0, 1 - marginX * 2, 1 - marginY * 2);
			}
			else
			{
				currentRect = new Rect(marginX, marginY, 1 - marginX * 2, 1 - marginY * 2);
			}

			SetRects(currentRect);
		}

		void SetRects(Rect rect)
		{
			if (cameras2D == null) return;

			float camera2DOrthographicSize = (float)currentHeight / (2 * pixelsToUnits);
			foreach (Camera camera2d in cameras2D)
			{
				camera2d.rect = rect;
				camera2d.orthographicSize = camera2DOrthographicSize;
			}
			if (cameras3D == null) return;
			foreach (Camera camera3d in cameras3D)
			{
				camera3d.rect = rect;
			}
		}

		/// <summary>
		/// キャプチャ用のテクスチャを作る(yield return new WaitForEndOfFrame()の後に呼ぶこと)
		/// </summary>
		/// <returns>キャプチャ画像</returns>
		public Texture2D CaptureScreen()
		{
			return CaptureScreen(TextureFormat.RGB24);
		}

		/// <summary>
		/// キャプチャ用のテクスチャを作る(yield return new WaitForEndOfFrame()の後に呼ぶこと)
		/// </summary>
		/// <param name="format">テクスチャフォーマット</param>
		/// <returns>キャプチャ画像</returns>
		public Texture2D CaptureScreen(TextureFormat format)
		{
			int x = (int)(currentRect.x * Screen.width);
			int y = (int)(currentRect.y * Screen.height);
			int width = (int)(currentRect.width * Screen.width);
			int height = (int)(currentRect.height * Screen.height);
			Texture2D tex = new Texture2D(width, height, format, false);
			try
			{
				Rect rect = new Rect(x, y, width, height);
				tex.ReadPixels(rect, 0, 0);
				tex.Apply();
			}
			catch
			{
			}
			return tex;
		}

		float CalcAspectRatio(ASPECT aspect, float customAspect)
		{
			switch (aspect)
			{
				case ASPECT._1x2:
					return 1.0f / 2;
				case ASPECT._9x16:
					return 9.0f / 16;
				case ASPECT._2x3:
					return 2.0f / 3;
				case ASPECT._3x4:
					return 3.0f / 4;
				case ASPECT._1x1:
					return 1;
				case ASPECT._4x3:
					return 4.0f / 3;
				case ASPECT._3x2:
					return 3.0f / 2;
				case ASPECT._16x9:
					return 16.0f / 9;
				case ASPECT._2x1:
					return 2.0f;
				case ASPECT.Custom:
				default:
					return customAspect;
			}
		}

		/// <summary>
		/// 指定のレイヤーの2Dカメラを取得
		/// </summary>
		/// <param name="layer">レイヤー</param>
		/// <returns>2Dカメラ</returns>
		public Camera Find2DCamera(int layer)
		{
			foreach (Camera cam in cameras2D)
			{
				if (cam.gameObject.layer == layer)
				{
					return cam;
				}
			}
			return null;
		}

		/// <summary>
		/// ゲームの終了演出
		/// Androidで、iPhoneっぽくアプリを終了させる(描画範囲を中央に向けて閉じる)
		/// </summary>
		/// <returns></returns>
		public IEnumerator CoGameExit()
		{
			yield return StartCoroutine(CoGameExit(0.2f));
		}

		/// <summary>
		/// ゲームの終了演出
		/// Androidで、iPhoneっぽくアプリを終了させる(描画範囲を中央に向けて閉じる)
		/// </summary>
		/// <param name="fadeTime">演出時間</param>
		/// <returns></returns>
		public IEnumerator CoGameExit(float fadeTime)
		{
			float time = 0;
			Rect start = currentRect;
			Rect rect = currentRect;
			while (true)
			{
				float rate = 1.0f - time / fadeTime;
				if (rate <= 0)
				{
					break;
				}
				rect.width = start.width * rate;
				rect.height = start.height * rate;
				rect.center = start.center;
				SetRects(rect);
				yield return 0;
				time += Time.deltaTime;
			}
			foreach (Camera camera2d in cameras2D)
			{
				camera2d.gameObject.SetActive(false);
			}
			foreach (Camera camera3d in cameras3D)
			{
				camera3d.gameObject.SetActive(false);
			}
			yield return 0;
			yield return 0;
		}
	}
}