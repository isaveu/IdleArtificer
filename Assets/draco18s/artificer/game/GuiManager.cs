﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GuiManager : MonoBehaviour {
	public static GuiManager instance;
	public GameObject mainCanvas;
	public GameObject gridArea;
	public GameObject buildingList;
	public GameObject infoPanel;
	public GameObject topPanel;
	public GameObject currentMoney;
	public GameObject craftTab;
	public GameObject craftArea;
	public GameObject craftHeader;
	public GameObject enchantTab;
	public GameObject enchantArea;
	public GameObject enchantHeader;
	public GameObject questTab;
	public GameObject questArea;
	public GameObject questHeader;
	public GameObject guildTab;
	public GameObject guildArea;
	public GameObject guildHeader;
	public GameObject autoBuildBar;
	public GameObject buyVendorsArea;
	public GameObject buyApprenticesArea;
	public GameObject buyJourneymenArea;
	public GameObject tooltip;

	public Sprite gray_square;
	public Sprite selTab;
	public Sprite unselTab;
	public Sprite[] req_icons;

	

	void Start() {
		instance = this;
		req_icons = Resources.LoadAll<Sprite>("items/req_icons");
	}

	public static void ShowTooltip(Vector3 p, string v) {
		ShowTooltip(p, v, 1);
	}
	public static void ShowTooltip(Vector3 pos, string v, float ratio) {
		if(v.Length == 0) return;

		instance.tooltip.SetActive(true);
		((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
		((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 64);
		instance.tooltip.transform.position = pos;
		Text t = instance.tooltip.transform.FindChild("Text").GetComponent<Text>();
		t.text = v;
		//width + 7.5
		//height + 6
		bool fits = false;
		if(t.preferredWidth < 610) {
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (t.preferredWidth / 4) + 8);
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (t.preferredHeight / 4) + 7.5f);
			fits = true;
		}
		/*if(t.preferredHeight < 232) {
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (t.preferredHeight / 4) + 7.5f);
			fits = true;
		}*/
		float w = 64;// t.preferredWidth;
		if(!fits) {
			float h = 68;
			do {
				w += 64;
				((RectTransform)t.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
				h = t.preferredHeight;
			} while(h * ratio > w);

			((RectTransform)t.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
			((RectTransform)t.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
			h = t.preferredHeight;
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (w / 4) + 8);
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (h / 4) + 7.5f);
		}
		float wid = ((RectTransform)instance.tooltip.transform).rect.width;
		if(instance.tooltip.transform.position.x + wid > Screen.width) {
			//shift the tooltip down. No check for off-screen
			instance.tooltip.transform.position = new Vector3(Screen.width - wid/2 - 5, instance.tooltip.transform.position.y - ((RectTransform)instance.tooltip.transform).rect.height, 0);
		}
		else {
			instance.tooltip.transform.position += new Vector3(wid/2, 0, 0);
		}
	}
}
