//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// シーンをまたいでもアクセス可能なParameterContainerを扱うクラス。
	/// </summary>
#else
	/// <summary>
	/// Class dealing with the accessible ParameterContainer even across the scene.
	/// </summary>
#endif
	[AddComponentMenu("")]
	public class GlobalParameterContainerInternal : ParameterContainerBase
	{
		static Dictionary<ParameterContainerInternal, ParameterContainerInternal> _Instancies = new Dictionary<ParameterContainerInternal, ParameterContainerInternal>();

#if UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void OnSubsystemRegistration()
		{
			_Instancies.Clear();
		}
#endif

		#region Serialize fields

		/// <summary>
		/// シーンを跨ぐ際に使用する共通のParameterContainerのプレハブを指定する。
		/// </summary>
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private ParameterContainerInternal _Prefab = null;

		#endregion // Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 元のParameterContainerを返す。
		/// </summary>
#else
		/// <summary>
		///It returns the original ParameterContainer.
		/// </summary>
#endif
		public ParameterContainerInternal prefab
		{
			get
			{
				return _Prefab;
			}
		}

		private ParameterContainerInternal _Instance;

#if ARBOR_DOC_JA
		/// <summary>
		/// 実体のParameterContainerを返す。
		/// </summary>
#else
		/// <summary>
		/// It returns the ParameterContainer entity.
		/// </summary>
#endif
		public ParameterContainerInternal instance
		{
			get
			{
				MakeInstance();
				return _Instance;
			}
		}

		void MakeInstance()
		{
			if (_Prefab != null && _Instance == null)
			{
				if (!_Instancies.TryGetValue(_Prefab, out _Instance))
				{
					_Instance = (ParameterContainerInternal)Instantiate(_Prefab);
					DontDestroyOnLoad(_Instance.gameObject);
					_Instancies.Add(_Prefab, _Instance);
				}
			}
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		void Awake()
		{
			MakeInstance();
		}
	}
}
