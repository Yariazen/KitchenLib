using KitchenData;
using System.Collections.Generic;
using System;

namespace KitchenLib.Customs
{
    public abstract class CustomSubProcess
    {
        public abstract void Convert(out Item.ItemProcess itemProcess);
        public abstract void Convert(out Appliance.ApplianceProcesses applianceProcess);

        public virtual string UniqueName { get; internal set; }

		public static Dictionary<string, CustomSubProcess> SubProcesses = new Dictionary<string, CustomSubProcess>();
		public static Dictionary<Type, CustomSubProcess> SubProcessesByType = new Dictionary<Type, CustomSubProcess>();

        public static T RegisterSubProcess<T>(T subproc) where T : CustomSubProcess
		{

			if (SubProcesses.ContainsKey(subproc.UniqueName))
			{
				return null;
			}

			SubProcesses.Add(subproc.UniqueName, subproc);
			SubProcessesByType.Add(subproc.GetType(), subproc);

			return subproc;
		}

		public static CustomSubProcess GetSubProcess(string uniqueName)
		{
			SubProcesses.TryGetValue(uniqueName, out var result);
			return result;
		}

		public static CustomSubProcess GetSubProcess<T>()
		{
			SubProcessesByType.TryGetValue(typeof(T), out var result);
			return result;
		}
    }
}