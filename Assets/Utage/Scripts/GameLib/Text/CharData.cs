//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;


namespace Utage
{
	/// <summary>
	/// 文字データ（色などメタデータも含む）
	/// </summary>
	public class CharData
	{

		/// <summary>
		/// 文字
		/// </summary>
		public char Char { get { return this.c; } }
		char c;

		/// <summary>
		/// 色
		/// </summary>
		public Color Color { get { return this.color; } }
		Color color;

		/// <summary>
		/// サイズ
		/// </summary>
		public int Size { get { return this.size; } }
		int size;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="c">文字</param>
		/// <param name="color">色</param>
		/// <param name="size">サイズ</param>
		public CharData(char c, Color color, int size)
		{
			this.c = c;
			this.color = color;
			this.size = size;
		}

		/// <summary>
		/// 改行コードか
		/// </summary>
		public bool IsBr { get { return (Char == '\n'); } }

	};
}