using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace KR
{
#if UNITY_EDITOR
	namespace Editor {
		public static class Utility {

			public static void DisplayProgressBar( string title, string info, float duration, Action task){
			
				float t = 0;
				UnityEditor.EditorUtility.DisplayProgressBar(title, info, t);
				task?.Invoke();
				duration *= 1000f;
				while( t < 1){
					t += 1 / duration;
					UnityEditor.EditorUtility.DisplayProgressBar(title, info, t);
				}
				UnityEditor.EditorUtility.ClearProgressBar();
			}
		}
	}
#endif

	namespace SceneManagement{
		public  class SceneManager{
			public static void LoadScene(string sceneName,  Action onStart){

					onStart?.Invoke();
					UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                
			}

            public static void LoadScene(int sceneIndex, Action onStart)
            {
                
                    onStart?.Invoke();
					UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
                
            }
        }
	}
	namespace YieldInstruction
	{
		public class WhileLoop : CustomYieldInstruction
		{
			public override bool keepWaiting
			{
				get
				{
					action?.Invoke(t);
					t += Time.deltaTime / duration.clamp(0.001f); 
					return t < 1;
				}
			}
			public float duration;
			Action<float> action;
			private float t;
			public WhileLoop(float duration, Action<float> action)
			{
				this.duration = duration;
				this.action = action;
			}


		}



	}
	#region Serializable
	[Serializable]
	public struct CurrencyFormatter
	{
		public double maxValue;
		public string output;
		public CurrencyFormatter(double maxValue, string output )
		{
			this.maxValue = maxValue;
			this.output = output;
		}


		private static CurrencyFormatter[] _Standard;
		public static CurrencyFormatter[] Standard
		{
			get
			{
				return _Standard ??
					(_Standard = new CurrencyFormatter[]{
					new CurrencyFormatter(10e2, " "),
					new CurrencyFormatter(10e5, "K"),
					new CurrencyFormatter(10e8, "M"),
					new CurrencyFormatter(10e11, "B")
				});
			}
		}
	}
	#endregion
	public static class Delegate
	{

public static void QueueSchedule(this MonoBehaviour behaviour, float t, Action callback, Action onQueue, Func<bool> predicate){
			behaviour.StartCoroutine(IQueueSchedule(t, callback, onQueue, predicate));
		}

		static IEnumerator IQueueSchedule(float t, Action callback,Action onQueue, Func<bool> predicate){

			while(predicate()){
				yield return null;
			}
			onQueue?.Invoke();
			yield return new WaitForSeconds(t);
			callback?.Invoke();

		}


		public static void Schedule(this MonoBehaviour behaviour, float time, Action callback)
		{
			behaviour.StartCoroutine(ISchedule(time, callback));
		}
		static IEnumerator ISchedule(float t, Action callback)
		{
			yield return new WaitForSecondsRealtime(t);
			callback?.Invoke();
		}

        public static void ScheduleScaled(this MonoBehaviour behaviour, float time, Action callback)
        {
            behaviour.StartCoroutine(IScheduleScaled(time, callback));
        }
        static IEnumerator IScheduleScaled(float t, Action callback)
        {
            yield return new WaitForSeconds(t);
            callback?.Invoke();
        }

    }



	public static class Array
	{
		public static T[] clear<T>(this T[] value, T dflt = default(T))
		{
			int leng = value.Length;
			for (int i = 0; i < leng; i++)
			{
				value[i] = dflt;
			}
			return value;
		}

		public static T GetRandomElement<T>(this IEnumerable<T> ar){
			int r = UnityEngine.Random.Range(0, ar.Count());
			int i = 0;
			foreach (var it in ar)
            {
				if (i == r)
					return it;
				i++;
            }

			throw new ArgumentOutOfRangeException();

		}

		public static void forEach<T>(this IEnumerable<T> ar, Action<T, int> callback)
		{
			int i = 0;
			foreach (var it in ar)
			{
				callback(it, i);
				i++;
			}
		}

		public static void forEach<T>(this IEnumerable<T> ar, Action<T> callback)
		{
			foreach (var it in ar)
			{
				callback(it);
			}
		}
	}
	public static class Int
	{

		public static int log(this int value, int @base)
		{
			return Mathf.Log(value, @base).round();
		}
		public static int log2(this int value)
		{
			return Mathf.Log(value, 2).round();
		}
		public static int abs(this int value)
		{
			return value < 0 ? Mathf.Abs(value) : value;
		}
		public static int round(this float value)
		{
			return Mathf.RoundToInt(value);
		}

		public static int floor(this float value)
		{
			return Mathf.FloorToInt(value);
		}
		public static int toInt(this byte value)
		{
			return (int)value;
		}
		public static int floor(this int value, int target)
		{
			return (target * (value / target) - (value % target));
		}
		/// <summary>
		/// Clamp the specified value, min and max.
		/// </summary>
		public static int clamp(this int value, int min = int.MinValue, int max = int.MaxValue)
		{
			return value < min ? min : value > max ? max : value;
		}

	}
	public static class Bool
	{

        
		public static bool ColorCompare(this Color a, Color b, int acurate = 256){
			bool req = ((int)(a.r * acurate) == (int)(b.r * acurate));
			bool geq = ((int)(a.g * acurate) == (int)(b.g * acurate));
			bool beq = ((int)(a.b * acurate) == (int)(b.b * acurate));
			bool aeq = ((int)(a.a * acurate) == (int)(b.a * acurate));

			//Debug.LogFormat("BR: {0}, AR: {1}", (int)(a.b * acurate), (int)(b.b * acurate));
			//Debug.LogFormat("R: {0}, G: {1}, B: {2}, A: {3}", req, geq, beq, aeq);


			return req && geq && beq && aeq;


		}

		// public static bool equals<T>(this T value, T compare) where T : System.Enum
		// {
		// 	return (value & compare) == compare;

		// }


	public static bool IsPointerOverUIObject(this EventSystem eventSystem)
        {
			PointerEventData eventDataCurrentPosition = new PointerEventData(eventSystem);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

		/// <summary>
		/// d= sqrt(pow(xp−xc)2+(yp−yc),2)
		/// </summary>
		/// <returns><c>true</c>, if circle was insided, <c>false</c> otherwise.</returns>
		/// <param name="radius">Radius.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		public static bool insideCircle(this float radius, Vector3 from, Vector3 to)
		{
			return ((to.x - from.x).pow(2) + (to.y - from.y).pow(2)).sqrt().pow(2) < radius.pow(2);

		}




		public static bool nullPtr<T>(this T[] value)
		{
			return !(value != null && value.Length > 0);
		}
		public static bool nullPtr<T>(this T value)
		{
			return !(value != null);
		}
		public static bool nullPtr<T>(this T value, Action callback)
		{
			var result = value.nullPtr();
			callback();
			return result;
		}
		public static bool nullPtr<T>(this T[] value, Action<bool> callback)
		{
			var result = value.nullPtr();
			callback(result);
			return (result);
		}
	}
	public static class Corountine
	{

		public static Coroutine Invoke(this IEnumerator enumerator, MonoBehaviour behaviour)
		{
			return behaviour.StartCoroutine(enumerator);

		}

		//public static void CallDelay(this MonoBehaviour behaviour, IEnumerator enumerator, float delay){
		//behaviour.StartCoroutine(enumerator);
		//}
	}
	public static class Float
	{
		/// <summary>
		/// 
		/// Clamp the specified value, min and max.


		/// </summary>
		public static float abs(this float value)
		{

			return value > 0 ? value : -value;
		}

		public static double Lerp(this MonoBehaviour behaviour, double a, double b, float t)
		{
			t = t.clamp(0, 1);
			return ((1 - t) * a) + (t * b);
		}
		public static float distance(this float from, float to)
		{
			return (to - from).pow(2).sqrt();
		}

		public static float pow(this float value, int p)
		{
			return Mathf.Pow(value, p);
		}
		public static float sqrt(this float value)
		{
			return Mathf.Sqrt(value);
		}
		public static float ceil(this float value)
		{
			return Mathf.Ceil(value);
		}
		public static int pow(this float value, float pow)
		{
			return Mathf.Pow(value, pow).round();
		}

		public static float log(this float value, int @base)
		{
			return Mathf.Log(value, @base);
		}
		public static float log2(this float value)
		{
			return Mathf.Log(value, 2).round();
		}
		public static float clamp(this float value, float min = float.MinValue, float max = float.MaxValue)
		{
			return value < min ? min : value > max ? max : value;
		}
		public static float floor(this float value, float target)
		{
			return (target * (value / target) - (value % target));
		}
		public static float toFloat(this double value)
		{
			return (value > float.MaxValue ? float.MaxValue : value < float.MinValue ? float.MinValue : (float)value);
		}
		public static float roundTo(this float value, float round)
		{
			if (round == 0) return value;

			return (round * (value / round) - (value % round) + (value % round == 0 ? 0 : round));
		}
		public static float floorTo(this float value, float round)
        {
            if (round == 0) return value;

            return (round * (value / round) + (value % round) + (value % round == 0 ? 0 : round));
        }
		public static int roundTo(this int value, int round)
		{
			if (round == 0) return value;

			return (round * (value / round) - (value % round) + (value % round == 0 ? 0 : round));
		}
		public static IEnumerable<double> GetDigits(this double source)
		{
			double individualFactor = 0;
			double tennerFactor = Convert.ToDouble(Math.Pow(10, source.ToString().Length));
			do
			{
				source -= tennerFactor * individualFactor;
				tennerFactor /= 10f;
				individualFactor = source / tennerFactor;

				yield return individualFactor;
			} while (tennerFactor > 1);

		}
	}
	public static class Vec2
	{

		public static Vector2 set(this Vector2 vector, float x = float.NaN, float y = float.NaN)
		{
			vector.x = !float.IsNaN(x) ? x : vector.x;
			vector.y = !float.IsNaN(y) ? y : vector.y;

			return vector;
		}
		public static Vector2 abs(this Vector2 value, bool x = true, bool y = true)
		{
			value.x = x ? Mathf.Abs(value.x) : value.x;
			value.y = y ? Mathf.Abs(value.y) : value.y;
			return value;
		}
		public static Vector2 neg(this Vector2 value, bool x = true, bool y = true)
		{
			value.x = x ? -Mathf.Abs(value.x) : value.x;
			value.y = y ? -Mathf.Abs(value.y) : value.y;
			return value;

		}
		public static Vector2 lerp(this Vector2 value, Vector2 target, float t)
		{
			return Vector2.Lerp(value, target, t);
		}
		public static Vector2 round(this Vector2 value, bool x = true, bool y = true)
		{
			value.x = x ? Mathf.Round(value.x) : value.x;
			value.y = y ? Mathf.Round(value.y) : value.y;
			return value;
		}
		public static Vector2 floor(this Vector2 value, bool x = true, bool y = true)
		{
			value.x = x ? Mathf.Floor(value.x) : value.x;
			value.y = y ? Mathf.Floor(value.y) : value.y;
			return value;
		}

	}
	public static class Vec3
	{
		public static Vector3 clamp(this Vector3 vector, Vector3 min, Vector3 max)
		{
			vector.x = vector.x < min.x ? min.x : vector.x > max.x ? max.x : vector.x;
			vector.y = vector.y < min.y ? min.y : vector.y > max.y ? max.y : vector.y;
			vector.z = vector.z < min.z ? min.z : vector.z > max.z ? max.z : vector.z;


			return vector;
		}
		public static Vector3 set(this Vector3 vector, float x = float.NaN, float y = float.NaN, float z = float.NaN)
		{
			vector.x = !float.IsNaN(x) ? x : vector.x;
			vector.y = !float.IsNaN(y) ? y : vector.y;
			vector.z = !float.IsNaN(z) ? z : vector.z;
			return vector;
		}
		public static Vector3 abs(this Vector3 value, bool x = true, bool y = true, bool z = true)
		{
			value.x = x ? Mathf.Abs(value.x) : value.x;
			value.y = y ? Mathf.Abs(value.y) : value.y;
			value.z = z ? Mathf.Abs(value.z) : value.z;
			return value;
		}
		public static Vector3 lerp(this Vector3 value, Vector3 target, float t)
		{
			return Vector3.Lerp(value, target, t);
		}
		public static Vector3 round(this Vector3 value, bool x = true, bool y = true, bool z = true)
		{

			//float oddX = 0, oddY = 0, oddZ = 0;

			//var sx = value.x.ToString();
			//var sy = value.y.ToString();
			//var sz = value.z.ToString();

			//         //if value is x.5, y.5, or z.5, we save odd and substract it later.
			//if(sx.Contains(".") && value.x - (value.x - 0.5f) >= 0.4999f){
			//	//int i = sx.IndexOf(".");
			//	//var ox = sx.Substring(i, sx.Length - i);
			//	//Single.TryParse(ox, out oddX);
			//	oddX = -0.5f;
			//}
			//if (sy.Contains(".") && value.y - (value.y - 0.5f) >= 0.4999f)
			//         {
			// //            int i = sx.IndexOf(".");
			// //            var ox = sx.Substring(i, sx.Length - i);
			//	//Single.TryParse(ox, out oddY);
			//	oddY = -0.5f;

			//}
			//if (sz.Contains(".") && value.y - (value.y - 0.5f) >= 0.4999f)
			//        {
			//oddZ = -0.5f;
			////            int i = sx.IndexOf(".");
			////            var ox = sx.Substring(i, sx.Length - i);
			////Single.TryParse(ox, out oddZ);
			//}


			//value.x = x ? value.x.floorTo(0.1f) : value.x; 
			//value.y = x ? value.y.floorTo(0.1f) : value.y; 
			//value.z = x ? value.x.floorTo(0.1f) : value.x; 

			value.x = x ? Mathf.Round(value.x) : value.x;
			value.y = y ? Mathf.Round(value.y) : value.y;
			value.z = z ? Mathf.Round(value.z) : value.z;

			//value.x += oddX;
			//value.y += oddY;
			//value.z += oddZ;

			return value;
		}
		public static Vector3 floor(this Vector3 value, bool x = true, bool y = true, bool z = true)
		{
			value.x = x ? Mathf.Floor(value.x) : value.x;
			value.y = y ? Mathf.Floor(value.y) : value.y;
			value.z = z ? Mathf.Floor(value.z) : value.z;
			return value;
		}


	}
	public static class String
	{
		public static string format(this string input, object result)
		{
			return string.Format(input, result);

		}
		public static string format(this string input, params object[] result)
		{
			return string.Format(input, result);

		}


		public static string color(this string stringInput, Color color)
		{
			byte maxBytes = 255;
			string hex = (color.r * maxBytes).round().toHex() + (color.g * maxBytes).round().toHex() + (color.b * maxBytes).round().toHex() + (color.a * maxBytes).round().toHex();
			return string.Format("<color=#{1}>{0}</color>", stringInput, hex);
		}
		public static string color(this string value, Color32 color)
		{
			string hex = color.r.toInt().toHex() + color.g.toInt().toHex() + color.b.toInt().toHex() + color.a.toInt().toHex();
			return string.Format("<color=#{1}>{0}</color>", value, hex);
		}
		public static string color(this string value, string color)
		{
			return string.Format("<color=#{1}>{0}</color>", value, color);
		}

		public static string toCurrency(this double value, params CurrencyFormatter[] formats)
		{
			int leng = formats.Length;

			value = Math.Round(value, 0, MidpointRounding.AwayFromZero);

			if(value < 1000)
			return value.ToString();

			var matches = formats.Where(f => f.maxValue > value);
			var minimum = matches.FirstOrDefault(m => m.maxValue == matches.Min(mc => mc.maxValue));

			var firstNumber = Math.Floor((value / (minimum.maxValue / 1000)));
			var odd = (value - (1000 * firstNumber));
			return firstNumber.ToString()  + "." + odd.ToString()  + minimum.output;
		}
	
		public static string toCurrency(this int value, params CurrencyFormatter[] formats)
		{
			int leng = formats.Length;

			value = Mathf.RoundToInt(value);

			if (value < 1000)
				return value.ToString();
			

			//value = Mathf.Round(value, 0, MidpointRounding.AwayFromZero);

			var matches = formats.Where(f => f.maxValue > value);
			var minimum = matches.FirstOrDefault(m => m.maxValue == matches.Min(mc => mc.maxValue));

			var firstNumber = Math.Floor((value / (minimum.maxValue / 1000)));
			var odd = (value - (1000 * firstNumber));
			return firstNumber.ToString()  + "." + odd.ToString()  + minimum.output;
		}
		//      public static string toHex (this Color color){
		//
		//
		//      }

		private static string getHexString(this int value, bool doubleZero = false)
		{
			var hexChar = "abcdef";
			if (doubleZero && value == 0)
				return "00";

			var result = value <= 9 ? "0" + value.ToString() : hexChar[(hexChar.Length - 1) - (15 - value)].ToString();
			return result;
		}
		public static string endline(this string value)
		{
			return value + "\n";
		}
		public static string toHex(this int value, string result = "")
		{
			if (value < 16)
			{
				result = result.insertBefore(value.getHexString(true));
				return result;
			}
			var odd = (value % 16f).round();

			result = result.insertBefore(odd.getHexString());
			var newValue = ((value - odd) / 16f).round();
			return toHex(newValue, result);
		}
		public static string insertBefore(this string value, string insertor)
		{
			return insertor + value;
		}
		public static string insertAfter(this string value, string insertor)
		{
			return value + insertor;
		}
		public static string insert(this string value, string before, string after)
		{
			return before + value + after;
		}

	}


}