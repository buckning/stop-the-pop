using UnityEngine;
using System.Collections;

public class LocalisationManager : MonoBehaviour {

	public static void LoadLanguageFile() {
		SystemLanguage systemLanguage = Application.systemLanguage;

		if (systemLanguage == SystemLanguage.English) {
			LoadEnglishLanguageFile ();
		} else if (systemLanguage == SystemLanguage.French) {
			LoadFrenchLanguageFile ();
		} else if (systemLanguage == SystemLanguage.German) {
			LoadGermanLanguageFile ();
		} else if (systemLanguage == SystemLanguage.Italian) {
			LoadItalianLanguageFile ();
		} else if (systemLanguage == SystemLanguage.Spanish) {
			LoadSpanishLanguageFile ();
		} else if (systemLanguage == SystemLanguage.Portuguese) {
			LoadPortugueseLanguageFile ();
		} else if (systemLanguage == SystemLanguage.Russian) {
			LoadRussianLanguageFile ();
		} else {
			LoadEnglishLanguageFile ();
		}
	}

	private static void LoadEnglishLanguageFile() {
		LocalisedStrings englishStrings = new EnglishStrings();
		Strings.LoadLocalisedStrings (englishStrings);
	}

	private static void LoadFrenchLanguageFile() {
		LocalisedStrings frenchStrings = new FrenchStrings();
		Strings.LoadLocalisedStrings (frenchStrings);
	}

	private static void LoadGermanLanguageFile() {
		LocalisedStrings germanStrings = new GermanStrings();
		Strings.LoadLocalisedStrings (germanStrings);
	}

	private static void LoadItalianLanguageFile() {
		LocalisedStrings italianStrings = new ItalianStrings ();
		Strings.LoadLocalisedStrings (italianStrings);
	}

	private static void LoadSpanishLanguageFile() {
		LocalisedStrings spanishStrings = new SpanishStrings ();
		Strings.LoadLocalisedStrings (spanishStrings);
	}

	private static void LoadPortugueseLanguageFile() {
		LocalisedStrings portugueseStrings = new PortugueseStrings ();
		Strings.LoadLocalisedStrings (portugueseStrings);
	}

	private static void LoadRussianLanguageFile() {
		LocalisedStrings russianStrings = new RussianStrings ();
		Strings.LoadLocalisedStrings (russianStrings);
	}
}
