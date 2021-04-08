using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp12
{
    // TODO: сделать ранги и домены передаваемыми
    // TODO: вернуть флип
    class Encoder
    {
        private Bitmap bmp;
        private int rangRate;
        private int domainRate;

        public Encoder(Bitmap bmp, int rangRate)
        {
            this.bmp = bmp;
            this.rangRate = rangRate;
            this.domainRate = 2 * rangRate;
        }

        private List<Transformation> GenerateTransformations()
        {
            var w = bmp.Width / domainRate;
            var h = bmp.Height / domainRate;

            var transformations = new List<Transformation>();

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var domain = Utils.GetBitmapPart(bmp, j * domainRate, i * domainRate, domainRate);
                    
                    for (var k = 0; k < 6; k++)
                    {
                        var res = Utils.ApplyTransforms(domain, k * 90, 1, 0, rangRate);

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
            var w = bmp.Width / rangRate;
            var h = bmp.Height / rangRate;

            var transformations = GenerateTransformations();

            var transformationsMatrix = new MemTransformation[h][];

            for (int i = 0; i < h; i++)
            {
                transformationsMatrix[i] = new MemTransformation[w];

                for (int j = 0; j < w; j++)
                {
                    float min = int.MaxValue;
                    var rang = Utils.GetBitmapPart(
                        bmp,
                        rangRate * j,
                        rangRate * i,
                        rangRate
                    );

                    var rangAverage = Utils.GetAverage(rang);

                    foreach (var trans in transformations)
                    {
                        float coeff = rangAverage / trans.avg;

                        int[][] approximated = Utils.CopyRang(trans.result, rangRate);
                        Utils.MultiplyRang(approximated, coeff, rangRate);

                        var metric = Utils.GetRangMetrik(rang, approximated, rangRate);

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
                    var noiseDomain = Utils.GenerateNoise(rangRate);
                    
                    Utils.ApplyRangPart(bmp, noiseDomain, j * rangRate, i * rangRate, rangRate);
                }
            }

            for (int k = 0; k < 15; k++)
            {
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        var trans = transformationsMatrix[i][j];
                        var domain = Utils.GetBitmapPart(bmp, trans.x * domainRate, trans.y * domainRate, domainRate);
                        var res = Utils.ApplyTransforms(domain, trans.angle, trans.c, trans.h, rangRate);

                        Utils.ApplyRangPart(bmp, res, j * rangRate, i * rangRate, rangRate);
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

            var encoder = new Encoder(bits, 20);
            encoder.Encode();
        }
    }
}
