//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：シナリオラベル
	/// </summary>
	internal class AdvCommandScenarioLabel : AdvCommand
	{
		public AdvCommandScenarioLabel(StringGridRow row)
		{
			this.scenarioLabel = AdvCommandParser.ParseScenarioLabel(row, AdvColumnName.Command);
			this.title = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg1, this.scenarioLabel);
		}


		public override void DoCommand(AdvEngine engine)
		{
		}

		//シナリオラベルか
		public override string GetScenarioLabel() { return scenarioLabel; }
		//タイトル
		public string GetScenarioTitle() { return title; }

		string scenarioLabel;
		string title;
	}
}