using System;
using UnityEngine;
using System.Collections;

namespace Utage
{

	/// <summary>
	/// ボタン
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/UiEffect")]
	[RequireComponent(typeof(BoxCollider2D))]
	public class UiEffect : MonoBehaviour
	{
		enum EffectPattern
		{
			None,
			Default,
		};
		[SerializeField]
		EffectPattern pattern = EffectPattern.Default;

		static float PressedScale = 0.9f;
		const float PressedDuration = 0.2f;

		/// <summary>
		/// エフェクトをかける対象のオブジェクト(未設定なら自分自身のGameObjectになる)
		/// </summary>
		public Transform EffectTarget
		{
			get { if (effectTarget == null) EffectTarget = this.transform; return effectTarget; }
//			set { effectTarget = value; defaultScale = effectTarget.transform.localScale; }
			set { effectTarget = value; }
		}
		[SerializeField]
		Transform effectTarget;

		Transform cachedTransform;
		Transform CachedTransform { get { if (null == cachedTransform) cachedTransform = this.transform; return cachedTransform; } }

		bool isEffectInit = false;
//		Vector3 effectTargetLocalPosition;
		Vector3 effectTargetLocalScale;
//		Vector3 effectTargetLocalEulerAngles;

		/// <summary>
		/// ボックスコライダー
		/// </summary>
		public BoxCollider2D BoxCollider2D
		{
			get
			{
				if (null == boxCollider2D)
				{
					boxCollider2D = GetComponent<BoxCollider2D>();
					defaultColloderSize = boxCollider2D.size;
				}
				return boxCollider2D;
			}
		}
		BoxCollider2D boxCollider2D;
		Vector2 defaultColloderSize;

		/// <summary>
		/// 有効になったとき
		/// </summary>
		protected virtual void OnEnable()
		{
			ResetEffect();
		}

		/// <summary>
		/// タッチしたとき
		/// </summary>
		/// <param name="touch">タッチ入力データ</param>
		protected virtual void OnTouchDown(TouchData2D touch)
		{
			EffectDown();
		}

		/// <summary>
		/// タッチがはずれたとき
		/// </summary>
		/// <param name="touch">タッチ入力データ</param>
		protected virtual void OnTouchOver(TouchData2D touch)
		{
			EffectUp();
		}

		/// <summary>
		/// タッチが離されたとき
		/// </summary>
		/// <param name="touch">タッチ入力データ</param>
		protected virtual void OnTouchUp(TouchData2D touch)
		{
			EffectUp();
		}

		/// <summary>
		/// タッチダウンエフェクト処理
		/// </summary>
		public virtual void EffectDown()
		{
			EffectInit();
			StopAllCoroutines();
			if (!enabled) return;
			switch (pattern)
			{
				case EffectPattern.Default:
					StartCoroutine(CoEffectDefault(PressedDuration, PressedScale));
					break;
				case EffectPattern.None:
				default:
					break;
			}
		}

		/// <summary>
		/// タッチアップエフェクト処理
		/// </summary>
		public virtual void EffectUp()
		{
			EffectInit();
			StopAllCoroutines();
			if (!enabled) return;
			switch (pattern)
			{
				case EffectPattern.Default:
					StartCoroutine(CoEffectDefault(PressedDuration, 1.0f));
					break;
				case EffectPattern.None:
				default:
					break;
			}
		}

		/// <summary>
		/// エフェクトをリセット
		/// </summary>
		public virtual void ResetEffect()
		{
			EffectInit();
			switch (pattern) 
			{
				case EffectPattern.Default:
					EffectTarget.localScale = effectTargetLocalScale;
					break;
				case EffectPattern.None:
				default:
					break;
			}
		}

		void EffectInit()
		{
			if( isEffectInit ) return;
//			effectTargetLocalPosition = EffectTarget.localPosition;
			effectTargetLocalScale = EffectTarget.localScale;
//			effectTargetLocalEulerAngles = EffectTarget.localEulerAngles;
			isEffectInit = true;
		}

		IEnumerator CoEffectDefault(float time, float scale)
		{
			float currentTime = 0;
			while (currentTime <= time)
			{
				Vector3 localScale = (effectTargetLocalScale * scale + CachedTransform.localScale * 3) / 4.0f;
				CachedTransform.localScale = localScale;
				FixColliderSize(localScale);
				currentTime += Time.deltaTime;
				yield return 0;
			};
			CachedTransform.localScale = effectTargetLocalScale * scale;
			FixColliderSize(effectTargetLocalScale * scale);
		}

		//コライダーのサイズを固定する
		void FixColliderSize(Vector2 scale)
		{
			if (EffectTarget != CachedTransform) return;

			if (scale.x != 0 && scale.y != 0)
			{
				BoxCollider2D.size = new Vector2(defaultColloderSize.x / scale.x, defaultColloderSize.y / scale.y);
			}
			else
			{
				BoxCollider2D.size = Vector2.zero;
			}
		}
	}
}