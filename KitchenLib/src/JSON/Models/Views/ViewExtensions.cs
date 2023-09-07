using Kitchen;
using KitchenLib.JSON.Models.Containers;
using KitchenLib.Utils;
using System.Reflection;
using UnityEngine;

namespace KitchenLib.JSON.Models.Views
{
	public static class ViewExtensions
	{
		public static void Setup(this ObjectsSplittableView view, GameObject Prefab, ItemViewContainer container)
		{
			FieldInfo fObjects = ReflectionUtils.GetField<JsonObjectsSplittableView>("Objects");
			fObjects.SetValue(view, container[Prefab]);
		}

		public static void Setup(this PositionSplittableView view, GameObject Prefab, ItemViewContainer container)
		{
			FieldInfo fFullPosition = ReflectionUtils.GetField<JsonPositionSplittableView>("FullPosition");
			fFullPosition.SetValue(view, container.FullPosition);

			FieldInfo fEmptyPosition = ReflectionUtils.GetField<JsonPositionSplittableView>("EmptyPosition");
			fEmptyPosition.SetValue(view, container.EmptyPosition);

			FieldInfo fObjects = ReflectionUtils.GetField<JsonPositionSplittableView>("Objects");
			fObjects.SetValue(view, container[Prefab]);
		}
	}
}
