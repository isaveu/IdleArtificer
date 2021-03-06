﻿using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge.goals {
	public class GoalRepairDam : ObstacleType, IQuestGoal {
		public GoalRepairDam() : base("making repairs", new RequireWrapper(RequirementType.WOOD), new RequireWrapper(RequirementType.TOOLS)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result = EnumResult.MIXED - fails;

			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			if(theQuest.testStrength(questBonus)) {
				result += 1;
			}
			else {
				result -= 1;
				if(theQuest.testAgility(questBonus)) {
					result += 1;
				}
			}
			
			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.harmHero(30, DamageType.DROWN);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(120);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					if(questBonus < 3) {
						questBonus += 1;
						theQuest.repeatTask();
					}
					break;
				case EnumResult.CRIT_SUCCESS:
					ChallengeTypes.Loot.AddCommonResource(theQuest);
					break;
			}
		}

		public string relicDescription(ItemStack stack) {
			return "Aided repairing a dam";
		}

		public string relicNames(ItemStack stack) {
			return "Repair";
		}

		public int getNumTotalEncounters() {
			return 7;
		}
	}
}
