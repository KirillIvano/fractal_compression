// класс трансформации, которая будет записана в итоговый файл
namespace ConsoleApp12
{
    class MemTransformation
    {
        public int x;
        public int y;
        public float c;
        public int h;

        public int angle;

        public MemTransformation(int x, int y, int angle, float c, int h)
        {
            this.x = x;
            this.y = y;

            this.angle = angle;
            this.h = h;
            this.c = c;
        }
    }
}
