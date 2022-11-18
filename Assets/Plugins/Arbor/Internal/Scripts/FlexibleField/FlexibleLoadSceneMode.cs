//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine.SceneManagement;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なLoadSceneMode型を扱うクラス。
	/// </summary>
#else
	/// <summary>
	/// Class to handle a flexible LoadSceneMode type reference method there is more than one.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class FlexibleLoadSceneMode : FlexibleField<LoadSceneMode>
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleLoadSceneModeデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleLoadSceneMode default constructor
		/// </summary>
#endif
		public FlexibleLoadSceneMode()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleLoadSceneModeコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleLoadSceneMode constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleLoadSceneMode(LoadSceneMode value) : base(value)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleLoadSceneModeコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleLoadSceneMode constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleLoadSceneMode(AnyParameterReference parameter) : base(parameter)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleLoadSceneModeコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleLoadSceneMode constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleLoadSceneMode(InputSlotAny slot) : base(slot)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleLoadSceneModeをLoadSceneModeにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleLoadSceneMode</param>
#else
		/// <summary>
		/// Cast FlexibleLoadSceneMode to LoadSceneMode.
		/// </summary>
		/// <param name="flexible">FlexibleLoadSceneMode</param>
#endif
		public static explicit operator LoadSceneMode(FlexibleLoadSceneMode flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// LoadSceneModeをFlexibleLoadSceneModeにキャスト。
		/// </summary>
		/// <param name="value">LoadSceneMode</param>
#else
		/// <summary>
		/// Cast LoadSceneMode to FlexibleLoadSceneMode.
		/// </summary>
		/// <param name="value">LoadSceneMode</param>
#endif
		public static explicit operator FlexibleLoadSceneMode(LoadSceneMode value)
		{
			return new FlexibleLoadSceneMode(value);
		}
	}
}