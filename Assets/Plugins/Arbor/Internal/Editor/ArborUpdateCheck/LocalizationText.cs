//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace ArborEditor.UpdateCheck
{
	[System.Reflection.Obfuscation(Exclude = true)]
	[System.Serializable]
	public sealed class LocalizationText
	{
		public string Ja = string.Empty;
		public string En = string.Empty;

		public string GetText()
		{
			if (ArborSettings.currnentLanguage == SystemLanguage.Japanese)
			{
				return Ja;
			}
			return En;
		}

		public override string ToString()
		{
			return string.Format("URL_JA : {0}\nURL_EN : {1}", Ja, En);
		}
	}
}