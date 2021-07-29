/*Copyright 2015 Esri
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.?*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ESRI.ArcGIS.JTX.EnvVariableParser
{
    [Guid("1ED985C7-81BB-48e7-BAF7-A6E506C50FFB")]
    [ProgId("JTXSamples.JTXEnvVariableParser")]
    public class JTXSamples: IJTXTokenParser2
    {
        #region IJTXTokenParser2 Members

        public bool CanTranslateToken(string token)
        {
            bool results = false;
            token = token.Substring(1, token.Length - 2).ToUpper();
            string[] parts = token.Split(new char[] {':'},2);
            // Make sure it is a Environment token (starts with "ENV:")
            if (parts.Length == 2)
            {
                if (parts[0].Equals("ENV"))
                {
                    // Try to get the env variable
                    string value = Environment.GetEnvironmentVariable(parts[1]);
                    if (value != null)
                        results = true;
                }
            }

            return results;
        }

        public string Caption
        {
            get { return "Environment Variable Parser"; }
        }

        public string[] GetSupportedTokens()
        {

            System.Collections.ICollection keys = Environment.GetEnvironmentVariables().Keys;
            List<string> tokens = new List<string>();
            foreach (object o in keys)
            {
                if (o is string)
                {
                    string s = o as string;
                    tokens.Add("[ENV:" + s + "]");

                }
            }
            return tokens.ToArray();
        }

        public void GetSupportedTokens2(IJTXDatabase pDB, out string[] supportedTokens, out string[] tokenDescriptions)
        {
            System.Collections.ICollection keys = Environment.GetEnvironmentVariables().Keys;
            List<string> tokens = new List<string>();
            List<string> descriptions = new List<string>();
            
            // Copy each key to the list of tokens, and make it a token
            foreach (object o in keys)
            {
                if (o is string)
                {
                    string s = o as string;
                    tokens.Add("[ENV:" + s + "]");
                    
                }
            }
            // Sort the tokens
            tokens.Sort();

            // Get the descriptions for each token
            foreach (string s in tokens)
            {
                string value = null;

                value = Environment.GetEnvironmentVariable(s.Substring(5,s.Length-6));

                if (value != null)
                    descriptions.Add("Current Value: " + value);
                else
                    descriptions.Add("Current Value: [NOT SET]");
            }

            supportedTokens = tokens.ToArray();
            tokenDescriptions = descriptions.ToArray();
        }

        public string Name
        {
            get { return "Environment Variable Parser"; }
        }

        public string Parse(string sourceText, IJTXDatabase pDB, IJTXJob pJob, ESRI.ArcGIS.esriSystem.IPropertySet pOverrides)
        {
            // Find the token
            string strSource = sourceText;
            string strFinal = sourceText;
            int curpos = strSource.IndexOf('[');
            int startmarker;
            int endmarker = -1;
            int nestedcount;
            int curtokenpos;

            string strToken;
            string strNewValue;
            while (curpos >= 0)
            {
                nestedcount = 1;
                curtokenpos = curpos + 1;
                strToken = "";
                //need to find the whole token (including any nested tokens)
                while (nestedcount > 0)
                {
                    startmarker = strSource.IndexOf('[', curtokenpos);
                    endmarker = strSource.IndexOf(']', curtokenpos);

                    if (startmarker >= 0 && startmarker < endmarker)
                    {
                        nestedcount++;
                        curtokenpos = startmarker + 1;
                    }
                    else if (endmarker < 0)
                    {
                        break;
                    }
                    else
                    {
                        nestedcount--;
                        curtokenpos = endmarker + 1;
                    }

                }
                if (endmarker >= 0)
                {
                    strToken = strSource.Substring(curpos, endmarker - curpos + 1);
                    strNewValue = ReplaceToken(strToken, pDB, pJob, pOverrides);
                    strFinal = strFinal.Replace(strToken, strNewValue);
                }

                curpos = strSource.IndexOf('[', curpos + 1);
            }

            return strFinal;
        }

        #endregion

        private string ReplaceToken(string token, IJTXDatabase pDB, IJTXJob pJob, ESRI.ArcGIS.esriSystem.IPropertySet pOverrides)
        {
            string tokenout = token;
            // Drop the [] and convert to uppercase
            token = token.Substring(1,token.Length - 2).ToUpper();

            // Split the prefix from the actual token
            string[] parts = token.Split(new char[] { ':' }, 2);
            // make sure it is a environment token (starts with "ENV:")
            if (parts.Length == 2)
            {
                if (parts[0].Equals("ENV"))
                {
                    // try to get the env variable
                    string value = Environment.GetEnvironmentVariable(parts[1]);
                    if (value != null)
                    {
                        tokenout = value;
                    }
                }
            }
            
            return tokenout;
        }
    }
}
