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
    ///     A helicopter flying across the map. It broadcasts news on high traffic areas, tornados and monsters. When it does
    ///     not have enough fuel to make it back to the airport, it will crash.
    /// </summary>
    public class HelicopterSprite : Sprite
    {
        private static readonly int[] CDX = {0, 0, 3, 5, 3, 0, -3, -5, -3};
        private static readonly int[] CDY = {0, -5, -3, 0, 3, 5, 3, 0, -3};
        private readonly int _origX;
        private readonly int _origY;
        private readonly int SOUND_FREQ = 200;
        private int _count;

        /// <summary>
        ///     The x-coordinate the helicopter is flying to.
        /// </summary>
        public int DestX;

        /// <summary>
        ///     The y-coordinate the helicopter is flying to.
        /// </summary>
        public int DestY;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HelicopterSprite" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="xpos">The x-coordinate of the helicopter.</param>
        /// <param name="ypos">The y-coordinate of the helictopter.</param>
        public HelicopterSprite(Micropolis engine, int xpos, int ypos)
            : base(engine, SpriteKinds.SpriteKind["COP"])
        {
            X = xpos*16 + 8;
            Y = ypos*16 + 8;
            Width = 32;
            Height = 32;
            Offx = -16;
            Offy = -16;

            DestX = City.Prng.Next(City.GetWidth())*16 + 8;
            DestY = City.Prng.Next(City.GetHeight())*16 + 8;

            _origX = X;
            _origY = Y;
            _count = 1500;
            Frame = 5;
        }


        /// <summary>
        ///     Actually does the movement and animation
        ///     of this helicopter sprite. Setting this.frame to zero will cause the
        ///     sprite to be unallocated.
        /// </summary>
        /// <remarks>
        ///     Helicopters target tornados and monsters. They need to land at an airport and have limited fuel.
        /// </remarks>
        protected override void MoveImpl()
        {
            if (_count > 0)
            {
                _count--;
            }

            if (_count == 0)
            {
                // attract copter to monster and tornado so it blows up more often
                if (City.HasSprite(SpriteKinds.SpriteKind["GOD"]))
                {
                    var monster = (MonsterSprite) City.GetSprite(SpriteKinds.SpriteKind["GOD"]);
                    DestX = monster.X;
                    DestY = monster.Y;
                }
                else if (City.HasSprite(SpriteKinds.SpriteKind["TOR"]))
                {
                    var tornado = (TornadoSprite) City.GetSprite(SpriteKinds.SpriteKind["TOR"]);
                    DestX = tornado.X;
                    DestY = tornado.Y;
                }
                else
                {
                    DestX = _origX;
                    DestY = _origY;
                }

                if (GetDis(X, Y, _origX, _origY) < 30)
                {
                    // made it back to airport, go ahead and land.
                    Frame = 0;
                    return;
                }
            }

            if (City.Acycle%SOUND_FREQ == 0)
            {
                // send report, if hovering over high traffic area
                int xpos = X/16;
                int ypos = Y/16;

                if (City.GetTrafficDensity(xpos, ypos) > 170 &&
                    City.Prng.Next(8) == 0)
                {
                    City.SendMessageAt(MicropolisMessages.HEAVY_TRAFFIC_REPORT,
                        xpos, ypos);
                    City.MakeSound(xpos, ypos, Sounds.Sound["HEAVYTRAFFIC"]);
                }
            }

            int z = Frame;
            if (City.Acycle%3 == 0)
            {
                int d = GetDir(X, Y, DestX, DestY);
                z = TurnTo(z, d);
                Frame = z;
            }
            X += CDX[z];
            Y += CDY[z];
        }
        public override string ToString()
        {
            return "destX: " + DestX + ", destY: " + DestY;
        }
    }
}