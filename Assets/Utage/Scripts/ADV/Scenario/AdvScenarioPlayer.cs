//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utage
{

	/// <summary>
	/// シナリオを進めていくプレイヤー
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/Config")]
	public class AdvScenarioPlayer : MonoBehaviour
	{
		/// <summary>
		/// 「SendMessage」コマンドが実行されたときにSendMessageを受け取るGameObject
		/// </summary>
		public GameObject SendMessageTarget { get { return sendMessageTarget; } }
		[SerializeField]
		GameObject sendMessageTarget;

		[System.Flags]
		enum DebugOutPut
		{
			Log = 0x01,
			Waiting = 0x02,
			CommandEnd = 0x04,
		};
		[SerializeField]
		[EnumFlags]
		DebugOutPut debugOutPut = 0;

		/// <summary>
		/// シナリオデータ
		/// </summary>
		AdvScenarioData scearioData;


		/// <summary>
		/// 現在のシナリオラベル
		/// </summary>
		public string CurrentScenarioLabel { get { return this.currentScenarioLabel; } }
		string currentScenarioLabel;

		/// <summary>
		/// 現在の、シーン回想用のシーンラベル
		/// </summary>
		public string CurrentGallerySceneLabel { get { return this.currentGallerySceneLabel;}  set{ this.currentGallerySceneLabel = value;} }
		string currentGallerySceneLabel = "";

		/// <summary>
		/// 現在のページ(シナリオラベルからの相対)
		/// </summary>
		public int CurrentPage { get { return this.currentPage; } }
		int currentPage;

		int currentIndex;

		/// <summary>
		/// ロード中か
		/// </summary>
		public bool IsWaitLoading { get { return this.isWaitLoading ; } }
		bool isWaitLoading = false;

		/// <summary>
		/// シナリオ終了したか
		/// </summary>
		public bool IsEndScenario { get { return this.isEndScenario; } }
		bool isEndScenario = false;

		/// <summary>
		/// シナリオ停止したか
		/// </summary>
		public bool IsStopScenario { get { return this.isStopScenario; } }
		bool isStopScenario = false;


		//If文制御のマネージャー
		internal AdvIfManager IfManager { get { return this.ifManager; } }
		AdvIfManager ifManager = new AdvIfManager();

		const int MAX_PRELOAD_FILES = 20;	///事前にロードするファイルの最大数
		HashSet<AssetFile> preloadFileSet = new HashSet<AssetFile>();

		/// <summary>
		/// 最初の状態に
		/// </summary>
		void Reset()
		{
			currentPage = 0;
			currentIndex = 0;
			isWaitLoading = false;
			ifManager.Clear();
			ClearPreload();
		}

		/// <summary>
		/// シナリオ終了
		/// </summary>
		public void EndScenario()
		{
			Reset();
			StopAllCoroutines();
			isEndScenario = true;
			isStopScenario= true;
		}

		//先読みファイルをクリア
		void ClearPreload()
		{
			//直前の先読みファイルは参照を減算しておく
			foreach (AssetFile file in preloadFileSet)
			{
				file.Unuse(this);
			}
			preloadFileSet.Clear();
		}

		/// <summary>
		/// ジャンプ処理の準備
		/// </summary>
		public void JumpReady(AdvEngine engine )
		{
			//既読ページとして記録
			engine.SystemSaveData.ReadData.AddReadPage(engine.Page.ScenarioLabel,engine.Page.PageNo);
		}

		/// <summary>
		/// シナリオの実行開始
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		/// <param name="scenarioLabel">ジャンプ先のシナリオラベル</param>
		/// <param name="page">シナリオラベルからのページ数</param>
		/// <param name="scenarioLabel">ジャンプ先のシーン回想用シナリオラベル</param>
		public void StartScenario(AdvEngine engine, string label, int page, string gallerySceneLabel)
		{
			isEndScenario = false;
			isStopScenario= false;
			//前回の実行がまだ回ってるかもしれないので止める
			StopAllCoroutines();
			StartCoroutine(CoStartScenario(engine, label, page, gallerySceneLabel));
		}

		IEnumerator CoStartScenario(AdvEngine engine, string label, int page, string gallerySceneLabel)
		{
			if ((debugOutPut & DebugOutPut.Log) == DebugOutPut.Log) Debug.Log("Jump : " + label + " :" + page);

			if (!engine.DataManager.IsReadySettingData)
			{
				Debug.LogError("Not ready SettingData");
			}

			isWaitLoading = true;
			while (!engine.DataManager.IsLoadEndScenarioLabel(label))
			{
				yield return 0;
			}
			scearioData = engine.DataManager.FindScenarioData(label);

			Reset();
			//指定のページまでジャンプ
			currentIndex = scearioData.SeekPageIndex(label, page);
			currentScenarioLabel = label;
			currentPage = (page < 0) ?  page : page -1;
			currentGallerySceneLabel = gallerySceneLabel;
			engine.Page.BeginPage(currentScenarioLabel, currentPage);
			UpdateSceneGallery(currentScenarioLabel, engine);

			isWaitLoading = false;
			if (preloadFileSet.Count > 0)
			{
				Debug.LogError("Error Preload Clear");
			}

			AdvCommand command = scearioData.GetCommand(currentIndex);
			while (null != command)
			{
				//ロード
				command.Load();

				//プリロードを更新
				if (command.IsExistLoadFile())
				{
					UpdatePreLoadFiles(currentIndex, MAX_PRELOAD_FILES);
				}

				//ロード待ち
				while (!command.IsLoadEnd())
				{
					isWaitLoading = true;
					yield return 0;
				}
				isWaitLoading = false;

				///シナリオラベルの更新
				if ( !string.IsNullOrEmpty(command.GetScenarioLabel()) )
				{
					currentScenarioLabel = command.GetScenarioLabel();
					currentPage = -1;
					///ページ開始処理
					engine.Page.BeginPage(currentScenarioLabel, currentPage);
					UpdateSceneGallery(currentScenarioLabel, engine);
				}

				//コマンド実行
				if ((debugOutPut & DebugOutPut.Log) == DebugOutPut.Log) Debug.Log("Command : " + command.GetType() );
				command.DoCommand(engine);
				///ページ末端・オートセーブデータを更新
				if (command.IsTypePageEnd())
				{
					++currentPage;
					///ページ開始処理
					engine.Page.BeginPage(currentScenarioLabel, currentPage);
					engine.SaveManager.UpdateAutoSaveData(engine);
				}

				//コマンド実行後にファイルをアンロード
				command.Unload();

				//コマンドの処理待ち
				while (command.Wait(engine) )
				{
					if ((debugOutPut & DebugOutPut.Waiting) == DebugOutPut.Waiting) Debug.Log("Wait..." + command.GetType() );
					yield return 0;
				}

				if ((debugOutPut & DebugOutPut.CommandEnd) == DebugOutPut.CommandEnd) Debug.Log("End :" + command.GetType() + " " + label + ":" + page);

				///改ページ処理
				if (command.IsTypePageEnd())
				{
					engine.SystemSaveData.ReadData.AddReadPage(engine.Page.ScenarioLabel, engine.Page.PageNo);
					engine.Page.EndPage();
				}

				//次のコマンドへ
				do
				{
					++currentIndex;
					command = scearioData.GetCommand(currentIndex);

					//ifスキップチェック
					if (!ifManager.CheckSkip(command))
					{
						break;
					}
					else
					{
						///ページ末端
						if (command.IsTypePageEnd())
						{
							++currentPage;
						}
					}
				} while (true);
			}

			EndScenario();
		}

		//先読みかけておく
		void UpdatePreLoadFiles(int commandIndex, int fileCount)
		{
			HashSet<AssetFile> lastPreloadFileSet = preloadFileSet;

			preloadFileSet = new HashSet<AssetFile>();
			while (true)
			{
				++commandIndex;
				AdvCommand command = scearioData.GetCommand(commandIndex);
				if (null == command)
				{
					break;
				}

				if (command.LoadFileList != null)
				{
					foreach (AssetFile file in command.LoadFileList)
					{
						preloadFileSet.Add(file);
					}
				}
				if (preloadFileSet.Count > fileCount)
				{
					break;
				}
			};

			//リストに従って先読み
			foreach (AssetFile file in preloadFileSet)
			{
				AssetFileManager.Preload(file, this);
				//もうプリロードされなくなったリストを作るために
				if (lastPreloadFileSet.Contains(file))
				{
					lastPreloadFileSet.Remove(file);
				}
			}
			//直前の先読みファイルは参照を減算しておく
			foreach (AssetFile file in lastPreloadFileSet)
			{
				file.Unuse(this);
			}
		}

		/// <summary>
		/// シーン回想のためにシーンラベルを更新
		/// </summary>
		/// <param name="label">シーンラベル</param>
		/// <param name="engine">ADVエンジン</param>
		void UpdateSceneGallery(string label, AdvEngine engine)
		{
			AdvSceneGallerySetting galleryData = engine.DataManager.SettingDataManager.SceneGallerySetting;
			if (galleryData.ContainsKey(label))
			{
				if (currentGallerySceneLabel != label)
				{
					if (!string.IsNullOrEmpty(currentGallerySceneLabel))
					{
						//別のシーンが終わってないのに、新しいシーンに移っている
						Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.UpdateSceneLabel, currentGallerySceneLabel, label));
					}
					currentGallerySceneLabel = label;
				}
			}
		}

		/// <summary>
		/// シーン回想のためのシーンの終了処理
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		public void EndSceneGallery(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(currentGallerySceneLabel))
			{
				//シーン回想に登録されていないのに、シーン回想終了がされています
				Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.EndSceneGallery));
			}
			else
			{
				engine.SystemSaveData.GalleryData.AddSceneLabel(currentGallerySceneLabel);
				currentGallerySceneLabel = "";
			}
		}
	}
}