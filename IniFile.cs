using System;
using System.Collections.Generic;
using System.IO;

namespace IniFileReaderWriter
{
    public class IniFile
    {
        Dictionary<string, string> _items = new Dictionary<string, string>();
        string _iniFile = string.Empty;
        string[] _lines;
        char[] trimChars = { '[', ']' };

        /// <summary>
        /// IniFile(String path)  -- Constructor requires full path to ini file
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="Exception"></exception>
        public IniFile(String path)
        {
            _iniFile = path;
            string section = string.Empty;

            try
            {
                if (!File.Exists(_iniFile))
                {
                    throw new FileNotFoundException("Error file not found", _iniFile);
                }

                _lines = File.ReadAllLines(_iniFile);
                foreach (string line in _lines)
                {
                    if (line.Contains("[") && line.Contains("]"))
                    {
                        section = line.Trim(trimChars).ToLower();
                    }
                    else
                    {
                        if (line.Contains("="))
                        {
                            string[] parts = line.Split('=');
                            string key = section + parts[0].ToLower();
                            _items.Add(key, parts[1]);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }


        /// <summary>
        /// ReadValue(string section, string key, string defValue) -- reads a key value from a section
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public string ReadValue(string section, string key, string defValue)
        {
            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentNullException("section", "parameter is null or missing");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "parameter is null or missing");
            }

            if (string.IsNullOrEmpty(defValue))
                defValue = String.Empty;

            string retValue = string.Empty;
            string keyval = section.Trim(trimChars).ToLower() + key.ToLower();

            if (_items.TryGetValue(keyval, out retValue))
            {
                retValue = retValue.Trim();
            }
            else
            {
                retValue = defValue;
            }

            return retValue;
        }

        /// <summary>
        /// WriteValue(string section, string key, dynamic value) -- write the key value pair in the section
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteValue(string section, string key, string value)
        {
            if (string.IsNullOrEmpty(section))
            {
                throw new ArgumentNullException("section", "parameter is null or missing");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "parameter is null or missing");
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value", "parameter is null or missing");
            }

            List<string> items = new List<string>();
            bool bKeyFound = false;
            bool bFoundSection = false;
            string keyVal = section.Trim(trimChars).ToLower() + key.ToLower();
            string sec;

            if (section.Contains("[") && section.Contains("]"))
                sec = section.ToUpper();
            else
                sec = "[" + section.ToUpper() + "]";

            string[] lines = File.ReadAllLines(_iniFile);

            if (_items.ContainsKey(keyVal))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line.Equals(sec, StringComparison.OrdinalIgnoreCase))
                    {
                        bFoundSection = true;
                        continue;
                    }

                    if (bFoundSection)
                    {
                        if (line.Contains(key, StringComparison.OrdinalIgnoreCase))
                        {
                            bKeyFound = true;
                            lines[i] = key + "=" + value;
                            File.WriteAllLines(_iniFile, lines);
                            break;
                        }
                    }
                }

                if (!bFoundSection)
                {
                    items.AddRange(lines);
                    items.Add("\n\n##");
                    items.Add(sec);

                    if (!bKeyFound)
                    {
                        items.Add(key + "=" + value);
                        bKeyFound = true;
                    }
                }

                if (!bKeyFound)
                {
                    items.Add(key + "=" + value);
                    bKeyFound = true;
                }

                if (items.Count > 0)
                {
                    string[] iList = items.ToArray();
                    File.WriteAllLines(_iniFile, iList);
                }

            }
            else
            {
                items.AddRange(lines);
                items.Add(sec);
                items.Add(key + "=" + value);
                string[] iList = items.ToArray();
                File.WriteAllLines(_iniFile, iList);
            }

            _lines = File.ReadAllLines(_iniFile);
            _items.Clear();

            foreach (string line in _lines)
            {
                if (line.Contains("[") && line.Contains("]"))
                {
                    section = line.Trim(trimChars).ToLower();
                }
                else
                {
                    if (line.Contains("="))
                    {
                        string[] parts = line.Split('=');
                        string key1 = section + parts[0].ToLower();
                        _items.Add(key1, parts[1]);
                    }
                }
            }
        }
    }
}
