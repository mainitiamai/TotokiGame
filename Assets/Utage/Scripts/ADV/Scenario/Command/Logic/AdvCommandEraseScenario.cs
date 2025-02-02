//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Add : every-studio custom
// 毎日工房用にゲーム内で連動させるための表示削除コマンド
// Copyright 2015 mainitiamai
//----------------------------------------------


namespace Utage
{

	/// <summary>
	/// コマンド：シナリオ停止
	/// </summary>
	internal class AdvCommandEraseScenario : AdvCommand
	{
		public AdvCommandEraseScenario(StringGridRow row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.EraseScenario();
		}

		//コマンド終了待ち
		public override bool Wait(AdvEngine engine) { return true; }

	}
}