//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


using UnityEngine;

namespace Utage
{

	/// <summary>
	/// アセットファイルのパス情報
	/// </summary>
	public class AssetFilePathInfo
	{
		public AssetFilePathInfo(string path, int version)
		{
			this.path = path;
			this.version = version;
		}

		/// <summary>
		/// ファイルパス（またはURL）
		/// </summary>
		public string Path
		{
			get { return path; }
			set { path = value; }
		}
		string path;

		/// <summary>
		/// ファイルバージョン
		/// </summary>
		public int Version
		{
			get { return version; }
			set { version = value; }
		}
		int version;

	};
}