//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
#if !ARBOR_DLL && UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

#if ARBOR_DLL
using System;
using System.Reflection;
#endif

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Profiler.BeginSample / EndSampleを管理するDisposable ヘルパークラス。
	/// <para>usingを使用して簡略してProfiler.BeginSample / EndSampleを記述できます。</para>
	/// </summary>
#else
	/// <summary>
	/// Disposable helper class that manages the Profiler.BeginSample / EndSample.
	/// <para>Simple to use the using You can write Profiler.BeginSample / EndSample.</para>
	/// </summary>
#endif
	public struct ProfilerScope : System.IDisposable
	{
#if ARBOR_DLL
		delegate void DelegateBeginSample( string name );
		delegate void DelegateEndSample();

		static readonly DelegateBeginSample s_BeginSample;
		static readonly DelegateEndSample s_EndSample;

		static ProfilerScope()
		{
			Assembly assembly = Assembly.Load( "UnityEngine.dll" );
			Type type = assembly.GetType( "UnityEngine.Profiling.Profiler" );
			if( type == null )
			{
				type = assembly.GetType( "UnityEngine.Profiler" );
			}

			if( type != null )
			{
				MethodInfo beginSampleMethod = type.GetMethod( "BeginSample", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof( string ) }, null );
				s_BeginSample = (DelegateBeginSample)Delegate.CreateDelegate( typeof(DelegateBeginSample), beginSampleMethod );

				MethodInfo endSampleMethod = type.GetMethod( "EndSample", BindingFlags.Static | BindingFlags.Public );
				s_EndSample = (DelegateEndSample)Delegate.CreateDelegate( typeof(DelegateEndSample), endSampleMethod );
			}
		}

		static void BeginSample( string name )
		{
			if( s_BeginSample != null )
			{
				s_BeginSample( name );
			}
		}

		static void EndSample()
		{
			if( s_EndSample != null )
			{
				s_EndSample();
			}
		}
#else
		private static void BeginSample(string name)
		{
			Profiler.BeginSample(name);
		}

		private static void EndSample()
		{
			Profiler.EndSample();
		}
#endif

		private bool _Disposed;

#if NOSTALGIA_DOC_JA
		/// <summary>
		/// 新しいProfilerScopeを作成し、プロファイラのサンプリングを開始します。
		/// </summary>
		/// <param name="name">サンプリングの名前</param>
#else
		/// <summary>
		/// Create a new ProfilerScope, to start the sampling of the profiler.
		/// </summary>
		/// <param name="name">The name of the sampling</param>
#endif
		public ProfilerScope(string name)
		{
			_Disposed = false;
			BeginSample(name);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 廃棄する。
		/// </summary>
#else
		/// <summary>
		/// Dispose.
		/// </summary>
#endif
		public void Dispose()
		{
			if (_Disposed)
			{
				return;
			}

			_Disposed = true;
			EndSample();
		}
	}
}
