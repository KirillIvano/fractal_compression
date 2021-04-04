using System;
using System.Collections.Generic;
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
                    mat[j][i] = (int) (mat[j][i] * c);
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

    class Transformation
    {
        public int x;
        public int y;

        public int angle;
        public int[][] result;
        public double avg;

        public Transformation(int x, int y, int angle, int[][] result, double avg)
        {
            this.x = x;
            this.y = y;

            this.angle = angle;
            this.result = result;

            this.avg = avg;
        }
    }

    class MemTransformation
    {
        public int x;
        public int y;
        public double c;
        public int h;

        public int angle;

        public MemTransformation(int x, int y, int angle, double c, int h)
        {
            this.x = x;
            this.y = y;

            this.angle = angle;
            this.h = h;
            this.c = c;
        }
    }

    class Encoder
    {
        private Bitmap bmp;

        public Encoder(Bitmap bmp)
        {
            this.bmp = bmp;
        }

        private int[][] ApplyTransforms(int[][] domain, int rotateAngle, double contrast, double brightness)
        {
            var rang = Utils.ReduceDomainPart(domain);

            Utils.RotateRang(rang, rotateAngle);

            Utils.MultiplyRang(rang, contrast);
            Utils.AddToRang(rang, (int) brightness);

            return rang;
        }

        private List<Transformation> GenerateTransformations()
        {
            var w = bmp.Width / Utils.DOMAIN_RATE;
            var h = bmp.Height / Utils.DOMAIN_RATE;

            var transformations = new List<Transformation>();

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var domain = Utils.GetBitmapPart(bmp, j * Utils.DOMAIN_RATE, i * Utils.DOMAIN_RATE, Utils.DOMAIN_RATE);
                    
                    for (var k = 0; k < 6; k++)
                    {
                        var res = ApplyTransforms(domain, k * 90, 1, 0);

                        transformations.Add(new Transformation(
                            j,
                            i,
                            k * 90,
                            res,
                            Utils.GetAverage(res)
                        ));
                    }
                }
            }

            return transformations;
        }

        public void Encode()
        {
            var w = bmp.Width / Utils.RANG_RATE;
            var h = bmp.Height / Utils.RANG_RATE;

            var transformations = GenerateTransformations();

            var transformationsMatrix = new MemTransformation[h][];

            for (int i = 0; i < h; i++)
            {
                transformationsMatrix[i] = new MemTransformation[w];

                for (int j = 0; j < w; j++)
                {
                    double min = int.MaxValue;
                    var rang = Utils.GetBitmapPart(
                        bmp,
                        Utils.RANG_RATE * j,
                        Utils.RANG_RATE * i,
                        Utils.RANG_RATE
                    );

                    var rangAverage = Utils.GetAverage(rang);

                    foreach (var trans in transformations)
                    {
                        double coeff = rangAverage / trans.avg;

                        int[][] approximated = Utils.CopyRang(trans.result);
                        Utils.MultiplyRang(approximated, coeff);

                        var metric = Utils.GetRangMetrik(rang, approximated);

                        if (metric < min)
                        {
                            transformationsMatrix[i][j] = new MemTransformation(trans.x, trans.y, trans.angle, coeff, 0);
                            min = metric;
                        }
                    }
                }

                Console.WriteLine("apply: " + i);

            }

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var noiseDomain = Utils.GenerateNoise(Utils.RANG_RATE);
                   
                    Utils.ApplyRangPart(bmp, noiseDomain, j * Utils.RANG_RATE, i * Utils.RANG_RATE);
                }
            }

            for (int k = 0; k < 15; k++)
            {
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        var trans = transformationsMatrix[i][j];
                        var domain = Utils.GetBitmapPart(bmp, trans.x * Utils.DOMAIN_RATE, trans.y * Utils.DOMAIN_RATE, Utils.DOMAIN_RATE);
                        var res = ApplyTransforms(domain, trans.angle, trans.c, trans.h);

                        Utils.ApplyRangPart(bmp, res, j * Utils.RANG_RATE, i * Utils.RANG_RATE);
                    }
                }

                bmp.Save($"C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/out{k}.bmp");
            }
        }

    }

    class Program
    {
        static void TranformToGrayScale(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    var pix = bmp.GetPixel(i, j);

                    var lightness = (int)(pix.R * .2126 + pix.G * .7152 + pix.B * .0722);

                    bmp.SetPixel(i, j, Utils.GetGrayscaleColor(lightness));
                }
            }
        }        

        static void Main(string[] args)
        {
            var bits = new Bitmap("C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/sample.bmp");
            TranformToGrayScale(bits);
            bits.Save("C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/gray.bmp");
            var encoder = new Encoder(bits);
            encoder.Encode();

        }
    }
}
