namespace Engine
{
    /*
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
    */

    /// <summary>
    ///     Airplane flying across the map and taking off and landing at airports.
    /// </summary>
    /// <remarks>
    ///     An airplane needs some fields on the east of an airport in order to take off.
    ///     If disasters are switched on in settings, airplanes can explode from time to time.
    /// </remarks>
    public class AirplaneSprite : Sprite
    {
        /// <summary>
        ///     Note: frames 1-8 used for regular movement
        ///     9-11 used for Taking off
        /// </summary>
        private static readonly int[] CDX = {0, 0, 6, 8, 6, 0, -6, -8, -6, 8, 8, 8};

        /// <summary>
        ///     Note: frames 1-8 used for regular movement
        ///     9-11 used for Taking off
        /// </summary>
        private static readonly int[] CDY = {0, -8, -6, 0, 6, 8, 6, 0, -6, 0, 0, 0};

        /// <summary>
        ///     The X-coordinate on the map where the plane flies to
        /// </summary>
        private int _destX;

        /// <summary>
        ///     The Y-coordinate on the map where the plane flies to
        /// </summary>
        private int _destY;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AirplaneSprite" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public AirplaneSprite(Micropolis engine, int xpos, int ypos) : base(engine, SpriteKinds.SpriteKind["AIR"])
        {
            X = xpos*16 + 8;
            Y = ypos*16 + 8;
            Width = 48;
            Height = 48;
            Offx = -24;
            Offy = -24;

            _destY = Y;
            if (xpos > engine.GetWidth() - 20)
            {
                // not enough room to east of airport for taking off
                _destX = X - 200;
                Frame = 7;
            }
            else
            {
                _destX = X + 200;
                Frame = 11;
            }
        }

        /// <summary>
        ///     Actually does the movement and animation
        ///     of this airplane sprite. Setting this.frame to zero will cause the
        ///     sprite to be unallocated.
        /// </summary>
        protected override void MoveImpl()
        {
            int z = Frame;

            if (City.Acycle%5 == 0)
            {
                if (z > 8)
                {
                    //plane is still taking off
                    z--;
                    if (z < 9)
                    {
                        z = 3;
                    }
                    Frame = z;
                }
                else
                {
                    // go to destination
                    int d = GetDir(X, Y, _destX, _destY);
                    z = TurnTo(z, d);
                    Frame = z;
                }
            }

            if (GetDis(X, Y, _destX, _destY) < 50)
            {
                // at destination
                //FIXME- original code allows destination to be off-the-map
                _destX = City.Prng.Next(City.GetWidth())*16 + 8;
                _destY = City.Prng.Next(City.GetHeight())*16 + 8;
            }

            if (!City.NoDisasters)
            {
                bool explode = false;

                foreach (Sprite s  in City.AllSprites())
                {
                    if (s != this &&
                        (s.Kind == SpriteKinds.SpriteKind["AIR"] || s.Kind == SpriteKinds.SpriteKind["COP"]) &&
                        CheckSpriteCollision(s))
                    {
                        s.ExplodeSprite();
                        explode = true;
                    }
                }
                if (explode)
                {
                    ExplodeSprite();
                }
            }

            X += CDX[z];
            Y += CDY[z];
        }

        public override string ToString()
        {
            return "destX: " + _destX + ", destY: " + _destY;
        }
    }
}