//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utage
{

	//「Utage」のシナリオデータ用のエクセルファイルの管理エディタウイドウ
	public class CreateNewProjectWindow : EditorWindow
	{
		enum Type
		{
			CreateNewAdvScene,			//ADV用新規シーンを作成
			AddToCurrentScene,			//現在のシーンに追加
			CreateScenarioAssetOnly,	//シナリオ用プロジェクトファイルのみ作成
		};
		Type createType;
		string newProjectName = "";

		int gameScreenWidth = 800;
		int gameScreenHeight = 600;

		void OnGUI()
		{
			UtageEditorToolKit.BeginGroup("Create New Project");
			GUIStyle style = new GUIStyle();
			GUILayout.Space(4f);
			GUILayout.Label("<b>" + "Input New Project Name" + "</b>", style, GUILayout.Width(80f));
			newProjectName = GUILayout.TextField(newProjectName);

			GUILayout.Space(4f);
			GUILayout.Label("<b>" + "Select Create Type" + "</b>", style, GUILayout.Width(80f));
			Type type = (Type)EditorGUILayout.EnumPopup("Type", createType);
			if (createType != type)
			{
				createType = type;
			}
			UtageEditorToolKit.EndGroup();

			bool isGameSizeEnable = (createType != Type.CreateScenarioAssetOnly);
			EditorGUI.BeginDisabledGroup(!isGameSizeEnable);
			GUILayout.Space(4f);
			UtageEditorToolKit.BeginGroup("Game Screen Size");
			int width = EditorGUILayout.IntField("Width", gameScreenWidth);
			if (gameScreenWidth != width && width > 0)
			{
				gameScreenWidth = width;
			}
			int height = EditorGUILayout.IntField("Hegiht", gameScreenHeight);
			if (gameScreenHeight != height && height > 0)
			{
				gameScreenHeight = height;
			}
			UtageEditorToolKit.EndGroup();
			EditorGUI.EndDisabledGroup();

			bool isProjectNameEnable = IsEnableProjcetName(newProjectName);
			EditorGUI.BeginDisabledGroup(!isProjectNameEnable);
			bool isCreate = GUILayout.Button("Create", GUILayout.Width(80f));
			EditorGUI.EndDisabledGroup();
			if(isCreate) Create();
		}

		//新たなプロジェクトを作成
		void Create()
		{
			switch (createType)
			{
				case Type.CreateNewAdvScene:
					if (!EditorApplication.SaveCurrentSceneIfUserWantsTo()) return;
					break;
				default:
					break;
			}

			//テンプレートをコピー
			CopyTemplate();
			string dir = ToProjectDirPath(newProjectName);

			//プロジェクトファイルを作成
			string path = FileUtil.GetProjectRelativePath(dir + newProjectName + ".project.asset");
			AdvScenarioDataProject ProjectData = UtageEditorToolKit.CreateNewUniqueAsset<AdvScenarioDataProject>(path);

			//プロジェクトにエクセルファイルを設定
			ProjectData.AddExcelAsset( UtageEditorToolKit.LoadAssetAtPath<Object>(GetExcelRelativePath() ));
			//プロジェクトファイルを設定してインポート
			AdvScenarioDataBuilderWindow.ProjectData = ProjectData;
			AdvScenarioDataBuilderWindow.Import();

			switch (createType)
			{
				case Type.CreateNewAdvScene:
					//ADV用新規シーンを作成
					CreateNewAdvScene();
					break;
				case Type.AddToCurrentScene:
					//テンプレートシーンをコピー
					AddToCurrentScene();
					break;
			}
		}

		//ADV用新規シーンを作成
		void CreateNewAdvScene()
		{
			//シーンを開く
			string scenePath = GetSceneRelativePath();
			EditorApplication.OpenScene(scenePath);

			//「宴」エンジンの初期化
			InitUtageEngine();

			//シーンをセーブ
			EditorApplication.SaveScene();
			Selection.activeObject = AssetDatabase.LoadAssetAtPath(scenePath, typeof(Object));
		}

		void AddToCurrentScene()
		{
			//シーンを開く
			string scenePath = GetSceneRelativePath();
			EditorApplication.OpenSceneAdditive(scenePath);

			//余分なオブジェクトを削除
			UtageUiTitle title = GameObject.FindObjectOfType<UtageUiTitle>();
			GameObject.DestroyImmediate(title.transform.parent.gameObject);
			SystemUi systemUi = GameObject.FindObjectOfType<SystemUi>();
			GameObject.DestroyImmediate(systemUi.gameObject);

			//シーンのアセットを削除
			AssetDatabase.DeleteAsset(scenePath);

			//「宴」エンジンの初期化
			InitUtageEngine();

			//エンジン休止状態に
			AdvEngine engine = GameObject.FindObjectOfType<AdvEngine>();
			engine.gameObject.SetActive(false);

			Selection.activeObject = AssetDatabase.LoadAssetAtPath(scenePath, typeof(Object));
		}

		void InitUtageEngine()
		{
			//シナリオデータの設定
			AdvEngine engine = GameObject.FindObjectOfType<AdvEngine>();
			AdvEngineStarter starter = GameObject.FindObjectOfType<AdvEngineStarter>();

			AdvSettingDataManager settingDataAsset = UtageEditorToolKit.LoadAssetAtPath<AdvSettingDataManager>(GetSettingAssetRelativePath());
			AdvScenarioDataExported exportedScenarioAsset = UtageEditorToolKit.LoadAssetAtPath<AdvScenarioDataExported>(GetScenarioAssetRelativePath());
			AdvScenarioDataExported[] exportedScenarioDataTbl = { exportedScenarioAsset };
			starter.InitOnCreate( engine, settingDataAsset, exportedScenarioDataTbl, newProjectName);
			
			//カメラに画面サイズを設定
			CameraManager cameraManager = GameObject.FindObjectOfType<CameraManager>();
			cameraManager.InitOnCreate(gameScreenWidth, gameScreenHeight);
			//入力枠のサイズ調整
			AdvInputManager inputManager = GameObject.FindObjectOfType<AdvInputManager>();
			BoxCollider2D collider = inputManager.GetComponent<BoxCollider2D>();
			collider.size = new Vector2(1.0f * gameScreenWidth / 100, 1.0f * gameScreenHeight / 100);
		}

		bool IsEnableProjcetName(string name)
		{
			if( string.IsNullOrEmpty(name) ) return false;
			if (System.IO.Directory.Exists(ToProjectDirPath(name))) return false;
			return true;
		}
		string ToProjectDirPath(string name)
		{
			return Application.dataPath + "/" + name + "/";
		}

		string GetProjectRelativePath()
		{
			return "Assets/" + newProjectName + "/" + newProjectName;
		}
		string GetExcelRelativePath()
		{
			return GetProjectRelativePath() + ".xls";
		}
		string GetSceneRelativePath()
		{
			return GetProjectRelativePath() + ".unity";
		}
		string GetSettingAssetRelativePath()
		{
			return GetProjectRelativePath() + AdvExcelImporter.SettingAssetExt;
		}
		string GetScenarioAssetRelativePath()
		{
			return GetProjectRelativePath() + AdvExcelImporter.ScenarioAssetExt;
		}

		void CopyTemplate()
		{
			const string TemplateName = "Template";
			string from = "Assets/Utage/" + TemplateName;
			string to = "Assets/"+newProjectName;
			FileUtil.CopyFileOrDirectory(from,to);
			//リフレッシュ必須
			AssetDatabase.Refresh();

			string projectPath = Application.dataPath + "/" + newProjectName;
			//Templateというファイル名をリネーム
			foreach (string filePath in System.IO.Directory.GetFiles(projectPath, "*", SearchOption.AllDirectories))
			{
				if (Path.GetFileNameWithoutExtension(filePath) == TemplateName && Path.GetExtension(filePath) != ".meta")
				{
					string src = FileUtil.GetProjectRelativePath(filePath).Replace("\\", "/");
					string error = AssetDatabase.RenameAsset(src, newProjectName);
					if (!string.IsNullOrEmpty(error))
					{
						Debug.LogError(src + " " + error);
					}
				}
			}
			//Templateというフォルダ名をリネーム
			foreach (string dirPath in System.IO.Directory.GetDirectories(projectPath, "*", SearchOption.AllDirectories))
			{
				if (Path.GetFileName(dirPath) == TemplateName)
				{
					string src = FileUtil.GetProjectRelativePath(dirPath).Replace("\\", "/");
					string error = AssetDatabase.RenameAsset(src, newProjectName);
					if (!string.IsNullOrEmpty(error))
					{
						Debug.LogError(src + " " + error);
					}
				}
			}
			AssetDatabase.Refresh();
		}

		/*
				static AdvScenarioDataProject ProjectData
				{
					get	{
						if (scenarioDataProject == null)
						{
							scenarioDataProject = UtageEditorPrefs.LoadAsset<AdvScenarioDataProject>(UtageEditorPrefs.Key.AdvScenarioProject);
						}
						return scenarioDataProject;
					}
					set
					{
						if (scenarioDataProject != value)
						{
							scenarioDataProject = value;
							UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.AdvScenarioProject, scenarioDataProject);
						}
					}
				}
				//プロジェクトデータ
				static AdvScenarioDataProject scenarioDataProject;
				//プロジェクトデータの基本パス
				const string excelProjectPath = "Assets/Utage/Editor/EditorSave/";

				/// <summary>
				/// エクセルをコンバート
				/// </summary>
				public static void Convert()
				{
					if (ProjectData == null)
					{
						Debug.LogWarning("Scenario Data Excel project is no found");
						return;
					}
					if (string.IsNullOrEmpty(ProjectData.ConvertPath))
					{
						Debug.LogWarning("Convert Path is not defined");
						return;
					}
					AdvExcelCsvConverter converter = new AdvExcelCsvConverter();
					if ( !converter.Convert(ProjectData.ConvertPath, ProjectData.ExcelPathList, ProjectData.ConvertVersion ) )
					{
						Debug.LogWarning("Convert is failed");
						return;
					}
					if (ProjectData.IsAutoUpdateVersionAfterConvert)
					{
						ProjectData.ConvertVersion = ProjectData.ConvertVersion + 1;
						EditorUtility.SetDirty(ProjectData);
					}
				}

				/// <summary>
				/// 管理対象のエクセルをインポート
				/// </summary>
				static void Import()
				{
					AdvExcelImporter importer = new AdvExcelImporter();
					importer.Import(ProjectData.ExcelPathList);
					if (ProjectData.IsAutoConvertOnImport)
					{
						Convert();
					}
				}

				/// <summary>
				/// インポートされたアセットが管理対象ならインポート
				/// </summary>
				public static void Import( string[] importedAssets )
				{
					if (ProjectData == null)
					{
						//シナリオが設定されてないのでインポートしない
						return;
					}

					bool isContains = false;
					List<string> assetPathList = ProjectData.ExcelPathList;
					foreach (string importedAsset in importedAssets)
					{
						isContains |= assetPathList.Contains(importedAsset);
					}
					if (isContains)
					{
						Import();
					}
				}
		*/
		/*

				bool isOpenNewProject = false;
				string newProjectName = "";

				void OnGUI()
				{
					if (isOpenNewProject)
					{
						DrawNewProject();
					}
					else
					{
						DrawDefault();
					}
				}

				void DrawNewProject()
				{
					GUIStyle style = new GUIStyle();
					GUILayout.Space(4f);
					GUILayout.Label("<b>" + "Input New Project Name" + "</b>", style, GUILayout.Width(80f));
					newProjectName = GUILayout.TextField(newProjectName);

					EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(newProjectName));
					if (GUILayout.Button("Create", GUILayout.Width(80f)))
					{
						isOpenNewProject = false;
						ProjectData = UtageEditorToolKit.CreateNewUniqueAsset<AdvScenarioDataProject>(excelProjectPath + newProjectName + ".project.asset");
						Selection.activeObject = ProjectData;
					}
					EditorGUI.EndDisabledGroup();
					if (GUILayout.Button("Cancel", GUILayout.Width(80f)))
					{
						isOpenNewProject = false;
					}
				}

				void DrawDefault()
				{
					GUILayout.Space(4f);
					EditorGUILayout.BeginHorizontal();
					GUIStyle style = new GUIStyle();
					style.richText = true;
					GUILayout.Label("<b>" + "Project" + "</b>", style, GUILayout.Width(80f) );
					if (GUILayout.Button("Create New", GUILayout.Width(80f)))
					{
						isOpenNewProject = true;
					}
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(4f);

					AdvScenarioDataProject project = EditorGUILayout.ObjectField("", ProjectData, typeof(AdvScenarioDataProject), false) as AdvScenarioDataProject;
					if (project != ProjectData)
					{
						ProjectData = project;
					}

					if (ProjectData != null)
					{
						DrawProject();
					}
				}

				void DrawProject()
				{
					SerializedObject serializedObject = new SerializedObject(ProjectData);
					serializedObject.Update();

					UtageEditorToolKit.BeginGroup("Import Files");
					UtageEditorToolKit.PropertyFieldArray(serializedObject, "excelList", "Excel List");
					UtageEditorToolKit.EndGroup();

					GUILayout.Space(8f);

					EditorGUI.BeginDisabledGroup(!ProjectData.IsEnableImport);
					if (GUILayout.Button("Import", GUILayout.Width(180)))
					{
						Import();
					}
					EditorGUI.EndDisabledGroup();

					GUILayout.Space(8f);

					UtageEditorToolKit.BeginGroup("Covert Setting");

					EditorGUILayout.BeginHorizontal();
					UtageEditorToolKit.PropertyField(serializedObject, "convertPath", "Output directory");
					if (GUILayout.Button("Select", GUILayout.Width(100)))
					{
						string convertPath = ProjectData.ConvertPath;
						string dir = string.IsNullOrEmpty(convertPath) ? "" : Path.GetDirectoryName(convertPath);
						string name = string.IsNullOrEmpty(convertPath) ? "" : Path.GetFileName(convertPath);
						string path = EditorUtility.SaveFolderPanel("Select folder to output", dir, name);
						Debug.Log(path);
						if (!string.IsNullOrEmpty(path))
						{
							ProjectData.ConvertPath = path;
						}
					}
					EditorGUILayout.EndHorizontal();

					UtageEditorToolKit.PropertyField(serializedObject, "convertVersion", "Version");

					UtageEditorToolKit.PropertyField(serializedObject, "isAutoUpdateVersionAfterConvert", "Auto Update Version");


					UtageEditorToolKit.EndGroup();
					GUILayout.Space(4f);

					EditorGUI.BeginDisabledGroup(!ProjectData.IsEnableConvert);
					UtageEditorToolKit.PropertyField(serializedObject, "isAutoConvertOnImport", "Auto Convert OnImport");
					if (GUILayout.Button("Convert", GUILayout.Width(180)))
					{
						Convert();
					}
					EditorGUI.EndDisabledGroup();


					serializedObject.ApplyModifiedProperties();
				}*/
	}
}