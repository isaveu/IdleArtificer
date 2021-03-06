﻿using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.challenge {
	public class ObstacleDarkWoods : ObstacleType {
		public ObstacleDarkWoods() : base("traveling in a dark forest", new RequireWrapper(RequirementType.LIGHT)) {

		}
		public override EnumResult MakeAttempt(Quest theQuest, int fails, int partials, int questBonus) {
			EnumResult result;// = EnumResult.CRIT_FAIL;
			if(fails > 0) result = EnumResult.MIXED;
			else result = EnumResult.SUCCESS;

			if(theQuest.testIntelligence(0)) {
				result += 1;
			}
			else {
				if(theQuest.testLuck(questBonus + 2) == 0) {
					result -= 1;
				}
				result -= 1;
			}

			return result;
		}

		public override void OnAttempt(EnumResult result, Quest theQuest, ref int questBonus) {
			theQuest.harmHero(5, DamageType.STARVE); //minor damage
			switch(result) {
				case EnumResult.CRIT_FAIL:
					theQuest.hastenQuestEnding(60);
					theQuest.repeatTask();
					break;
				case EnumResult.FAIL:
					theQuest.hastenQuestEnding(30);
					theQuest.repeatTask();
					break;
				case EnumResult.MIXED:
					questBonus += 1;
					theQuest.repeatTask();
					break;
				case EnumResult.SUCCESS:
					break;
				case EnumResult.CRIT_SUCCESS:
					theQuest.hastenQuestEnding(-90);
					theQuest.addSubTask(new QuestChallenge(ChallengeTypes.Loot.getRandom(theQuest.questRand), 1));
					break;
			}
		}
	}
}
