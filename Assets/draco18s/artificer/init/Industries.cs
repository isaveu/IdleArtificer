﻿using UnityEngine;
using System.Collections;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.quests.requirement;
using Assets.draco18s.util;
using Koopakiller.Numerics;

namespace Assets.draco18s.artificer.init {
	public class Industries {
		public static readonly Industry WOOD =				new Industry("wood",									  10, 1, 1, Scalar._0_RAW).addReqType(RequirementType.WOOD).setMaxStackSize(10).setStackSizeForQuest(7).setConsumable(true);
		public static readonly Industry CHARCOAL =			new Industry("charcoal",								  50, 5, 1, Scalar._1_REFINED, new IndustryInput(WOOD, 2)).setDisallowedForQuests().setMaxStackSize(5);
		public static readonly Industry TORCHES =			new Industry("torches",								     250, 5, 4, Scalar._2_SIMPLE, new IndustryInput(WOOD, 1), new IndustryInput(CHARCOAL, 1)).addReqType(RequirementType.LIGHT).setConsumable(true).setMaxStackSize(5).setStackSizeForQuest(2);
		public static readonly Industry RED_BERRIES =		new Industry("red_berries",								1250, 2, 10, Scalar._0_RAW).addAidType(AidType.HEALING_TINY).addReqType(RequirementType.HERB).setConsumable(true).setMaxStackSize(10).setVendorSizeMulti(5).setStackSizeForQuest(5);
		public static readonly Industry BLUE_BERRIES =		new Industry("blue_berries",							1250, 2, 10, Scalar._0_RAW).addAidType(AidType.MANA_TINY).addReqType(RequirementType.HERB).setConsumable(true).setMaxStackSize(10).setVendorSizeMulti(5).setStackSizeForQuest(5);
		public static readonly Industry LEATHER =			new Industry("leather",									6000, 16, 3, Scalar._0_RAW).addAidType(AidType.LIGHT_ARMOR).setConsumable(true).addReqType(RequirementType.LEATHER).setMaxStackSize(5).setStackSizeForQuest(5);
		public static readonly Industry ARMOR_LEATHER =		new Industry("leather_armor",						   25000, 400, 1, Scalar._2_SIMPLE, new IndustryInput(LEATHER, 12)).addAidType(AidType.LIGHT_ARMOR).setMaxStackSize(1).setIsViableRelic(true).setEquipType(ItemEquipType.ARMOR).setStackSizeForQuest(1);
		public static readonly Industry SIMPLE_TOOLS =		new Industry("simple_tools",						   50000, 300, 2, Scalar._1_REFINED, new IndustryInput(WOOD, 1)).addReqType(RequirementType.TOOLS).setConsumable(true).setMaxStackSize(10).setStackSizeForQuest(3);
		public static readonly Industry QUARTERSTAFF =		new Industry("quarterstaff",						  100000, 800, 1, Scalar._1_REFINED, new IndustryInput(WOOD, 2)).addAidType(AidType.WEAPON).setConsumable(true).setMaxStackSize(10).setStackSizeForQuest(3);
		public static readonly Industry LIGHT_CROSSBOW =	new Industry("light_crossbow",						  200000, 1600, 1, Scalar._1_REFINED, new IndustryInput(WOOD, 2), new IndustryInput(LEATHER, 1)).addAidType(AidType.RANGED_WEAPON).setConsumable(true).setMaxStackSize(20).setStackSizeForQuest(3);
		public static readonly Industry GLASS =				new Industry("glass",								  400000, 3200, 1, Scalar._0_RAW).setMaxStackSize(1).setDisallowedForQuests();
		public static readonly Industry GLASS_PHIAL =		new Industry("glass_phial",							 1600000, 1200, 5, Scalar._1_REFINED, new IndustryInput(GLASS, 1)).setDisallowedForQuests().setMaxStackSize(4);
		public static readonly Industry NIGHTSHADE =		new Industry("nightshade",							 6400000,  400, 5, Scalar._0_RAW).addReqType(RequirementType.POISON_DAMAGE).addReqType(RequirementType.HERB).setConsumable(true).setMaxStackSize(10).setVendorSizeMulti(2).setStackSizeForQuest(1);
		public static readonly Industry POT_HEALTH =		new Industry("health_potion",						12500000, 6400, 1, Scalar._2_SIMPLE, new IndustryInput(GLASS_PHIAL, 1), new IndustryInput(RED_BERRIES, 3)).addAidType(AidType.HEALING_SMALL).setConsumable(true).setMaxStackSize(10).setStackSizeForQuest(5);
		public static readonly Industry POT_MANA =			new Industry("mana_potion",							12500000, 6400, 1, Scalar._2_SIMPLE, new IndustryInput(GLASS_PHIAL, 1), new IndustryInput(BLUE_BERRIES, 3)).addAidType(AidType.MANA_SMALL).setConsumable(true).setMaxStackSize(10).setStackSizeForQuest(5);
		public static readonly Industry POT_POISON =		new Industry("poison_potion",						12500000, 6400, 1, Scalar._1_REFINED, new IndustryInput(GLASS_PHIAL, 1), new IndustryInput(NIGHTSHADE, 2)).addReqType(RequirementType.POISON_DAMAGE).setConsumable(true).setMaxStackSize(5).setStackSizeForQuest(5);
		public static readonly Industry WOOD_BUCKLER =		new Industry("wooden_buckler",					    25000000, 12500, 1, Scalar._1_REFINED, new IndustryInput(WOOD, 4)).addAidType(AidType.LIGHT_SHIELD).setEquipType(ItemEquipType.SHIELD).setMaxStackSize(1).setIsViableRelic(true).setStackSizeForQuest(1);
		public static readonly Industry GLASS_BOTTLES =		new Industry("glass_bottles",					    50000000,  5000, 5, Scalar._1_REFINED, new IndustryInput(GLASS, 2), new IndustryInput(CHARCOAL, 1)).setDisallowedForQuests().setMaxStackSize(4);
		public static readonly Industry LIGHT_CLOAK =		new Industry("light_cloak",							75000000, 25000, 1, Scalar._1_REFINED, new IndustryInput(LEATHER, 2)).setMaxStackSize(5).setConsumable(true).addReqType(RequirementType.STEALTH).setStackSizeForQuest(1);
		public static readonly Industry MANDRAKE =			new Industry("mandrake_root",					   225000000, 20000, 5, Scalar._0_RAW).addReqType(RequirementType.HERB).setConsumable(true).setMaxStackSize(15).setVendorSizeMulti(2).setStackSizeForQuest(1);
		public static readonly Industry BLOOD_MOSS =		new Industry("blood_moss",						   225000000, 20000, 5, Scalar._0_RAW).addReqType(RequirementType.HERB).setConsumable(true).setMaxStackSize(15).setVendorSizeMulti(2).setStackSizeForQuest(1);
		public static readonly Industry BANROOT =			new Industry("banroot",							   225000000, 20000, 5, Scalar._0_RAW).addReqType(RequirementType.HERB).setConsumable(true).setMaxStackSize(15).setVendorSizeMulti(2).setStackSizeForQuest(1);
		public static readonly Industry MUSHROOMS =			new Industry("mushrooms",						   225000000, 20000, 5, Scalar._0_RAW).addReqType(RequirementType.HERB).setConsumable(true).setMaxStackSize(15).setVendorSizeMulti(2).setStackSizeForQuest(1);
		public static readonly Industry POT_STRENGTH =		new Industry("strength_potion",					   225000000, 200000, 1, Scalar._1_REFINED, new IndustryInput(GLASS_BOTTLES, 1), new IndustryInput(MANDRAKE, 3)).addReqType(RequirementType.STRENGTH).setConsumable(true).setMaxStackSize(5).setStackSizeForQuest(2);
		public static readonly Industry POT_AGILITY =		new Industry("agility_potion",					   750000000, 200000, 1, Scalar._1_REFINED, new IndustryInput(GLASS_BOTTLES, 1), new IndustryInput(BLOOD_MOSS, 3)).addReqType(RequirementType.AGILITY).setConsumable(true).setMaxStackSize(5).setStackSizeForQuest(2);
		public static readonly Industry POT_INTELLIGENCE =	new Industry("intelligence_potion",				   750000000, 200000, 1, Scalar._1_REFINED, new IndustryInput(GLASS_BOTTLES, 1), new IndustryInput(BANROOT, 3)).addReqType(RequirementType.INTELLIGENCE).setConsumable(true).setMaxStackSize(5).setStackSizeForQuest(2);
		public static readonly Industry POT_CHARISMA =		new Industry("charisma_potion",					   750000000, 200000, 1, Scalar._1_REFINED, new IndustryInput(GLASS_BOTTLES, 1), new IndustryInput(MUSHROOMS, 3)).addReqType(RequirementType.CHARISMA).setConsumable(true).setMaxStackSize(5).setStackSizeForQuest(2);
		public static readonly Industry POT_RESTORATION =	new Industry("restoration_potion",				   750000000, 200000, 2, Scalar._3_COMPLEX, new IndustryInput(POT_HEALTH, 1), new IndustryInput(POT_MANA, 1)).addAidType(AidType.HEALING_SMALL).addAidType(AidType.MANA_SMALL).setConsumable(true).setMaxStackSize(5).setStackSizeForQuest(1);
		public static readonly Industry IRON_ORE =			new Industry("iron_ore",					      2000000000, 400000, 1, Scalar._0_RAW).setDisallowedForQuests();
		public static readonly Industry IRON_INGOTS =		new Industry("iron_ingots",					     10000000000, 320000, 5, Scalar._0_RAW, new IndustryInput(IRON_ORE, 1)).addReqType(RequirementType.IRON).setMaxStackSize(5).setConsumable(true).setStackSizeForQuest(2);
		public static readonly Industry IRON_SWORD =		new Industry("iron_sword",					     50000000000, 3200000, 1, Scalar._1_REFINED, new IndustryInput(IRON_INGOTS, 2)).addAidType(AidType.WEAPON).setEquipType(ItemEquipType.WEAPON).setIsViableRelic(true).setMaxStackSize(1).setStackSizeForQuest(2);
		public static readonly Industry LANTERN =			new Industry("lantern",				 new BigInteger(150, 9),  3200000, 2, Scalar._2_SIMPLE, new IndustryInput(TORCHES, 2), new IndustryInput(GLASS, 1), new IndustryInput(IRON_INGOTS, 2)).addReqType(RequirementType.LIGHT).setIsViableRelic(true).setEquipType(ItemEquipType.MISC).setMaxStackSize(1).setStackSizeForQuest(2);
		public static readonly Industry IMPROVED_CLOAK =	new Industry("improved_cloak",		 new BigInteger(750, 9),  8000000, 1, Scalar._2_SIMPLE, new IndustryInput(LIGHT_CLOAK, 2)).setMaxStackSize(1).setIsViableRelic(true).addReqType(RequirementType.STEALTH).setEquipType(ItemEquipType.CLOAK).setStackSizeForQuest(1);
		public static readonly Industry IRON_HELMET =		new Industry("iron_helm",			 new BigInteger(3, 12),   12500000, 1, Scalar._2_SIMPLE, new IndustryInput(IRON_INGOTS, 4)).setEquipType(ItemEquipType.HELMET).setDisallowedForQuests();
		public static readonly Industry POT_WEAKNESS = 		new Industry("weakness_potion",		 new BigInteger(20, 12),  25000000, 1, Scalar._2_SIMPLE, new IndustryInput(POT_STRENGTH,1),new IndustryInput(POT_POISON,1)).addReqType(RequirementType.WEAKNESS).setMaxStackSize(5).setStackSizeForQuest(1);
		public static readonly Industry POT_CLUMSINESS = 	new Industry("clumsiness_potion",	 new BigInteger(20, 12),  25000000, 1, Scalar._2_SIMPLE, new IndustryInput(POT_AGILITY, 1), new IndustryInput(POT_POISON, 1)).addReqType(RequirementType.CLUMSINESS).setMaxStackSize(5).setStackSizeForQuest(1);
		public static readonly Industry POT_STUPIDITY= 		new Industry("stupidity_potion",	 new BigInteger(20, 12),  25000000, 1, Scalar._2_SIMPLE, new IndustryInput(POT_INTELLIGENCE, 1), new IndustryInput(POT_POISON, 1)).addReqType(RequirementType.STUPIDITY).setMaxStackSize(5).setStackSizeForQuest(1);
		public static readonly Industry POT_UGLINESS = 		new Industry("ugliness_potion",		 new BigInteger(20, 12),  25000000, 1, Scalar._2_SIMPLE, new IndustryInput(POT_CHARISMA, 1), new IndustryInput(POT_POISON, 1)).addReqType(RequirementType.UGLINESS).setMaxStackSize(5).setStackSizeForQuest(1);
		public static readonly Industry IRON_RING = 		new Industry("iron_ring",			 new BigInteger(150, 12), 25000000, 2, Scalar._1_REFINED, new IndustryInput(IRON_INGOTS, 1)).setMaxStackSize(1).setEquipType(ItemEquipType.RING).setIsViableRelic(true).setStackSizeForQuest(1);
		public static readonly Industry GOLD_ORE =			new Industry("gold_ore",			 new BigInteger(1,15),    100000000, 1, Scalar._0_RAW).setDisallowedForQuests();
		public static readonly Industry GOLD_DUST = 		new Industry("gold_dust",			 new BigInteger(25,14),   100000000, 2, Scalar._1_REFINED, new IndustryInput(GOLD_ORE,1)).setDisallowedForQuests();
		public static readonly Industry GOLD_INGOTS = 		new Industry("gold_ingots",			 new BigInteger(5,15),    200000000, 2, Scalar._1_REFINED, new IndustryInput(GOLD_DUST,1)).setDisallowedForQuests();
		public static readonly Industry HOLY_SYMBOL = 		new Industry("holy_symbol",			 new BigInteger(30,15),   500000000, 1, Scalar._1_REFINED, new IndustryInput(IRON_INGOTS,1),new IndustryInput(GOLD_INGOTS,1)).setConsumable(true).setMaxStackSize(3).addReqType(RequirementType.UNHOLY_IMMUNE).setStackSizeForQuest(5);
		public static readonly Industry IRON_SHIELD = 		new Industry("iron_shield",			 new BigInteger(225,15),  1000000000, 1, Scalar._2_SIMPLE, new IndustryInput(IRON_INGOTS,3)).addAidType(AidType.LIGHT_SHIELD).setMaxStackSize(1).setIsViableRelic(true).setEquipType(ItemEquipType.SHIELD).setStackSizeForQuest(1);
		public static readonly Industry FANCY_BOTTLES =		new Industry("fancy_bottles",		 new BigInteger(15,17),   500000000, 5, Scalar._2_SIMPLE, new IndustryInput(GLASS, 4), new IndustryInput(CHARCOAL, 1), new IndustryInput(GOLD_INGOTS, 1)).setDisallowedForQuests().setMaxStackSize(4);
		public static readonly Industry HOLY_WATER =		new Industry("holy_water",			 new BigInteger(5,18),    6000000000, 1, Scalar._3_COMPLEX, new IndustryInput(FANCY_BOTTLES, 1), new IndustryInput(HOLY_SYMBOL, 1)).setConsumable(true).setMaxStackSize(5).addReqType(RequirementType.HOLY_DAMAGE).setStackSizeForQuest(2);
		//end cost tweak
		public static readonly Industry KELPWEED =			new Industry("kelpweed",			 new BigInteger(15,18),   2500000000, 5, Scalar._0_RAW).setDisallowedForQuests();
		public static readonly Industry STONEROOT =			new Industry("stoneroot",			 new BigInteger(15,18),   2500000000, 5, Scalar._0_RAW).setDisallowedForQuests();
		public static readonly Industry SAGE_GRASS =		new Industry("sage_grass",			 new BigInteger(15,18),   2500000000, 5, Scalar._0_RAW).setDisallowedForQuests();
		public static readonly Industry PHOENIX_FEATHERS =	new Industry("phoenix_feather",		 new BigInteger(15,18),   2500000000, 5, Scalar._0_RAW).setDisallowedForQuests();
		public static readonly Industry POT_WATER_BREATH =	new Industry("water_breath_potion",	 new BigInteger(50,18),   25000000000, 1, Scalar._3_COMPLEX, new IndustryInput(FANCY_BOTTLES, 1), new IndustryInput(KELPWEED, 1)).setConsumable(true).setMaxStackSize(5).addReqType(RequirementType.WATER_BREATH).setStackSizeForQuest(2);
		public static readonly Industry POT_BARKSKIN =		new Industry("barkskin_potion",		 new BigInteger(50,18),   25000000000, 1, Scalar._3_COMPLEX, new IndustryInput(FANCY_BOTTLES, 1), new IndustryInput(STONEROOT, 1)).setConsumable(true).setMaxStackSize(5).addReqType(RequirementType.ARMOR).setStackSizeForQuest(2);
		public static readonly Industry POT_ALERTNESS =		new Industry("alertness_potion",	 new BigInteger(50,18),   25000000000, 1, Scalar._3_COMPLEX, new IndustryInput(FANCY_BOTTLES, 1), new IndustryInput(SAGE_GRASS, 1)).setConsumable(true).setMaxStackSize(5).addReqType(RequirementType.HOLY_DAMAGE).setStackSizeForQuest(2);
		public static readonly Industry POT_REVIVE =		new Industry("revive_potion",		 new BigInteger(50,18),   25000000000, 1, Scalar._3_COMPLEX, new IndustryInput(FANCY_BOTTLES, 1), new IndustryInput(PHOENIX_FEATHERS, 1), new IndustryInput(POT_RESTORATION, 1)).setConsumable(true).setMaxStackSize(1).addAidType(AidType.RESSURECTION).setStackSizeForQuest(1);
		
		//gold rings

	}
}