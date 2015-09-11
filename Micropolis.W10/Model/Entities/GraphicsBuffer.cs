using System;
using System.Collections.Generic;
using System.Text;

namespace Micropolis
{
    internal class GraphicsBuffer
    {
        private int[][] _tileBuffer;

        /// <summary>
        /// Sets the buffer at Rectangle (x,y) to (x+width,y+height) so those parts get redrawn next time.
        /// </summary>
        /// <param name="x">xpos top left</param>
        /// <param name="y">ypos top left</param>
        /// <param name="width">width of rectangle</param>
        /// <param name="height">height of rectangle</param>
        internal void SetBuffer(double x, double y, int width, int height, int tileWidth, int tileHeight)
        {
            int xstart = (int)x / tileWidth - 3;
            int ystart = (int)y / tileHeight - 3;
            xstart = xstart < 0 ? 0 : xstart;
            ystart = ystart < 0 ? 0 : ystart;
            int xend = width / tileWidth + xstart + 3;
            int yend = height / tileHeight + ystart + 3;
            xend = xend > _tileBuffer[0].Length ? _tileBuffer[0].Length : xend;
            yend = yend > _tileBuffer.Length ? _tileBuffer.Length : yend;
            for (int yindex = ystart; yindex < yend; yindex++)
            {
                for (int xindex = xstart; xindex < xend; xindex++)
                {
                    _tileBuffer[yindex][xindex] = -1;
                }
            }
        }

        internal void Reset(int width, int height)
        {
            if (_tileBuffer == null)
            {
                return; 
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _tileBuffer[y][x] = -1;
                }
            }
        }

        internal void Reset()
        {
            if (_tileBuffer == null)
            {
                return;
            }
            Reset(_tileBuffer[0].Length,_tileBuffer.Length);
        }

        internal int Get(int x, int y)
        {
            return _tileBuffer[y][x];
        }

        internal void Set(int x, int y, int value)
        {
            _tileBuffer[y][x] = value;
        }

        internal void CreateBuffer(int width, int height)
        {
            if (_tileBuffer == null)
            {
                _tileBuffer = new int[height][];
                for (int i = 0; i < height; i++)
                {
                    _tileBuffer[i] = new int[width];
                    for (int o = 0; o < width; o++)
                    {
                        _tileBuffer[i][o] = -1;
                    }
                }
            }
        }
    }
}
