﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 更新に関する設定。
	/// </summary>
#else
	/// <summary>
	/// Settings related to updating.
	/// </summary>
#endif
	[System.Serializable]
	[Internal.Documentable]
	public sealed class UpdateSettings
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 更新タイプ。
		/// </summary>
#else
		/// <summary>
		/// Update type.
		/// </summary>
#endif
		public UpdateType type = UpdateType.EveryFrame;

#if ARBOR_DOC_JA
		/// <summary>
		/// 時間タイプ(SpecifySecondsのみ)。
		/// </summary>
#else
		/// <summary>
		/// Time type(only SpecifySeconds).
		/// </summary>
#endif
		public TimeType timeType = TimeType.Normal;

#if ARBOR_DOC_JA
		/// <summary>
		/// 更新間隔(SpecifySecondsのみ)。
		/// </summary>
#else
		/// <summary>
		/// Update interval(only SpecifySeconds).
		/// </summary>
#endif
		public float seconds = 0.1f;

		private float _UpdateTime = 0f;

#if ARBOR_DOC_JA
		/// <summary>
		/// Update()で更新できるかを返す
		/// </summary>
#else
		/// <summary>
		/// It returns whether it can be updated with Update()
		/// </summary>
#endif
		public bool isUpdatableOnUpdate
		{
			get
			{
				switch (type)
				{
					case UpdateType.EveryFrame:
						return true;
					case UpdateType.SpecifySeconds:
						float currentTime = TimeUtility.CurrentTime(timeType);
						if (currentTime - _UpdateTime >= seconds)
						{
							_UpdateTime = currentTime;
							return true;
						}
						break;
					case UpdateType.Manual:
						return false;
				}
				return false;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 更新時間をクリア
		/// </summary>
#else
		/// <summary>
		/// Clear update time
		/// </summary>
#endif
		public void ClearTime()
		{
			_UpdateTime = 0f;
		}
	}
}