using System;
using System.Drawing;

namespace ConsoleApp12
{
    class Utils
    {
        public const int RANG_RATE = 30;
        public const int DOMAIN_RATE = RANG_RATE * 2;

        public static double GetRangMetrik(int[][] r1, int[][] r2)
        {
            double acc = 0;

            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    acc += Math.Pow(r1[j][i] - r2[j][i], 2);
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

        public static int[][] ApplyTransforms(int[][] domain, int rotateAngle, double contrast, double brightness)
        {
            var rang = ReduceDomainPart(domain);

            RotateRang(rang, rotateAngle);

            MultiplyRang(rang, contrast);
            AddToRang(rang, (int)brightness);

            return rang;
        }

        // Перенос значений из ранговой области на итоговый битмап
        public static void ApplyRangPart(Bitmap bits, int[][] rang, int x, int y)
        {

            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
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
        public static int[][] ReduceDomainPart(int[][] domain)
        {
            var res = new int[RANG_RATE][];

            for (int i = 0; i < RANG_RATE; i++)
            {
                res[i] = new int[RANG_RATE];
            }

            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
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
        public static double GetAverage(int[][] matrix)
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
        public static void MultiplyRang(int[][] mat, double c)
        {
            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    mat[j][i] = (int)(mat[j][i] * c);
                }
            }
        }

        public static int[][] CopyRang(int[][] src)
        {
            var res = new int[RANG_RATE][];

            for (int i = 0; i < RANG_RATE; i++)
            {
                res[i] = new int[RANG_RATE];
            }

            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    res[i][j] = src[i][j];
                }
            }

            return res;
        }

        // Добавление значения ко всем элементам
        public static void AddToRang(int[][] mat, int val)
        {
            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    mat[j][i] += val;
                }
            }
        }

        // Вращение против часовой стрелки без вспомогательных массивов
        public static void RotateRang(int[][] rang, int angle)
        {
            for (int curr = angle; curr > 0; curr -= 90)
            {
                for (int i = 0; i < RANG_RATE / 2; i++)
                {
                    for (int j = i; j < RANG_RATE - i - 1; j++)
                    {
                        int temp = rang[i][j];
                        rang[i][j] = rang[RANG_RATE - 1 - j][i] + 3;
                        rang[RANG_RATE - 1 - j][i] = rang[RANG_RATE - 1 - i][RANG_RATE - 1 - j];
                        rang[RANG_RATE - 1 - i][RANG_RATE - 1 - j] = rang[j][RANG_RATE - 1 - i];
                        rang[j][RANG_RATE - 1 - i] = temp;
                    }
                }
            }
        }
    }
}
