//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;

namespace Utage
{

	public class UtageMeuToolOutPutFile : ScriptableObject
	{
		/// <summary>
		/// セーブデータフォルダを開く
		/// </summary>
		[MenuItem(MeuToolOpen.MeuToolRoot + "Open Output Folder/SaveData", priority = 30)]
		static void OpenSaveDataFolder()
		{
			OpenFilePanelCreateIfMissing("Open utage save data folder", FileIOManager.SdkPersistentDataPath);
		}

		/// <summary>
		/// キャッシュデータフォルダを開く
		/// </summary>
		[MenuItem(MeuToolOpen.MeuToolRoot + "Open Output Folder/Cache", priority = 31)]
		static void OpenCacheFolder()
		{
			OpenFilePanelCreateIfMissing("Open utage cache folder", FileIOManager.SdkTemporaryCachePath);
		}
		
		/// <summary>
		/// セーブデータを全て削除
		/// </summary>
		[MenuItem(MeuToolOpen.MeuToolRoot + "Delete Output Files/SaveData", priority = 32)]
		static void DeleteSaveDataFiles()
		{
			if( EditorUtility.DisplayDialog(
				LanguageSystemText.LocalizeText(SystemText.DeleteAllSaveDataFilesTitle),
				LanguageSystemText.LocalizeText(SystemText.DeleteAllSaveDataFilesMessage),
				LanguageSystemText.LocalizeText(SystemText.Ok),
				LanguageSystemText.LocalizeText(SystemText.Cancel)
				))
			{
				DeleteFolder(FileIOManager.SdkPersistentDataPath);
			}
		}

		/// <summary>
		/// キャッシュファイルを全て削除
		/// </summary>
		[MenuItem(MeuToolOpen.MeuToolRoot + "Delete Output Files/Cache", priority = 33)]
		static void DeleteCacheFiles()
		{
			if (EditorUtility.DisplayDialog(
				LanguageSystemText.LocalizeText(SystemText.DeleteAllCacheFilesTitle),
				LanguageSystemText.LocalizeText(SystemText.DeleteAllCacheFilesMessage),
				LanguageSystemText.LocalizeText(SystemText.Ok),
				LanguageSystemText.LocalizeText(SystemText.Cancel)
				))
			{
				DeleteFolder(FileIOManager.SdkTemporaryCachePath);
			}
		}

		/// <summary>
		/// 全ファイルを全て削除
		/// </summary>
		[MenuItem(MeuToolOpen.MeuToolRoot + "Delete Output Files/All Files", priority = 34)]
		static void DeleteAllFiles()
		{
			if (EditorUtility.DisplayDialog(
				LanguageSystemText.LocalizeText(SystemText.DeleteAllOutputFilesTitle),
				LanguageSystemText.LocalizeText(SystemText.DeleteAllOutputFilesMessage),
				LanguageSystemText.LocalizeText(SystemText.Ok),
				LanguageSystemText.LocalizeText(SystemText.Cancel)
				))
			{
				DeleteSaveDataFiles();
				DeleteCacheFiles();
			}
		}

		static void DeleteFolder(string path)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
				Debug.Log("Delete " + path);
			}
			else
			{
				Debug.Log("Not found " + path);
			}
		}

		static void OpenFilePanelCreateIfMissing(string title, string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			EditorUtility.OpenFilePanel(title, path,"");
		}
	}
}