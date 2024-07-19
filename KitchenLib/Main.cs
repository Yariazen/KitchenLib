using Kitchen;
using KitchenLib.Customs;
using KitchenLib.DevUI;
using KitchenLib.Event;
using KitchenLib.Preferences;
using KitchenLib.UI;
using KitchenMods;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KitchenLib.UI.PlateUp;
using KitchenLib.Logging;
using KitchenLib.Logging.Exceptions;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using KitchenLib.Achievements;
using KitchenLib.Components;
using KitchenLib.Utils;
using KitchenLib.Views;
using Achievement = KitchenLib.Achievements.Achievement;

namespace KitchenLib
{
	/// <summary>
	/// The main class of the KitchenLib mod.
	/// </summary>
	public class Main : BaseMod
	{
		/// <summary>
		/// The ID of the mod.
		/// </summary>
		internal const string MOD_ID = "kitchenlib";

		/// <summary>
		/// The name of the mod.
		/// </summary>
		internal const string MOD_NAME = "KitchenLib";

		/// <summary>
		/// The author of the mod.
		/// </summary>
		internal const string MOD_AUTHOR = "KitchenMods";

		/// <summary>
		/// The version of the mod.
		/// </summary>
		internal const string MOD_VERSION = "0.8.2";

		/// <summary>
		/// The beta version of the mod.
		/// </summary>
		internal const string MOD_BETA_VERSION = "10";

		/// <summary>
		/// The compatible versions of the mod.
		/// </summary>
		internal const string MOD_COMPATIBLE_VERSIONS = ">=1.1.9";

		/// <summary>
		/// The asset bundle for the mod.
		/// </summary>
		internal static AssetBundle bundle;

		/// <summary>
		/// The preference manager for the mod.
		/// </summary>
		internal static PreferenceManager manager;

		/// <summary>
		/// The global preference manager for the mod.
		/// </summary>
		internal static PreferenceManager globalManager;

		/// <summary>
		/// The achievement manager for the mod.
		/// </summary>
		internal static AchievementsManager achievementsManager;

		/// <summary>
		/// The logger for the mod.
		/// </summary>
		internal static KitchenLogger Logger;
		
		/// <summary>
		/// The type of the preference system menu.
		/// </summary>
		internal static Type preferenceSystemMenuType = null;

		/// <summary>
		/// Whether or not extra debug info will be displayed. True when either the isDebug property in kitchenlib.json is True or when any mods (other than KL beta) are installed locally
		/// </summary>
		internal static bool debugLogging = false;

		/// <summary>
		/// Whether or not data should be synced with the Steam Cloud.
		/// </summary>
		internal static bool steamCloud = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="Main"/> class.
		/// </summary>
		public Main() : base(MOD_ID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_BETA_VERSION, MOD_COMPATIBLE_VERSIONS, Assembly.GetExecutingAssembly()) { }

		/// <summary>
		/// Called after the mod is activated.
		/// </summary>
		/// <param name="mod">The mod instance.</param>
		protected override void OnPostActivate(Mod mod)
		{	
			Logger = InitLogger();
			
			globalManager = new PreferenceManager(MOD_ID + ".global", true);
			globalManager.RegisterPreference(new PreferenceInt("steamCloud", 0));
			globalManager.Load();
			globalManager.Save();
			
			manager = new PreferenceManager(MOD_ID);
			manager.RegisterPreference(new PreferenceBool("enableChangingMenu", true));
			manager.RegisterPreference(new PreferenceBool("mergeWithPreferenceSystem", false));
			manager.RegisterPreference(new PreferenceBool("forceLocalDishes", true));
			manager.RegisterPreference(new PreferenceBool("isDebug", false));
			manager.RegisterPreference(new PreferenceInt("cosmeticWidth", 4));
			manager.RegisterPreference(new PreferenceInt("cosmeticHeight", 2));
			manager.RegisterPreference(new PreferenceInt("modSyncMethod", 0));
			manager.RegisterPreference(new PreferenceInt("achievementNotificatonDisplay", 1));
			manager.Load();
			manager.Save();
			
			determineDebugLoggingStatus();
			bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).FirstOrDefault() ?? throw new MissingAssetBundleException(MOD_ID);
			preferenceSystemMenuType = GetPreferenceSystemMenuType();
			SetupMenus();
			RegisterMenu<NewNewMaterialUI>();
			RegisterMenu<DebugMenu>();
			
			achievementsManager = new AchievementsManager(MOD_ID, MOD_NAME);
			achievementsManager.RegisterAchievement(new Achievement("test", "Super Wow!", "This is a super cool test achivement!", bundle.LoadAsset<Texture2D>("wow")));
			achievementsManager.RegisterAchievement(new Achievement("test2", "Another Wow", "This is a su2per cool test achivement!", bundle.LoadAsset<Texture2D>("vest"), new List<string>{"test"}));
			achievementsManager.RegisterAchievement(new Achievement("test3", "Triple Wow?", "This is a su3per cool test achivement!", bundle.LoadAsset<Texture2D>("wow"), new List<string>{"test"}));
			achievementsManager.Load();
			achievementsManager.Save();

			ViewUtils.RegisterView("KitchenLib.Views.SyncMods", typeof(SModSync), typeof(SyncMods));

			switch (manager.GetPreference<PreferenceInt>("achievementNotificatonDisplay").Value)
			{
				case 0:
					ViewUtils.RegisterView("KitchenLib.Views.AchievementNotification.None", typeof(SAchievementDisplayView.Marker), typeof(AchievementNotification), ViewMode.Screen, new Vector3(1, 1, 0));
					break;
				case 1:
					ViewUtils.RegisterView("KitchenLib.Views.AchievementNotification.Ticket", typeof(SAchievementDisplayView.Marker), typeof(AchievementNotification), ViewMode.Screen, new Vector3(1, 1, 0));
					break;
				case 2:
					ViewUtils.RegisterView("KitchenLib.Views.AchievementNotification.SteamClone", typeof(SAchievementDisplayView.Marker), typeof(AchievementNotification), ViewMode.Screen, new Vector3(1, 1, 0));
					break;
			}

			LogInfo(" __  ___  __  .___________.  ______  __    __   _______ .__   __.  __       __  .______  ");
			LogInfo("|  |/  / |  | |           | /      ||  |  |  | |   ____||  \\ |  | |  |     |  | |   _  \\ ");
			LogInfo("|  '  /  |  | `---|  |----`|  ,----'|  |__|  | |  |__   |   \\|  | |  |     |  | |  |_)  |");
			LogInfo("|    <   |  |     |  |     |  |     |   __   | |   __|  |  . `  | |  |     |  | |   _  <  ");
			LogInfo("|  .  \\  |  |     |  |     |  `----.|  |  |  | |  |____ |  |\\   | |  `----.|  | |  |_)  |");
			LogInfo("|__|\\__\\ |__|     |__|      \\______||__|  |__| |_______||__| \\__| |_______||__| |______/ " + $"   v{MOD_VERSION}b{MOD_BETA_VERSION}");

			Events.BuildGameDataEvent += (sender, args) => { if (args.firstBuild) AchievementsManager.SetupMenuElement(); };
		}

		private void determineDebugLoggingStatus() {
			int localModCount = ModPreload.Mods.Where(mod => mod.Name == "").Count();
			if (MOD_BETA_VERSION != "") {
				localModCount--;
			}
			bool isDebug = manager.GetPreference<PreferenceBool>("isDebug").Value;
			debugLogging = localModCount > 0 || isDebug;
			LogInfo($"GDO debug logging: {debugLogging} (local mods: {localModCount}, isDebug: {isDebug})");
		}


		/// <summary>
		/// Called during the initialization phase.
		/// </summary>
		protected override void OnInitialise()
		{
			GameObject go = new GameObject();
			go.AddComponent<DevUIController>();
		}

		private Type GetPreferenceSystemMenuType()
		{
			if (preferenceSystemMenuType != null)
				return preferenceSystemMenuType;
			
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type t in assembly.GetTypes())
				{
					if (t.FullName == "PreferenceSystem.Menus.PreferenceSystemMenu`1")
					{
						return t;
					}
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Sets up the menus for the mod.
		/// </summary>
		private void SetupMenus()
		{
			ModsPreferencesMenu<PauseMenuAction>.RegisterMenu("KitchenLib", typeof(PreferenceMenu<PauseMenuAction>), typeof(PauseMenuAction));
			ModsPreferencesMenu<MainMenuAction>.RegisterMenu("KitchenLib", typeof(PreferenceMenu<MainMenuAction>), typeof(MainMenuAction));
			
			Events.MainMenuView_SetupMenusEvent += (s, args) =>
			{
				args.addMenu.Invoke(args.instance, new object[] { typeof(SaveDataDisclosure), new SaveDataDisclosure(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(RevisedMainMenu), new RevisedMainMenu(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(FailedGDOsMenu), new FailedGDOsMenu(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(FailedModsMenu), new FailedModsMenu(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(ModsMenu<MainMenuAction>), new ModsMenu<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(ModsPreferencesMenu<MainMenuAction>), new ModsPreferencesMenu<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(DeveloperOptions<MainMenuAction>), new DeveloperOptions<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(UserOptions<MainMenuAction>), new UserOptions<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(ModAchievementsMenu<MainMenuAction>), new ModAchievementsMenu<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
			};

			//Setting Up For Pause Menu
			Events.MainMenu_SetupEvent += (s, args) =>
			{
				args.addSubmenuButton.Invoke(args.instance, new object[] { "Mods", typeof(ModsMenu<PauseMenuAction>), false });
				if (!manager.GetPreference<PreferenceBool>("mergeWithPreferenceSystem").Value && preferenceSystemMenuType != null || preferenceSystemMenuType == null)
					args.addSubmenuButton.Invoke(args.instance, new object[] { "Mod Preferences", typeof(ModsPreferencesMenu<PauseMenuAction>), false });
				args.addSubmenuButton.Invoke(args.instance, new object[] { "Mod Achievements", typeof(ModAchievementsMenu<PauseMenuAction>), false });
			};
			Events.PlayerPauseView_SetupMenusEvent += (s, args) =>
			{
				args.addMenu.Invoke(args.instance, new object[] { typeof(ModsMenu<PauseMenuAction>), new ModsMenu<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(ModsPreferencesMenu<PauseMenuAction>), new ModsPreferencesMenu<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(ModSyncMenu), new ModSyncMenu(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(ConfirmModSync), new ConfirmModSync(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(DeveloperOptions<PauseMenuAction>), new DeveloperOptions<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(UserOptions<PauseMenuAction>), new UserOptions<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
				args.addMenu.Invoke(args.instance, new object[] { typeof(ModAchievementsMenu<PauseMenuAction>), new ModAchievementsMenu<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
			};

			Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
			{
				args.Menus.Add(typeof(PreferenceMenu<PauseMenuAction>), new PreferenceMenu<PauseMenuAction>(args.Container, args.Module_list));
			};

			Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
			{
				args.Menus.Add(typeof(PreferenceMenu<MainMenuAction>), new PreferenceMenu<MainMenuAction>(args.Container, args.Module_list));
			};
		}
		
		/// <summary>
		/// Registers a new cape.
		/// </summary>
		/// <typeparam name="T">The type of the cape.</typeparam>
		/// <param name="id">The ID of the cape.</param>
		/// <param name="display">The display name of the cape.</param>
		private void RegisterNewCape<T>(string id, string display) where T : CustomPlayerCosmetic, new()
		{
			AddGameDataObject<T>();
		}

		[Obsolete]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LogInfo(object message)
		{
			Debug.Log($"[{MOD_NAME}] " + message);
		}

		[Obsolete]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LogWarning(object message)
		{
			Debug.LogWarning($"[{MOD_NAME}] " + message);
		}

		[Obsolete]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LogError(object message)
		{
			Debug.LogError($"[{MOD_NAME}] " + message);
		}

		[Obsolete]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LogDebug(object message)
		{
			if (debugLogging)
				Debug.Log($"[{MOD_NAME}] [DEBUG] " + message);
		}
	}
}