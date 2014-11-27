using System;

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
    ///     A sprite is an object on the game map, e.g. a ship, airplane, residential zone.
    /// </summary>
    public abstract class Sprite
    {
        protected Micropolis City;

        /// <summary>
        ///     The direction of this sprite if moving
        /// </summary>
        protected int Dir;

        public int Frame;
        public int Height = 32;

        //TODO- enforce read-only nature of the following properties
        // (i.e. do not let them be modified directly by other classes)

        /// <summary>
        ///     The kind of this sprite
        /// </summary>
        public SpriteKind Kind;

        /// <summary>
        ///     The last x-coordinate if this sprite moved
        /// </summary>
        public int LastX;

        /// <summary>
        ///     The last y-coordinate if this sprite moved
        /// </summary>
        public int LastY;

        public int Offx;
        public int Offy;
        public int Width = 32;

        /// <summary>
        ///     The x-coordinate
        /// </summary>
        public int X;

        /// <summary>
        ///     The y-coordinate
        /// </summary>
        public int Y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="kind">The kind.</param>
        protected Sprite(Micropolis engine, SpriteKind kind)
        {
            City = engine;
            Kind = kind;
        }

        /// <summary>
        ///     Gets the tile at coordinate.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected int GetChar(int x, int y)
        {
            int xpos = x/16;
            int ypos = y/16;
            if (City.TestBounds(xpos, ypos))
            {
                return City.GetTile(xpos, ypos);
            }
            return -1;
        }


        /// <summary>
        ///     For subclasses to override.
        ///     Actually does the movement and animation of this particular sprite.
        ///     Setting this.frame to zero will cause the sprite to be unallocated.
        /// </summary>
        protected abstract void MoveImpl();


        /// <summary>
        ///     Perform this agent's movement and animation.
        /// </summary>
        public void Move()
        {
            LastX = X;
            LastY = Y;
            MoveImpl();
            City.FireSpriteMoved(this);
        }


        /// <summary>
        ///     Tells whether this sprite is visible.
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            return Frame != 0;
        }


        /// <summary>
        ///     Computes direction from one point to another.
        /// </summary>
        /// <param name="orgX">The org x.</param>
        /// <param name="orgY">The org y.</param>
        /// <param name="desX">The DES x.</param>
        /// <param name="desY">The DES y.</param>
        /// <returns>
        ///     integer between 1 and 8, with
        ///     1 == north,
        ///     3 == east,
        ///     5 == south,
        ///     7 == west.
        /// </returns>
        protected static int GetDir(int orgX, int orgY, int desX, int desY)
        {
            int[] gdtab = {0, 3, 2, 1, 3, 4, 5, 7, 6, 5, 7, 8, 1};
            int dispX = desX - orgX;
            int dispY = desY - orgY;

            int z = dispX < 0 ? (dispY < 0 ? 11 : 8) : (dispY < 0 ? 2 : 5);

            dispX = Math.Abs(dispX);
            dispY = Math.Abs(dispY);
            int absDist = dispX + dispY;

            if (dispX*2 < dispY) z++;
            else if (dispY*2 < dispX) z--;

            if (z >= 1 && z <= 12)
            {
                return gdtab[z];
            }
            //assert false;
            return 0;
        }


        /// <summary>
        ///     Computes manhatten distance between two points.
        /// </summary>
        /// <param name="x0">The x0.</param>
        /// <param name="y0">The y0.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <returns>manhatten distance</returns>
        public static int GetDis(int x0, int y0, int x1, int y1)
        {
            return Math.Abs(x0 - x1) + Math.Abs(y0 - y1);
        }


        /// <summary>
        ///     Replaces this sprite with an exploding sprite.
        /// </summary>
        public void ExplodeSprite()
        {
            Frame = 0;

            City.MakeExplosionAt(X, Y);
            int xpos = X/16;
            int ypos = Y/16;

            if (Kind == SpriteKinds.SpriteKind["AIR"])
            {
                City.CrashLocation = new CityLocation(xpos, ypos);
                City.SendMessageAt(MicropolisMessages.PLANECRASH_REPORT, xpos, ypos);
            }
            else if (Kind == SpriteKinds.SpriteKind["SHI"])
            {
                City.CrashLocation = new CityLocation(xpos, ypos);
                City.SendMessageAt(MicropolisMessages.SHIPWRECK_REPORT, xpos, ypos);
            }
            else if (Kind == SpriteKinds.SpriteKind["TRA"] || Kind == SpriteKinds.SpriteKind["BUS"])
            {
                City.CrashLocation = new CityLocation(xpos, ypos);
                City.SendMessageAt(MicropolisMessages.TRAIN_CRASH_REPORT, xpos, ypos);
            }
            else if (Kind == SpriteKinds.SpriteKind["COP"])
            {
                City.CrashLocation = new CityLocation(xpos, ypos);
                City.SendMessageAt(MicropolisMessages.COPTER_CRASH_REPORT, xpos, ypos);
            }

            City.MakeSound(xpos, ypos, Sounds.Sound["EXPLOSION_HIGH"]);
        }

        /// <summary>
        ///     Checks whether another sprite is in collision ranges.
        /// </summary>
        /// <param name="otherSprite">The other sprite.</param>
        /// <returns>true iff the sprite is in collision range</returns>
        protected bool CheckSpriteCollision(Sprite otherSprite)
        {
            if (!IsVisible()) return false;
            if (!otherSprite.IsVisible()) return false;

            return (GetDis(X, Y, otherSprite.X, otherSprite.Y) < 30);
        }


        /// <summary>
        ///     Destroys whatever is at the specified location, replacing it with fire, rubble, or water as appropriate.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        protected void DestroyTile(int xpos, int ypos)
        {
            if (!City.TestBounds(xpos, ypos))
                return;

            int t = City.GetTile(xpos, ypos);

            if (t >= TileConstants.TREEBASE)
            {
                if (TileConstants.IsBridge(t))
                {
                    City.SetTile(xpos, ypos, TileConstants.RIVER);
                    return;
                }
                if (!TileConstants.IsCombustible(t))
                {
                    return; //cannot destroy it
                }
                if (TileConstants.IsZoneCenter(t))
                {
                    City.KillZone(xpos, ypos, t);
                    if (t > TileConstants.RZB)
                    {
                        City.MakeExplosion(xpos, ypos);
                    }
                }
                if (TileConstants.CheckWet(t))
                {
                    City.SetTile(xpos, ypos, TileConstants.RIVER);
                }
                else
                {
                    City.SetTile(xpos, ypos, TileConstants.TINYEXP);
                }
            }
        }


        /// <summary>
        ///     Helper function for rotating a sprite.
        /// </summary>
        /// <param name="p">the sprite's current attitude (1-8)</param>
        /// <param name="d">the desired attitude (1-8)</param>
        /// <returns> the new attitude</returns>
        protected static int TurnTo(int p, int d)
        {
            if (p == d)
                return p;
            if (p < d)
            {
                if (d - p < 4) p++;
                else p--;
            }
            else
            {
                if (p - d < 4) p--;
                else p++;
            }
            if (p > 8) return 1;
            if (p < 1) return 8;
            return p;
        }

        public override string ToString()
        {
            return "X: " + X + ", Y: " + Y;
        }
    }
}