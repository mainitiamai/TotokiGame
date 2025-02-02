//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


namespace Utage
{

	/// <summary>
	/// コマンド：BGM停止
	/// </summary>
	internal class AdvCommandStopBgm : AdvCommand
	{
		public AdvCommandStopBgm(StringGridRow row)
		{
			this.fadeTime = AdvParser.ParseCellOptional<float>(row, AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.Stop(SoundManager.StreamType.Bgm, fadeTime);
		}

		float fadeTime;
	}
}