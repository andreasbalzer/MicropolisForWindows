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
    ///     A monster walking over the map, destroying everything in its path. It either originates from water or from the
    ///     middle of the map.
    /// </summary>
    public class MonsterSprite : Sprite
    {
        //GODZILLA FRAMES
        //   1...3 : northeast
        //   4...6 : southeast
        //   7...9 : southwest
        //  10..12 : northwest
        //      13 : north
        //      14 : east
        //      15 : south
        //      16 : west

        // movement deltas
        private static readonly int[] Gx = {2, 2, -2, -2, 0};
        private static readonly int[] Gy = {-2, 2, 2, -2, 0};

        private static readonly int[] ND1 = {0, 1, 2, 3};
        private static readonly int[] ND2 = {1, 2, 3, 0};
        private static readonly int[] Nn1 = {2, 5, 8, 11};
        private static readonly int[] Nn2 = {11, 2, 5, 8};
        private readonly int _origX;
        private readonly int _origY;

        /// <summary>
        ///     The count of simulation steps the monster should exist
        /// </summary>
        public int Count;

        /// <summary>
        ///     The dest x-coordinate
        /// </summary>
        public int DestX;

        /// <summary>
        ///     The destination y-coordinate
        /// </summary>
        public int DestY;

        /// <summary>
        ///     The flag, true if the monster wants to return home
        /// </summary>
        public bool Flag;

        /// <summary>
        ///     The sound count
        /// </summary>
        public int SoundCount;

        private int _step;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonsterSprite" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public MonsterSprite(Micropolis engine, int xpos, int ypos) : base(engine, SpriteKinds.SpriteKind["GOD"])
        {
            X = xpos*16 + 8;
            Y = ypos*16 + 8;
            Width = 48;
            Height = 48;
            Offx = -24;
            Offy = -24;

            _origX = X;
            _origY = Y;

            Frame = xpos > City.GetWidth()/2
                ? (ypos > City.GetHeight()/2 ? 10 : 7)
                : (ypos > City.GetHeight()/2 ? 1 : 4);

            Count = 1000;
            CityLocation p = City.GetLocationOfMaxPollution();
            DestX = p.X*16 + 8;
            DestY = p.Y*16 + 8;
            Flag = false;
            _step = 1;
        }


        /// <summary>
        ///     Actually does the movement and animation
        ///     of this monster sprite. Setting this.frame to zero will cause the
        ///     sprite to be unallocated.
        /// </summary>
        protected override void MoveImpl()
        {
            if (Frame == 0)
            {
                return;
            }

            if (SoundCount > 0)
            {
                SoundCount--;
            }

            int d = (Frame - 1)/3; // basic direction
            int z = (Frame - 1)%3; // step index (only valid for d<4)

            if (d < 4)
            {
                //turn n s e w
                //assert step == -1 || step == 1;
                if (z == 2) _step = -1;
                if (z == 0) _step = 1;
                z += _step;

                if (GetDis(X, Y, DestX, DestY) < 60)
                {
                    // reached destination

                    if (!Flag)
                    {
                        // destination was the pollution center;
                        // now head for home
                        Flag = true;
                        DestX = _origX;
                        DestY = _origY;
                    }
                    else
                    {
                        // destination was origX, origY;
                        // hide the sprite
                        Frame = 0;
                        return;
                    }
                }

                int c = GetDir(X, Y, DestX, DestY);
                c = (c - 1)/2; //convert to one of four basic headings
                //assert c >= 0 && c < 4;

                if ((c != d) && City.Prng.Next(11) == 0)
                {
                    // randomly determine direction to turn
                    if (City.Prng.Next(2) == 0)
                    {
                        z = ND1[d];
                    }
                    else
                    {
                        z = ND2[d];
                    }
                    d = 4; //transition heading

                    if (SoundCount == 0)
                    {
                        City.MakeSound(X/16, Y/16, Sounds.Sound["MONSTER"]);
                        SoundCount = 50 + City.Prng.Next(101);
                    }
                }
            }
            else
            {
                //assert this.frame >= 13 && this.frame <= 16;

                int z2 = (Frame - 13)%4;

                if (City.Prng.Next(4) == 0)
                {
                    int newFrame;
                    if (City.Prng.Next(2) == 0)
                    {
                        newFrame = Nn1[z2];
                    }
                    else
                    {
                        newFrame = Nn2[z2];
                    }
                    d = (newFrame - 1)/3;
                    z = (newFrame - 1)%3;

                    //assert d < 4;
                }
                else
                {
                    d = 4;
                }
            }

            Frame = ((d*3) + z) + 1;

            //assert this.frame >= 1 && this.frame <= 16;

            X += Gx[d];
            Y += Gy[d];

            if (Count > 0)
            {
                Count--;
            }

            int cNew = GetChar(X, Y);
            if (cNew == -1 || (cNew == TileConstants.RIVER && Count != 0 && false))
            {
                Frame = 0; //kill zilla
            }

            foreach (Sprite s in City.AllSprites())
            {
                if (CheckSpriteCollision(s) &&
                    (s.Kind == SpriteKinds.SpriteKind["AIR"] ||
                     s.Kind == SpriteKinds.SpriteKind["COP"] ||
                     s.Kind == SpriteKinds.SpriteKind["SHI"] ||
                     s.Kind == SpriteKinds.SpriteKind["TRA"])
                    )
                {
                    s.ExplodeSprite();
                }
            }

            DestroyTile(X/16, Y/16);
        }

        public override string ToString()
        {
            return "DestX: " + DestX + ", DestY: " + DestY;
        }
    }
}