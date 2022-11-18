//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace ArborEditor
{
	using Arbor;
	using Events;

	[System.Reflection.Obfuscation(Exclude = true)]
	[FilePathAttribute("Library/ArborEditorCache.asset", FilePathAttribute.Location.ProjectFolder)]
	public sealed class ArborEditorCache : EditorSettings<ArborEditorCache>
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _BehaviourSearch = string.Empty;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _CalculatorSearch = string.Empty;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _ActionBehaviourSearch = string.Empty;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _CompositeBehaviourSearch = string.Empty;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _DecoratorSearch = string.Empty;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _ServiceSearch = string.Empty;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _TypeSearch = string.Empty;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private TypeFilterFlags _TypeFilterFlags = (TypeFilterFlags)(-1);

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private TypeFilterFlags _TypeFilterMask = (TypeFilterFlags)(-1);

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _TypeFilterEnable = false;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _MethodSearch = string.Empty;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private MemberFilterFlags _MemberFilterFlags = (MemberFilterFlags)(-1);

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private MemberFilterFlags _MemberFilterMask = (MemberFilterFlags)(-1);

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _MemberFilterEnable = false;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _CaptureDirectory = "";

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _WelcomeWindowDontDisplayNextVersion = false;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private string _WelcomeWindowOpenedVersion = "";

		public static string behaviourSearch
		{
			get
			{
				return instance._BehaviourSearch;
			}
			set
			{
				if (instance._BehaviourSearch != value)
				{
					instance._BehaviourSearch = value;

					Save();
				}
			}
		}

		public static string calculatorSearch
		{
			get
			{
				return instance._CalculatorSearch;
			}
			set
			{
				if (instance._CalculatorSearch != value)
				{
					instance._CalculatorSearch = value;

					Save();
				}
			}
		}

		public static string actionBehaviourSearch
		{
			get
			{
				return instance._ActionBehaviourSearch;
			}
			set
			{
				if (instance._ActionBehaviourSearch != value)
				{
					instance._ActionBehaviourSearch = value;

					Save();
				}
			}
		}

		public static string compositeBehaviourSearch
		{
			get
			{
				return instance._CompositeBehaviourSearch;
			}
			set
			{
				if (instance._CompositeBehaviourSearch != value)
				{
					instance._CompositeBehaviourSearch = value;

					Save();
				}
			}
		}

		public static string decoratorSearch
		{
			get
			{
				return instance._DecoratorSearch;
			}
			set
			{
				if (instance._DecoratorSearch != value)
				{
					instance._DecoratorSearch = value;

					Save();
				}
			}
		}

		public static string serviceSearch
		{
			get
			{
				return instance._ServiceSearch;
			}
			set
			{
				if (instance._ServiceSearch != value)
				{
					instance._ServiceSearch = value;

					Save();
				}
			}
		}

		public static string typeSearch
		{
			get
			{
				return instance._TypeSearch;
			}
			set
			{
				if (instance._TypeSearch != value)
				{
					instance._TypeSearch = value;

					Save();
				}
			}
		}

		public static TypeFilterFlags typeFilterMask
		{
			get
			{
				return instance._TypeFilterMask;
			}
			set
			{
				if (instance._TypeFilterMask != value)
				{
					instance._TypeFilterMask = value;

					Save();
				}
			}
		}

		public static TypeFilterFlags typeFilterFlags
		{
			get
			{
				return instance._TypeFilterFlags;
			}
			set
			{
				if (instance._TypeFilterFlags != value)
				{
					instance._TypeFilterFlags = value;

					Save();
				}
			}
		}

		public static bool typeFilterEnable
		{
			get
			{
				return instance._TypeFilterEnable;
			}
			set
			{
				if (instance._TypeFilterEnable != value)
				{
					instance._TypeFilterEnable = value;

					Save();
				}
			}
		}

		public static string methodSearch
		{
			get
			{
				return instance._MethodSearch;
			}
			set
			{
				if (instance._MethodSearch != value)
				{
					instance._MethodSearch = value;

					Save();
				}
			}
		}

		public static MemberFilterFlags memberFilterMask
		{
			get
			{
				return instance._MemberFilterMask;
			}
			set
			{
				if (instance._MemberFilterMask != value)
				{
					instance._MemberFilterMask = value;

					Save();
				}
			}
		}

		public static MemberFilterFlags memberFilterFlags
		{
			get
			{
				return instance._MemberFilterFlags;
			}
			set
			{
				if (instance._MemberFilterFlags != value)
				{
					instance._MemberFilterFlags = value;

					Save();
				}
			}
		}

		public static bool memberFilterEnable
		{
			get
			{
				return instance._MemberFilterEnable;
			}
			set
			{
				if (instance._MemberFilterEnable != value)
				{
					instance._MemberFilterEnable = value;

					Save();
				}
			}
		}

		public static string captureDirectory
		{
			get
			{
				return instance._CaptureDirectory;
			}
			set
			{
				if (instance._CaptureDirectory != value)
				{
					instance._CaptureDirectory = value;

					Save();
				}
			}
		}

		public static bool welcomeWindowDontDisplayNextVersion
		{
			get
			{
				return instance._WelcomeWindowDontDisplayNextVersion;
			}
			set
			{
				if (instance._WelcomeWindowDontDisplayNextVersion != value)
				{
					instance._WelcomeWindowDontDisplayNextVersion = value;

					Save();
				}
			}
		}

		public static string welcomeWindowOpenedVersion
		{
			get
			{
				return instance._WelcomeWindowOpenedVersion;
			}
			set
			{
				if (instance._WelcomeWindowOpenedVersion != value)
				{
					instance._WelcomeWindowOpenedVersion = value;

					Save();
				}
			}
		}
	}
}