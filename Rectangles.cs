using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading;

namespace CellularAutomata
{
    class Rect
    {
        
        public const int NUMBER_OF_RECTANGLES = Form1.NUMBER_OF_LINES;
        public const int DIMENSION_X = Form1.LINE_SPACING - 1;
        public const int DIMENSION_Y = Form1.LINE_SPACING - 1; //9x9 by default
        public int minX = 0;
        public int minY = 0;
        public int maxX = 0;
        public int maxY = 0;
        public int x = 0;
        public int y = 0;
        public int state = 0;
        public int nextState = 0;
   //     private Thread updateState;

        //public static void InitializeArray()
        //{
        //  //  Form1.
        //    if (initialized)
        //    {
        //        return;
        //    }
        //    initialized = true;
        //    for (int iteratorX = 0; iteratorX < NUMBER_OF_RECTANGLES; iteratorX++)
        //    {
        //        for (int iteratorY = 0; iteratorY < NUMBER_OF_RECTANGLES; iteratorY++)
        //        {
        //            Rect temp = rectangles[iteratorX, iteratorY];
        //            temp.x = iteratorX;
        //            temp.y = iteratorY;
        //            //rectangles need to be offset by a pixel, and are one less than Form1.LINE_SPACING in dimensions
        //            //this is so the rectangles don't interfere/overwrite the lines
        //            temp.minX = (iteratorX * Form1.LINE_SPACING + 1);
        //            temp.minY = (iteratorY * Form1.LINE_SPACING + 1);
        //            temp.maxX = (iteratorX * Form1.LINE_SPACING + DIMENSION_X);
        //            temp.maxY = (iteratorY * Form1.LINE_SPACING + DIMENSION_Y);
        //        }
        //    }
        //}

       /* private void Rectangles()
        {
            this.minX = 0;
            this.minY = 0;
            this.maxX = 0;
            this.maxY = 0;
        }*/
    }
}
