using KitchenLib.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenLib.JSON.Models.Containers
{
	public class ItemViewContainer 
	{
		[JsonProperty("Objects")]
		public List<string> _Objects;
		public List<GameObject> this[GameObject Prefab] => _Objects.ConvertAll(_ => GameObjectUtils.GetChildObject(Prefab, _));

		public Vector3 FullPosition;
		public Vector3 EmptyPosition;
	}
}
