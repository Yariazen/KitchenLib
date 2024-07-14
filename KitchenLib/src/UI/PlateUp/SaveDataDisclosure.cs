﻿using System.IO;
using Kitchen;
using Kitchen.Modules;
using KitchenLib.Achievements;
using KitchenLib.Preferences;
using UnityEngine;

namespace KitchenLib.UI.PlateUp
{
	public class SaveDataDisclosure : KLMenu<MainMenuAction>
	{
		public SaveDataDisclosure(Transform container, ModuleList module_list) : base(container, module_list)
		{
		}
		
		public override void Setup(int player_id)
		{
			AddLabel("Save Data Disclosure");
			AddInfo("KitchenLib will save your preferences and achievements locally and backed up via Steam Cloud across all devices.");
			AddInfo("In order to achieve this, KitchenLib will store your preferences and achievements in the following location :");
			New<SpacerElement>();
			AddInfo($"{Path.Combine(Application.persistentDataPath, "ModData", "KitchenLib", ".plateupsave")}");
			New<SpacerElement>();
			AddInfo("This is in the same location PlateUp! data is already stored.");
			AddInfo("Absolutely no data is shared with any third party other than the Steam Cloud service.");
			
			AddButton("Ok - Lets Cook!", delegate (int i)
			{
				Main.globalManager.GetPreference<PreferenceInt>("steamCloud").Set(2);
				Main.globalManager.Save();
				
				foreach (PreferenceManager manager in PreferenceManager.Managers)
				{
					Debug.Log("Changing PreferenceManager");
					manager.ChangeFileType(".plateupsave");
				}
				
				foreach (AchievementsManager manager in AchievementsManager.Managers)
				{
					Debug.Log("Changing AchievementsManager");
					manager.ChangeFileType(".plateupsave");
				}
				
				RequestSubMenu(typeof(RevisedMainMenu));
			}, 0, 1f, 0.2f);
			
			AddButton("No Thanks", delegate (int i)
			{
				Main.globalManager.GetPreference<PreferenceInt>("steamCloud").Set(1);
				Main.globalManager.Save();
				
				foreach (PreferenceManager manager in PreferenceManager.Managers)
				{
					manager.ChangeFileType(".json");
				}
				
				foreach (AchievementsManager manager in AchievementsManager.Managers)
				{
					manager.ChangeFileType(".json");
				}
				
				RequestSubMenu(typeof(RevisedMainMenu));
			}, 0, 1f, 0.2f);
			
			AddButton("Data Information", delegate (int i)
			{
				RequestSubMenu(typeof(RevisedMainMenu));
			}, 0, 1f, 0.2f);
		}
	}
}