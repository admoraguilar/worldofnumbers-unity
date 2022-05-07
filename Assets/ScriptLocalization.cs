using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{

		public static string TitleText 		{ get{ return LocalizationManager.GetTranslation ("TitleText"); } }
	}

    public static class ScriptTerms
	{

		public const string TitleText = "TitleText";
	}
}