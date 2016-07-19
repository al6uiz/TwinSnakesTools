using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace TwinSnakesTools.Data
{
    public class FontData
    {
        private const int COLUMS_PER_ROW = 20;

        public static Bitmap GenerateFontBitmap(char[] letters)
        {
            var fontName = "나눔바른고딕";
            var fontSize = 16f;
            var offsetX = -1;
            var offsetY = 1;


            var rowCount = (int)Math.Ceiling(letters.Length / 20.0);

            var bitmap = new Bitmap(24 * COLUMS_PER_ROW, 24 * rowCount);

            using (var font = new Font(fontName, fontSize))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Black);
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < COLUMS_PER_ROW; j++)
                        {
                            g.DrawLine(Pens.Red, j * 24, 0, j * 24, rowCount * 24);
                            g.DrawLine(Pens.Red, (j + 1) * 24 - 1, 0, (j + 1) * 24 - 1, rowCount * 24);
                        }

                        g.DrawLine(Pens.Red, 0, i * 24, COLUMS_PER_ROW * 24, i * 24);
                        g.DrawLine(Pens.Red, 0, (i + 1) * 24 - 1, COLUMS_PER_ROW * 24, (i + 1) * 24 - 1);
                    }

                    for (int i = 0; i < letters.Length; i++)
                    {
                        var row = i / COLUMS_PER_ROW;
                        var column = i % COLUMS_PER_ROW;

                        var x = column * 24;
                        var y = row * 24;

                        var letter = new string(letters[i], 1);
                        g.DrawString(letter, font, Brushes.White, x + offsetX, y + offsetY);
                    }
                }
            }

            return bitmap;
        }

        public unsafe static byte[] GetBytes(int count, Bitmap bitmap)
        {
            var result = new byte[count * 6 * 24];

            var bits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int* pBitmap = (int*)bits.Scan0;

            fixed (byte* pResult = result)
            {
                var destOffset = 0;

                for (int i = 0; i < count; i++)
                {
                    var column = i % COLUMS_PER_ROW;
                    var row = i / COLUMS_PER_ROW;

                    for (int y = 0; y < 24; y++)
                    {
                        var srcOffset = (row * 24 + y) * COLUMS_PER_ROW * 24 + column * 24;

                        for (int x = 0; x < 6; x++)
                        {
                            var value = (byte)(
                                GetBitValue(pBitmap, srcOffset++) << 6 |
                                GetBitValue(pBitmap, srcOffset++) << 4 |
                                GetBitValue(pBitmap, srcOffset++) << 2 |
                                GetBitValue(pBitmap, srcOffset++) << 0);

                            *(pResult + destOffset++) = value;
                        }
                    }

                }
            }
            bitmap.UnlockBits(bits);

            return result;

        }

        private static unsafe int GetBitValue(int* pBitmap, int offset)
        {
            return (*(pBitmap + offset) == -1 ? 0x3 : 0);
        }

        public unsafe static Bitmap GetBitmap(byte[] raw)
        {
            var count = (int)(raw.Length / (6.0 * 24));


            var rows = (int)Math.Ceiling((double)count / COLUMS_PER_ROW);

            var width = COLUMS_PER_ROW * 24;
            var height = rows * 24;
            var stride = 24 * width;

            var bitmap = new Bitmap(width, height);
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Black);
                }

                var bit = bitmap.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                var pData = (byte*)bit.Scan0;
                var offset = 0;

                for (int i = 0; i < count; i++)
                {
                    var x = i % COLUMS_PER_ROW;
                    var y = i / COLUMS_PER_ROW;

                    for (int j = 0; j < 24; j++)
                    {
                        var yOffset = y * stride + j * width;
                        var xOffset = x * 24;


                        Write((yOffset + xOffset + 0) * 4, pData, raw[offset++]);
                        Write((yOffset + xOffset + 4) * 4, pData, raw[offset++]);
                        Write((yOffset + xOffset + 8) * 4, pData, raw[offset++]);
                        Write((yOffset + xOffset + 12) * 4, pData, raw[offset++]);
                        Write((yOffset + xOffset + 16) * 4, pData, raw[offset++]);
                        Write((yOffset + xOffset + 20) * 4, pData, raw[offset++]);

                    }
                }

                bitmap.UnlockBits(bit);

            }

            return bitmap;
        }

        private unsafe static void Write(int i, byte* raw, byte value)
        {
            SetColor(raw, i, (value >> 6) & 0x3);
            SetColor(raw, i + 4, (value >> 4) & 0x3);
            SetColor(raw, i + 8, (value >> 2) & 0x3);
            SetColor(raw, i + 12, (value >> 0) & 0x3);
        }

        private unsafe static void SetColor(byte* raw, int i, int v)
        {
            if (v != 0)
            {
                *(raw + i + 0) = 0xFF;
                *(raw + i + 1) = 0xFF;
                *(raw + i + 2) = 0xFF;
                *(raw + i + 3) = 0xFF;
            }
        }
    }
}
