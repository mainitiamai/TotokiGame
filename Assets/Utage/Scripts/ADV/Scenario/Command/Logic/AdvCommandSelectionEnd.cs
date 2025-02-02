//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：選択肢追加終了
	/// </summary>
	internal class AdvCommandSelectionEnd : AdvCommand
	{
		public AdvCommandSelectionEnd()
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.Config.StopSkipInSelection();
			engine.SelectionManager.StartWaiting();
			engine.Page.SetSelectionWait();
		}

		//コマンド終了待ち
		public override bool Wait(AdvEngine engine)
		{
			if (!engine.SelectionManager.IsWaitSelect)
			{
				AdvSelection selected = engine.SelectionManager.Selected;
				string label = selected.JumpLabel;
				if (selected.Exp != null)
				{
					engine.Param.CalcExpression(selected.Exp);
				}
				engine.SelectionManager.Clear();
				engine.JumpScenario(label);
			}
			//JumpScenarioした場合でも常にtrueを返すのが必須
			return true;
		}

		//ページ区切りのコマンドか
		public override bool IsTypePageEnd() { return true; }
	}
}