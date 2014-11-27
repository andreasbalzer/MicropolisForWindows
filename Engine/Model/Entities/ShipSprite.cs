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
    ///     A ship travelling across rivers and the sea.
    /// </summary>
    public class ShipSprite : Sprite
    {
        private static readonly int[] BDX = {0, 0, 1, 1, 1, 0, -1, -1, -1};
        private static readonly int[] BDY = {0, -1, -1, 0, 1, 1, 1, 0, -1};
        private static readonly int[] BPX = {0, 0, 2, 2, 2, 0, -2, -2, -2};
        private static readonly int[] BPY = {0, -2, -2, 0, 2, 2, 2, 0, -2};

        private static readonly int[] BTCLRTAB =
        {
            TileConstants.RIVER, TileConstants.CHANNEL, TileConstants.POWERBASE, TileConstants.POWERBASE + 1,
            TileConstants.RAILBASE, TileConstants.RAILBASE + 1, TileConstants.BRWH, TileConstants.BRWV
        };

        public static readonly int NORTH_EDGE = 5;
        public static readonly int EAST_EDGE = 7;
        public static readonly int SOUTH_EDGE = 1;
        public static readonly int WEST_EDGE = 3;
        private int _count;
        private int _newDir;
        private int _soundCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShipSprite" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <param name="edge">The edge.</param>
        public ShipSprite(Micropolis engine, int xpos, int ypos, int edge)
            : base(engine, SpriteKinds.SpriteKind["SHI"])
        {
            X = xpos*16 + 8;
            Y = ypos*16 + 8;
            Width = 48;
            Height = 48;
            Offx = -24;
            Offy = -24;
            Frame = edge;
            _newDir = edge;
            Dir = 10;
            _count = 1;
        }

        /// <summary>
        ///     Actually does the movement and animation
        ///     of this ship sprite. Setting this.frame to zero will cause the
        ///     sprite to be unallocated.
        /// </summary>
        /// <remarks>Ship has a specific direction. Paths are searched based on simple random sized counter. If the direction set is blocked, e.g. a bridge with traffic does not open, the ship explodes.</remarks>
        protected override void MoveImpl()
        {
            int t = TileConstants.RIVER;

            _soundCount--;
            if (_soundCount <= 0)
            {
                if (City.Prng.Next(4) == 0)
                {
                    City.MakeSound(X/16, Y/16, Sounds.Sound["HONKHONK_LOW"]);
                }
                _soundCount = 200;
            }

            _count--;
            if (_count <= 0)
            {
                _count = 9;
                if (_newDir != Frame)
                {
                    Frame = TurnTo(Frame, _newDir);
                    return;
                }
                int tem = City.Prng.Next(8);
                int pem;
                for (pem = tem; pem < (tem + 8); pem++)
                {
                    int z = (pem%8) + 1;
                    if (z == Dir)
                        continue;

                    int xpos = X/16 + BDX[z];
                    int ypos = Y/16 + BDY[z];

                    if (City.TestBounds(xpos, ypos))
                    {
                        t = City.GetTile(xpos, ypos);
                        if ((t == TileConstants.CHANNEL) || (t == TileConstants.BRWH) || (t == TileConstants.BRWV) ||
                            tryOther(t, Dir, z)) //channel or horizontal open bridge or vertical open bridge or tryother
                        {
                            _newDir = z;
                            Frame = TurnTo(Frame, _newDir);
                            Dir = z + 4;
                            if (Dir > 8)
                            {
                                Dir -= 8;
                            }
                            break;
                        }
                    }
                }

                if (pem == (tem + 8))
                {
                    Dir = 10;
                    _newDir = City.Prng.Next(8) + 1;
                }
            }
            else
            {
                int z = Frame;
                if (z == _newDir)
                {
                    X += BPX[z];
                    Y += BPY[z];
                }
            }

            if (!SpriteInBounds())
            {
                Frame = 0;
                return;
            }

            bool found = false;
            foreach (int z in BTCLRTAB)
            {
                if (t == z)
                {
                    found = true;
                }
            }
            if (!found)
            {
                ExplodeSprite();
                DestroyTile(X/16, Y/16);
            }
        }

        private bool tryOther(int tile, int oldDir, int newDir)
        {
            int z = oldDir + 4;
            if (z > 8) z -= 8;
            if (newDir != z) return false;

            return (tile == TileConstants.POWERBASE || tile == TileConstants.POWERBASE + 1 ||
                    tile == TileConstants.RAILBASE || tile == TileConstants.RAILBASE + 1);
        }

        private bool SpriteInBounds()
        {
            int xpos = X/16;
            int ypos = Y/16;
            return City.TestBounds(xpos, ypos);
        }

        public override string ToString()
        {
            return "X: " + X + ", Y:" + Y;
        }
    }
}