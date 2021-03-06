﻿using Assets.draco18s.artificer.game;
using Assets.draco18s.artificer.init;
using Assets.draco18s.util;
using Koopakiller.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.artificer.upgrades {
	class UpgradeRenownMulti : Upgrade {
		protected readonly float amount;
		public UpgradeRenownMulti(BigInteger upgradeCost, float amount, string saveName) : base(UpgradeType.RENOWN_MULTI, upgradeCost, "Increase Renown Effectiveness by " + Mathf.FloorToInt(amount*100) +"%", saveName) {
			this.amount = amount;
		}

		public override void applyUpgrade() {
			base.applyUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value += amount;
		}
		public override void revokeUpgrade() {
			base.revokeUpgrade();
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			((UpgradeFloatValue)wrap).value -= amount;
		}

		public override string getTooltip() {
			UpgradeValueWrapper wrap;
			Main.instance.player.upgrades.TryGetValue(upgradeType, out wrap);
			double baseval = (((UpgradeFloatValue)wrap).value + SkillList.RenownMulti.getMultiplier()) * 100;
			return "Increases the effectiveness of renown on your cash income.\nThe base value is 2% extra income per renown, currently it is " + Math.Round(baseval) + "%, and with this upgrade it would be " + Math.Round(baseval + (amount *100)) +"%";
		}

		public override string getIconName() {
			return "upgrades/renown_multi";
		}
	}
}
