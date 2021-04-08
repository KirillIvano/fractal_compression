using System;
using System.Drawing;

namespace ConsoleApp12
{
    class Utils
    {
        public static float GetRangMetrik(int[][] r1, int[][] r2, int rangRate)
        {
            float acc = 0;

            for (int i = 0; i < rangRate; i++)
            {
                for (int j = 0; j < rangRate; j++)
                {
                    acc += (float) Math.Pow(r1[j][i] - r2[j][i], 2);
                }
            }

            return acc;
        }

        public static int[][] GenerateNoise(int size)
        {
            int[][] res = new int[size][];
            var rand = new Random();

            for (int i = 0; i < size; i++)
            {
                res[i] = new int[size];

                for (int j = 0; j < size; j++)
                {
                    res[i][j] = rand.Next(0, 255);
                }
            }

            return res;
        }

        public static Color GetGrayscaleColor(int val)
        {
            int validVal = Math.Max(0, Math.Min(255, val));

            return Color.FromArgb(validVal, validVal, validVal);
        }

        public static void TranformToGrayScale(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    var pix = bmp.GetPixel(i, j);

                    var lightness = (int)(pix.R * .2126 + pix.G * .7152 + pix.B * .0722);

                    bmp.SetPixel(i, j, GetGrayscaleColor(lightness));
                }
            }
        }

        public static int[][] ApplyTransforms(int[][] domain, int rotateAngle, float contrast, float brightness, int rangRate)
        {
            var rang = ReduceDomainPart(domain, rangRate);

            RotateRang(rang, rotateAngle, rangRate);

            MultiplyRang(rang, contrast, rangRate);
            AddToRang(rang, (int)brightness, rangRate);

            return rang;
        }

        // Перенос значений из ранговой области на итоговый битмап
        public static void ApplyRangPart(Bitmap bits, int[][] rang, int x, int y, int rangRate)
        {

            for (int i = 0; i < rangRate; i++)
            {
                for (int j = 0; j < rangRate; j++)
                {
                    var val = rang[j][i];
                    bits.SetPixel(j + x, i + y, GetGrayscaleColor(val));
                }
            }
        }

        // Выделение доменной части из битмапа
        public static int[][] GetBitmapPart(Bitmap bits, int x, int y, int size)
        {
            int[][] res = new int[size][];
            int i, j;

            for (i = 0; i < size; i++)
            {
                res[i] = new int[size];
            }

            for (i = 0; i < size; i++)
            {
                for (j = 0; j < size; j++)
                {
                    res[j][i] = bits.GetPixel(j + x, i + y).R;
                }
            }

            return res;
        }

        // Приведение доменной области к ранговой
        public static int[][] ReduceDomainPart(int[][] domain, int rangRate)
        {
            var res = new int[rangRate][];

            for (int i = 0; i < rangRate; i++)
            {
                res[i] = new int[rangRate];
            }

            Console.WriteLine(rangRate);
            Console.WriteLine(domain.Length);

            for (int i = 0; i < rangRate; i++)
            {
                for (int j = 0; j < rangRate; j++)
                {
                    res[j][i] = (int)((
                        domain[2 * i][2 * j] +
                        domain[2 * i + 1][2 * j] +
                        domain[2 * i][2 * j + 1] +
                        domain[2 * i + 1][2 * j + 1]
                        ) / 4
                   );
                }
            }

            return res;
        }

        // Получение среднего значения по произвольной матрице
        public static float GetAverage(int[][] matrix)
        {
            int yl = matrix.Length;
            if (yl == 0)
            {
                return 0;
            }

            int xl = matrix[0].Length;
            int acc = 0;

            for (int i = 0; i < yl; i++)
            {
                for (int j = 0; j < xl; j++)
                {
                    acc += matrix[i][j];
                }
            }

            var avg = acc / (xl * yl);

            return avg;
        }

        // Умножение всех элементов на значение
        public static void MultiplyRang(int[][] mat, float c, int rangRate)
        {
            for (int i = 0; i < rangRate; i++)
            {
                for (int j = 0; j < rangRate; j++)
                {
                    mat[j][i] = (int)(mat[j][i] * c);
                }
            }
        }

        public static int[][] CopyRang(int[][] src, int rangRate)
        {
            var res = new int[rangRate][];

            for (int i = 0; i < rangRate; i++)
            {
                res[i] = new int[rangRate];
            }

            for (int i = 0; i < rangRate; i++)
            {
                for (int j = 0; j < rangRate; j++)
                {
                    res[i][j] = src[i][j];
                }
            }

            return res;
        }

        // Добавление значения ко всем элементам
        public static void AddToRang(int[][] mat, int val, int rangRate)
        {
            for (int i = 0; i < rangRate; i++)
            {
                for (int j = 0; j < rangRate; j++)
                {
                    mat[j][i] += val;
                }
            }
        }

        // Вращение против часовой стрелки без вспомогательных массивов
        public static void RotateRang(int[][] rang, int angle, int rangRate)
        {
            for (int curr = angle; curr > 0; curr -= 90)
            {
                for (int i = 0; i < rangRate / 2; i++)
                {
                    for (int j = i; j < rangRate - i - 1; j++)
                    {
                        int temp = rang[i][j];
                        rang[i][j] = rang[rangRate - 1 - j][i] + 3;
                        rang[rangRate - 1 - j][i] = rang[rangRate - 1 - i][rangRate - 1 - j];
                        rang[rangRate - 1 - i][rangRate - 1 - j] = rang[j][rangRate - 1 - i];
                        rang[j][rangRate - 1 - i] = temp;
                    }
                }
            }
        }
    }
}
