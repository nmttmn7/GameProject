﻿using Godot;
using System.Collections.Generic;

namespace TheLiquidFire.Extensions
{
	public static class ListExtensions
	{
		public static T Random<T> (this List<T> list)
		{
			
			int index = RNGFactory.RandiRange(0, list.Count - 1);
			//int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}

		public static T First<T> (this List<T> list)
		{
			return list[0];
		}

		public static T Last<T> (this List<T> list)
		{
			return list[ list.Count - 1 ];
		}

		public static T GetIndex<T>(this List<T> list, int index)
		{
			return list[index];
		}

		public static T Draw<T> (this List<T> list) {
			if (list.Count == 0)
				return default(T);

			
			int index = RNGFactory.RandiRange(0, list.Count - 1);
			var result = list [index];
			list.RemoveAt (index);
			return result;
		}

		public static List<T> Draw<T> (this List<T> list, int count) {
			int resultCount = Mathf.Min (count, list.Count);
			List<T> result = new List<T> (resultCount);
			for (int i = 0; i < resultCount; ++i) {
				T item = list.Draw ();
				result.Add (item);
			}
			return result;
		}

		public static List<T> ReturnNew<T> (this List<T> list) {
			List<T> result = new List<T> (list);
			return result;
		}


	}
}