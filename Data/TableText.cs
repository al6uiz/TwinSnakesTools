using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinSnakesTools.Data
{
    class TextTable
    {
        private static Dictionary<int, char> _table = new Dictionary<int, char>();

        public static Dictionary<int, char> Empty { get; private set; } = new Dictionary<int, char>();
        public static Dictionary<char, int> _tableReverse = new Dictionary<char, int>();


        static TextTable()
        {
            //_table[0x1F01] = 'Ä';
            //_table[0x1F02] = 'Å';
            //_table[0x1F03] = 'Ç';
            //_table[0x1F04] = 'É';
            //_table[0x1F05] = 'Ñ';
            //_table[0x1F06] = 'Ö';
            //_table[0x1F07] = 'Ü';
            //_table[0x1F08] = 'á';
            //_table[0x1F09] = 'à';
            //_table[0x1F0A] = 'â';
            //_table[0x1F0B] = 'ä';
            //_table[0x1F0C] = 'ã';
            //_table[0x1F0D] = 'å';
            //_table[0x1F0E] = 'ç';
            //_table[0x1F0F] = 'é';
            //_table[0x1F10] = 'è';
            //_table[0x1F11] = 'ê';
            //_table[0x1F12] = 'ë';
            //_table[0x1F13] = 'í';
            //_table[0x1F14] = 'ì';
            //_table[0x1F15] = 'î';
            //_table[0x1F16] = 'ï';
            //_table[0x1F17] = 'ñ';
            //_table[0x1F18] = 'ó';
            //_table[0x1F19] = 'ò';
            //_table[0x1F1A] = 'ô';
            //_table[0x1F1B] = 'ö';
            //_table[0x1F1C] = 'õ';
            //_table[0x1F1D] = 'ú';
            //_table[0x1F1E] = 'ù';
            //_table[0x1F1F] = 'û';
            //_table[0x1F20] = 'ü';
            //_table[0x1F21] = '†';
            //_table[0x1F22] = '°';
            //_table[0x1F23] = '¢';
            //_table[0x1F24] = '£';
            //_table[0x1F25] = '§';
            //_table[0x1F26] = '•';
            //_table[0x1F27] = '¶';
            //_table[0x1F28] = 'ß';
            //_table[0x1F29] = '®';
            //_table[0x1F2A] = '©';
            //_table[0x1F2B] = '™';
            //_table[0x1F2C] = '´';
            //_table[0x1F2D] = '¨';
            //_table[0x1F2E] = '≠';
            //_table[0x1F2F] = 'Æ';
            //_table[0x1F30] = 'Ø';
            //_table[0x1F31] = '∞';
            //_table[0x1F32] = '±';
            //_table[0x1F33] = '≤';
            //_table[0x1F34] = '≥';
            //_table[0x1F35] = '¥';
            //_table[0x1F36] = 'µ';
            //_table[0x1F37] = '∂';
            //_table[0x1F38] = '∑';
            //_table[0x1F39] = '∏';
            //_table[0x1F3A] = 'π';
            //_table[0x1F3B] = '∫';
            //_table[0x1F3C] = 'ª';
            //_table[0x1F3D] = 'º';
            //_table[0x1F3E] = 'Ω';
            //_table[0x1F3F] = 'æ';
            //_table[0x1F40] = 'ø';
            //_table[0x1F41] = '¿';
            //_table[0x1F42] = '¡';
            //_table[0x1F43] = '¬';
            //_table[0x1F44] = '√';
            //_table[0x1F45] = 'ƒ';
            //_table[0x1F46] = '≈';
            //_table[0x1F47] = '∆';
            //_table[0x1F48] = '«';
            //_table[0x1F49] = '»';
            //_table[0x1F4A] = '…';
            //_table[0x1F4B] = 'n';
            //_table[0x1F4C] = 'À';
            //_table[0x1F4D] = 'Ã';
            //_table[0x1F4E] = 'Õ';
            //_table[0x1F4F] = 'Œ';
            //_table[0x1F50] = 'œ';
            //_table[0x1F51] = '–';
            //_table[0x1F52] = '—';
            //_table[0x1F53] = '“';
            //_table[0x1F54] = '”';
            //_table[0x1F55] = '‘';
            //_table[0x1F56] = '’';
            //_table[0x1F57] = '÷';
            //_table[0x1F58] = '◊';
            //_table[0x1F59] = 'ÿ';
            //_table[0x1F5A] = 'Ÿ';
            //_table[0x1F5B] = '⁄';
            //_table[0x1F5C] = '€';
            //_table[0x1F5D] = '‹';
            //_table[0x1F5E] = '›';
            //_table[0x1F5F] = 'ﬁ';
            //_table[0x1F60] = 'ﬂ';
            //_table[0x1F61] = '‡';
            //_table[0x1F62] = '·';
            //_table[0x1F63] = '‚';
            //_table[0x1F64] = '„';
            //_table[0x1F65] = '‰';
            //_table[0x1F66] = 'Â';
            //_table[0x1F67] = 'Ê';
            //_table[0x1F68] = 'Á';
            //_table[0x1F69] = 'Ë';
            //_table[0x1F6A] = 'È';
            //_table[0x1F6B] = 'Í';
            //_table[0x1F6C] = 'Î';
            //_table[0x1F6D] = 'Ï';
            //_table[0x1F6E] = 'Ì';
            //_table[0x1F6F] = 'Ó';
            //_table[0x1F70] = 'Ô';
            //_table[0x1F72] = 'Ò';
            //_table[0x1F73] = 'Ú';
            //_table[0x1F74] = 'Û';
            //_table[0x1F75] = 'Ù';
            //_table[0x1F76] = 'ı';
            //_table[0x1F77] = 'ˆ';
            //_table[0x1F78] = '˜';
            //_table[0x1F79] = '¯';
            //_table[0x1F7A] = '˘';
            //_table[0x1F7B] = '˙';
            //_table[0x1F7C] = '˚';
            //_table[0x1F7D] = '¸';
            //_table[0x1F7E] = '˝';

            _table[0xC101] = 'ぁ';
            _table[0x8102] = 'あ';
            _table[0xC103] = 'ぃ';
            _table[0x8104] = 'い';
            _table[0xC105] = 'ぅ';
            _table[0x8106] = 'う';
            _table[0xC107] = 'ぇ';
            _table[0x8108] = 'え';
            _table[0xC109] = 'ぉ';
            _table[0x810A] = 'お';
            _table[0x810B] = 'か';
            _table[0x810C] = 'が';
            _table[0x810D] = 'き';
            _table[0x810E] = 'ぎ';
            _table[0x810F] = 'く';
            _table[0x8110] = 'ぐ';
            _table[0x8111] = 'け';
            _table[0x8112] = 'げ';
            _table[0x8113] = 'こ';
            _table[0x8114] = 'ご';
            _table[0x8115] = 'さ';
            _table[0x8116] = 'ざ';
            _table[0x8117] = 'し';
            _table[0x8118] = 'じ';
            _table[0x8119] = 'す';
            _table[0x811A] = 'ず';
            _table[0x811B] = 'せ';
            _table[0x811C] = 'ぜ';
            _table[0x811D] = 'そ';
            _table[0x811E] = 'ぞ';
            _table[0x811F] = 'た';
            _table[0x8120] = 'だ';
            _table[0x8121] = 'ち';
            _table[0x8122] = 'ぢ';
            _table[0xC123] = 'っ';
            _table[0x8124] = 'つ';
            _table[0x8125] = 'づ';
            _table[0x8126] = 'て';
            _table[0x8127] = 'で';
            _table[0x8128] = 'と';
            _table[0x8129] = 'ど';
            _table[0x812A] = 'な';
            _table[0x812B] = 'に';
            _table[0x812C] = 'ぬ';
            _table[0x812D] = 'ね';
            _table[0x812E] = 'の';
            _table[0x812F] = 'は';
            _table[0x8130] = 'ば';
            _table[0x8131] = 'ぱ';
            _table[0x8132] = 'ひ';
            _table[0x8133] = 'び';
            _table[0x8134] = 'ぴ';
            _table[0x8135] = 'ふ';
            _table[0x8136] = 'ぶ';
            _table[0x8137] = 'ぷ';
            _table[0x8138] = 'へ';
            _table[0x8139] = 'べ';
            _table[0x813A] = 'ぺ';
            _table[0x813B] = 'ほ';
            _table[0x813C] = 'ぼ';
            _table[0x813D] = 'ぽ';
            _table[0x813E] = 'ま';
            _table[0x813F] = 'み';
            _table[0x8140] = 'む';
            _table[0x8141] = 'め';
            _table[0x8142] = 'も';
            _table[0xC143] = 'ゃ';
            _table[0x8144] = 'や';
            _table[0xC145] = 'ゅ';
            _table[0x8146] = 'ゆ';
            _table[0xC147] = 'ょ';
            _table[0x8148] = 'よ';
            _table[0x8149] = 'ら';
            _table[0x814A] = 'り';
            _table[0x814B] = 'る';
            _table[0x814C] = 'れ';
            _table[0x814D] = 'ろ';
            _table[0x814E] = 'ゎ';
            _table[0x814F] = 'わ';
            _table[0x8150] = 'を';
            _table[0x8151] = 'ん';

            _table[0xC201] = 'ァ';
            _table[0x8202] = 'ア';
            _table[0xC203] = 'ィ';
            _table[0x8204] = 'イ';
            _table[0xC205] = 'ゥ';
            _table[0x8206] = 'ウ';
            _table[0xC207] = 'ェ';
            _table[0x8208] = 'エ';
            _table[0xC209] = 'ォ';
            _table[0x820A] = 'オ';
            _table[0x820B] = 'カ';
            _table[0x820C] = 'ガ';
            _table[0x820D] = 'キ';
            _table[0x820E] = 'ギ';
            _table[0x820F] = 'ク';
            _table[0x8210] = 'グ';
            _table[0x8211] = 'ケ';
            _table[0x8212] = 'ゲ';
            _table[0x8213] = 'コ';
            _table[0x8214] = 'ゴ';
            _table[0x8215] = 'サ';
            _table[0x8216] = 'ザ';
            _table[0x8217] = 'シ';
            _table[0x8218] = 'ジ';
            _table[0x8219] = 'ス';
            _table[0x821A] = 'ズ';
            _table[0x821B] = 'セ';
            _table[0x821C] = 'ゼ';
            _table[0x821D] = 'ソ';
            _table[0x821E] = 'ゾ';
            _table[0x821F] = 'タ';
            _table[0x8220] = 'ダ';
            _table[0x8221] = 'チ';
            _table[0x8222] = 'ヂ';
            _table[0xC223] = 'ッ';
            _table[0x8224] = 'ツ';
            _table[0x8225] = 'ヅ';
            _table[0x8226] = 'テ';
            _table[0x8227] = 'デ';
            _table[0x8228] = 'ト';
            _table[0x8229] = 'ド';
            _table[0x822A] = 'ナ';
            _table[0x822B] = 'ニ';
            _table[0x822C] = 'ヌ';
            _table[0x822D] = 'ネ';
            _table[0x822E] = 'ノ';
            _table[0x822F] = 'ハ';
            _table[0x8230] = 'バ';
            _table[0x8231] = 'パ';
            _table[0x8232] = 'ヒ';
            _table[0x8233] = 'ビ';
            _table[0x8234] = 'ピ';
            _table[0x8235] = 'フ';
            _table[0x8236] = 'ブ';
            _table[0x8237] = 'プ';
            _table[0x8238] = 'ヘ';
            _table[0x8239] = 'ベ';
            _table[0x823A] = 'ペ';
            _table[0x823B] = 'ホ';
            _table[0x823C] = 'ボ';
            _table[0x823D] = 'ポ';
            _table[0x823E] = 'マ';
            _table[0x823F] = 'ミ';
            _table[0x8240] = 'ム';
            _table[0x8241] = 'メ';
            _table[0x8242] = 'モ';
            _table[0xC243] = 'ャ';
            _table[0x8244] = 'ヤ';
            _table[0xC245] = 'ュ';
            _table[0x8246] = 'ユ';
            _table[0xC247] = 'ョ';
            _table[0x8248] = 'ヨ';
            _table[0x8249] = 'ラ';
            _table[0x824A] = 'リ';
            _table[0x824B] = 'ル';
            _table[0x824C] = 'レ';
            _table[0x824D] = 'ロ';
            _table[0xC24E] = 'ヮ';
            _table[0x824F] = 'ワ';
            _table[0x8250] = 'ヲ';
            _table[0x8251] = 'ン';
            _table[0x8252] = 'ヴ';
            _table[0x8253] = 'ヵ';
            _table[0x8254] = 'ヶ';

            _table[0x8301] = '　';
            _table[0x8302] = '△';
            _table[0x8303] = '□';
            _table[0x8304] = '×';

            _table[0xC306] = '！';
            _table[0xC307] = '？';
            _table[0xC308] = '、';
            _table[0xC309] = '。';
            _table[0xC30A] = '」';
            _table[0xC30B] = '』';
            _table[0xC30C] = '］';
            _table[0xC30D] = '〉';
            _table[0xC30E] = '》';
            _table[0xC30F] = '】';
            _table[0xC300] = '”';
            _table[0xC311] = '’';
            _table[0x8312] = '‐';
            _table[0x8313] = '―';
            _table[0x8314] = '…';
            _table[0x8315] = '‥';

            _table[0xA316] = '「';
            _table[0xA317] = '『';
            _table[0xA318] = '《';
            _table[0xA319] = '［';
            _table[0xA31A] = '【';
            _table[0xA31B] = '“';
            _table[0xA31C] = '‘';


            //_table[0xC02E] = '.';


            foreach (var item in _table)
            {
                if (!_tableReverse.ContainsKey(item.Value))
                {
                    _tableReverse.Add(item.Value, item.Key);
                }
                else
                {
                    Debug.WriteLine($"{item.Key} {item.Value}");
                }
            }
        }

        internal static byte[] GetBytes(string text, bool useDouble)
        {
            var buffer = new List<byte>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\\')
                {
                    var letter = text.Substring(i + 2, 4);
                    var value = ushort.Parse(letter, System.Globalization.NumberStyles.HexNumber);
                    buffer.Add((byte)(value >> 8));
                    buffer.Add((byte)(value & 0xFF));
                    i += 5;
                }
                else if (text[i] == '[')
                {
                    if (text[i + 1] == 'B' && text[i + 2] == ']')
                    {
                        buffer.Add(0xA0);
                        buffer.Add(0x7B);
                        i += 2;
                    }
                    else if (text[i + 1] == 'N' && text[i + 2] == ']')
                    {
                        buffer.Add(0x0A);
                        i += 2;
                    }
                    else if (text[i + 1] == '/' && text[i + 2] == 'B' && text[i + 3] == ']')
                    {
                        buffer.Add(0xC0);
                        buffer.Add(0x7D);
                        i += 3;
                    }
                    else if (text[i + 1] == 'R' && text[i + 2] == 'N' && text[i + 3] == ']')
                    {
                        buffer.Add(0x0D);
                        buffer.Add(0x0A);
                        i += 3;
                    }
                    else if (text[i + 1] == 'N' && text[i + 2] == 'L' && text[i + 3] == ']')
                    {
                        buffer.Add(0x80);
                        buffer.Add(0x7C);
                        i += 3;
                    }
                    //else if (text[i + 1] == 'E' && text[i + 2] == 'S' && text[i + 3] == ']')
                    //{
                    //    buffer.Add(0x2E);
                    //    i += 3;
                    //}
                    else
                    {

                    }
                }
                else
                {
                    var letter = text[i];
                    if (useDouble)
                    {
                        //if (letter >= '0' && letter <= '9' || letter >= 'A' && letter <= 'Z' || letter >= 'a' && letter <= 'z')
                        //{
                        //    buffer.Add((byte)0x80);
                        //    buffer.Add((byte)letter);
                        //    continue;
                        //}

                        if (letter < 0x80 && letter > 0x20)
                        {
                            buffer.Add((byte)0x80);
                            buffer.Add((byte)letter);
                            continue;

                        }
                    }

                    int value;
                    if (_tableReverse.TryGetValue(letter, out value))
                    {
                        buffer.Add((byte)(value >> 8));
                        buffer.Add((byte)(value & 0xFF));
                    }
                    else
                    {
                        if (letter < 0x80)
                        {
                            buffer.Add((byte)letter);
                        }
                        else
                        {

                        }
                    }
                }
            }

            buffer.Add(0);

            return buffer.ToArray();

        }

        public static string GetString(byte[] data)
        {
            return GetString(data, Empty);
        }

        public static string GetString(byte[] data, Dictionary<int, char> externalTable)
        {
            var buffer = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                //if (data[i] == 0x2E)
                //{
                //    buffer.Append("[ES]");
                //}                else
                if (data[i] < 0x20 || data[i] > 0x7e)
                {
                    if (data[i] == 0x0D && data[i + 1] == 0x0A)
                    {
                        buffer.Append("[RN]");
                        i++;
                    }
                    else if (data[i] == 0x0A)
                    {
                        buffer.Append("[N]");
                    }
                    else if (data[i] == 0)
                    {
                        break;
                    }
                    else
                    {
                        int value = data[i];
                        if (i + 1 < data.Length)
                        {
                            value = (value << 8) | data[++i];
                        }
                        else { }

                        var first = (value & 0xFF00) >> 8;

                        if (first == 0x80)
                        {
                            if (value == 0x807C)
                            {
                                buffer.Append("[NL]");
                            }
                            else
                            {
                                buffer.Append((char)(value & 0xFF));
                            }
                        }
                        else
                        {
                            var table = _table;

                            if ((first & 0x8C) == 0x8c)
                            {
                                table = externalTable;
                            }


                            char letter;
                            if (table.TryGetValue(value, out letter))
                            {
                                buffer.Append(letter);
                            }
                            else
                            {
                                var text = $"\\x{value:X4}";

                                if ((first & 0x8C) != 0x8c && (first & 0x9C) != 0x90)
                                {
                                    switch (value)
                                    {
                                        case 0xA07B: text = "[B]"; break;
                                        case 0xC07D: text = "[/B]"; break;

                                        default:
                                            Debug.WriteLine(text);
                                            break;
                                    }

                                    buffer.Append(text);

                                }
                                else
                                {
                                    buffer.Append(text);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var letter = (char)data[i];

                    buffer.Append(letter);
                }
            }

            return buffer.ToString();
        }


        public static Dictionary<int, char> ToTable(string letters)
        {
            var result = new Dictionary<int, char> { };
            var key = 0x8c01;

            foreach (var item in letters.Where(x => x > '~').ToArray())
            {
                if ((key & 0xFF) == 0) key++;

                result[key] = item;

                key++;
            }

            return result;
        }
    }

}
