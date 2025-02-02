//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// iTweenのプレイヤー
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/iTweenPlayer")]
	internal class iTweenPlayer : MonoBehaviour
	{
		iTweenData data;
		Hashtable hashTbl;
		Action<iTweenPlayer> callbackComplete;
		bool isColorSprite;
		int count;
		string tweenName;

		/// <summary>
		/// 無限ループするか
		/// </summary>
		public bool IsEndlessLoop { get { return data.IsEndlessLoop; } }
		
		/// <summary>
		/// 再生中か
		/// </summary>
		public bool IsPlaying { get { return isPlaying; } }
		bool isPlaying = false;

		Node2D node2D;

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="type">Tweenのデータ</param>
		/// <param name="hashObjects">Tweenのパラメーター</param>
		/// <param name="loopCount">ループ回数</param>
		/// <param name="pixelsToUnits">座標1.0単位辺りのピクセル数</param>
		/// <param name="skipSpeed">スキップ中の演出速度の倍率。0ならスキップなし</param>
		/// <param name="callbackComplete">終了時に呼ばれるコールバック</param>
		public void Init( iTweenData data, float pixelsToUnits, float skipSpeed, Action<iTweenPlayer> callbackComplete)
		{
			this.data = data;
			if (data.Type == iTweenType.Stop) return;

			this.callbackComplete = callbackComplete;

			data.ReInit();
			hashTbl = iTween.Hash(data.HashObjects.ToArray());

			//2D座標にあわせる
			if (iTweenData.IsPostionType(data.Type))
			{
				if (hashTbl.ContainsKey("x")) hashTbl["x"] = (float)hashTbl["x"] / pixelsToUnits;
				if (hashTbl.ContainsKey("y")) hashTbl["y"] = (float)hashTbl["y"] / pixelsToUnits;
				if (hashTbl.ContainsKey("z")) hashTbl["z"] = (float)hashTbl["z"] / pixelsToUnits;
			}
			//スキップ中なら演出時間を調整
			if (skipSpeed > 0)
			{
				bool isSpeed = hashTbl.ContainsKey("speed");
				if (isSpeed) hashTbl["speed"] = (float)hashTbl["speed"] * skipSpeed;

				bool isTime = hashTbl.ContainsKey("time");
				if(isTime)
				{
					hashTbl["time"] = (float)hashTbl["time"]/ skipSpeed;
				}
				else if(!isSpeed)
				{
					hashTbl["time"] = 1.0f / skipSpeed;
				}
			}

			//カラーの処理を2D仕様に
			if (data.Type == iTweenType.ColorTo || data.Type == iTweenType.ColorFrom)
			{
				this.node2D = this.gameObject.GetComponent<Node2D>() as Node2D;
				if (node2D != null)
				{
					if (data.Type == iTweenType.ColorTo)
					{
						hashTbl["from"] = node2D.LocalColor;
						hashTbl["to"] = ParaseTargetColor( hashTbl, node2D.LocalColor );
					}
					else if (data.Type == iTweenType.ColorFrom)
					{
						hashTbl["from"] = ParaseTargetColor( hashTbl, node2D.LocalColor );
						hashTbl["to"] = node2D.LocalColor;
					}
					hashTbl["onupdate"] = "OnColorUpdate";
					isColorSprite = true;
				}
			}

			//終了時に呼ばれるメッセージを登録
			hashTbl["oncomplete"] = "OnComplete";
			hashTbl["oncompletetarget"] = this.gameObject;
			hashTbl["oncompleteparams"] = this;

			//停止処理用に名前を設定
			tweenName = this.GetHashCode().ToString();
			hashTbl["name"] = tweenName;
		}


		/// <summary>
		/// Tween処理開始
		/// </summary>
		public void Play()
		{
			isPlaying = true;
			if (data.Type == iTweenType.Stop)
			{
				iTween.Stop(gameObject);
				return;
			}
			else if (isColorSprite)
			{
				iTween.ValueTo(gameObject, hashTbl);
				return;
			}

			switch (data.Type)
			{
				case iTweenType.ColorFrom:
					iTween.ColorFrom(gameObject, hashTbl);
					break;
				case iTweenType.ColorTo:
					iTween.ColorTo(gameObject, hashTbl);
					break;
				case iTweenType.MoveAdd:
					iTween.MoveAdd(gameObject, hashTbl);
					break;
				case iTweenType.MoveBy:
					iTween.MoveBy(gameObject, hashTbl);
					break;
				case iTweenType.MoveFrom:
					iTween.MoveFrom(gameObject, hashTbl);
					break;
				case iTweenType.MoveTo:
					iTween.MoveTo(gameObject, hashTbl);
					break;
				case iTweenType.PunchPosition:
					iTween.PunchPosition(gameObject, hashTbl);
					break;
				case iTweenType.PunchRotation:
					iTween.PunchRotation(gameObject, hashTbl);
					break;
				case iTweenType.PunchScale:
					iTween.PunchScale(gameObject, hashTbl);
					break;
				case iTweenType.RotateAdd:
					iTween.RotateAdd(gameObject, hashTbl);
					break;
				case iTweenType.RotateBy:
					iTween.RotateBy(gameObject, hashTbl);
					break;
				case iTweenType.RotateFrom:
					iTween.RotateFrom(gameObject, hashTbl);
					break;
				case iTweenType.RotateTo:
					iTween.RotateTo(gameObject, hashTbl);
					break;
				case iTweenType.ScaleAdd:
					iTween.ScaleAdd(gameObject, hashTbl);
					break;
				case iTweenType.ScaleBy:
					iTween.ScaleBy(gameObject, hashTbl);
					break;
				case iTweenType.ScaleFrom:
					iTween.ScaleFrom(gameObject, hashTbl);
					break;
				case iTweenType.ScaleTo:
					iTween.ScaleTo(gameObject, hashTbl);
					break;
				case iTweenType.ShakePosition:
					iTween.ShakePosition(gameObject, hashTbl);
					break;
				case iTweenType.ShakeRotation:
					iTween.ShakeRotation(gameObject, hashTbl);
					break;
				case iTweenType.ShakeScale:
					iTween.ShakeScale(gameObject, hashTbl);
					break;
				default:
					isPlaying = false;
					Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownType, data.Type.ToString()));
					break;
			}
		}

		Color ParaseTargetColor( Hashtable hashTbl, Color color )
		{
			if (hashTbl.Contains( iTweenData.Color ))
			{
				color = (Color)hashTbl[iTweenData.Color];
			}
			else
			{
				if (hashTbl.Contains(iTweenData.R))
				{
					color.r = (float)hashTbl[iTweenData.R];
				}
				if (hashTbl.Contains(iTweenData.G))
				{
					color.g = (float)hashTbl[iTweenData.G];
				}
				if (hashTbl.Contains(iTweenData.B))
				{
					color.b = (float)hashTbl[iTweenData.B];
				}
				if (hashTbl.Contains(iTweenData.A))
				{
					color.a = (float)hashTbl[iTweenData.A];
				}
			}

			if (hashTbl.Contains(iTweenData.Alpha))
			{
				color.a = (float)hashTbl[iTweenData.Alpha];
			}

			return color;
		}

		/// <summary>
		/// 破棄するときに呼ばれる
		/// </summary>
		void OnDestroy()
		{
			if (callbackComplete != null)
			{
				callbackComplete(this);
			}
		}

		/// <summary>
		/// 再生終了時に呼ばれる
		/// </summary>
		void OnComplete(iTweenPlayer arg)
		{
			if (arg != this) return;

			++count;
			if (count >= this.data.LoopCount && !IsEndlessLoop)
			{
				if (callbackComplete != null) callbackComplete(this);
				callbackComplete = null;
				iTween.StopByName(this.gameObject, tweenName);
				isPlaying = false;
				UnityEngine.Object.Destroy(this);
			}
		}

		/// <summary>
		/// カラーの更新時に呼ばれる
		/// </summary>
		/// <param name="color"></param>
		void OnColorUpdate(Color color)
		{
			if( node2D!=null )
			{
				node2D.LocalColor = color;
			}
		}

		/// <summary>
		/// セーブデータ用のバイナリ書き込み
		/// </summary>
		/// <param name="writer">バイナリライター</param>
		public void Write(BinaryWriter writer)
		{
			data.Write(writer);
		}

		/// <summary>
		/// セーブデータ用のバイナリ読みこみ
		/// </summary>
		/// <param name="pixelsToUnits">座標1.0単位辺りのピクセル数</param>
		/// <param name="reader">バイナリリーダー</param>
		public void Read(BinaryReader reader, float pixelsToUnits)
		{
			iTweenData data = new iTweenData(reader);
			Init(data, pixelsToUnits, 1.0f, null);
		}

	}
}
