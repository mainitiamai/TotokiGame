//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utage
{

	/// <summary>
	/// 表示言語切り替え用のクラス
	/// </summary>
	public class CustomProjectSetting : ScriptableObject
	{
		/// <summary>
		/// シングルトンなインスタンスの取得
		/// </summary>
		/// <returns></returns>
		public static CustomProjectSetting Instance
		{
			get
			{
#if UNITY_EDITOR
				if (instance == null)
				{
					Instance = UtageEditorPrefs.LoadAsset<CustomProjectSetting>(UtageEditorPrefs.Key.CustomProjectSetting, "Assets/Utage/Examples/ScriptableObject/Example CustomProjectSetting.asset");
				}
#endif
				return instance;
			}

			set
			{
				instance = value;
				if (instance != null)
				{
					LanguageManagerBase.Instance = instance.language;
#if UNITY_EDITOR
					UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CustomProjectSetting, instance);
#endif
				}
			}
		}
		static CustomProjectSetting instance;

		/// <summary>
		/// 設定言語
		/// </summary>
		public LanguageManager Language
		{
			get { return language; }
		}
		[SerializeField]
		LanguageManager language;

		/// <summary>
		/// 設定言語
		/// </summary>
		public Node2DSortData Node2DSortData
		{
			get { return sortData2D; }
		}
		[SerializeField]
		Node2DSortData sortData2D;
	}
}
