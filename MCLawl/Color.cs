/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl) Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.osedu.org/licenses/ECL-2.0
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
using System;

namespace MCLawl
{
    public static class c
    {
        public const string black = "&0";
        public const string navy = "&1";
        public const string green = "&2";
        public const string teal = "&3";
        public const string maroon = "&4";
        public const string purple = "&5";
        public const string gold = "&6";
        public const string silver = "&7";
        public const string gray = "&8";
        public const string blue = "&9";
        public const string lime = "&a";
        public const string aqua = "&b";
        public const string red = "&c";
        public const string pink = "&d";
        public const string yellow = "&e";
        public const string white = "&f";

        public static string Parse(string str)
        {
            switch (str.ToLower())
            {
                case "black": return black;
                case "navy": return navy;
                case "green": return green;
                case "teal": return teal;
                case "maroon": return maroon;
                case "purple": return purple;
                case "gold": return gold;
                case "silver": return silver;
                case "gray": return gray;
                case "blue": return blue;
                case "lime": return lime;
                case "aqua": return aqua;
                case "red": return red;
                case "pink": return pink;
                case "yellow": return yellow;
                case "white": return white;
                default: return "";
            }
        }
        public static string Name(string str)
        {
            switch (str)
            {
                case black: return "black";
                case navy: return "navy";
                case green: return "green";
                case teal: return "teal";
                case maroon: return "maroon";
                case purple: return "purple";
                case gold: return "gold";
                case silver: return "silver";
                case gray: return "gray";
                case blue: return "blue";
                case lime: return "lime";
                case aqua: return "aqua";
                case red: return "red";
                case pink: return "pink";
                case yellow: return "yellow";
                case white: return "white";
                default: return "";
            }
        }
        public static ColorDesc DefaultCol(char code)
        {
            switch (code)
            {
                case '0': return new ColorDesc('0', "Black");
                case '1': return new ColorDesc('1', "Navy");
                case '2': return new ColorDesc('2', "Green");
                case '3': return new ColorDesc('3', "Teal");
                case '4': return new ColorDesc('4', "Maroon");
                case '5': return new ColorDesc('5', "Purple");
                case '6': return new ColorDesc('6', "Gold");
                case '7': return new ColorDesc('7', "Silver");
                case '8': return new ColorDesc('8', "Gray");
                case '9': return new ColorDesc('9', "Blue");
                case 'a': return new ColorDesc('a', "Lime");
                case 'b': return new ColorDesc('b', "Aqua");
                case 'c': return new ColorDesc('c', "Red");
                case 'd': return new ColorDesc('d', "Pink");
                case 'e': return new ColorDesc('e', "Yellow");
                case 'f': return new ColorDesc('f', "White");
            }

            ColorDesc col = default(ColorDesc);
            col.Code = code;
            return col;
        }
        public struct ColorDesc
        {
            public char Code, Fallback;
            public byte R, G, B, A;
            public string Name;
            public bool Undefined { get { return Fallback == '\0'; } }
            public byte Index { get { return (byte)Code.UnicodeToCp437(); } }

            public ColorDesc(byte r, byte g, byte b)
            {
                Code = '\0'; Fallback = '\0'; Name = null;
                R = r; G = g; B = b; A = 255;
            }

            internal ColorDesc(char code, string name)
            {
                Code = code; Fallback = code; Name = name; A = 255;

                if (code >= '0' && code <= '9')
                {
                    HexDecode(code - '0', out R, out G, out B);
                }
                else
                {
                    HexDecode(code - 'a' + 10, out R, out G, out B);
                }
            }

            static void HexDecode(int hex, out byte r, out byte g, out byte b)
            {
                r = (byte)(191 * ((hex >> 2) & 1) + 64 * (hex >> 3));
                g = (byte)(191 * ((hex >> 1) & 1) + 64 * (hex >> 3));
                b = (byte)(191 * ((hex >> 0) & 1) + 64 * (hex >> 3));
            }

            /// <summary> Whether this colour has been modified from its default values. </summary>
            public bool IsModified()
            {
                if ((Code >= '0' && Code <= '9') || (Code >= 'a' && Code <= 'f'))
                {
                    ColorDesc def = c.DefaultCol(Code);
                    return R != def.R || G != def.G || B != def.B || Name != def.Name;
                }
                return !Undefined;
            }
        }
        public static ColorDesc[] List = new ColorDesc[256];
        public static bool IsDefined(char c)
        {
            if (c >= ' ' && c <= '~') return List[c].Fallback != '\0';
            return List[c.UnicodeToCp437()].Fallback != '\0';
        }
        public static char Lookup(char col)
        {
            // inlined as this must be fast for line wrapper
            if (col >= 'A' && col <= 'F') return (char)(col + ' ');
            if ((col >= '0' && col <= '9') || (col >= 'a' && col <= 'f')) return col;

            if (col == 'S') return Server.DefaultColor[1];
            return IsDefined(col) ? col : '\0';
        }
        static bool UsedColor(string message, int i)
        {
            // handle & being last character in string
            if (i >= message.Length - 1) return false;
            return Lookup(message[i + 1]) != '\0';
        }

        public static string StripUsed(string message)
        {
            if (message.IndexOf('%') == -1 && message.IndexOf('&') == -1) return message;
            char[] output = new char[message.Length];
            int usedChars = 0;

            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                if ((c == '%' || c == '&') && UsedColor(message, i))
                {
                    i++; // Skip over the following color code
                }
                else
                {
                    output[usedChars++] = c;
                }
            }
            return new string(output, 0, usedChars);
        }
    }
}