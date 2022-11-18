//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
	public static class WaypointTimeUtility
	{
		public static float CurrentTime(WaypointTimeType type)
		{
			switch (type)
			{
				case WaypointTimeType.Normal:
					return Time.time;
				case WaypointTimeType.Unscaled:
					return Time.unscaledTime;
				case WaypointTimeType.Realtime:
					return Time.realtimeSinceStartup;
				case WaypointTimeType.FixedTime:
					return Time.fixedTime;
			}

			return Time.time;
		}
	}
}