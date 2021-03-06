﻿using Assets.draco18s.artificer.init;
using Assets.draco18s.artificer.items;
using Assets.draco18s.artificer.masters;
using Assets.draco18s.artificer.quests;
using Assets.draco18s.artificer.quests.challenge;
using Assets.draco18s.artificer.quests.hero;
using Assets.draco18s.artificer.statistics;
using Assets.draco18s.artificer.ui;
using Assets.draco18s.config;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.draco18s.artificer.game {
	public class Main : MonoBehaviour {
		public static int saveVersionFromDisk;
		public static Main instance;
		public readonly PlayerInfo player = new PlayerInfo();

		public bool debugMode;
		public bool debugAutoBuild;
		public GameObject errorWindow;
		public float mouseDownTime;
		private float autoClickTime;

		/*autobuild debugging*/
		public float timeSinceLastPurchase;
		public float timeTotal;
		//private StreamWriter csv_st;
		//private string lastPurchase = "";
		//private int lastTime = -1;
		public bool close_file = false;
		private float autosaveTimer = 0;

		void Start() {
			//UnityEngine.Profiling.Profiler.maxNumberOfSamplesPerFrame = -1;
			/*string path = "E:\\Users\\Major\\Desktop\\time_data.csv";
			if(File.Exists(path)) {
				File.Delete(path);
			}
			csv_st = File.CreateText(path);
			csv_st.WriteLine("TotalTime,TimeToNextBuilding,Income/Sec,CashOnHand,LastPurchase");*/
			instance = this;
			TutorialManager.init();
			NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
			Configuration.NumberFormat = nfi;

			Application.runInBackground = true;
			//player = new PlayerInfo();
			//money = new BigInteger(10000);
			GuiManager.instance.mainCanvas.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.FacilityUnselected(); });
			Localization.initialize();
			CraftingManager.setupUI();
			Upgrades.CashUpgradeInit();
			Upgrades.RenownUpgradeInit();
			EnchantingManager.OneTimeSetup();
			QuestManager.setupUI();
			GuildManager.OneTimeSetup();
			ResearchManager.OneTimeSetup();
			AchievementsManager.OneTimeSetup();
			RuntimeDataConfig.loadCurrentDirectory();

			InfoPanel panel = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			panel.ConsumeToggle.onValueChanged.AddListener(delegate { CraftingManager.ToggleAllowConsume(); });
			panel.ProduceToggle.onValueChanged.AddListener(delegate { CraftingManager.ToggleAllowProduction(); });
			panel.SellToggle.onValueChanged.AddListener(delegate { CraftingManager.ToggleSellStores(); });
			panel.BuildToggle.onValueChanged.AddListener(delegate { CraftingManager.ToggleAutoBuild(); });
			
			Button btn;
			btn = GuiManager.instance.infoPanel.transform.Find("Output").GetComponent<Button>();
			btn.onClick.AddListener(delegate { CraftingManager.AdvanceTimer(); });
			btn.OnRightClick(delegate { CraftingManager.AdvanceTimer(); });

			btn = GuiManager.instance.infoPanel.transform.Find("SellAll").GetComponent<Button>();
			btn.onClick.AddListener(delegate { CraftingManager.SellAll(); });
			//string veryLong = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam pharetra tincidunt mi, sed volutpat est elementum id. Etiam eleifend arcu vitae sem efficitur, ut congue massa ultricies. Ut facilisis, leo id tincidunt viverra, sem metus accumsan neque, ac tristique erat quam id lacus. Vivamus quis augue eros. Maecenas non laoreet ligula. Nunc id tellus consectetur ipsum volutpat convallis nec a arcu. Phasellus fermentum sapien eget porttitor convallis.";
			btn.AddHover(delegate (Vector3 p) {
				GuiManager.ShowTooltip(GuiManager.instance.infoPanel.transform.Find("SellAll").transform.position + Vector3.right * 45, "Sell all " + AsCurrency(CraftingManager.GetQuantity()) + " " + CraftingManager.GetName() + " for $" + AsCurrency(CraftingManager.SellAllValue()));
			});

			GuiManager.instance.craftHeader.transform.Find("MoneyArea").GetComponent<Image>().AddHover(delegate(Vector3 pos) {
				BigInteger income = ApproximateIncome() / 10;
				Vector3 p = GuiManager.instance.craftHeader.transform.Find("MoneyArea").position;
				GuiManager.ShowTooltip(p + Vector3.down * 35, "$" + AsCurrency(income,9) + "/sec",1,1.5f);
			});

			InfoPanel info = GuiManager.instance.infoPanel.GetComponent<InfoPanel>();
			info.transform.Find("Input1").GetComponent<Button>().onClick.AddListener(delegate () { CraftingManager.SelectInput(1); });
			info.transform.Find("Input2").GetComponent<Button>().onClick.AddListener(delegate () { CraftingManager.SelectInput(2); });
			info.transform.Find("Input3").GetComponent<Button>().onClick.AddListener(delegate () { CraftingManager.SelectInput(3); });
			info.UpgradeBtn.onClick.AddListener(delegate { CraftingManager.UpgradeCurrent(); });
			info.DowngradeBtn.onClick.AddListener(delegate { CraftingManager.DowngradeCurrent(); });
			info.ConfDowngradeBtn.onClick.AddListener(delegate { CraftingManager.Do_DowngradeCurrent(); });
			info.VendNum.transform.Find("IncVend").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseVendors(); });
			info.VendNum.transform.Find("DecVend").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseVendors(); });
			info.StartVend.transform.Find("IncStVend").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseStartingVendors(); });
			info.StartVend.transform.Find("DecStVend").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseStartingVendors(); });
			GuiManager.instance.infoPanel.transform.Find("NumAppr").Find("IncAppr").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseApprentices(); });
			GuiManager.instance.infoPanel.transform.Find("NumAppr").Find("DecAppr").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseApprentices(); });
			info.BuildNum.transform.Find("IncBuild").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseAutoBuild(); });
			info.BuildNum.transform.Find("DecBuild").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseAutoBuild(); });
			info.VendText.transform.GetChild(0).GetComponent<Button>().AddHover(delegate (Vector3 p) { GuiManager.ShowTooltip(info.VendText.transform.position + Vector3.right * 45, "Your vendors currently sell " + AsCurrency(CraftingManager.NumberSoldByVendors()) + " units per cycle.\nConsidering the Vendor Effectiveness multiplier, you'll earn $" + AsCurrency(CraftingManager.ValueSoldByVendors()) + " per cycle.", 1.61f); }, false);
			info.MagnitudeNum.transform.Find("IncBuild").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.IncreaseBuildMagnitude(); });
			info.MagnitudeNum.transform.Find("DecBuild").GetComponent<Button>().onClick.AddListener(delegate { CraftingManager.DecreaseBuildMagnitude(); });

			GuiManager.instance.craftTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.craftTab, GuiManager.instance.craftArea, GuiManager.instance.craftHeader); });
			GuiManager.instance.enchantTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.enchantTab, GuiManager.instance.enchantArea, GuiManager.instance.enchantHeader); });
			GuiManager.instance.questTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.questTab, GuiManager.instance.questArea, GuiManager.instance.questHeader); });
			GuiManager.instance.guildTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.guildTab, GuiManager.instance.guildArea, GuiManager.instance.guildHeader); });
			GuiManager.instance.researchTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.researchTab, GuiManager.instance.researchArea, GuiManager.instance.researchHeader); });
			GuiManager.instance.achievementsTab.GetComponent<Button>().onClick.AddListener(delegate { switchTabImage(GuiManager.instance.achievementsTab, GuiManager.instance.achievementsArea, GuiManager.instance.achievementsHeader); });

			GuiManager.instance.craftArea.GetComponent<Canvas>().enabled = true;
			GuiManager.instance.enchantArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.questArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.guildArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.researchArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.achievementsArea.GetComponent<Canvas>().enabled = false;

			btn = GuiManager.instance.craftHeader.transform.Find("ResetBtn").GetComponent<Button>();
			btn.onClick.AddListener(delegate { player.reset(); });
			btn.AddHover(delegate (Vector3 p) {
				/*BigInteger spentRenown = Main.instance.player.totalRenown - Main.instance.player.renown;
				BigInteger totalRenown = BigInteger.CubeRoot(Main.instance.player.lifetimeMoney);
				totalRenown /= 10000;
				BigInteger renown = totalRenown - spentRenown + Main.instance.player.questsCompleted;*/
				BigInteger renown = getCachedNewRenown() + Main.instance.player.questsCompletedRenown;
				GuiManager.ShowTooltip(GuiManager.instance.craftHeader.transform.Find("ResetBtn").transform.position, "You will gain " + Main.AsCurrency(renown) + " Renown if you reset now.", 2.5f);
			});
			btn = GuiManager.instance.craftHeader.transform.Find("SyncBtn").GetComponent<Button>();
			btn.onClick.AddListener(delegate { CraftingManager.SynchronizeInustries(); });
			btn.AddHover(delegate (Vector3 p) {
				GuiManager.ShowTooltip(btn.transform.position+Vector3.right*50, "Automatically synchronize the build timers.",3);
			}, false);
			GuiManager.instance.craftHeader.transform.Find("RecallBtn").GetComponent<Button>().onClick.AddListener(delegate {
				foreach(Industry ind in instance.player.builtItems) {
					ind.SetRawVendors(0);
				}
				instance.player.currentVendors = 0;
			});
			Toggle tog = GuiManager.instance.craftHeader.transform.Find("AutoToggle").GetComponent<Toggle>();
			tog.onValueChanged.AddListener(delegate { debugAutoBuild = !debugAutoBuild; });
			tog.AddHover(delegate (Vector3 pos) {
				GuiManager.ShowTooltip(tog.transform.position + Vector3.right * 50, "Builds industries in order of best cost:benefit ratio, up to the specified autobuild limits.", 3);
			}, false);

			GuiManager.instance.buyVendorsArea.transform.Find("BuyOne").GetComponent<Button>().onClick.AddListener(delegate { GuildManager.BuyVendor(); });
			GuiManager.instance.buyApprenticesArea.transform.Find("BuyOne").GetComponent<Button>().onClick.AddListener(delegate { GuildManager.BuyApprentice(); });
			GuiManager.instance.buyJourneymenArea.transform.Find("BuyOne").GetComponent<Button>().onClick.AddListener(delegate { GuildManager.BuyJourneyman(); });
			
			GuiManager.instance.topPanel.transform.Find("SaveBtn").GetComponent<Button>().onClick.AddListener(delegate {
				long nl = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
				StatisticsTracker.lastSavedTime.setValue((int)(nl / 5));
				if(Application.platform == RuntimePlatform.WebGLPlayer) {
					DataAccess.Save(player);
				}
				else {
					writeDataToSave();
				}
				autosaveTimer = 0;
				GuiManager.ShowNotification(new NotificationItem("Game saved!", "", GuiManager.instance.checkOn));
			});

			GuiManager.instance.achievementsHeader.transform.Find("ResetBtn").GetComponent<Button>().onClick.AddListener(delegate {
				TutorialManager.Reset();
			});
			GuiManager.instance.achievementsHeader.transform.Find("HardResetBtn").GetComponent<Button>().onClick.AddListener(delegate {
				GuiManager.instance.achievementsArea.transform.Find("ConfirmReset").gameObject.SetActive(true);
			});
			GuiManager.instance.achievementsArea.transform.Find("ConfirmReset").GetChild(0).Find("CloseBtn").GetComponent<Button>().onClick.AddListener(delegate {
				GuiManager.instance.achievementsArea.transform.Find("ConfirmReset").gameObject.SetActive(false);
			});
			GuiManager.instance.achievementsArea.transform.Find("ConfirmReset").GetChild(0).Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(delegate {
				if(Application.platform == RuntimePlatform.WebGLPlayer) {
					DataAccess.DeleteSave();
				}
				else {
					string path2 = RuntimeDataConfig.currentDirectory + "Save/savedata.dat";
					string path3 = RuntimeDataConfig.currentDirectory + "Save/savedata-temp.dat";
					if(File.Exists(path3)) {
						File.Delete(path3);
					}
					if(File.Exists(path2)) {
						File.Delete(path2);
					}
				}
				player.HardReset();
				QuestManager.availableQuests.Clear();
				QuestManager.activeQuests.Clear();
				QuestManager.availableRelics.Clear();
				NewSaveThings();
				if(Application.platform == RuntimePlatform.WebGLPlayer) {
					DataAccess.Save(player);
				}
				else {
					writeDataToSave();
				}
				autosaveTimer = 0;
				GuiManager.instance.achievementsArea.transform.Find("ConfirmReset").gameObject.SetActive(false);
			});

			/*relicInfo = trans.Find("RelicInfoOpen").GetChild(0);
			relicInfo.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(delegate { CloseInfo(); });*/

#pragma warning disable 0219
			//make sure these static class references have been created
			ObstacleType ob = ChallengeTypes.General.FESTIVAL;
			ob = ChallengeTypes.Goals.ALCHEMY_LAB;
			ob = ChallengeTypes.Goals.Sub.MUMMY;
			ob = ChallengeTypes.Goals.Bonus.KRAKEN;
			ob = ChallengeTypes.Goals.DeepGoalSpecial.HEART_TREE;
			ob = ChallengeTypes.Initial.AT_HOME;
			ob = ChallengeTypes.Initial.Sub.BAR_BRAWL;
			ob = ChallengeTypes.Initial.Town.GARDENS;
			ob = ChallengeTypes.Loot.TREASURE;
			ob = ChallengeTypes.Scenario.PIRATE_SHIP;
			ob = ChallengeTypes.Scenario.Pirates.MAROONED;
			ob = ChallengeTypes.Travel.SAIL_SEAS;
			ob = ChallengeTypes.Unexpected.THIEF;
			ob = ChallengeTypes.Unexpected.Sub.GENIE;
			ob = ChallengeTypes.Unexpected.Traps.MAGIC_TRAP_ACID;
			ob = ChallengeTypes.Unexpected.Monsters.ANIMANT_PLANT;

			Item it11 = Items.BANSHEE_WAIL; //old save compatibility!!!
			it11 = Items.SpecialItems.POWER_STONE;
#pragma warning restore 0219
			bool saveExists = false;
			if(Application.platform == RuntimePlatform.WebGLPlayer) {
				saveExists = DataAccess.Load();
			}
			else {
				saveExists = readDataFromSave();
			}
			instance.player.FinishLoad();

			if(!saveExists) {
				Debug.Log("No save data, generating quests");
				//QuestManager.tickAllQuests(3600);
				NewSaveThings();
			}
			else {
				//fixes screwups with the cursed stone
				List<ItemStack> allStones = new List<ItemStack>();
				foreach(ItemStack stack in player.miscInventory) {
					if(stack.item == Items.SpecialItems.POWER_STONE)
						allStones.Add(stack);
				}
				foreach(ItemStack stack in QuestManager.availableRelics) {
					if(stack.item == Items.SpecialItems.POWER_STONE)
						allStones.Add(stack);
				}
				allStones.Sort((x, y) => (x.antiquity * 1000 + (x.relicData == null ? 0 : x.relicData.Count)).CompareTo((y.antiquity * 1000 + (y.relicData == null ? 0 : y.relicData.Count))));
				ItemStack good = allStones.Find(x => x.item == Items.SpecialItems.POWER_STONE && x.relicData != null);

				bool wasIninventory = player.miscInventory.Contains(good);

				player.miscInventory.RemoveAll(x => x.item == Items.SpecialItems.POWER_STONE);
				QuestManager.availableRelics.RemoveAll(x => x.item == Items.SpecialItems.POWER_STONE);

				if(good == null) {
					ItemStack newRelic = new ItemStack(Items.SpecialItems.POWER_STONE, 1);
					QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
				}
				else {
					if(wasIninventory)
						player.miscInventory.Add(good);
					else
						QuestManager.availableRelics.Add(good);
				}
				
				//long nl = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
				/*long ll = StatisticsTracker.lastSavedTime.value * 5;
				DateTime lastSave = DateTime.ParseExact(ll.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
				TimeSpan offlineTime = DateTime.Now - lastSave;
				if(offlineTime > (DateTime.Now.AddMinutes(5) - DateTime.Now)) {
					double f = offlineTime.TotalSeconds;
					StartCoroutine(ProgressOfflineTime(f));
				}*/
				//StatisticsTracker.lastSavedTime.setValue((int)(nl / 5));
			}

			IEnumerator<StatAchievement> list = StatisticsTracker.getAchievementsList();
			while(list.MoveNext()) {
				StatAchievement ac = list.Current;
				ac.determineImage();
			}

			if(!StatisticsTracker.unlockedEnchanting.isAchieved()) GuiManager.instance.enchantTab.GetComponent<Button>().interactable = false;
			if(!StatisticsTracker.unlockedGuild.isAchieved()) GuiManager.instance.guildTab.GetComponent<Button>().interactable = false;
			if(!StatisticsTracker.unlockedQuesting.isAchieved()) GuiManager.instance.questTab.GetComponent<Button>().interactable = false;
			if(!StatisticsTracker.unlockedResearch.isAchieved()) GuiManager.instance.researchTab.GetComponent<Button>().interactable = false;
			gameObject.AddComponent<SteamManager>();
			GuildManager.update();
			checkDailyLogin();
		}

		private IEnumerator ProgressOfflineTime(double f) {
			while(f > 0) {
				float n = (float)(f > 60 ? 60 : f);
				UpdateBy(n);
				f -= n;
				yield return new WaitForSeconds(1);
			}
			yield return new WaitForSeconds(1);
		}

		private void NewSaveThings() {
			ItemStack newRelic = new ItemStack(Industries.IRON_SWORD, 1);
			QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
			newRelic = new ItemStack(Industries.IRON_RING, 1);
			int loopcount = 0;
			bool ret = true;
			System.Random rand = new System.Random();
			do {
				loopcount++;
				Item item = Items.getRandom(rand);
				if(item == Items.FOURFOIL)
					continue;
				Enchantment ench = GameRegistry.GetEnchantmentByItem(item);
				//if(ench != null)
					//Debug.Log(ench.name);
				//else
				//if(item != Items.FOURFOIL) {
					//Debug.Log("Null enchant! " + item.name);
					//throw new Exception("Null enchantment for " + item.name);
				//}
				if(ench != null && (newRelic.item.equipType & ench.enchantSlotRestriction) > 0) {
					newRelic.applyEnchantment(ench);
					ret = false;
				}
			} while(ret && loopcount < 30);
			QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
			newRelic = new ItemStack(Industries.IRON_HELMET, 1);
			QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
			newRelic = new ItemStack(Industries.IRON_BOOTS, 1);
			QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
			newRelic = new ItemStack(Industries.IMPROVED_CLOAK, 1);
			QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
			newRelic = new ItemStack(Items.SpecialItems.POWER_STONE, 1);
			QuestManager.availableRelics.Add(QuestManager.makeRelic(newRelic, new FirstRelics(), 1, "Unknown"));
			GuiManager.instance.enchantTab.GetComponent<Button>().interactable = false;
			GuiManager.instance.guildTab.GetComponent<Button>().interactable = false;
			GuiManager.instance.questTab.GetComponent<Button>().interactable = false;
			GuiManager.instance.researchTab.GetComponent<Button>().interactable = false;
			long nl = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
			StatisticsTracker.lastSavedTime.setValue((int)(nl / 5));
		}

		private void checkDailyLogin() {
			long nl = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
			long ll = StatisticsTracker.lastDailyLogin.value * 5;
			if(ll > 0) {
				DateTime nowLogin = DateTime.ParseExact(nl.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
				DateTime lastLogin = DateTime.ParseExact(ll.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
				if((nowLogin - lastLogin) >= TimeSpan.FromHours(23)) {
					if((nowLogin - lastLogin) > TimeSpan.FromHours(48)) {
						StatisticsTracker.sequentialDaysPlayed.resetValue();
					}
					//DAILY BONUS
					StatisticsTracker.sequentialDaysPlayed.addValue(1);
					StatisticsTracker.lastDailyLogin.setValue((int)(nl / 5));
				}
			}
			else {
				StatisticsTracker.sequentialDaysPlayed.resetValue();
				StatisticsTracker.sequentialDaysPlayed.addValue(1);
				StatisticsTracker.lastDailyLogin.setValue((int)(nl / 5));
			}
		}

		private float TimeSinceLastRequest = 20;
		private BigInteger CachedNewRenown = 0;
		public BigInteger getCachedNewRenown() {
			if(TimeSinceLastRequest >= 10) {
				TimeSinceLastRequest -= 10;
				BigInteger newRenown = BigInteger.CubeRoot(StatisticsTracker.lifetimeMoney.value);
				newRenown -= StatisticsTracker.lastResetRenownGain.value;
				newRenown /= 10000;
				CachedNewRenown = newRenown;
			}
			return CachedNewRenown;
		}

		public void resetIndustryCaches() {
			foreach(Industry ind in player.builtItems) {
				ind.resetCache();
			}
		}

		private void test(Vector3 pos) {

		}

		public static bool readDataFromSave() {
			//GuiManager.ShowNotification(new NotificationItem("Loading...", "", GuiManager.instance.checkOn));
			string path2 = RuntimeDataConfig.currentDirectory + "Save/savedata.dat"; //"E:\\Users\\Major\\Desktop\\savedata.dat";
			//System.Object readFromDisk;

			if(File.Exists(path2)) {
				FileStream fs = new FileStream(path2, FileMode.OpenOrCreate);
				BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
				try {
					formatter.Deserialize(fs);
					//readFromDisk = formatter.Deserialize(fs);
					//instance.player = (PlayerInfo)readFromDisk;
					//fs.Close();
					//fs = new FileStream(path2, FileMode.OpenOrCreate);
					//readFromDisk = formatter.Deserialize(fs);
				}
				catch(SerializationException e) {
					GuiManager.ShowNotification(new NotificationItem("Failed to load", e.Message, GuiManager.instance.checkOff));
					//Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
					throw;
				}
				finally {
					fs.Close();
				}
				//GuiManager.ShowNotification(new NotificationItem("Loaded data!", "", GuiManager.instance.checkOn));
				return true;
			}
			return false;
		}

		public static void writeDataToSave() {
			//GuiManager.ShowNotification(new NotificationItem("Saving...", "", GuiManager.instance.checkOn));
			string path2 = RuntimeDataConfig.currentDirectory + "Save/savedata.dat";
			string path3 = RuntimeDataConfig.currentDirectory + "Save/savedata-temp.dat";
			if(File.Exists(path3)) {
				File.Delete(path3);
			}

			FileStream s = new FileStream(path3, FileMode.Create);
			IFormatter formatter = new BinaryFormatter();
			try {
				formatter.Serialize(s, instance.player);
			}
			catch(SerializationException e) {
				DataAccess.PlatformSafeMessage("Failed to save: " + e.Message);
				//Assets.draco18s.util.DataAccess l;
				//GuiManager.ShowNotification(new NotificationItem("Failed to save", e.Message, GuiManager.instance.checkOff));
				//Console.WriteLine("Failed to serialize. Reason: " + e.Message);
				throw;
			}
			finally {
				s.Close();
			}
			if(File.Exists(path2)) {
				File.Delete(path2);
			}
			s = new FileStream(path2, FileMode.Create);
			try {
				formatter.Serialize(s, instance.player);
			}
			catch(SerializationException e) {
				DataAccess.PlatformSafeMessage("Failed to save: " + e.Message);
				//Assets.draco18s.util.DataAccess l;
				//GuiManager.ShowNotification(new NotificationItem("Failed to save", e.Message, GuiManager.instance.checkOff));
				//Console.WriteLine("Failed to serialize. Reason: " + e.Message);
				throw;
			}
			finally {
				s.Close();
			}
		}

		public static bool readDataFromCookie() {

			return false;
		}
		public static void writeDataToCookie() {

		}

		internal static int BitNum(long v) {
			int j = 0;
			while(v >= 1) {
				v = v >> 1;
				j++;
			}
			return j;
		}

		void Update() {
			float deltaTime = Time.deltaTime * GetSpeedMultiplier();
			autosaveTimer += Time.deltaTime;
			StatisticsTracker.timeCounter.addValue(Time.deltaTime);
			if(StatisticsTracker.timeCounter.floatValue >= 300) {
				StatisticsTracker.timeCounter.addValue(-300f);
				StatisticsTracker.totalTimePlayed.addValue(1);
			}
			UpdateBy(deltaTime);
		}

		void UpdateBy(float deltaTime) {
			autoClickTime += deltaTime * Main.instance.player.GetApprenticeClickSpeedMultiplier();
			TimeSinceLastRequest += deltaTime;
			bool doAutoClick = false;
			if(autoClickTime >= 1) {
				doAutoClick = true;
				autoClickTime -= 1;
			}

			TutorialManager.update();
			UnityEngine.Profiling.Profiler.BeginSample("Quest Manager");
			QuestManager.tickAllQuests(deltaTime);
			QuestManager.updateLists();
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Enchant Manager");
			EnchantingManager.update();
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Guild Manager");
			GuildManager.update();
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Research Manager");
			ResearchManager.update(deltaTime);
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Achievements Manager");
			AchievementsManager.update();
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Tick Built Items");
			bool needSynchro = false;
			
			float industryTime = deltaTime * Main.instance.player.currentGuildmaster.industryRateMultiplier();
			foreach(Industry i in player.builtItems) {
				if(!i.isProductionHalted) {
					if(i.getTimeRemaining() > float.MinValue) {
						needSynchro = i.addTime(-industryTime) || needSynchro;
					}
					if(doAutoClick) {
						i.tickApprentices();
					}
				}
				if(i.getTimeRemaining() <= 0) {
					//do {
					bool canExtract = true;
					foreach(IndustryInput input in i.inputs) {
						if(input.item.quantityStored < input.quantity * i.getTotalLevel() || input.item.isConsumersHalted) {
							canExtract = false;
						}
					}
					if(canExtract) {
						foreach(IndustryInput input in i.inputs) {
							input.item.quantityStored -= input.quantity * i.getTotalLevel();
						}
						if(i.getTimeRemaining() > float.MinValue) {
							i.didComplete++;
							i.addTimeRaw(10);
							i.quantityStored += i.ProduceOutput();
							//i.quantityStored += i.output * i.level;
						}
						else {
							i.setTimeRemaining(10);
						}
					}
					else {
						i.setTimeRemaining(float.MinValue);
					}
					//} while(i.getTimeRemaining() < 0 && i.getTimeRemaining() > float.MinValue);
				}
				//if(i.guiObj != null) {
				/**/
			Image img = i.craftingGridGO.transform.GetChild(0).GetChild(0).Find("Progress").GetComponent<Image>();
				img.material.SetFloat("_Cutoff", ((i.getTimeRemaining() >= 0 ? i.getTimeRemaining() : 10) / 10f));
				img.material.SetColor("_Color", i.productType.color);
				//}
			}
			//if(CraftingManager.doSynchronize)
			//Debug.Log(needSynchro);
			if(!needSynchro && CraftingManager.doSynchronize) {
				CraftingManager.doSynchronize = false;
				GuiManager.instance.craftHeader.transform.Find("SyncBtn").GetComponent<Button>().interactable = true;
			}
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Sell Items");
			player.clearCache();
			GetVendorValue();
			GetSellMultiplierFull();
			BigInteger maxSell;
			BigRational quant;
			BigInteger amt;
			BigRational val;
			foreach(Industry i in player.builtItems) {
				if(i.didComplete > 0) {
					do {
						maxSell = (i.isSellingStores ? -1 : (i.output * i.getTotalLevel()));
						quant = i.quantityStored;
						amt = i.getVendors() * GetVendorSize();
						if(maxSell >= 0)
							amt = MathHelper.Min(amt, maxSell);
						amt = MathHelper.Max(MathHelper.Min(amt, i.quantityStored - i.consumeAmount), 0);
						i.quantityStored -= amt;
						quant -= i.quantityStored;
						val = quant * GetVendorValue();
						val *= i.GetSellValue();
						AddMoney((BigInteger)val);
						//AddMoney(GetVendorValue() * (quant * i.GetSellValue()));
						i.didComplete--;
					} while(i.didComplete > 0);
				}
			}
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.BeginSample("Crafting Update");
			CraftingManager.update();
			UnityEngine.Profiling.Profiler.EndSample();
			if(Input.GetMouseButton(0)) {
				mouseDownTime += Time.deltaTime;
			}
			else {
				mouseDownTime = 0;
			}
			UnityEngine.Profiling.Profiler.BeginSample("Autobuild");
			if(debugAutoBuild) {
				autoBuildTimer += Time.deltaTime;
				if(autoBuildTimer > 5) {
					autoBuildTimer -= 5;
					bool ret = true;
					DateTime start = System.DateTime.Now;
					BigInteger currIncome = 0;
					foreach(Industry indu in player.builtItems) {
						currIncome += (BigInteger)indu.GetSellValue() * (indu.output < indu.getVendors() ? indu.output : indu.getVendors());
					}
					List<CostBenefitRatio> compares = GenerateComparisonList(currIncome);
					lastAutoBuilt = null;
					while(ret) {
						ret = SmartBuild(deltaTime, ref compares, currIncome);
						float timeTaken = (System.DateTime.Now - start).Milliseconds;
						//Debug.Log("Autobuild time used: " + timeTaken +"(cont? " + (ret?"yes":"no") + ")");
						if(timeTaken > 500) {
							ret = false;
						}
					}
					//Debug.Log("Total autobuild time used: " + (System.DateTime.Now - start).Milliseconds);
				}
				Material mat = GuiManager.instance.autoBuildBar.GetComponent<Image>().material;
				mat.SetFloat("_Cutoff", 1 - (autoBuildTimer / 5f));
			}
			else {
				autoBuildTimer = 0;
				Material mat = GuiManager.instance.autoBuildBar.GetComponent<Image>().material;
				mat.SetFloat("_Cutoff", 1);
				GuiManager.instance.autoBuildTarget.SetActive(false);
			}
			UnityEngine.Profiling.Profiler.EndSample();
			if(Mathf.FloorToInt(Time.time * 10) % 20 == 0) {
				foreach(Industry item in player.builtItems) {
					item.consumeAmount = 0;
				}
				foreach(Industry item in player.builtItems) {
					foreach(IndustryInput input in item.inputs) {
						if(!item.isProductionHalted && !input.item.isConsumersHalted) {
							float mod = (float)item.getHalveAndDouble() / input.item.getHalveAndDouble();
							float mod2 = player.getActiveDeepGoal().getSpeedModifier(item) / player.getActiveDeepGoal().getSpeedModifier(input.item);
							input.item.consumeAmount += Mathf.RoundToInt((input.quantity * item.getTotalLevel()) * mod * mod2);
						}
					}
				}
			}
			if(autosaveTimer >= 30) {
				autosaveTimer -= 30;
				checkDailyLogin();
				long nl = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
				StatisticsTracker.lastSavedTime.setValue((int)(nl/5));
				if(Application.platform == RuntimePlatform.WebGLPlayer) {
					DataAccess.Save(player);
				}
				else {
					writeDataToSave();
				}
			}
		}

		private List<CostBenefitRatio> GenerateComparisonList(BigInteger currIncome) {
			List<CostBenefitRatio> compares = new List<CostBenefitRatio>();
			FieldInfo[] fields = typeof(Industries).GetFields();
			
			//Debug.Log("start: "  + DateTime.Now.Millisecond);
			foreach(FieldInfo field in fields) {
				Industry ind = (Industry)field.GetValue(null);
				//player.itemData.TryGetValue(indBtn, out ind);
				CostBenefitRatio cb = GetCostBenefitFor(ind, currIncome);
				if(cb != null)
					compares.Add(cb);
			}
			return compares;
		}

		private CostBenefitRatio GetCostBenefitFor(Industry ind, BigInteger currIncome) {
			BigInteger seconds = currIncome > 0 ? ((BigInteger)ind.GetScaledCost() - player.money) / currIncome : -1000000;
			if(seconds < 60 && ind.doAutobuild && ind.autoBuildLevel > ind.level * ind.getHalveAndDouble() && ind.autoBuildMagnitude <= (player.money.ToString().Length - 1)) {
				BigInteger inputCosts = 0;
				int bonus = (ind.GetScaledCost() <= player.money ? 3 : (seconds <= 5 ? 5 : (seconds <= 30 ? 3 : 1)));
				double penalty = 1;
				foreach(IndustryInput input in ind.inputs) {
					if(input.item.level == 0) {
						penalty = 1000;
					}
					else if(input.item.consumeAmount >= input.item.output * input.item.getTotalLevel()) {
						penalty *= 10 * Math.Pow(input.item.productType.amount, 1 + input.item.getTotalLevel() + ((input.item.consumeAmount - (input.item.output * input.item.getTotalLevel())) / input.item.output));
						//Debug.Log(ind.name + " (" + input.item.name + "):" + penalty);
					}
					inputCosts += (input.item.GetBaseSellValue() * input.quantity);
				}
				if(ind.consumeAmount > ind.output * ind.level) {
					bonus *= 10;
				}
				if(ind.level == 0 && ind.GetScaledCost() <= player.money) {
					return new CostBenefitRatio(1, ((ind.GetBaseSellValue() * ind.output) - inputCosts) * bonus, ind);
				}
				else {
					return new CostBenefitRatio((BigInteger)(penalty * ind.GetScaledCost()), ((ind.GetBaseSellValue() * ind.output) - inputCosts) * bonus, ind);
				}
			}
			return null;
		}

		private static float autoBuildTimer = 0;
		private Industry lastAutoBuilt = null;

		private bool SmartBuild(float dt, ref List<CostBenefitRatio> compares, BigInteger curIncome) {
			bool ret = false;
			
			//Debug.Log("mid: " + DateTime.Now.Millisecond);
			compares.Sort();
			//Debug.Log("Current income/sec: " + currIncome);
			/*if(level == 8) {
				for(int i=0; i < compares.Count; i++) {
					Debug.Log("    " + i + ": " + compares[i].indust.unlocalizedName + " " + (compares[i].cost + "/" + compares[i].benefit) + "|" + (((BigInteger)compares[i].indust.GetScaledCost() - player.money)));
				}
			}*/
			Industry indust = (compares.Count > 0 ? compares[0].indust : null);
			if(indust != null && indust.GetScaledCost() <= player.money) {
				timeSinceLastPurchase = 0;
				CraftingManager.BuildIndustry(indust);
				foreach(IndustryInput input in indust.inputs) {
					float mod = (float)indust.getHalveAndDouble() / input.item.getHalveAndDouble();
					input.item.consumeAmount += Mathf.RoundToInt(input.quantity * mod);
				}
				CostBenefitRatio cb = GetCostBenefitFor(indust, curIncome);
				if(cb != null) {
					compares[0].cost = cb.cost;
					compares[0].benefit = cb.benefit;
				}
				else {
					compares.RemoveAt(0);
				}
				if(indust.getRawVendors() < indust.startingVendors && indust.level == 1 && PremiumUpgrades.VENDORREASSIGN.getIsPurchased()) {
					int needed = indust.startingVendors - indust.getRawVendors();
					int found = 0;
					foreach(Industry builtInd in player.builtItems) {
						if(builtInd.getRawVendors() > 0) {
							player.currentVendors -= builtInd.getRawVendors();
							found += builtInd.getRawVendors();
							builtInd.SetRawVendors(0);
							if(found >= needed) {
								break;
							}
						}
					}
					needed = Math.Min(needed, found);
					player.currentVendors += needed;
					indust.SetRawVendors(indust.getRawVendors() + needed);
				}
				//lastPurchase += indust.name + " ";
				ret = true;
			}
			if(indust != null) {
				lastAutoBuilt = indust;
				GuiManager.instance.autoBuildTarget.SetActive(true);
				//Debug.Log(indust.unlocalizedName);
				//if(indust.craftingGridGO != null)
					GuiManager.instance.autoBuildTarget.transform.localPosition = indust.getGridPos();
				//else
					//GuiManager.instance.autoBuildTarget.transform.position = Vector3.zero;
			}
			else {
				if(lastAutoBuilt == null)
					GuiManager.instance.autoBuildTarget.SetActive(false);
			}

			return ret;
		}

		public void CompleteQuest(ObstacleType goal) {
			player.QuestComplete(goal);
		}

		public static int GetNeededVendors(Industry indust) {
			int j = Mathf.CeilToInt((float)((indust.output * indust.getTotalLevel()) - indust.consumeAmount) / Main.instance.GetVendorSize());

			return j >= 0 ? j : 0;
		}

		private BigInteger ApproximateIncome() {
			BigInteger currIncome = 0;
			foreach(Industry indu in player.builtItems) {
				currIncome += (BigInteger)indu.GetSellValue() * (indu.output * indu.level < indu.getVendors() * GetVendorSize() ? indu.output * indu.level : indu.getVendors() * GetVendorSize());
			}
			return currIncome;
		}

		private BigInteger ApproximateIncome(FieldInfo[] fields) {
			BigInteger outval = 0;
			foreach(FieldInfo field in fields) {
				Industry ind = (Industry)field.GetValue(null);
				BigInteger o = ((ind.output * ind.getTotalLevel()) - ind.consumeAmount);
				outval += (BigInteger)(ind.GetSellValue() * (o >= 0 ? o : 0));
			}
			return outval;
		}

		public int GetQuestStackMultiplier(Industry selectedIndustry, long numAlreadyCompleted) {
			return Mathf.RoundToInt(1000 * player.GetQuestDifficultyMultiplier(numAlreadyCompleted));
		}

		public float GetSpeedMultiplier() {
			//TODO: Speed bonuses
			return (debugMode ? 1000 : 1);
		}

		public float GetClickRate() {
			return player.getClickRate();
		}

		public void AddMoney(BigInteger val) {
			//TODO: add various multipliers
			//money += val;
			player.AddMoney(val);
		}

		public int GetVendorSize() {
			//TODO: various bonuses
			return player.GetVendorSize();
		}

		public BigRational GetVendorValue() {
			return player.GetVendorValue();
		}

		public BigRational GetSellMultiplierFull() {
			return player.GetSellMultiplierFull();
		}

		public BigRational GetRelicSellMultiplier() {
			return player.GetRelicSellMultiplier();
		}

		private void switchTabImage(GameObject newTab, GameObject newArea, GameObject newHeader) {
			GuiManager.instance.craftTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.enchantTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.questTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.guildTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.researchTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			GuiManager.instance.achievementsTab.GetComponent<Image>().sprite = GuiManager.instance.unselTab;
			newTab.GetComponent<Image>().sprite = GuiManager.instance.selTab;

			GuiManager.instance.craftArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.enchantArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.questArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.guildArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.researchArea.GetComponent<Canvas>().enabled = false;
			GuiManager.instance.achievementsArea.GetComponent<Canvas>().enabled = false;
			newArea.GetComponent<Canvas>().enabled = true;

			GuiManager.instance.craftHeader.SetActive(false);
			GuiManager.instance.enchantHeader.SetActive(false);
			GuiManager.instance.questHeader.SetActive(false);
			GuiManager.instance.guildHeader.SetActive(false);
			GuiManager.instance.researchHeader.SetActive(false);
			GuiManager.instance.achievementsHeader.SetActive(false);
			newHeader.SetActive(true);

			if(newTab == GuiManager.instance.craftTab) {

			}
			if(newTab == GuiManager.instance.enchantTab) {
				EnchantingManager.setupUI();
			}
			if(newTab == GuiManager.instance.questTab) {
				QuestManager.setupUI();
			}
			if(newTab == GuiManager.instance.guildTab) {
				GuildManager.setupUI();
			}
			if(newTab == GuiManager.instance.researchTab) {
				ResearchManager.previousViewDate = ResearchManager.lastViewDate;
				ResearchManager.lastViewDate = DateTime.Now;
				ResearchManager.setupUI();
			}
			if(newTab == GuiManager.instance.achievementsTab) {
				AchievementsManager.setupUI();
			}
			GuiManager.instance.infoPanel.transform.localPosition = new Vector3(-1465, 55, 0);
			CraftingManager.FacilityUnselected(null);
		}

		public static string ToTitleCase(string stringToConvert) {
			bool convertNext = true;
			string output = "";
			foreach(char c in stringToConvert) {
				bool conv = convertNext;
				char newChar = (c == '_' ? ' ' : c);
				if(newChar == ' ') {
					convertNext = true;
				}
				else {
					convertNext = false;
				}
				if(!conv) {
					newChar = newChar.ToString().ToLower()[0];
				}
				else {
					newChar = newChar.ToString().ToUpper()[0];
				}
				output += newChar;
			}
			return output;
		}

		public static string AsCurrency(BigRational cost) {
			return AsCurrency(cost, 9);
		}

		public static string AsCurrency(BigRational cost, int maxDigits) {
			return AsCurrency(cost, maxDigits, false);
		}

		public static string AsCurrency(BigInteger cost) {
			return AsCurrency(cost, 9);
		}

		public static string AsCurrency(object cost) {
			if(cost is int || cost is float)
				return AsCurrency((int)cost, 9);
			if(cost is BigInteger || cost is BigRational)
				return AsCurrency((BigInteger)cost, 9);
			return cost.ToString();
		}

		public static string AsCurrency(object cost, int maxDigits) {
			if(cost is int || cost is float)
				return AsCurrency((int)cost, maxDigits);
			if(cost is BigInteger || cost is BigRational)
				return AsCurrency((BigInteger)cost, maxDigits);
			return cost.ToString();
		}

		public static string AsCurrency(BigInteger cost, int maxDigits) {
			return AsCurrency(cost, maxDigits, false);
		}

		public static string AsCurrency(BigRational valIn, int maxDigits, bool skipDecimal) {
			return AsCurrency((BigInteger)valIn, maxDigits, skipDecimal);
		}

		public static string AsCurrency(BigInteger val, int maxDigits, bool skipDecimal) {
			if(maxDigits < 4) maxDigits = 4;
			bool isNeg = val.IsNegative;
			BigInteger num = BigInteger.Abs(new BigInteger(val));
			string simple = num.ToString();
			string output = "";
			if(simple.Length > maxDigits) {
				int d = (simple.Length % 3);
				if(d == 0) d = 3;
				for(int i = 0; i < d; i++) {
					output += simple[i];
				}

				int m = 3;
				for(int i = d + 2; i >= d; i--) {
					if(simple[i].Equals('0')) {
						m--;
					}
					else {
						break;
					}
				}
				if(m == 0) {
					m = 1;
				}
				if(d + m > maxDigits) {
					m = maxDigits - d;
				}
				if(!skipDecimal) {
					output += ".";
					for(int i = d; i < d + m; i++) {
						output += simple[i];
					}
				}
				if(skipDecimal) {
					output += "e" + (simple.Length - d);
					if(output.Length > 6) { //this will fail at values greater than 9e99999
						int g = output.IndexOf('e');
						output = output.Substring(0, g - 1);
						output += "e" + (simple.Length - d + 1);
					}
				}
				else {
					output += " E" + (simple.Length - d);
				}
			}
			else {
				for(int i = 0; i < simple.Length; i++) {
					if(i % 3 == 0 && i != 0) {
						output = "," + output;
					}
					output = simple[(simple.Length - i - 1)] + output;
				}
			}
			if(isNeg) {
				output = "-" + output;
			}
			return output;
		}

		public static string SecondsToTime(float timeIn) {
			long time = (int)timeIn;
			float frac = timeIn - time;
			int fracInt = Mathf.RoundToInt(10 * frac);
			if(fracInt == 10) fracInt = 0;
			long seconds = (time % 60);
			long minutes = ((time - seconds) / 60) % 60;
			long hours = ((time - seconds - (minutes * 60)) / 3600);
			long days = ((time - seconds - (minutes * 60) - ((hours % 24) * 3600)) / 86400);
			if(hours > 0) {
				if(seconds > 30) minutes++;
				if(minutes > 59) {
					minutes = 0;
					hours++;
				}
				if(days > 0) {
					hours = hours % 24;
					return days + ":" + (hours < 10L ? "0" : "") + hours + "d";
				}
				return hours + ":" + (minutes < 10L ? "0" : "") + minutes + "h";
			}
			if(minutes > 0)
				return minutes + ":" + (seconds < 10L ? "0" : "") + seconds + "m";
			//return seconds + (fracInt > 0 ? (fracInt >= 10 ? "." + fracInt : ".0" + fracInt) : ".00") + "s";
			return seconds + "." + fracInt + "s";
		}

		public void writeCSVLine(string text) {
			//FieldInfo[] fields = typeof(Industries).GetFields();
			//csv_st.WriteLine(Mathf.FloorToInt(timeTotal) + "," + Mathf.FloorToInt(timeSinceLastPurchase) + "," + ApproximateIncome(fields) + "," + player.money + "," + text);
		}

		internal static void reportKongStats() {
///Kongregate statistics reporting
#if UNITY_WEBGL
			KongregateAPI.submitStat("lifetimeMoneyMagnitude", BigInteger.Log10(StatisticsTracker.lifetimeMoney.value));
			KongregateAPI.submitStat("lifetimeRenown", BigInteger.Log10(StatisticsTracker.lifetimeRenown.value));
			KongregateAPI.submitStat("questsCompletedEver", (StatisticsTracker.questsCompletedEver.value));
			KongregateAPI.submitStat("relicsIdentified", (StatisticsTracker.relicsIdentified.value));

			KongregateAPI.submitStat("impressiveAntiquity", (StatisticsTracker.impressiveAntiquity.isAchieved() ? 1 : 0));
			KongregateAPI.submitStat("clicksAch", ((AchievementMulti)StatisticsTracker.clicksAch).getNumAchieved());
			KongregateAPI.submitStat("allQuestsUnlocked", (StatisticsTracker.allQuestsUnlocked.isAchieved() ? 1 : 0));
			KongregateAPI.submitStat("completeHiddenQuest", (StatisticsTracker.defeatKraken.isAchieved() ? 1 : 0));
#endif
		}

		public class FirstRelics : IRelicMaker {
			public string relicDescription(ItemStack stack) {
				return "This relic predates recorded history.";
			}

			public string relicNames(ItemStack stack) {
				return "Lost";
			}
		}
	}
}
 