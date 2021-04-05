using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp12
{
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
}
