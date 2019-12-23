using System.IO;
using System.Text;
using UnityEngine;

namespace Lunar.Utils
{
    public class ColorTable
    {
        public Color32[] colors;
        public int size;

        public void Save(string fileName)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("GIMP Palette");
            s.AppendLine("Name: Lunar");
            s.AppendLine("Columns: 7");            
            s.AppendLine("#");

            for (int i=0; i<colors.Length; i++)
            {
                s.AppendFormat("{0} {1} {2} ({0}, {1}, {2})\n", colors[i].b, colors[i].g, colors[i].b);
            }

            File.WriteAllText(fileName, s.ToString());
        }

        public void LoadFromTexture(Texture2D source)
        {
            this.size = source.width * source.height;
            this.colors = source.GetPixels32();
        }

        public void LoadFromFile(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            int.TryParse(lines[2], out this.size);

            this.colors = new Color32[this.size];

            for (int i = 0; i < this.size; i++)
            {
                int n = 3 + i;
                if (n>=lines.Length)
                {
                    break;
                }

                int r, g, b;
                string[] s = lines[n].Split(new char[] { ' ' });

                int.TryParse(s[0], out r);
                int.TryParse(s[1], out g);
                int.TryParse(s[2], out b);

                colors[i] = new Color32((byte)r, (byte)g, (byte)b, 255);
            }
        }
         

        public Color32[] ApplyToTexture(Texture2D target, DitherMode mode)
        {
            var src_pixels = target.GetPixels32();
            var out_pixels = new Color32[src_pixels.Length];
            for (int y = 0; y < target.height; y++)
            {
                for (int x = 0; x < target.width; x++)
                {
                    int ofs = x + y * target.width;
                    Color32 c = src_pixels[ofs];
                    if (c.a<=0)
                    {
                        out_pixels[ofs] = Color.clear;
                        continue;
                    }


                    byte alpha = c.a;
                    int First, Second;
                    GetNearestDitherIndices(c, out First, out Second);

                    Color32 A = colors[First];
                    Color32 B = colors[Second];

                    float Val = ColorFindInterpolation(c, A, B);
                    var ditherVal = DitherUtils.ColorDither(mode, x, y, Val);

                    c = ditherVal  ? B : A;

                    Val = (float)alpha / 255.0f;
                    ditherVal = DitherUtils.ColorDither(mode, x, y, Val);
                    alpha = (byte)(ditherVal ? 255 : 0);

                    //Current := ColorGrey(255 * DitherVal);
                    c = new Color32(c.b, c.g, c.b, alpha);
                    out_pixels[ofs] = c;
                }
            }
            return out_pixels;
        }

        public int[] ApplyToTextureAsIndices(Texture2D target, DitherMode mode)
        {
            var src_pixels = target.GetPixels32();
            var out_indices = new int[src_pixels.Length];
            for (int y = 0; y < target.height; y++)
            {
                for (int x = 0; x < target.width; x++)
                {
                    int ofs = x + y * target.width;
                    Color32 c = src_pixels[ofs];
                    if (c.a <= 0)
                    {
                        out_indices[ofs] = -1;
                        continue;
                    }


                    byte alpha = c.a;
                    int First, Second;
                    GetNearestDitherIndices(c, out First, out Second);

                    Color32 A = colors[First];
                    Color32 B = colors[Second];

                    if (A.Equals(B))
                    {
                        out_indices[ofs] = First;
                    }
                    else
                    {
                        float Val = ColorFindInterpolation(c, A, B);
                        var ditherVal = DitherUtils.ColorDither(mode, x, y, Val);

                        out_indices[ofs] = (ditherVal ? Second : First);
                    }
                }
            }
            return out_indices;
        }


        public Color32 GetNearestPaletteColor(Color32 c)
        {
            int index = GetNearestPaletteIndex(c);
            return GetColorByIndex(index);
        }

        public int GetNearestPaletteIndex(Color32 c)
        {
            int Result = -1;

            int Best = 999999;

            for (int i = 0; i < size; i++)
            {
                Color Current = colors[i];
                int Dist = ColorDistance(Current, c);
                if (Dist < Best)
                {
                    Best = Dist;
                    Result = i;
                }
            }
            return Result;                 
        }

        public void GetNearestDitherIndices(Color32 c, out int First, out int Second)
        {
            First = -1;

            int Best = 999999;
            for (int i= 0; i<size; i++)
            {
                Color32 Current = colors[i];
                int Dist = ColorDistance(Current, c);
                if (Dist < Best)
                {
                    Best = Dist;
                    First = i;
                }
            }

            Second = First;

            if (Best == 0)
            {
                return;
            }

            Best = 999999;
            for (int i = 0; i<size; i++)
            {
                if (i == First)
                {
                    continue;
                }

                Color Current = colors[i];
                int Dist = ColorDistance(Current, c);
                if (Dist < Best)
                {
                    Best = Dist;
                    Second = i;
                }
            }
        }

        public int ColorDistance(Color32 A, Color32 B)
        {
            int DR = A.r - B.r;
            int DG = A.g - B.g;
            int DB = A.b - B.b;

            return ((DR*DR) + (DG*DG) + (DB* DB));
        }

        private float ColorFindInterpolation(Color32 val, Color32 a, Color32 b)
        {
            int DistA = ColorDistance(val, a);
            int DistB = ColorDistance(val, b);

            if (DistA == DistB || DistA == 0)
            {
                return 0;
            }

            if (DistB == 0)
            {
                return 1;
            }

            if (DistA < DistB)
            {
                //  A--C------B
                return  (float)DistA / (float)(DistA + DistB);
            }

            //  A------C--B
            return 0.5f + (DistB / (DistA + DistB));
        }


        public Color32 GetColorByIndex(int index)
        {
            if (index >= 0 && index < size)
            {
                return colors[index];
            }

            return new Color32(0, 0, 0, 0);
        }

    }


}
