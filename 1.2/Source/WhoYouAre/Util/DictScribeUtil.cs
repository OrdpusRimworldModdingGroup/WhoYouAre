using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WhoYouAre {
	public static class DictScribeUtil {

		public static Dictionary<K, V> ListToDict<K, V>(List<Entry_Value<K, V>> list) {
			var result = new Dictionary<K, V>();
			foreach (var item in list)
				result[item.Key] = item.Value;
			return result;
		}

		public static List<Entry_Value<K, V>> DictToList<K, V>(Dictionary<K, V> dict) {
			var result = new List<Entry_Value<K, V>>();
			foreach (var entry in dict)
				result.Add(new Entry_Value<K, V>(entry.Key, entry.Value));
			return result;
		}

	}

	public class Entry_Value<K, V> : IExposable {

		private K key;
		private V value;

		public K Key { get => key; }
		public V Value { get => value; }

		public Entry_Value(K key, V value) {
			this.key = key;
			this.value = value;
		}

		public Entry_Value() { }

		public void ExposeData() {
			Scribe_Values.Look(ref key, "key");
			Scribe_Values.Look(ref value, "value");
		}
	}
}
