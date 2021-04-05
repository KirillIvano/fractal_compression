using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp12
{
    class Utils
    {
        public const int RANG_RATE = 50;
        public const int DOMAIN_RATE = RANG_RATE * 2;

        public static double GetRangMetrik(Color[][] r1, Color[][] r2)
        {
            double acc = 0;

            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    acc += Math.Pow(r1[j][i].R - r2[j][i].R, 2) +
                        Math.Pow(r1[j][i].G - r2[j][i].G, 2) +
                        Math.Pow(r1[j][i].B - r2[j][i].B, 2);
                }
            }

            return acc;
        }

        public static Color[][] GenerateNoise(int size)
        {
            Color[][] res = new Color[size][];

            var rand = new Random();

            for (int i = 0; i < size; i++)
            {
                res[i] = new Color[size];

                for (int j = 0; j < size; j++)
                {
                    res[i][j] = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                }
            }

            return res;
        }

        public static Color GetGrayscaleColor(int val)
        {
            int validVal = Math.Max(0, Math.Min(255, val));

            return Color.FromArgb(validVal, validVal, validVal);
        }

        public static int GetSafeColor(double val)
        {
            int validVal = (int) Math.Max(0, Math.Min(255, val));

            return validVal;
        }

        // Перенос значений из ранговой области на итоговый битмап
        public static void ApplyRangPart(Bitmap bits, Color[][] rang, int x, int y)
        {

            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    var val = rang[j][i];
                    bits.SetPixel(j + x, i + y, val);
                }
            }
        }

        // Выделение доменной части из битмапа
        public static Color[][] GetBitmapPart(Bitmap bits, int x, int y, int size)
        {
            Color[][] res = new Color[size][];
            int i, j;

            for (i = 0; i < size; i++)
            {
                res[i] = new Color[size];
            }

            for (i = 0; i < size; i++)
            {
                for (j = 0; j < size; j++)
                {
                    res[j][i] = bits.GetPixel(j + x, i + y);
                }
            }

            return res;
        }

        // Приведение доменной области к ранговой
        public static Color[][] ReduceDomainPart(Color[][] domain)
        {
            var res = new Color[RANG_RATE][];

            for (int i = 0; i < RANG_RATE; i++)
            {
                res[i] = new Color[RANG_RATE];
            }

            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    var r = (int)((
                        domain[2 * i][2 * j].R +
                        domain[2 * i + 1][2 * j].R +
                        domain[2 * i][2 * j + 1].R +
                        domain[2 * i + 1][2 * j + 1].R
                        ) / 4);

                    var g = (int)((
                        domain[2 * i][2 * j].R +
                        domain[2 * i + 1][2 * j].R +
                        domain[2 * i][2 * j + 1].R +
                        domain[2 * i + 1][2 * j + 1].R
                        ) / 4);

                    var b = (int)((
                        domain[2 * i][2 * j].R +
                        domain[2 * i + 1][2 * j].R +
                        domain[2 * i][2 * j + 1].R +
                        domain[2 * i + 1][2 * j + 1].R
                        ) / 4);

                    res[j][i] = Color.FromArgb(r, g, b);
                }
            }

            return res;
        }

        // Получение среднего значения по произвольной матрице
        public static double GetAverage(Color[][] matrix, int component)
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
                    if (component == 0)
                    {
                        acc += matrix[i][j].R;
                    }
                    else if (component == 1)
                    {
                        acc += matrix[i][j].G;
                    }
                    else if (component == 2)
                    {
                        acc += matrix[i][j].B;
                    }
                }
            }

            var avg = acc / (xl * yl);

            return avg;
        }

        // Умножение всех элементов на значение
        public static void MultiplyRang(Color[][] mat, double[] c)
        {
            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    var curr = mat[j][i];
                    mat[j][i] = Color.FromArgb(
                        GetSafeColor(curr.R * c[0]),
                        GetSafeColor(curr.G * c[1]),
                        GetSafeColor(curr.B * c[2])
                    );
                }
            }
        }

        public static Color[][] CopyRang(Color[][] src)
        {
            var res = new Color[RANG_RATE][];

            for (int i = 0; i < RANG_RATE; i++)
            {
                res[i] = new Color[RANG_RATE];
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
        public static void AddToRang(Color[][] mat, Color val, int positive = 1)
        {
            for (int i = 0; i < RANG_RATE; i++)
            {
                for (int j = 0; j < RANG_RATE; j++)
                {
                    var prev = mat[j][i];
                    mat[j][i] = Color.FromArgb(
                        GetSafeColor(prev.R + val.R * positive),
                        GetSafeColor(prev.G + val.G * positive),
                        GetSafeColor(prev.B + val.B * positive)
                    );
                }
            }
        }

        // Вращение против часовой стрелки без вспомогательных массивов
        public static void RotateRang(Color[][] rang, int angle)
        {
            for (int curr = angle; curr > 0; curr -= 90)
            {
                for (int i = 0; i < RANG_RATE / 2; i++)
                {
                    for (int j = i; j < RANG_RATE - i - 1; j++)
                    {
                        Color temp = rang[i][j];
                        rang[i][j] = rang[RANG_RATE - 1 - j][i];
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
        public Color[][] result;
        public double[] avg;

        public Transformation(int x, int y, int angle, Color[][] result, double[] avg)
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
        public double[] c;

        public int angle;

        public MemTransformation(int x, int y, int angle, double[] c)
        {
            this.x = x;
            this.y = y;

            this.angle = angle;
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

        private Color[][] ApplyTransforms(Color[][] domain, int rotateAngle, double[] contrast)
        {
            var rang = Utils.ReduceDomainPart(domain);

            Utils.RotateRang(rang, rotateAngle);
            Utils.MultiplyRang(rang, contrast);

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
                        var res = ApplyTransforms(domain, k * 90, new double[] { 1, 1, 1 });

                        var avgR = Utils.GetAverage(res, 0);
                        var avgG = Utils.GetAverage(res, 1);
                        var avgB = Utils.GetAverage(res, 2);

                        transformations.Add(new Transformation(
                            j,
                            i,
                            k * 90,
                            res,
                            new double[] { avgR, avgG, avgB }
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

                    var rangAverageR = Utils.GetAverage(rang, 0);
                    var rangAverageG = Utils.GetAverage(rang, 1);
                    var rangAverageB = Utils.GetAverage(rang, 2);

                    foreach (var trans in transformations)
                    {
                        double coeffR = rangAverageR / trans.avg[0];
                        double coeffG = rangAverageG / trans.avg[1];
                        double coeffB = rangAverageB / trans.avg[2];

                        var coeffs = new double[] { coeffR, coeffG, coeffB };

                        Color[][] approximated = Utils.CopyRang(trans.result);
                        Utils.MultiplyRang(approximated, coeffs);

                        var metric = Utils.GetRangMetrik(rang, approximated);

                        if (metric < min)
                        {
                            transformationsMatrix[i][j] = new MemTransformation(trans.x, trans.y, trans.angle, coeffs);
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
                        var res = ApplyTransforms(domain, trans.angle, trans.c);

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

            bits.Save("C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/gray.bmp");
            var encoder = new Encoder(bits);
            encoder.Encode();

        }
    }
}
