using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public static class EditorApplicationUtility
	{
		public static List<System.Action> onNextUpdateCallbacks = new();

		[InitializeOnLoadMethod]
		public static void Initialise()
		{
			EditorApplication.update += OnUpdate;
		}

		public static void OnUpdate()
		{
			InvokeOnNextUpdateCallbacks();
		}

		public static void InvokeOnNextUpdateCallbacks()
		{
			if (onNextUpdateCallbacks == null || onNextUpdateCallbacks.Count == 0)
			{
				return;
			}

			foreach (var callback in onNextUpdateCallbacks)
			{
				try
				{
					callback?.Invoke();
				}
				catch (System.Exception e)
				{
					Debug.LogException(e);
				}
			}

			onNextUpdateCallbacks.Clear();
		}

		public static void AddNextUpdateCallback(System.Action action)
		{
			onNextUpdateCallbacks ??= new();

			onNextUpdateCallbacks.Add(action);
		}
	}
}
