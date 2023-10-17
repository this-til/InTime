using System;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Godot;
using Newtonsoft.Json.Linq;

namespace InTime;

public static class EventExtendMethod {
	[Event(eventAttributeType = EventAttributeType.no)]
	public static T onEvent<T>(this T @event) where T : Event => (T)World.getInstance().getEventBus().onEvent(@event);
}

public static class ArrayExtendMethod {
	public static void forEach<T>(this T[] t, Action<T> a) {
		foreach (var x1 in t) {
			if (x1 != null) {
				a(x1);
			}
		}
	}

	/// <summary>
	/// 判断数组是不是空
	/// </summary>
	public static bool isEmpty<T>(this T[]? t) => t == null || t.Length == 0;
}

public static class EntityExtendMethod {
	/// <summary>
	/// 货取实体中心点
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public static Vector3 getEntityCenterPos(this Entity e) => getEntityCenterTransform(e).Position;

	public static Node3D getEntityCenterTransform(this Entity e) {
		switch (e) {
			case EntityLiving entityLiving:
				return entityLiving.get(AllEntityPos.centerPos);
			default:
				return e;
		}
	}
}

public static class HashSetMethod {
	public static bool isEmpty<T>(this HashSet<T> t) => t == null || t.Count == 0;
}

public static class ListExtendMethod {
	public static bool isEmpty<T>(this List<T>? list) => list is null || list.Count <= 0;

	/// <summary>
	/// 数组两两配对
	/// </summary>
	public static List<DataStruct<T, T>> combineTwoElement<T>(this List<T> list) {
		List<DataStruct<T, T>> _list = new List<DataStruct<T, T>>();
		if (list.isEmpty()) {
			return _list;
		}
		int count = list.Count;
		for (var i = 0; i < count; i++) {
			for (var ii = i; ii < count; ii++) {
				_list.Add(new DataStruct<T, T>(list[i], list[ii]));
			}
		}
		return _list;
	}

	/// <summary>
	/// 通过枚举类类型返回所有枚举的集合
	/// </summary>
	public static List<E> to<E>(this Type e) where E : Enum {
		List<E> list = new List<E>();
		foreach (E _e in Enum.GetValues(typeof(E))) {
			list.Add(_e);
		}
		return list;
	}
}

public static class DictionaryExtendMethod {
	public static V put<K, V>(this Dictionary<K, V> dictionary, KeyValuePair<K, V> keyValuePair) => put(dictionary, keyValuePair.Key, keyValuePair.Value);

	public static V put<K, V>(this Dictionary<K, V> dictionary, DataStruct<K, V> dataStruct) => put(dictionary, dataStruct.k, dataStruct.v);

	public static V put<K, V>(this Dictionary<K, V> dictionary, K k, V v) {
		if (k == null) {
			return v;
		}

		if (dictionary.ContainsKey(k) && v != null) {
			dictionary[k] = v;
		}
		else {
			if (v is null) {
				dictionary.Remove(k);
			}
			else {
				dictionary.Add(k, v);
			}
		}

		return v;
	}

	/// <summary>
	/// 或取V，如果没有K或者K为null将会返回V，如果V不为null就将它添加到表
	/// </summary>
	/// <returns></returns>
	public static V get<K, V>(this Dictionary<K, V> dictionary, K? k) {
		if (k == null) {
			return default;
		}

		if (dictionary.ContainsKey(k)) {
			return dictionary[k];
		}

		return default;
	}

	public static V get<K, V>(this Dictionary<K, V> dictionary, K k, V v) {
		if (k == null) {
			return v;
		}
		if (dictionary.ContainsKey(k)) {
			V outV = dictionary[k];
			if (outV is null) {
				dictionary[k] = v;
				return v;
			}
			return outV;
		}
		dictionary.Add(k, v);
		return v;
	}

	/// <summary>
	/// 或取V，如果没有K或者K为null将会返回V，如果V不为null就将它添加到表
	/// </summary>
	/// <returns></returns>
	public static V get<K, V>(this Dictionary<K, V> dictionary, K k, Func<V> v) {
		if (k == null) {
			return v();
		}

		if (dictionary.ContainsKey(k)) {
			V outV = dictionary[k];
			if (outV is null) {
				V inV = v();
				if (inV != null) {
					dictionary[k] = inV;
					return inV;
				}
			}

			return outV;
		}
		else {
			V inV = v();
			if (inV != null) {
				dictionary.Add(k, inV);
			}

			return inV;
		}
	}

	public static void add<K, V>(this Dictionary<K, V> dictionary, IDictionary<K, V> d) => add(dictionary, d, (v1, v2) => v2);

	public static void add<K, V>(this Dictionary<K, V> dictionary, IDictionary<K, V> d, Func<V, V, V> choice) {
		foreach (var keyValuePair in d) {
			if (dictionary.ContainsKey(keyValuePair.Key)) {
				dictionary[keyValuePair.Key] = choice(d[keyValuePair.Key], keyValuePair.Value);
			}
			else {
				put(dictionary, keyValuePair.Key, keyValuePair.Value);
			}
		}
	}

	public static void forEach<K, V>(this Dictionary<K, V> dictionary, Action<K, V> action) {
		foreach (var keyValuePair in dictionary) {
			if (keyValuePair.Value != null) {
				action(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}

	public static bool contains<K, V>(this Dictionary<K, V> dictionary, K t) => t is not null && dictionary.ContainsKey(t);

	public static void remove<K, V>(this Dictionary<K, V> dictionary, K k, Action<K, V> action) {
		if (contains(dictionary, k)) {
			action(k, dictionary[k]);
			dictionary.Remove(k);
		}
	}

	public static bool isEmpty<K, V>(this Dictionary<K, V> dictionary) => dictionary.Count <= 0;
}

public static class StackExtendMethod {
	public static bool isEmpty<T>(this Stack<T> t) =>
		t.Count == 0;
}

public static class NumberExtendMethod {
	/// <summary>
	/// 根据布尔类型返回t或f
	/// </summary>
	public static int to(this bool b, int t, int f) => b ? t : f;

	/// <summary>
	/// 根据布尔类型返回1或-1
	/// </summary>
	public static int to(this bool b) => to(b, 1, -1);

	/// <summary>
	/// 有效检测，检测f的绝对值是不大于t的绝对值
	/// </summary>
	public static bool isEffective(this float f, float t) => f > t || f < -t;

	/// <summary>
	/// 有效检测，检测f的绝对值是不大于t的绝对值
	/// </summary>
	public static bool isEffective(this double f, double t) => f > t || f < -t;

	/// <summary>
	/// 门槛检测，当f的绝对值是大于t时返回f，不然返回0
	/// </summary>
	public static float threshold(this float f, float l) => f > l || f < -l ? f : 0;

	/// <summary>
	/// 门槛检测，当f的绝对值是大于t时返回f，不然返回0
	/// </summary>
	public static double threshold(this double f, double l) => f > l || f < -l ? f : 0;
}

public static class QuaternionExtendMethod {
	public static readonly Vector3 up = new Vector3(0, 1, 0);

	public static Quaternion LookRotation(Vector3 forward, Vector3 upwards) {
		Vector3 right = forward.Cross(upwards).Normalized(); // 计算右方向向量
		Vector3 newUp = right.Cross(forward).Normalized(); // 计算新的上方向向量
		float m00 = right.X;
		float m01 = right.Y;
		float m02 = right.Z;
		float m10 = newUp.X;
		float m11 = newUp.Y;
		float m12 = newUp.Z;
		float m20 = forward.X;
		float m21 = forward.Y;
		float m22 = forward.Z;

		float num8 = m00 + m11 + m22;
		Quaternion rotation = new Quaternion();

		if (num8 > 0f) {
			float num = Mathf.Sqrt(num8 + 1f);
			rotation.W = num * 0.5f;
			num = 0.5f / num;
			rotation.X = (m12 - m21) * num;
			rotation.Y = (m20 - m02) * num;
			rotation.Z = (m01 - m10) * num;
		}
		else if (m00 >= m11 && m00 >= m22) {
			float num7 = Mathf.Sqrt(1f + m00 - m11 - m22);
			float num4 = 0.5f / num7;
			rotation.X = 0.5f * num7;
			rotation.Y = (m01 + m10) * num4;
			rotation.Z = (m02 + m20) * num4;
			rotation.W = (m12 - m21) * num4;
		}
		else if (m11 > m22) {
			float num6 = Mathf.Sqrt(1f + m11 - m00 - m22);
			float num3 = 0.5f / num6;
			rotation.X = (m10 + m01) * num3;
			rotation.Y = 0.5f * num6;
			rotation.Z = (m21 + m12) * num3;
			rotation.W = (m20 - m02) * num3;
		}
		else {
			float num5 = Mathf.Sqrt(1f + m22 - m00 - m11);
			float num2 = 0.5f / num5;
			rotation.X = (m20 + m02) * num2;
			rotation.Y = (m21 + m12) * num2;
			rotation.Z = 0.5f * num5;
			rotation.W = (m01 - m10) * num2;
		}
		return rotation;
	}
}

public static class JObjectExtendMethod {
	public static T? gatAs<T>(this JObject jObject, string key) {
		if (jObject.ContainsKey(key)) {
			return jObject[key]!.Value<T>();
		}
		return default;
	}
}
