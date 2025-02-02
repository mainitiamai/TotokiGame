//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;

namespace Utage
{

	/// <summary>
	/// バックログ表示
	/// </summary>
	[RequireComponent(typeof(ListView))]
	[RequireComponent(typeof(Node2D))]
	[AddComponentMenu("Utage/ADV/UiBacklogManager")]
	public class AdvUiBacklogManager : MonoBehaviour
	{
		/// <summary>ADVエンジン</summary>
		[SerializeField]
		AdvEngine engine;

		//バックログデータへのインターフェース
		AdvBacklogManager BacklogManager { get { return engine.BacklogManager; } }

		/// <summary>選択肢のリストビュー</summary>
		public ListView ListView
		{
			get { return listView ?? (listView = GetComponent<ListView>()); }
		}
		ListView listView;


		/// <summary>
		/// 初期化。スクリプトから動的に生成する場合に
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		public void InitOnCreate(AdvEngine engine, AdvUiBacklog backlogItemPrefab)
		{
			this.engine = engine;
			this.ListView.ItemPrefab = backlogItemPrefab.GetComponent<ListViewItem>();
		}


		/// <summary>開いているか</summary>
		public bool IsOpen { get { return this.gameObject.activeSelf; } }

		/// <summary>
		/// 閉じる
		/// </summary>
		public void Close()
		{
			ListView.Close();
			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// 開く
		/// </summary>
		public void Open()
		{
			this.gameObject.SetActive(true);
			ListView.Open(BacklogManager.Backlogs.Count, CallbackCreateItem);
		}

		/// <summary>
		/// リストビューのアイテムが作られたときに呼ばれるコールバック
		/// </summary>
		/// <param name="go">作られたアイテムのGameObject</param>
		/// <param name="index">アイテムのインデックス</param>
		void CallbackCreateItem(GameObject go, int index)
		{
			AdvBacklog data = BacklogManager.Backlogs[index];
			AdvUiBacklog backlog = go.GetComponent<AdvUiBacklog>();
			backlog.Init(data, this.gameObject, index);
		}

		/// <summary>
		/// 音声再生ボタンが押された
		/// </summary>
		/// <param name="button">押されたボタン</param>
		void OnTapSound(Button button)
		{
			BacklogManager.TryPlayVoice(engine, button.Index);
		}

		// 戻るボタンが押された
		void OnTapBack()
		{
			Back();
		}

		// 更新
		void Update()
		{
			if (InputUtil.IsInputScrollWheelDown() || InputUtil.IsMousceRightButtonDown())
			{
				Back();
			}
		}

		// バックログ閉じて、メッセージウィンドウ開く
		void Back()
		{
			this.Close();
			engine.UiManager.Status = AdvUiManager.UiStatus.Default;
		}
	}
}
