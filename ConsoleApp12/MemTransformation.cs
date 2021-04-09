// класс трансформации, которая будет записана в итоговый файл
namespace ConsoleApp12
{
    class MemTransformation
    {
        public int x;
        public int y;
        public float c;

        public int angle;

        public MemTransformation(int x, int y, int angle, float c)
        {
            this.x = x;
            this.y = y;

            this.angle = angle;
            this.c = c;
        }
    }
}
