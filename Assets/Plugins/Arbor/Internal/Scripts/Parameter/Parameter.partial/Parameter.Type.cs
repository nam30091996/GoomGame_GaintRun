//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
namespace Arbor
{
	public sealed partial class Parameter
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータの型。
		/// </summary>
#else
		/// <summary>
		/// Parameter type.
		/// </summary>
#endif
		public enum Type
		{
#if ARBOR_DOC_JA
			/// <summary>
			/// Int型。
			/// </summary>
#else
			/// <summary>
			/// Int type.
			/// </summary>
#endif
			Int,

#if ARBOR_DOC_JA
			/// <summary>
			/// Float型。
			/// </summary>
#else
			/// <summary>
			/// Float type.
			/// </summary>
#endif
			Float,

#if ARBOR_DOC_JA
			/// <summary>
			/// Bool型。
			/// </summary>
#else
			/// <summary>
			/// Bool type.
			/// </summary>
#endif
			Bool,

#if ARBOR_DOC_JA
			/// <summary>
			/// GameObject型。
			/// </summary>
#else
			/// <summary>
			/// GameObject type.
			/// </summary>
#endif
			GameObject,

#if ARBOR_DOC_JA
			/// <summary>
			/// String型。
			/// </summary>
#else
			/// <summary>
			/// String type.
			/// </summary>
#endif
			String,

#if ARBOR_DOC_JA
			/// <summary>
			/// Enum型。
			/// </summary>
#else
			/// <summary>
			/// Enum type.
			/// </summary>
#endif
			Enum,

#if ARBOR_DOC_JA
			/// <summary>
			/// Vector2型。
			/// </summary>
#else
			/// <summary>
			/// Vector2 type.
			/// </summary>
#endif
			Vector2 = 1000,

#if ARBOR_DOC_JA
			/// <summary>
			/// Vector3型。
			/// </summary>
#else
			/// <summary>
			/// Vector3 type.
			/// </summary>
#endif
			Vector3,

#if ARBOR_DOC_JA
			/// <summary>
			/// Quaternion型。
			/// </summary>
#else
			/// <summary>
			/// Quaternion type.
			/// </summary>
#endif
			Quaternion,

#if ARBOR_DOC_JA
			/// <summary>
			/// Rect型。
			/// </summary>
#else
			/// <summary>
			/// Rect type.
			/// </summary>
#endif
			Rect,

#if ARBOR_DOC_JA
			/// <summary>
			/// Bounds型。
			/// </summary>
#else
			/// <summary>
			/// Bounds type.
			/// </summary>
#endif
			Bounds,

#if ARBOR_DOC_JA
			/// <summary>
			/// Color型。
			/// </summary>
#else
			/// <summary>
			/// Color type.
			/// </summary>
#endif
			Color,

#if ARBOR_DOC_JA
			/// <summary>
			/// Transform型。
			/// </summary>
#else
			/// <summary>
			/// Transform type.
			/// </summary>
#endif
			Transform = 2000,

#if ARBOR_DOC_JA
			/// <summary>
			/// RectTransform型。
			/// </summary>
#else
			/// <summary>
			/// RectTransform type.
			/// </summary>
#endif
			RectTransform,

#if ARBOR_DOC_JA
			/// <summary>
			/// Rigidbody型。
			/// </summary>
#else
			/// <summary>
			/// Rigidbody type.
			/// </summary>
#endif
			Rigidbody,

#if ARBOR_DOC_JA
			/// <summary>
			/// Rigidbody2D型。
			/// </summary>
#else
			/// <summary>
			/// Rigidbody2D type.
			/// </summary>
#endif
			Rigidbody2D,

#if ARBOR_DOC_JA
			/// <summary>
			/// Component型。
			/// </summary>
#else
			/// <summary>
			/// Component type.
			/// </summary>
#endif
			Component,

#if ARBOR_DOC_JA
			/// <summary>
			/// Long型。
			/// </summary>
#else
			/// <summary>
			/// Long type.
			/// </summary>
#endif
			Long,

#if ARBOR_DOC_JA
			/// <summary>
			/// Object型(Asset)。
			/// </summary>
#else
			/// <summary>
			/// Object type(Asset).
			/// </summary>
#endif
			AssetObject,

#if ARBOR_DOC_JA
			/// <summary>
			/// Variable型。
			/// </summary>
#else
			/// <summary>
			/// Variable type.
			/// </summary>
#endif
			Variable = 3000,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;int&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;int&gt; type.
			/// </summary>
#endif
			IntList = 4000,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;long&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;long&gt; type.
			/// </summary>
#endif
			LongList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;float&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;float&gt; type.
			/// </summary>
#endif
			FloatList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;bool&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;bool&gt; type.
			/// </summary>
#endif
			BoolList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;string&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;string&gt; type.
			/// </summary>
#endif
			StringList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Enum&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Enum&gt; type.
			/// </summary>
#endif
			EnumList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Vector2&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Vector2&gt; type.
			/// </summary>
#endif
			Vector2List = 5000,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Vector3&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Vector3&gt; type.
			/// </summary>
#endif
			Vector3List,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Quaternion&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Quaternion&gt; type.
			/// </summary>
#endif
			QuaternionList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Rect&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Rect&gt; type.
			/// </summary>
#endif
			RectList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Bounds&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Bounds&gt; type.
			/// </summary>
#endif
			BoundsList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Color&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Color&gt; type.
			/// </summary>
#endif
			ColorList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;GameObject&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;GameObject&gt; type.
			/// </summary>
#endif
			GameObjectList = 6000,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Component&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Component&gt; type.
			/// </summary>
#endif
			ComponentList,

#if ARBOR_DOC_JA
			/// <summary>
			/// List&lt;Object(Asset)&gt;型。
			/// </summary>
#else
			/// <summary>
			/// List&lt;Object(Asset)&gt; type.
			/// </summary>
#endif
			AssetObjectList,

#if ARBOR_DOC_JA
			/// <summary>
			/// VariableList型。
			/// </summary>
#else
			/// <summary>
			/// VariableList type.
			/// </summary>
#endif
			VariableList = 7000,
		}
	}
}