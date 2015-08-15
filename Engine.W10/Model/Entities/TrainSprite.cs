namespace Engine
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
    /// Implements the commuter train. 
    /// The commuter train appears if the city has a certain amount of 
    /// railroad track. It wanders around the city's available track 
    /// randomly. 
    ///  
    ///     A train offers transportation for citizens when shopping
    /// </summary>
    public class TrainSprite : Sprite
    {
        public static int[] Cx = {0, 16, 0, -16};
        public static int[] Cy = {-16, 0, 16, 0};
        public static int[] Dx = {0, 4, 0, -4, 0};
        public static int[] Dy = {-4, 0, 4, 0, 0};
        public static int[] TrainPic2 = {1, 2, 1, 2, 5};
        
        public static readonly int TRA_GROOVE_X = 8;

        public static readonly int TRA_GROOVE_Y = 8;

        public static readonly int FRAME_NORTHSOUTH = 1;
        public static readonly int FRAME_EASTWEST = 2;
        public static readonly int FRAME_NW_SE = 3;
        public static readonly int FRAME_SW_NE = 4;
        public static readonly int FRAME_UNDERWATER = 5;

        public static readonly int DIR_NORTH = 0;
        public static readonly int DIR_EAST = 1;
        public static readonly int DIR_SOUTH = 2;
        public static readonly int DIR_WEST = 3;
        public static readonly int DIR_NONE = 4; //not moving
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="TrainSprite" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public TrainSprite(Micropolis engine, int xpos, int ypos)
            : base(engine, SpriteKinds.SpriteKind["TRA"])
        {
            X = xpos*16 + TRA_GROOVE_X;
            Y = ypos*16 + TRA_GROOVE_Y;
            Offx = -16;
            Offy = -16;
            Dir = DIR_NONE; //not moving
        }

        /// <summary>
        ///     For subclasses to override.
        ///     Actually does the movement and animation of this train sprite.
        ///     Setting this.frame to zero will cause the sprite to be unallocated.
        /// </summary>
        protected override void MoveImpl()
        {
            if (Frame == 3 || Frame == 4)
            {
                Frame = TrainPic2[Dir];
            }
            X += Dx[Dir];
            Y += Dy[Dir];
            if (City.Acycle%4 == 0)
            {
                // should be at the center of a cell, if not, correct it
                X = (X/16)*16 + TRA_GROOVE_X;
                Y = (Y/16)*16 + TRA_GROOVE_Y;
                int d1 = City.Prng.Next(4);
                for (int z = d1; z < d1 + 4; z++)
                {
                    int d2 = z%4;
                    if (Dir != DIR_NONE)
                    {
                        //impossible?
                        if (d2 == (Dir + 2)%4)
                            continue;
                    }

                    int c = GetChar(X + Cx[d2], Y + Cy[d2]);
                    if (((c >= TileConstants.RAILBASE) && (c <= TileConstants.LASTRAIL)) || //track?
                        (c == TileConstants.RAILVPOWERH) ||
                        (c == TileConstants.RAILHPOWERV))
                    {
                        if ((Dir != d2) && (Dir != DIR_NONE))
                        {
                            if (Dir + d2 == 3)
                                Frame = FRAME_NW_SE;
                            else
                                Frame = FRAME_SW_NE;
                        }
                        else
                        {
                            Frame = TrainPic2[d2];
                        }

                        if ((c == TileConstants.RAILBASE) || (c == (TileConstants.RAILBASE + 1)))
                        {
                            //underwater
                            Frame = FRAME_UNDERWATER;
                        }
                        Dir = d2;
                        return;
                    }
                }
                if (Dir == DIR_NONE)
                {
                    // train has nowhere to go, so retire
                    Frame = 0;
                    return;
                }
                Dir = DIR_NONE;
            }
        }
        public override string ToString()
        {
            return "X: " + X + ", Y: " + Y;
        }
    }
}