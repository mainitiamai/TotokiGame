//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


namespace Utage
{

	/// <summary>
	/// コマンド：シナリオ終了
	/// </summary>
	internal class AdvCommandEndScenario : AdvCommand
	{
		public AdvCommandEndScenario(StringGridRow row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.EndScenario();
		}

		//コマンド終了待ち
		public override bool Wait(AdvEngine engine) { return true; }

	}
}