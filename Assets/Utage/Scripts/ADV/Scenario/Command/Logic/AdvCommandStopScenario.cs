//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Add : every-studio custom
// 毎日工房用にゲーム内で連動させるための停止スクリプトを追加
// Copyright 2015 mainitiamai
//----------------------------------------------


namespace Utage
{

	/// <summary>
	/// コマンド：シナリオ停止
	/// </summary>
	internal class AdvCommandStopScenario : AdvCommand
	{
		public AdvCommandStopScenario(StringGridRow row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.StopScenario();
		}

		//コマンド終了待ち
		public override bool Wait(AdvEngine engine) { return true; }

	}
}