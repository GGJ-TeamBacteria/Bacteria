/**********************************************************************************
* Blueprint Reality Inc. CONFIDENTIAL
* 2017 Blueprint Reality Inc.
* All Rights Reserved.
*
* NOTICE:  All information contained herein is, and remains, the property of
* Blueprint Reality Inc. and its suppliers, if any.  The intellectual and
* technical concepts contained herein are proprietary to Blueprint Reality Inc.
* and its suppliers and may be covered by Patents, pending patents, and are
* protected by trade secret or copyright law.
*
* Dissemination of this information or reproduction of this material is strictly
* forbidden unless prior written permission is obtained from Blueprint Reality Inc.
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlueprintReality.Text {
	public static class Localization {
        public const string CSV_RES_PATH = "BPR_Localization";

        public const string DEFAULT_LANG = "english";

        public const string NOT_FOUND_STR_FORMAT = "#{0}#";

        private static string curLanguage;
        public static string CurrentLanguage
        {
            get { return curLanguage; }
            set
            {
                curLanguage = value;
                if (LanguageChanged != null)
                    LanguageChanged();
            }
        }
        public static event System.Action LanguageChanged;

        private static Dictionary<string, Dictionary<string,string>> languageTable;
        public static List<string> GetSupportedLanguages()
        {
            return new List<string>(languageTable.Keys);
        }
        
        static void Initialize()
        {
            TextAsset locAsset = Resources.Load<TextAsset>(CSV_RES_PATH);
            if (locAsset == null)
                Debug.Log("No localization file found at " + CSV_RES_PATH);

            ParseLocalizationData(locAsset.text);
        }

        static void ParseLocalizationData(string text)
        {
            if (languageTable == null)
                languageTable = new Dictionary<string, Dictionary<string, string>>();

            StringReader reader = new StringReader(text);

            string firstCell;
            List<string> languages;
            string readerLine = reader.ReadLine();
            if( !ParseLine(readerLine, out firstCell, out languages) )
            {
                Debug.LogError("Couldn't parse header of localization");
                return;
            }

            foreach (string language in languages)
                if( !languageTable.ContainsKey(language) )
                    languageTable[language] = new Dictionary<string, string>();

            int lineCount = 0;
            while (reader.Peek() > 0)
            {
                readerLine = reader.ReadLine();
                lineCount++;

                string key;
                List<string> vals;
                if (!ParseLine(readerLine, out key, out vals))
                {
                    Debug.LogError("Unmatched quotation marks in localization file on line " + lineCount);
                    continue;
                }

                int langIndex = 0;
                foreach (string lang in languageTable.Keys)
                    if( langIndex < vals.Count )
                        languageTable[lang][key] = vals[langIndex++];
            }
        }

        static bool ParseLine(string line, out string key, out List<string> vals)
        {
            List<int> commas = new List<int>();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ',' && inQuotes == false)
                    commas.Add(i);
                else if (line[i] == '"' && (i == 0 || line[i - 1] != '\\'))
                    inQuotes = !inQuotes;
            }
            
            if( inQuotes )
            {
                key = null;
                vals = null;
                return false;
            }

            key = line.Substring(0, commas[0]);
            key = key.Trim();
            vals = new List<string>();

            if (key.Length == 0)
                return true;
   
            for (int i = 0; i < commas.Count; i++)
            {
                int firstBreakIndex = commas[i];
                int secondBreakIndex;
                if (i < commas.Count - 1)
                    secondBreakIndex = commas[i + 1];
                else
                    secondBreakIndex = line.Length;

                string val = line.Substring(firstBreakIndex + 1, secondBreakIndex - firstBreakIndex - 1);
                val = val.Trim();

                if (val.Length > 0 && val.StartsWith("\"") && val.EndsWith("\""))
                    val = val.Substring(1, val.Length - 2);

                vals.Add(val);
            }

            return true;
        }

        public static string Get(string key)
        {
            if (languageTable == null)
                Initialize();

            string lang = CurrentLanguage;
            if (string.IsNullOrEmpty(lang))
                lang = DEFAULT_LANG;

            Dictionary<string,string> valTable = languageTable[lang];
            if (valTable.ContainsKey(key))
                return valTable[key];
            else
                return string.Format(NOT_FOUND_STR_FORMAT, key);
        }
	}
}