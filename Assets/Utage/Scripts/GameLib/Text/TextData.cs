//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// テキストデータ（文字列のほかの色などメタデータも）
	/// </summary>
	public class TextData
	{

		/// <summary>
		/// 文字データリスト
		/// </summary>
		public List<CharData> CharList { get { return this.charList; } }
		List<CharData> charList = new List<CharData>();

		/// <summary>
		/// デフォルトカラー
		/// </summary>
		public Color DefaultColor { get { return defaultColor; } set { this.defaultColor = value; } }
		Color defaultColor = Color.white;

		/// <summary>
		/// デフォルトサイズ
		/// </summary>
		public int DefaultSize { get { return defaultSize; } set { this.defaultSize = value; } }
		int defaultSize = 0;

		/// <summary>
		/// 文字列から数式を計算するコールバック
		/// </summary> 
		public static Func<string, object> CallbackCalcExpression;

		/// <summary>
		/// エラーメッセージ
		/// </summary>
		public string ErrorMsg { get { return this.errorMsg; } }
		string errorMsg = null;
		void AddErrorMsg(string msg)
		{
			if (string.IsNullOrEmpty(errorMsg)) errorMsg = "";
			else errorMsg += "\n";

			errorMsg += msg;
		}

		/// <summary>
		/// 表示文字数（メタデータを覗く）
		/// </summary>
		public int Length { get { return CharList.Count; } }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="text">メタデータを含むテキスト</param>
		public TextData(string text)
		{
			Parse(text);
		}

		/// <summary>
		/// メタ情報なしの文字列を取得
		/// </summary>
		/// <returns>メタ情報なしの文字列</returns>
		public override string ToString()
		{
			return ToString(0, CharList.Count);
		}

		/// <summary>
		/// メタ情報なしの文字列を取得
		/// </summary>
		/// <param name="index">文字の先頭インデックス</param>
		/// <param name="count">文字数</param>
		/// <returns>メタ情報なしの文字列を取得</returns>
		public string ToString(int index, int count)
		{
			int max = Mathf.Min(index + count, CharList.Count);
			string str = "";
			for (int i = index; i < max; ++i)
			{
				str += CharList[i].Char;
			}
			return str;
		}


		/// <summary>
		/// Unityのリッチテキストフォーマットのテキストを取得
		/// </summary>
		/// <returns>Unityのリッチテキストフォーマットのテキスト</returns>
		public string ToUnityRitchText()
		{
			return ToUnityRitchText(0, CharList.Count);
		}

		/// <summary>
		/// Unityのリッチテキストフォーマットのテキストを取得
		/// </summary>
		/// <param name="index">文字の先頭インデックス</param>
		/// <param name="count">文字数</param>
		/// <returns>Unityのリッチテキストフォーマットのテキスト</returns>
		public string ToUnityRitchText(int index, int count)
		{
			int max = Mathf.Min(index + count, CharList.Count);
			string str = "";
			Color lastColor = defaultColor;
			for (int i = index; i < max; ++i)
			{
				CharData c = CharList[i];
				if (c.Color != lastColor)
				{
					if (lastColor != defaultColor)
					{
						str += "</color>";
					}
					if (c.Color != defaultColor)
					{
						str += "<color=#" + ColorUtil.ToColorString(c.Color) + ">";
					}
				}
				str += CharList[i].Char;
				lastColor = c.Color;
			}
			if (lastColor != defaultColor)
			{
				str += "</color>";
			}
			return str;
		}

		/// <summary>
		/// NUGIフォーマットのテキストを取得
		/// </summary>
		/// <returns>NUGIフォーマットのテキスト</returns>
		public string ToNguiText()
		{
			return ToNguiText(0, CharList.Count);
		}

		/// <summary>
		/// NUGIフォーマットのテキストを取得
		/// </summary>
		/// <param name="index">文字の先頭インデックス</param>
		/// <param name="count">文字数</param>
		/// <returns>NUGIフォーマットのテキスト</returns>
		public string ToNguiText(int index, int count)
		{
			int max = Mathf.Min(index + count, CharList.Count);
			string str = "";
			Color lastColor = defaultColor;
			for (int i = index; i < max; ++i)
			{
				CharData c = CharList[i];
				if (c.Color != lastColor)
				{
					if (lastColor != defaultColor)
					{
						str += "[-]";
					}
					if (c.Color != defaultColor)
					{
						str += "[" + ColorUtil.ToNguiColorString(c.Color) + "]";
					}
				}
				str += CharList[i].Char;
				lastColor = c.Color;
			}
			if (lastColor != defaultColor)
			{
				str += "[-]";
			}
			return str;
		}


		/// <summary>
		/// メタデータを含むテキストデータを解析
		/// </summary>
		/// <param name="text">解析するテキスト</param>
		void Parse(string text)
		{
			try
			{
				text = text.Replace("\\n", "\n");

				Color currentColor = defaultColor;
				int currentSize = defaultSize;

				int max = text.Length;
				int index = 0;
				while (index < max)
				{
					if (!ParseTag(text, ref index, ref currentColor, ref currentSize))
					{
						AddChar(text[index], currentColor, currentSize);
						++index;
					}
				}
			}
			catch( System.Exception e )
			{
				AddErrorMsg(e.Message);
			}
		}

		enum TAG_TYPE
		{
			UNKNOWN,
			BOLD_BEGIN,
			BOLD_END,
			ITALIC_BEGIN,
			ITALIC_END,
			SIZE_BEGIN,
			SIZE_END,
			COLOR_BEGIN,
			COLOR_END,
			PARAM,
			FORMAT,
		}

		//タグを解析
		bool ParseTag(string text, ref int index, ref Color currentColor, ref int currentSize)
		{
			if (text[index] == '<')
			{
				int startIndex = index + 1;
				int length = 0;
				List<string> args;
				int size;
				TAG_TYPE tag = ParaseTag(text, startIndex, out length, out args);
				switch (tag)
				{
					case TAG_TYPE.COLOR_BEGIN:
						if (ColorUtil.TryParseColor(args[0], ref currentColor))
						{
							break;
						}
						else
						{
							return false;
						}
					case TAG_TYPE.COLOR_END:
						currentColor = defaultColor;
						break;
					case TAG_TYPE.SIZE_BEGIN:
						if (int.TryParse(args[0], out size))
						{
							currentSize = size;
							break;
						}
						else
						{
							return false;
						}
					case TAG_TYPE.SIZE_END:
						currentSize = defaultSize;
						break;
					case TAG_TYPE.PARAM:
						{
							string str = ExpressionToString(args[0]);
							AddStrng(str, currentColor, currentSize);
							break;
						}
					case TAG_TYPE.FORMAT:
						{
							int num = args.Count - 1;
							string[] parmKeys = new string[num];
							args.CopyTo(1, parmKeys, 0, num);
							string str = FormatExpressionToString(args[0], parmKeys);
							AddStrng(str, currentColor, currentSize);
							break;
						}
					default:
						AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextTagParse, text.Substring(startIndex, length) ));
						return false;
				};
				index += (length + 1);
				return true;
			}
			else
			{
				return false;
			}
		}

		//文字列を追加
		void AddStrng(string text, Color currentColor, int currentSize)
		{
			foreach (char c in text)
			{
				AddChar(c, currentColor, currentSize);
			}
		}

		//文字を追加
		void AddChar(char c, Color color, int size)
		{
			CharData data = new CharData(c, color, size);
			charList.Add(data);
		}

		TAG_TYPE ParaseTag(string text, int startIndex, out int length, out List<string> args)
		{
			args = null;
			length = 0;
			int index = startIndex;
			//先頭がスペースなら削除
			while (index < text.Length)
			{
				if (!char.IsWhiteSpace(text[index]))
				{
					break;
				}
				else
				{
					++index;
				}
			}

			bool isEndTag = false;
			//最初の文字が / なら終了タグフラグ
			if (text[index] == '/')
			{
				isEndTag = true;
				++index;
			}

			int tagStart = index;
			++index;
			while (index < text.Length)
			{
				if (text[index] == '>')
				{
					//タグの区切り文字があれば、タグの解析
					length = index - startIndex + 1;

					if (text[index - 1] == '/')
					{
						//単発のタグ
						--index;
					}
					--index;
					int tagLen = index - tagStart + 1;
					char[] separator = { '=', ':' };
					string[] tagTexts = text.Substring(tagStart, tagLen).Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
					return ParseTagSub(tagTexts, isEndTag, out args );
				}
				else
				{
					++index;
				}
			}
			return TAG_TYPE.UNKNOWN;
		}

		TAG_TYPE ParseTagSub(string[] tagTexts, bool isEndTag, out List<string> args)
		{
			args = new List<string>();
			if (tagTexts.Length == 1)
			{
				switch (tagTexts[0])
				{
					case "b":
						return isEndTag ? TAG_TYPE.BOLD_END : TAG_TYPE.BOLD_BEGIN;
					case "i":
						return isEndTag ? TAG_TYPE.ITALIC_END : TAG_TYPE.ITALIC_BEGIN;
					case "size":
						return isEndTag ? TAG_TYPE.SIZE_END : TAG_TYPE.UNKNOWN;
					case "color":
						return isEndTag ? TAG_TYPE.COLOR_END : TAG_TYPE.UNKNOWN;
					default:
						return TAG_TYPE.UNKNOWN;
				}
			}
			else if (tagTexts.Length >= 2)
			{
				switch (tagTexts[0])
				{
					case "b":
						if (!isEndTag)
						{
							args.Add(tagTexts[1]);
							return TAG_TYPE.BOLD_BEGIN;
						}
						else
						{
							return TAG_TYPE.UNKNOWN;
						}
					case "size":
						if (!isEndTag)
						{
							args.Add( tagTexts[1] );
							return TAG_TYPE.SIZE_BEGIN;
						}
						else
						{
							return TAG_TYPE.UNKNOWN;
						}
					case "color":
						if (!isEndTag)
						{
							args.Add(tagTexts[1]);
							return TAG_TYPE.COLOR_BEGIN;
						}
						else
						{
							return TAG_TYPE.UNKNOWN;
						}
					case "param":
						args.Add( tagTexts[1] );
						return TAG_TYPE.PARAM;
					case "format":
						for (int i = 1; i < tagTexts.Length; ++i)
						{
							args.Add(tagTexts[i]);
						}
						return TAG_TYPE.FORMAT;
					default:
						return TAG_TYPE.UNKNOWN;
				}
			}
			else
			{
				return TAG_TYPE.UNKNOWN;
			}
		}

		/// <summary>
		/// 数式の結果を文字列にする
		/// </summary>
		/// <param name="exp">数式の文字列</param>
		/// <returns>結果の値の文字列</returns>
		string ExpressionToString(string exp)
		{
			if (null == CallbackCalcExpression)
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextCallbackCalcExpression));
				return "";
			}
			else
			{
				object obj = CallbackCalcExpression(exp);
				if (obj == null)
				{
					AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextFailedCalcExpression));
					return "";
				}
				else
				{
					return obj.ToString();
				}
			}
		}



		/// <summary>
		/// フォーマットつき数式の結果を文字列にする
		/// </summary>
		/// <param name="format">出力フォーマット</param>
		/// <param name="exps">数式の文字列のテーブル</param>
		/// <returns>結果の値の文字列</returns>
		string FormatExpressionToString(string format, string[] exps)
		{
			if (null == CallbackCalcExpression)
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextCallbackCalcExpression));
				return "";
			}
			else
			{
				List<object> args = new List<object>();
				foreach (string exp in exps)
				{
					args.Add(CallbackCalcExpression(exp));
				}
				return string.Format(format, args.ToArray());
			}
		}
	}
}