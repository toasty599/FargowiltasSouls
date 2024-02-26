using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FargowiltasSouls.Common.StateMachines
{
	/// <summary>
	/// Allows for autocalling all methods in a given class with this attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class AutoloadMethod : Attribute
	{
		public static void LoadMethods<TInstanceType>(TInstanceType instance)
		{
			var methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
			if (methods == null || methods.Length == 0)
				return;

			foreach (var method in methods)
			{
				var autoloadAttribute = method.GetCustomAttribute<AutoloadMethod>();
				if (autoloadAttribute != null)
					method.Invoke(instance, null);
			}
		}
	}
}
