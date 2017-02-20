﻿//using UnityEngine;
using System.Collections;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.requirement;
using System;
using System.Reflection;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge.goals;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.statistics;

namespace Assets.draco18s.artificer.init {
	public static class ChallengeTypes {

		public static class Goals {
			#region easy
			//wood, leather, armor, weapon, torches--all basic stuff
			//public static ObstacleType FIND_COWS = new GoalFindCows().setRewardScalar(1).setReqScalar(0.5f);//???
			public static ObstacleType REPAIR_DAMN = new GoalRepairDam().setRewardScalar(1).setReqScalar(1.5f);//wood
			public static ObstacleType SCARY_CAVE = new GoalExploreCave().setRewardScalar(1).setReqScalar(0.75f);//torches
			public static ObstacleType PATROLS = new GoalPatrolDuty().setRewardScalar(1);//light + healing
			public static ObstacleType ESTABLISH_HOME = new GoalBuildHome().setRewardScalar(1).setReqScalar(1.5f);//wood + leather
			public static ObstacleType TRAINING = new GoalCombatTraining().setRewardScalar(1);//weapon + armor
			public static ObstacleType TARGET_PRACTICE = new GoalTargetPractice().setRewardScalar(1);//ranged weapon + (agl)
			#endregion
			
			#region moderate
			//can be done with relatively simple products, but has semi-optional higher teir requirement
			public static ObstacleType RAT_INFESTATION = new GoalClearRatInfestation().setRewardScalar(2);//poison
			public static ObstacleType PLANT_GARDEN = new GoalPlantGarden().setRewardScalar(2).setReqScalar(1.5f);//herbs*3
			public static ObstacleType ALCHEMY_LAB = new GoalAlchemyLab().setRewardScalar(2).setReqScalar(1.5f);//herb + healing + mana + poison
			public static ObstacleType TOWN_INFRASTRUCTURE = new GoalUpgradeTown().setRewardScalar(2).setReqScalar(2f);//wood + iron
			public static ObstacleType GUARD_TEMPLE = new GoalGuardTemple().setRewardScalar(2).setReqScalar(0.75f);//armor + detection
			#endregion

			#region semi-advanced
			//require several basic or mid-level items
			public static ObstacleType DEFEND_VILLAGE = new GoalDefendVillage().setRewardScalar(4); //armor*2 + weapon*2
			public static ObstacleType DUKE = new GoalPetitionTheDuke().setRewardScalar(4);//cha
			public static ObstacleType SPHINX = new GoalSphynxRiddles().setRewardScalar(4);//int + mana
			public static ObstacleType INFILTRATE_CASTLE = new GoalInfiltrateCastle().setRewardScalar(4);//stealth + agl
			public static ObstacleType OBSERVE_ENEMY = new GoalObserveTroopMovements().setRewardScalar(4);//stealth + int
			#endregion

			#region advanced
			//require high-level potions
			public static ObstacleType EXPLORE_TOMB = new GoalExploreTomb().setRewardScalar(8);//danger sense + fire damage
			public static ObstacleType DRAGON = new GoalKillDragon().setRewardScalar(8);//fire immunity + cold damage
			#endregion
			#region very advanced
			//all require some kind of enchantment
			public static ObstacleType FREE_SLAVES = new GoalFreeSlaves().setRewardScalar(8);//firm resolve
			//public static ObstacleType SKELETON_INFESTATION = new GoalSkeletons().setRewardScalar(8);//disruption
			public static ObstacleType LITCH = new GoalKillLitch().setRewardScalar(8);//spell resist
			public static ObstacleType GORGON = new GoalSlayGorgon().setRewardScalar(8);//mirrored
			public static ObstacleType HYDRA = new GoalBeheadHydra().setRewardScalar(8);//vorpal, free move
			#endregion

			public static class Sub {
				public static ObstacleType MUMMY = new GoalExploreTomb_Mummy();
				public static ObstacleType SKELETONS = new GoalExploreTomb_Skeletons();
			}

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Goals).GetFields();
				int min = StatisticsTracker.minQuestDifficulty.value;
				int max = StatisticsTracker.maxQuestDifficulty.value;
				if(max >= fields.Length-1) {
					StatisticsTracker.allQuestsUnlocked.setAchieved();
				}
				max = Math.Min(max, fields.Length-1);
				int v = rand.Next(max - min) + min;
				FieldInfo field = fields[v];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}

		public static class Initial {
			public static ObstacleType AT_HOME = new ObstacleHome();
			public static ObstacleType TOWN_OUTSKIRTS = new ObstacleOutskirts();
			public static ObstacleType BUY_EQUIPMENT = new ObstacleBuyEquipment("starting");
			public static ObstacleType DECYPHER_MAP = new ObstacleDecypherMap();
			public static ObstacleType CHURCHYARD = new ObstacleChurchyard();
			public static ObstacleType EXPLORE_TOWN = new ObstacleExploreTown();
			public static ObstacleType TAVERN = new ObstacleTavern();
			public static ObstacleType DETAINED = new ObstacleDetained();

			public static class Town {
				public static ObstacleType HARBOR = new ObstacleExploreTown_Harbor();
				public static ObstacleType MARKET = new ObstacleExploreTown_Market();
				public static ObstacleType GARDENS = new ObstacleExploreTown_Gardens();
				public static ObstacleType TEMPLE = new ObstacleExploreTown_Temples();

				public static ObstacleType GRAVEYARD = new ObstacleGraveyard();
			}

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Initial).GetFields();

				int min = UnityEngine.Mathf.FloorToInt(StatisticsTracker.minQuestDifficulty.value / 20f * (fields.Length - 1));
				int max = UnityEngine.Mathf.FloorToInt(StatisticsTracker.maxQuestDifficulty.value / 20f * (fields.Length - 1));
				if(min == max) {
					if(min > 0) min--;
					else max++;
				}
				int v = rand.Next(max - min) + min;

				FieldInfo field = fields[rand.Next(v)];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}
		public static class Travel {
			public static ObstacleType MOUNTAIN_PATH = new ObstacleMountainPath();
			public static ObstacleType HICH_RIDE = new ObstacleHitchRide();
			public static ObstacleType SAIL_SEAS = new ObstacleSailFriendlySeas();
			public static ObstacleType OPEN_ROAD = new ObstacleOpenRoad();
			public static ObstacleType GOTO_TOWN = new ObstacleTravelToTown();
			public static ObstacleType DARK_FOREST = new ObstacleDarkWoods();
			public static ObstacleType RIVER = new ObstacleRiver();
			public static ObstacleType SWAMP = new ObstacleSwamp();

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Travel).GetFields();
				FieldInfo field = fields[rand.Next(fields.Length)];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}
		public static class Unexpected {
			public static ObstacleType AMBUSH = new ObstacleAmbush();
			public static ObstacleType LOST = new ObstacleLost();
			public static ObstacleType THIEF = new ObstacleThief();
			public static ObstacleType MADE_MESS = new ObstacleCleanMess();
			public static ObstacleType QUICKSAND = new ObstacleQuicksand();
			public static ObstacleType STEAL_STUFF = new ObstcaleStealSupplies();
			public static ObstacleType PERSUED = new ObstaclePersued();

			public static class Traps {
				public static ObstacleType TRAPPED_PASSAGE_FIRE = new ObstacleTrappedPassage(DamageType.FIRE);
				public static ObstacleType TRAPPED_PASSAGE_COLD = new ObstacleTrappedPassage(DamageType.COLD);
				public static ObstacleType TRAPPED_PASSAGE_ACID = new ObstacleTrappedPassage(DamageType.ACID);
				public static ObstacleType TRAPPED_PASSAGE_POISON = new ObstacleTrappedPassage(DamageType.POISON);
				public static ObstacleType TRAPPED_PASSAGE_GENERIC = new ObstacleTrappedPassage(DamageType.GENERIC);

				public static ObstacleType MAGIC_TRAP_FIRE = new ObstacleMagicTrap(DamageType.FIRE);
				public static ObstacleType MAGIC_TRAP_COLD = new ObstacleMagicTrap(DamageType.COLD);
				public static ObstacleType MAGIC_TRAP_ACID = new ObstacleMagicTrap(DamageType.ACID);
				public static ObstacleType MAGIC_TRAP_POISON = new ObstacleMagicTrap(DamageType.POISON);
				public static ObstacleType MAGIC_TRAP_HOLY = new ObstacleMagicTrap(DamageType.HOLY);
				public static ObstacleType MAGIC_TRAP_UNHOLY = new ObstacleMagicTrap(DamageType.UNHOLY);
			}
			public static class Monsters {
				public static ObstacleType BANDIT = new ObstacleMonster("bandit", DamageType.GENERIC, RequirementType.WEAPON);

				public static ObstacleType HELLHOUND = new ObstacleMonster("hellhound",DamageType.FIRE, RequirementType.COLD_DAMAGE);
				public static ObstacleType OOZE = new ObstacleMonster("ooze", DamageType.ACID, RequirementType.FIRE_DAMAGE);
				public static ObstacleType WINTERWOLF = new ObstacleMonster("winterwolf", DamageType.COLD, RequirementType.POISON_DAMAGE);
				public static ObstacleType ANIMANT_PLANT = new ObstacleMonster("animated plant", DamageType.POISON, RequirementType.ACID_DAMAGE);
				public static ObstacleType UNDEAD = new ObstacleMonster("undead", DamageType.UNHOLY, RequirementType.HOLY_DAMAGE);
				public static ObstacleType ARCHON = new ObstacleMonster("archon", DamageType.HOLY, RequirementType.UNHOLY_DAMAGE);

				public static ObstacleType NYMPH = new ObstacleMonster("nymph", DamageType.GENERIC, RequirementType.UGLINESS);
				public static ObstacleType VIPER = new ObstacleMonster("sand viper", DamageType.POISON, RequirementType.CLUMSINESS);
				public static ObstacleType TROLL = new ObstacleMonster("troll", DamageType.GENERIC, RequirementType.WEAKNESS);
				public static ObstacleType SPIDER = new ObstacleMonster("giant spider", DamageType.POISON, RequirementType.STUPIDITY);
			}

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Unexpected).GetFields();
				int r = rand.Next(fields.Length + 2);
				if(r < fields.Length) {
					FieldInfo field = fields[r];
					return (ObstacleType)field.GetValue(null);
				}
				else if(r == fields.Length) { //traps
					r = rand.Next(fields.Length);
					if(r > 4) r = rand.Next(fields.Length); //less likely to roll magic
					FieldInfo field = fields[r];
					fields = typeof(Traps).GetFields();
					return (ObstacleType)field.GetValue(null);
				}
				else if(r == fields.Length+1) { //monsters
					r = rand.Next(fields.Length);
					if(r > 0) r = rand.Next(fields.Length); //less likely to roll thing other than bandits
					if(r > 6) r = rand.Next(fields.Length); //less likely to roll anit-attribute monsters
					FieldInfo field = fields[r];
					fields = typeof(Monsters).GetFields();
					return (ObstacleType)field.GetValue(null);
				}
				return null;
			}
		}
		public static class Loot {
			public static ObstacleType TREASURE = new ObstacleTreasure();
			public static ObstacleType COMMON_ITEM = new ObstacleResourceCacheSimple();
			public static ObstacleType RARE_ITEM = new ObstacleResourceCache();
			public static ObstacleType TRAVELING_MERCHANT = new ObstacleBuyEquipment("traveling merchant");

			public static ObstacleType getRandom(Random rand) {
				return getRandom(rand, false);
			}

			public static ObstacleType getRandom(Random rand, bool final) {
				FieldInfo[] fields = typeof(Loot).GetFields();
				int r = rand.Next(fields.Length);
				while(r == 3 && final) {
					r = rand.Next(fields.Length);
				}
				FieldInfo field = fields[r];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}

			public static void AddRandomStatPotion(Quest theQuest, int v) {
				int r;
				for(int i = 0; i < v; i++) {
					r = theQuest.testLuck(4);
					switch(r) {
						case 0:
							theQuest.addItemToInventory(new ItemStack(Industries.POT_STRENGTH, 1));
							break;
						case 1:
							theQuest.addItemToInventory(new ItemStack(Industries.POT_AGILITY, 1));
							break;
						case 2:
							theQuest.addItemToInventory(new ItemStack(Industries.POT_INTELLIGENCE, 1));
							break;
						case 3:
							theQuest.addItemToInventory(new ItemStack(Industries.POT_CHARISMA, 1));
							break;
					}
				}
			}

			public static void AddCommonResource(Quest theQuest) {
				Item i = Items.getRandom(theQuest.questRand, 0, 8);
				int s = theQuest.questRand.Next(i.maxStackSize - i.minStackSize + 1) + i.minStackSize;
				s += checkHerbalism(theQuest, i);
				Main.instance.player.addItemToInventory(new ItemStack(i, s));
			}

			public static void AddUncommonResource(Quest theQuest) {
				Item i = Items.getRandom(theQuest.questRand, 8, 18);
				int s = theQuest.questRand.Next(i.maxStackSize - i.minStackSize + 1) + i.minStackSize;
				s += checkHerbalism(theQuest, i);
				Main.instance.player.addItemToInventory(new ItemStack(i, s));
			}

			public static void AddRareResource(Quest theQuest) {
				Item i = Items.getRandom(theQuest.questRand, 18, 26);
				int s = theQuest.questRand.Next(i.maxStackSize - i.minStackSize + 1) + i.minStackSize;
				s += checkHerbalism(theQuest, i);
				Main.instance.player.addItemToInventory(new ItemStack(i, s));
			}

			private static int checkHerbalism(Quest theQuest, Item item) {
				if(item.maxStackSize == 1) return 1;
				bool hasHerbalism = false;
				foreach(ItemStack stack in theQuest.inventory) {
					if(stack.doesStackHave(Enchantments.HERBALISM)) {
						hasHerbalism = true;
					}
				}
				if(hasHerbalism) {
					return 1 + theQuest.questRand.Next(3);
				}
				return 0;
			}

			public static void AddRelic(Quest theQuest) {
				ItemStack stack = QuestManager.getRandomTreasure(theQuest);
				if(stack != null)
					theQuest.addItemToInventory(stack);
				else {
					AddRareResource(theQuest);
					AddRareResource(theQuest);
				}
			}
		}
		public static class Scenario {
			public static ObstacleType PIRATE_SHIP = new ObstaclePirateShip();
			public static class Pirates {
				public static ObstacleType MAROONED = new ObstacleMarooned();
				public static ObstacleType SAIL_PIRATE_WATERS = new ObstacleSailPirateWaters();
				public static ObstacleType UNDERWATER_RUINS = new ObstacleUnderwaterRuins();
				public static ObstacleType LOST_LAGOON = new ObstacleLostLagoon();
			}

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(Scenario).GetFields();
				FieldInfo field = fields[rand.Next(fields.Length)];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}
		public static class General {
			public static ObstacleType SNEAKING = new ObstacleSneaking();
			public static ObstacleType MAIDEN = new ObstacleFairMaiden();
			public static ObstacleType FESTIVAL = new ObstacleFestival();
			public static ObstacleType UNEVENTFUL = new ObstacleUneventful();
			public static ObstacleType WILDERNESS = new ObstacleWilderness();
			public static ObstacleType NIGHT_WATCH = new ObstacleNightWatch();

			public static ObstacleType getRandom(Random rand) {
				FieldInfo[] fields = typeof(General).GetFields();
				FieldInfo field = fields[rand.Next(fields.Length)];
				ObstacleType item = (ObstacleType)field.GetValue(null);
				return item;
			}
		}
	}
}