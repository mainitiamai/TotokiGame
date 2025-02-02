//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// 2D用のObjectの基底クラス
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/2D/Node")]
	public class Node2D : MonoBehaviour
	{

		/// <summary>
		/// 描画順を親オブジェクトとリンクさせるか
		/// </summary>
		public bool IsLinkSorting2D { set { isLinkSorting2D = value; MarkAsChanged(); } get { return isLinkSorting2D; } }
		[SerializeField]
		bool isLinkSorting2D = true;

		/// <summary>
		/// ローカルな2Dレイヤー
		/// これが何も設定されていない場合、親オブジェクトとのリンクがあれば親の2Dレイヤーが、なければdefaultがレイヤーになる
		/// </summary>
		public string LocalSortingLayer { set { localSortingLayer = value; MarkAsChanged(); } get { return localSortingLayer; } }
		[SerializeField]
		string localSortingLayer;

		/// <summary>
		/// 2Dレイヤー
		/// </summary>
		public string SortingLayer { get { if (hasChanged) Refresh(); return sortingLayer; } }
		string sortingLayer;

		/// <summary>
		/// ローカルな2Dレイヤー内の描画順。
		/// 親オブジェクトとのリンクがあれば親のオブジェクトの描画順からの相対値になる
		/// </summary>
		public int LocalOrderInLayer { set { localOrderInLayer = value; MarkAsChanged(); } get { return localOrderInLayer; } }
		[SerializeField]
		int localOrderInLayer;

		/// <summary>
		/// 2Dレイヤー内の描画順
		/// </summary>
		public int OrderInLayer { get { if (hasChanged) Refresh(); return orderInLayer; } }
		int orderInLayer;

		/// <summary>
		/// ローカルな描画順を設定
		/// </summary>
		/// <param name="localSortingLayer">ローカルな2Dレイヤー</param>
		/// <param name="localOrderInLayer">ローカルな2Dレイヤー内の描画順。</param>
		public void SetLocalSort(string localSortingLayer, int localOrderInLayer)
		{
			this.localSortingLayer = localSortingLayer;
			this.localOrderInLayer = localOrderInLayer;
			MarkAsChanged();
		}

		/// <summary>
		/// カラーを親オブジェクトとリンクさせるか
		/// </summary>
		public bool IsLinkColor { set { isLinkColor = value; MarkAsChanged(); } get { return isLinkColor; } }
		[SerializeField]
		bool isLinkColor = true;

		/// <summary>
		/// ローカルなカラー
		/// 親オブジェクトとのリングあれば、この値に親オブジェクトのカラーを乗算した値が実際のカラーになる
		/// </summary>
		public Color LocalColor { set { localColor = value; MarkAsChanged(); } get { return localColor; } }
		[SerializeField]
		Color localColor = Color.white;

		/// <summary>
		/// カラー
		/// </summary>
		public Color Color { get { if (hasChanged) Refresh(); return color; } }
		Color color;

		/// <summary>
		/// ローカルなカラーの不透明度
		/// 親オブジェクトとのリングあれば、この値に親オブジェクトのカラーの不透明度を乗算した値が実際のカラーの不透明度になる
		/// </summary>
		public float LocalAlpha { set { if (!Mathf.Approximately(localColor.a, value)) { localColor.a = value; MarkAsChanged(); } } get { return localColor.a; } }


		/// <summary>
		/// ソート関係のデータテーブル
		/// </summary>
//		public Node2DSortData SortData { set { sortData = value; MarkAsChanged(); } get { return sortData; } }
//		[SerializeField]
//		Node2DSortData sortData;

		/// <summary>
		/// ソート関係のデータテーブルのキー
		/// これが設定されていると、ローカルのレイヤー、描画順、Z値をデータに従って上書きする
		/// </summary>
		public string SortKey { set { sortKey = value; MarkAsChanged(); } get { return sortKey; } }
		[SerializeField]
		string sortKey = "";

		/// <summary>
		/// ソート関係のデータテーブルのキーが設定されているか
		/// </summary>
		public bool IsEmptySortData { get { return (string.IsNullOrEmpty(sortKey) || (sortKey == Node2DSortData.KeyNone)); } }

		/// <summary>
		/// トランスフォームのキャッシュ(this.transformだと低速なため)
		/// </summary>
		public Transform CachedTransform { get { if (null == cachedTransform) cachedTransform = this.transform; return cachedTransform; } }
		Transform cachedTransform;

		/// <summary>
		/// 直前のフレームまでの親
		/// </summary>
		protected Transform lastParent;

		/// <summary>
		/// スプライトコンポーネント(アタッチされてない場合はnull)
		/// </summary>
		public SpriteRenderer CachedSpriteRenderer { get { if (null == cachedSpriteRenderer) cachedSpriteRenderer = this.GetComponent<SpriteRenderer>(); return cachedSpriteRenderer; } }
		SpriteRenderer cachedSpriteRenderer;

		/// <summary>
		/// 親ノード
		/// </summary>
		Node2D parentNode;

		/// <summary>
		/// 子ノード
		/// </summary>
		public List<Node2D> NodeChidren { get { return nodeChidren; } }
		List<Node2D> nodeChidren = new List<Node2D>();

		/// <summary>
		/// 更新が必要かのフラグ
		/// </summary>
		protected bool hasChanged = true;

		LinearValue fadeValue = new LinearValue();

		/// <summary>
		/// 変更があったことを記録
		/// </summary>
		public void MarkAsChanged()
		{
			hasChanged = true;
		}

		/// <summary>
		/// 有効になったとき
		/// </summary>
		protected virtual void OnEnable()
		{
			MarkAsChanged();
		}

		/// <summary>
		/// 破棄するとき
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (parentNode != null)
			{
				parentNode.RemoveChildNode(this);
			}
		}

		/// <summary>
		/// インスペクターから値が変更された場合
		/// </summary>
		protected virtual void OnValidate()
		{
			MarkAsChanged();
		}
		
		/// <summary>
		/// 毎フレームの最後の更新
		/// </summary>
		protected virtual void LateUpdate()
		{
			if ( CachedTransform.parent != lastParent || hasChanged )
			{	//構造に変化があった
				Refresh();
			}
		}


		/// <summary>
		/// データの更新をかける
		/// </summary>
		public void Refresh()
		{
			RefreshNode();
			RefreshColor();
			RefreshSort();
			hasChanged = false;
			
			RefreshCustom();
			lastParent = CachedTransform.parent;
			List<Node2D> nodeChidren = new List<Node2D>(NodeChidren);
			foreach (Node2D child in nodeChidren)
			{
				if (null != child) child.Refresh();
			}
		}

		/// <summary>
		/// サブクラス用の更新をかける
		/// </summary>
		public virtual void RefreshCustom(){}

		/// <summary>
		/// ノード構成を更新
		/// </summary>
		void RefreshNode()
		{
			Node2D newParentNode = UtageToolKit.FindParentComponent<Node2D>(this.transform);
			if (newParentNode != parentNode)
			{
				if (parentNode != null) parentNode.RemoveChildNode(this);
				parentNode = newParentNode;
				if (parentNode != null) parentNode.AddChildNode(this);
			}
		}

		/// <summary>
		/// 子ノードを追加
		/// </summary>
		/// <param name="child">追加する子ノード</param>
		void AddChildNode(Node2D child)
		{
			if (!nodeChidren.Contains(child)) nodeChidren.Add(child);
		}

		/// <summary>
		/// 子ノードを削除
		/// </summary>
		/// <param name="child">削除する子ノード</param>
		void RemoveChildNode(Node2D child)
		{
			if (nodeChidren.Contains(child)) nodeChidren.Remove(child);
		}



		/// <summary>
		/// 色を更新
		/// </summary>
		void RefreshColor()
		{
			color = LocalColor;
			if (IsLinkColor && CachedTransform.parent != null)
			{
				if (null != parentNode)
				{
					color = parentNode.Color * LocalColor;
				}
			}
			if (null != CachedSpriteRenderer)
			{
				CachedSpriteRenderer.color = color;
			}
		}

		/// <summary>
		/// ソートデータを更新
		/// </summary>
		void RefreshSort()
		{
			if (!IsEmptySortData)
			{
				Node2DSortData.SortData2D data;
				if (Node2DSortData.Instance.TryGetValue(sortKey, out data))
				{
					if (data.z != CachedTransform.localPosition.z)
					{
						CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y, data.z);
					}

					localSortingLayer = data.sortingLayer;
					localOrderInLayer = data.orderInLayer;
				}
			}
			sortingLayer = LocalSortingLayer;
			orderInLayer = LocalOrderInLayer;
			if (IsLinkSorting2D && CachedTransform.parent != null)
			{
				if (parentNode != null)
				{
					if (string.IsNullOrEmpty(LocalSortingLayer)) sortingLayer = parentNode.SortingLayer;
					orderInLayer = parentNode.OrderInLayer + LocalOrderInLayer;
				}
			}
			if (null != CachedSpriteRenderer)
			{
				CachedSpriteRenderer.sortingLayerName = sortingLayer;
				CachedSpriteRenderer.sortingOrder = orderInLayer;
			}
		}

		/// <summary>
		/// フェードイン開始
		/// </summary>
		/// <param name="fadeTime">フェードする時間</param>

		public void FadeIn(float fadeTime)
		{
			fadeValue.Init(fadeTime, 0, 1);
			StopCoroutine("CoFade");
			StartCoroutine("CoFade", false);
		}

		/// <summary>
		/// フェードアウト開始
		/// </summary>
		/// <param name="fadeTime">フェードする時間</param>
		/// <param name="autiomaticDestoroy">フェード終了後、自動的に自分自身のGameObjectをDestoryする</param>
		public void FadeOut(float fadeTime, bool autiomaticDestoroy )
		{
			fadeValue.Init(fadeTime, LocalAlpha, 0);
			StopCoroutine("CoFade");
			StartCoroutine("CoFade", autiomaticDestoroy);
		}

		IEnumerator CoFade(bool autiomaticDestoroy)
		{
			while (!fadeValue.IsEnd())
			{
				fadeValue.IncTime();
				LocalAlpha = fadeValue.GetValue();
				yield return 0;
			}
			if (autiomaticDestoroy)
			{
				GameObject.Destroy(this.gameObject);
			}
		}
	}
}