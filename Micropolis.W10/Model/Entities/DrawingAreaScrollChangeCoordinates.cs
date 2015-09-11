using System;
using System.Collections.Generic;
using System.Text;

namespace Micropolis.Model.Entities
{
    public class DrawingAreaScrollChangeCoordinates
    {
        public double X { get; set; }
        public double Y { get; set; }

        public float ZoomFactor { get; set; }

        public DrawingAreaScrollChangeCoordinates(double x, double y, float zoomFactor)
        {
            X = x;
            Y = y;
            ZoomFactor = zoomFactor;
        }
    }
}
