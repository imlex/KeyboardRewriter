using System.Linq;
using System.Runtime.InteropServices;

namespace KeyboardRewriter.Interception
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStroke
    {
        public const ushort CommandsStartCode = ushort.MaxValue - 32;
        public const ushort SleepCode = CommandsStartCode;
        public const ushort ForwardCode = CommandsStartCode + 1;

        public const ushort CharacterClassesStartCode = CommandsStartCode + 16;
        public const ushort AnyClassCode = CharacterClassesStartCode;
        public const ushort DigitClassCode = CharacterClassesStartCode + 1;
        public const ushort LetterClassCode = CharacterClassesStartCode + 2;

        public ushort Code;
        public KeyState State;
        public uint Information;

        public static KeyStroke CreateSleepKeyStroke(uint sleepTime)
        {
            return new KeyStroke {Code = SleepCode, Information = sleepTime};
        }

        public static KeyStroke CreateForwardKeyStroke(uint forwardIndex, uint forwardLength)
        {
            return new KeyStroke {Code = ForwardCode, Information = forwardIndex | (forwardLength << 16)};
        }

        public bool Match(KeyStroke keyStroke)
        {
            if (State != keyStroke.State)
                return false;

            if (Code == keyStroke.Code)
                return true;

            if (Code == AnyClassCode)
            {
                Information = keyStroke.Code;
                return true;
            }

            if (Code == DigitClassCode &&
                (0x02 <= keyStroke.Code && keyStroke.Code <= 0x0B))
            {
                Information = keyStroke.Code;
                return true;
            }

            if (Code == LetterClassCode &&
                ((0x10 <= keyStroke.Code && keyStroke.Code <= 0x19) ||
                 (0x1E <= keyStroke.Code && keyStroke.Code <= 0x26) ||
                 (0x2C <= keyStroke.Code && keyStroke.Code <= 0x32)))
            {
                Information = keyStroke.Code;
                return true;
            }

            return false;
        }

        public KeyStroke[] BuildForwardKeyStrokes(KeyStroke[] receiveKeyStrokes)
        {
            var forwardIndex = Information & 0xFFFF;
            var forwardLength = Information >> 16;

            var keyStrokes = new KeyStroke[forwardLength];
            for (int i = 0; i < forwardLength; i++)
            {
                var receiveKeyStroke = receiveKeyStrokes[forwardIndex + i];

                keyStrokes[i] = receiveKeyStroke.Code >= CharacterClassesStartCode
                    ? new KeyStroke {Code = (ushort) receiveKeyStroke.Information, State = receiveKeyStroke.State}
                    : receiveKeyStroke;
            }

            return keyStrokes;
        }
    }
}