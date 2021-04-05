using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp12
{
    class Encoder
    {
        private Bitmap bmp;

        public Encoder(Bitmap bmp)
        {
            this.bmp = bmp;
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
                        var res = Utils.ApplyTransforms(domain, k * 90, 1, 0);

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
                        var res = Utils.ApplyTransforms(domain, trans.angle, trans.c, trans.h);

                        Utils.ApplyRangPart(bmp, res, j * Utils.RANG_RATE, i * Utils.RANG_RATE);
                    }
                }

                bmp.Save($"C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/out{k}.bmp");
            }
        }

    }

    class Program
    {     
        static void Main(string[] args)
        {
            var bits = new Bitmap("C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/sample.bmp");
            Utils.TranformToGrayScale(bits);
            bits.Save("C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/gray.bmp");
            var encoder = new Encoder(bits);
            encoder.Encode();

        }
    }
}
