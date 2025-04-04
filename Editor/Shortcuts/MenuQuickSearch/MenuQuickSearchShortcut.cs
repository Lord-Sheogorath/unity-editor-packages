using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using System.Linq;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using System;
using LordSheo.Editor.UI;

namespace LordSheo.Editor.Shortcuts
{
	public static class MenuQuickSearchShortcut
	{
		public static string[] selectedMenus;
		public static UnityEngine.Object[] selectedObjects;

		public static MenuQuickSearchSettings Settings => MenuQuickSearchSettings.Instance;

		public static void OnNextUpdate()
		{
			if (selectedMenus == null || selectedMenus.Length == 0)
			{
				return;
			}

			var prevWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

			Selection.objects = selectedObjects;

			foreach (var menu in selectedMenus)
			{
				EditorApplication.ExecuteMenuItem(menu);
			}

			var newWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

			var openedWindow = newWindows
				.Except(prevWindows)
				.FirstOrDefault();

			EditorApplicationUtility.ForceFocusWindow(openedWindow);

			selectedMenus = null;
			selectedObjects = null;
		}

		[Shortcut(ConstValues.NAMESPACE_PATH + "Menu/QuickSearch", KeyCode.Period, ShortcutModifiers.Control)]
		public static void OnShortcut()
		{
			selectedObjects = Selection.objects;

			// NOTE: Can cache this
			var menus = MenuProxy.cachedMenuRoots
				.SelectMany(m => MenuProxy.GetMenuItems(m, true, true))
				.Where(m => m != null && Menu.GetEnabled(m.path))
				.Where(m => MenuProxy.MenuItemExists(m.path))
				.Where(m => Settings.IsValid(m))

				// NOTE: Remove open windows toolbar thingy
				.Where(m => m.path.StartsWith("Window/Panel") == false)
				.OrderBy(m => m.path)
				.ThenBy(m => m.priority);

			var items = menus
				.Select(m => new GenericSelectorItem<StringSearch>(m.path, m.path))
				.ToArray();

			var menu = new BetterGenericSelector<StringSearch>(null, false, items);
			menu.showItemPathsAsNames = Settings.showMenuPathsAsNames;
			
			var window = menu.ShowInPopup(500);

			menu.SetWindow(window);
			menu.SelectionConfirmed += OnSelectionConfirmed;
		}

		public static void OnSelectionConfirmed(IEnumerable<StringSearch> selection)
		{
			// NOTE: Have to execute outside the scope of the popup
			selectedMenus = selection.Select(s => s.value).ToArray();

			EditorApplicationUtility.AddNextUpdateCallback(OnNextUpdate);
		}
	}
}
