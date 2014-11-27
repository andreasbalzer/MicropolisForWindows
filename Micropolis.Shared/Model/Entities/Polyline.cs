using System.Collections.Generic;
using Windows.Foundation;

namespace Micropolis
{
    // This file is part of Micropolis for WinRT.
    // Copyright (C) 2014 Andreas Balzer, Felix Dietrich, Florian Thurnwald and Ivo Vutov
    // Portions Copyright (C) MicropolisJ by Jason Long
    // Portions Copyright (C) Micropolis Don Hopkins
    // Portions Copyright (C) 1989-2007 Electronic Arts Inc.
    //
    // Micropolis for WinRT is free software; you can redistribute it and/or modify
    // it under the terms of the GNU GPLv3, with Additional terms.
    // See the README file, included in this distribution, for details.
    // Project website: http://code.google.com/p/micropolis/

    /// <summary>
    ///     Descibes a directed path from one point to another.
    /// </summary>
    public class Polyline
    {
        private readonly List<Point> _points = new List<Point>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Polyline" /> class.
        /// </summary>
        public Polyline()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Polyline" /> class.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public Polyline(double x1, double y1, double x2, double y2)
        {
            var origin = new Point(x1, y1);
            var destination = new Point(x2, y2);
            _points.Add(origin);
            _points.Add(destination);
        }

        /// <summary>
        /// x-coordinate of first point
        /// </summary>
        public double X1
        {
            get { return _points[0].X; }
        }

        /// <summary>
        /// x-coordinate of second point
        /// </summary>
        public double X2
        {
            get { return _points[1].X; }
        }

        /// <summary>
        /// y-coordinate of first point
        /// </summary>
        public double Y1
        {
            get { return _points[0].Y; }
        }

        /// <summary>
        /// y-coordinate of second point
        /// </summary>
        public double Y2
        {
            get { return _points[1].Y; }
        }

        /// <summary>
        ///     Moves the Polyline to the point.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public void MoveTo(double x, double y)
        {
            _points.Clear();
            var origin = new Point(x, y);
            _points.Add(origin);
        }

        /// <summary>
        ///     Adds a new point to the Polyline.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void LineTo(double x, double y)
        {
            var newPoint = new Point(x, y);
            _points.Add(newPoint);
        }

        /// <summary>
        /// Gets last point of the Polyline.
        /// </summary>
        /// <returns></returns>
        public Point GetCurrentPoint()
        {
            return _points[_points.Count - 1];
        }

        /// <summary>
        /// Converts this Polyline to an array of integers. Fields are grouped by points.
        /// </summary>
        /// <remarks>Format: x1, y1, x2, y2</remarks>
        /// <returns>Array of coordinates.</returns>
        internal int[] ToArray()
        {
            var result = new int[2*_points.Count];
            int curPoint = 0;
            foreach (Point point in _points)
            {
                result[curPoint] = (int) point.X;
                result[curPoint + 1] = (int) point.Y;
                curPoint += 2;
            }

            return result;
        }
    }
}