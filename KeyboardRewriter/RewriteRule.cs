using System;
using System.Collections.Generic;
using System.Linq;
using KeyboardRewriter.Interception;

namespace KeyboardRewriter
{
    public class RewriteRule
    {
        public RewriteRule(string receiveKeys, string sendKeys)
        {
            ReceiveKeyStrokes = GenerateKeyStrokes(receiveKeys).ToArray();
            SendKeyStrokes = GenerateKeyStrokes(sendKeys).ToArray();
        }

        public KeyStroke[] ReceiveKeyStrokes { get; }

        public int ReceivePosition { get; set; }

        public KeyStroke[] SendKeyStrokes { get; }

        private static IEnumerable<KeyStroke> GenerateKeyStrokes(string keys)
        {
            var i = 0;
            while (i < keys.Length)
            {
                var currentChar = keys[i++];

                if (currentChar == '{')
                {
                    if (i == keys.Length)
                        throw new ArgumentException("Keys '" + keys + "' format is invalid.");

                    if (keys[i] != '{')
                    {
                        var j = keys.IndexOf('}', i);
                        if (j == -1)
                            throw new ArgumentException("Keys '" + keys + "' format is invalid.");

                        foreach (var keyStroke in GenerateKeyStrokesForShortCut(keys.Substring(i, j - i)))
                            yield return keyStroke;

                        i = j + 1;
                        continue;
                    }

                    i++;
                }
                else if (currentChar == '}')
                {
                    if (i < keys.Length && keys[i] == '}')
                        i++;
                    else
                        throw new ArgumentException("Keys '" + keys + "' format is invalid.");
                }

                foreach (var keyStroke in GenerateKeyStrokes(currentChar))
                    yield return keyStroke;
            }
        }

        private static IEnumerable<KeyStroke> GenerateKeyStrokes(char c)
        {
            var code = GetCharCode(c, out var shift);

            if (shift)
                yield return new KeyStroke {Code = 0x2A, State = KeyState.Down};

            yield return new KeyStroke {Code = code, State = KeyState.Down};
            yield return new KeyStroke {Code = code, State = KeyState.Up};

            if (shift)
                yield return new KeyStroke {Code = 0x2A, State = KeyState.Up};
        }

        private static readonly char[] __shortCutSeparators = {' ', '+'};

        private static IEnumerable<KeyStroke> GenerateKeyStrokesForShortCut(string shortCut)
        {
            var keys = shortCut.ToLower().Split(__shortCutSeparators, StringSplitOptions.RemoveEmptyEntries);

            var keyStrokes = new KeyStroke[2 * keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];

                var shift = false;
                var code = key.Length == 1 ? GetCharCode(key[0], out shift) : GetKeyCode(key);

                if (shift)
                    throw new ArgumentException("Shortcut '" + shortCut + "' format is invalid.");

                keyStrokes[i] = new KeyStroke {Code = code, State = KeyState.Down};

                keyStrokes[2 * keys.Length - i - 1] = new KeyStroke {Code = code, State = KeyState.Up};
            }

            return keyStrokes;
        }

        private static ushort GetCharCode(char c, out bool shift)
        {
            ushort code;

            switch (c)
            {
// @formatter:off — disable formatter after this line
                case ' ': shift = true; code = 0x39; break;
                case '!': shift = true; code = 0x02; break;
                case '"': shift = true; code = 0x28; break;
                case '#': shift = true; code = 0x04; break;
                case '$': shift = true; code = 0x05; break;
                case '%': shift = true; code = 0x06; break;
                case '&': shift = true; code = 0x08; break;
                case '\'': shift = false; code = 0x28; break;
                case '(': shift = true; code = 0x0A; break;
                case ')': shift = true; code = 0x0B; break;
                case '*': shift = true; code = 0x09; break;
                case '+': shift = true; code = 0x0D; break;
                case ',': shift = false; code = 0x33; break;
                case '-': shift = false; code = 0x0C; break;
                case '.': shift = false; code = 0x34; break;
                case '/': shift = false; code = 0x35; break;
                case '0': shift = false; code = 0x0B; break;
                case '1': shift = false; code = 0x02; break;
                case '2': shift = false; code = 0x03; break;
                case '3': shift = false; code = 0x04; break;
                case '4': shift = false; code = 0x05; break;
                case '5': shift = false; code = 0x06; break;
                case '6': shift = false; code = 0x07; break;
                case '7': shift = false; code = 0x08; break;
                case '8': shift = false; code = 0x09; break;
                case '9': shift = false; code = 0x0A; break;
                case ':': shift = true; code = 0x27; break;
                case ';': shift = false; code = 0x27; break;
                case '<': shift = true; code = 0x33; break;
                case '=': shift = false; code = 0x0D; break;
                case '>': shift = true; code = 0x34; break;
                case '?': shift = true; code = 0x35; break;
                case '@': shift = true; code = 0x03; break;
                case 'A': shift = true; code = 0x1E; break;
                case 'B': shift = true; code = 0x30; break;
                case 'C': shift = true; code = 0x2E; break;
                case 'D': shift = true; code = 0x20; break;
                case 'E': shift = true; code = 0x12; break;
                case 'F': shift = true; code = 0x21; break;
                case 'G': shift = true; code = 0x22; break;
                case 'H': shift = true; code = 0x23; break;
                case 'I': shift = true; code = 0x17; break;
                case 'J': shift = true; code = 0x24; break;
                case 'K': shift = true; code = 0x25; break;
                case 'L': shift = true; code = 0x26; break;
                case 'M': shift = true; code = 0x32; break;
                case 'N': shift = true; code = 0x31; break;
                case 'O': shift = true; code = 0x18; break;
                case 'P': shift = true; code = 0x19; break;
                case 'Q': shift = true; code = 0x10; break;
                case 'R': shift = true; code = 0x13; break;
                case 'S': shift = true; code = 0x1F; break;
                case 'T': shift = true; code = 0x14; break;
                case 'U': shift = true; code = 0x16; break;
                case 'V': shift = true; code = 0x2F; break;
                case 'W': shift = true; code = 0x11; break;
                case 'X': shift = true; code = 0x2D; break;
                case 'Y': shift = true; code = 0x15; break;
                case 'Z': shift = true; code = 0x2C; break;
                case '[': shift = false; code = 0x1A; break;
                case '\\': shift = false; code = 0x2B; break;
                case ']': shift = false; code = 0x1B; break;
                case '^': shift = true; code = 0x07; break;
                case '_': shift = true; code = 0x0C; break;
                case '`': shift = false; code = 0x29; break;
                case 'a': shift = false; code = 0x1E; break;
                case 'b': shift = false; code = 0x30; break;
                case 'c': shift = false; code = 0x2E; break;
                case 'd': shift = false; code = 0x20; break;
                case 'e': shift = false; code = 0x12; break;
                case 'f': shift = false; code = 0x21; break;
                case 'g': shift = false; code = 0x22; break;
                case 'h': shift = false; code = 0x23; break;
                case 'i': shift = false; code = 0x17; break;
                case 'j': shift = false; code = 0x24; break;
                case 'k': shift = false; code = 0x25; break;
                case 'l': shift = false; code = 0x26; break;
                case 'm': shift = false; code = 0x32; break;
                case 'n': shift = false; code = 0x31; break;
                case 'o': shift = false; code = 0x18; break;
                case 'p': shift = false; code = 0x19; break;
                case 'q': shift = false; code = 0x10; break;
                case 'r': shift = false; code = 0x13; break;
                case 's': shift = false; code = 0x1F; break;
                case 't': shift = false; code = 0x14; break;
                case 'u': shift = false; code = 0x16; break;
                case 'v': shift = false; code = 0x2F; break;
                case 'w': shift = false; code = 0x11; break;
                case 'x': shift = false; code = 0x2D; break;
                case 'y': shift = false; code = 0x15; break;
                case 'z': shift = false; code = 0x2C; break;
                case '{': shift = true; code = 0x1A; break;
                case '|': shift = true; code = 0x2B; break;
                case '}': shift = true; code = 0x1B; break;
                case '~': shift = true; code = 0x29; break;
// @formatter:on — enable formatter after this line
                default:
                    throw new ArgumentException("Char '" + c + "' is not supported! Yet?");
            }

            return code;
        }

        private static ushort GetKeyCode(string key)
        {
            ushort code;

            switch (key)
            {
// @formatter:off — disable formatter after this line
                case "escape": code = 0x01; break;
                case "f1": code = 0x3B; break;
                case "f2": code = 0x3C; break;
                case "f3": code = 0x3D; break;
                case "f4": code = 0x3E; break;
                case "f5": code = 0x3F; break;
                case "f6": code = 0x40; break;
                case "f7": code = 0x41; break;
                case "f8": code = 0x42; break;
                case "f9": code = 0x43; break;
                case "f10": code = 0x44; break;
                case "f11": code = 0xD9; break;
                case "f12": code = 0xDA; break;

                case "backspace": code = 0x0E; break;
                case "tab": code = 0x0F; break;
                case "enter": code = 0x1C; break;

                case "alt": code = 0x38; break;
                case "ctrl": code = 0x1D; break;
                case "shift": code = 0x2A; break;
// @formatter:on — enable formatter after this line
                default:
                    throw new ArgumentException("Key '" + key + "' is not supported! Yet?");
            }

            return code;
        }
    }
}