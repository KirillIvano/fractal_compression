using System.Drawing;
using System.IO;

namespace ConsoleApp12
{
    class Decoder
    {
        private string srcPath;
        private string outFolder;

        public Decoder(string srcPath, string outFolder)
        {
            this.srcPath = srcPath;
            this.outFolder = outFolder;
        }

        public void Decode()
        {
            var (w, h, rangRate, transforms) = new Persistance(srcPath).ReadBinaryRepr();
            var domainRate = 2 * rangRate;
            var bmp = new Bitmap(w * rangRate, h * rangRate);

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var noiseDomain = Utils.GenerateNoise(rangRate);

                    Utils.ApplyRangPart(bmp, noiseDomain, j * rangRate, i * rangRate, rangRate);
                }
            }

            for (int k = 0; k < 10; k++)
            {
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        var trans = transforms[i][j];
                        var domain = Utils.GetBitmapPart(bmp, trans.x * domainRate, trans.y * domainRate, domainRate);
                        var res = Utils.ApplyTransforms(domain, trans.angle, trans.c, rangRate);

                        Utils.ApplyRangPart(bmp, res, j * rangRate, i * rangRate, rangRate);
                    }
                }

                bmp.Save(Path.Join(outFolder, $"x_{k}.bmp"));
            }
        }
    }
}
