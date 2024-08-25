﻿using System.Linq;
using UnityEditor;
using System.Reflection;
using System;

namespace Editor
{
	public static class MenuProxy
	{
		public static MethodInfo getMenuItemsMethod;

		public static MethodInfo removeMenuItemMethod;
		public static MethodInfo addMenuItemMethod;

		public static MethodInfo menuItemExistsMethod;

		static MenuProxy()
		{
			var type = typeof(Menu);

			getMenuItemsMethod = type
				.GetMethod("GetMenuItems", BindingFlags.NonPublic | BindingFlags.Static);

			removeMenuItemMethod = type
				.GetMethod("RemoveMenuItem", BindingFlags.NonPublic | BindingFlags.Static);

			addMenuItemMethod = type
				.GetMethod("AddMenuItem", BindingFlags.NonPublic | BindingFlags.Static);

			menuItemExistsMethod = type
				.GetMethod("MenuItemExists", BindingFlags.NonPublic | BindingFlags.Static);
		}

		public static ScriptingMenuItemProxy[] GetMenuItems(string menuPath, bool includeSeparators, bool localized)
		{
			var result = (Array)getMenuItemsMethod
				.Invoke(null, new object[] { menuPath, includeSeparators, localized });

			var menus = new ScriptingMenuItemProxy[result.Length];

			for (int i = 0; i < result.Length; i++)
			{
				var item = result.GetValue(i);
				menus[i] = new ScriptingMenuItemProxy(item);
			}

			return menus;
		}

		public static void RemoveMenuItem(string menuItem)
		{
			removeMenuItemMethod.Invoke(null, new object[] { menuItem });
		}
		public static void AddMenuItem(string name, string shortcut, bool isChecked, int priority, System.Action action, Func<bool> validate)
		{
			addMenuItemMethod.Invoke(null, new object[] { name, shortcut, isChecked, priority, action, validate });
		}

		public static bool MenuItemExists(string name)
		{
			return (bool)menuItemExistsMethod.Invoke(null, new object[] { name });
		}
	}
}
