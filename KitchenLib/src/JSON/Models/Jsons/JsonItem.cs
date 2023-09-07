using KitchenData;
using KitchenLib.Customs;
using KitchenLib.JSON.Interfaces;
using KitchenLib.JSON.Models.Containers;
using KitchenLib.JSON.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using KitchenLib.Utils;
using KitchenLib.JSON.Models.Views;
using Kitchen;

namespace KitchenLib.JSON.Models.Jsons
{
	internal class JsonItem : CustomItem, IHasSidePrefab
	{
		[field: JsonProperty("UniqueNameID", Required = Required.Always)]
		[JsonIgnore]
		public override string UniqueNameID { get; }

		[JsonProperty("GDOName")]
		public string GDOName { get; set; }

		[JsonProperty("Materials")]
		public MaterialsContainer Materials { get; set; }

		[JsonProperty("View")]
		public ItemViewContainer View { get; set; }

		[JsonIgnore]
		public override GameObject Prefab { get; protected set; }
		[JsonProperty("Prefab")]
		[JsonConverter(typeof(PrefabConverter))]
		public object TempPrefab { get; set; }

		[JsonIgnore]
		public override GameObject SidePrefab { get; protected set; }
		[JsonProperty("SidePrefab")]
		[JsonConverter(typeof(PrefabConverter))]
		public object TempSidePrefab { get; set; }

		[JsonIgnore]
		public override List<Item.ItemProcess> Processes { get; protected set; } = new();
		[JsonProperty("Processes")]
		public List<ItemProcessContainer> TempProcesses { get; set; } = new();

		[JsonIgnore]
		public override Item.ItemProcess AutomaticItemProcess { get; protected set; } = new();
		[JsonProperty("AutomaticItemProcess")]
		public ItemProcessContainer TempAutomaticItemProcess { get; set; } = new();

		[JsonIgnore]
		public override Item DirtiesTo { get; protected set; }
		[JsonProperty("DirtiesTo")]
		public string TempDirtiesTo { get; set; }

		[JsonIgnore]
		public override List<Item> MayRequestExtraItems { get; protected set; } = new();
		[JsonProperty("MayRequestExtraItems")]
		public List<string> TempMayRequestExtraItems { get; set; } = new();

		[JsonIgnore]
		public override List<Item> SatisfiedBy { get; protected set; } = new();
		[JsonProperty("SatisfiedBy")]
		public List<string> TempSatisfiedBy { get; set; } = new();

		[JsonIgnore]
		public override List<Item> NeedsIngredients { get; protected set; } = new();
		[JsonProperty("NeedsIngredients")]
		public List<string> TempNeedsIngredients { get; set; } = new();

		[JsonIgnore]
		public override Item SplitSubItem { get; protected set; }
		[JsonProperty("SplitSubItem")]
		public string TempSplitSubItem { get; set; }

		[JsonIgnore]
		public override List<Item> SplitDepletedItems { get; protected set; } = new();
		[JsonProperty("SplitDepletedItems")]
		public List<string> TempSplitDepletedItems { get; set; } = new();

		[JsonIgnore]
		public override Item SplitByComponentsHolder { get; protected set; }
		[JsonProperty("SplitByComponentsHolder")]
		public string TempSplitByComponentsHolder { get; set; }

		[JsonIgnore]
		public override Item SplitByComponentsWrapper { get; protected set; }
		[JsonProperty("SplitByComponentsWrapper")]
		public string TempSplitByComponentsWrapper { get; set; }

		[JsonIgnore]
		public override Item RefuseSplitWith { get; protected set; }
		[JsonProperty("RefuseSplitWith")]
		public string TempRefuseSplitWith { get; set; }

		[JsonIgnore]
		public override Item DisposesTo { get; protected set; }
		[JsonProperty("DisposesTo")]
		public string TempDisposesTo { get; set; }

		[JsonIgnore]
		public override Appliance DedicatedProvider { get; protected set; }
		[JsonProperty("DedicatedProvider")]
		public string TempDedicatedProvider { get; set; }

		[JsonIgnore]
		public override Dish CreditSourceDish { get; protected set; }
		[JsonProperty("CreditSourceDish")]
		public string TempCreditSourceDish { get; set; }

		[JsonIgnore]
		public override Item ExtendedDirtItem { get; protected set; }
		[JsonProperty("ExtendedDirtItem")]
		public string TempExtendedDirtItem { get; set; }

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			Tuple<string, string> Context = (Tuple<string, string>)context.Context;
			ModName = Context.Item2;
			ModID = Context.Item2;
		}

		public override void OnRegister(Item gameDataObject)
		{
			gameDataObject.name = GDOName;

			Materials.Convert(gameDataObject.Prefab);

			if (View != null)
			{
				if (View.FullPosition != null && View.EmptyPosition != null)
				{
					if (Prefab.HasComponent<PositionSplittableView>())
					{
						Prefab.AddComponent<PositionSplittableView>().Setup(Prefab, View);
					}
				}
				if (Prefab.HasComponent<ObjectsSplittableView>())
				{
					Prefab.AddComponent<ObjectsSplittableView>().Setup(Prefab, View);
				}
			}
		}
	}
}
