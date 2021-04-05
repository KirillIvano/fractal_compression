namespace ConsoleApp12
{
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
}
