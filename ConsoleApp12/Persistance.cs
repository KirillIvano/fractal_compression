using System.IO;

namespace ConsoleApp12
{
    class Persistance
    {
        private string filePath;

        public Persistance(string filepath)
        {
            this.filePath = filepath;
        }

        public void SaveBinaryRepr(int w, int h, int rang, MemTransformation[][] transforms)
        {
            var wr = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate));

            wr.Write(w);
            wr.Write(h);
            wr.Write(rang);

            foreach (var tr in transforms)
            {
                foreach (var tv in tr)
                {
                    wr.Write(tv.x);
                    wr.Write(tv.y);
                    wr.Write(tv.c);
                    wr.Write(tv.angle);
                }
            }

            wr.Close();
        }

        public (int, int, int, MemTransformation[][]) ReadBinaryRepr()
        {
            var reader = new BinaryReader(File.Open(filePath, FileMode.Open));

            var w = reader.ReadInt32();
            var h = reader.ReadInt32();
            var rang = reader.ReadInt32();

            MemTransformation[][] transforms = new MemTransformation[h][];

            for (int i = 0; i < h; i++)
            {
                transforms[i] = new MemTransformation[w];

                for (int j = 0; j < w; j++)
                {
                    var x = reader.ReadInt32();
                    var y = reader.ReadInt32();
                    var c = reader.ReadSingle();
                    var angle = reader.ReadInt32();

                    transforms[i][j] = new MemTransformation(x, y, angle, c);
                }
            }

            reader.Close();

            return (w, h, rang, transforms);
        }
    }
}
