//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// サウンドルーム用のUIのサンプル
/// </summary>
[AddComponentMenu("Utage/Examples/UtageUiCgGalleryItem")]
[RequireComponent(typeof(ListViewItem))]
public class UtageUiCgGalleryItem : MonoBehaviour
{
	public Sprite2D texture;
	public TextArea2D count;
	public float pixelsToUnits = 100;

	public ListViewItem ListViewItem { get { return this.listViewItem ?? (this.listViewItem = GetComponent<ListViewItem>()); } }
	ListViewItem listViewItem;
	
	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="data">セーブデータ</param>
	/// <param name="index">インデックス</param>
	public void Init(AdvCgGalleryData data, int index)
	{
		bool isOpen = (data.NumOpen > 0 );
		ListViewItem.IsEnableButton = isOpen;
		if (isOpen)
		{
			texture.SetTextureFile(data.ThumbnailPath, pixelsToUnits);
			count.text = string.Format("{0,2}/{1,2}", data.NumOpen, data.NumTotal);
		}
		else
		{
			texture.LocalAlpha = 0;
			count.text = "";
		}
	}
}
