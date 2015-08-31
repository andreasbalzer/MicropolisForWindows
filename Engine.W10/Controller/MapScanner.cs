﻿namespace Engine
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
    ///     Process individual tiles of the map for each cycle. In each sim cycle each tile will get activated, and this class
    ///     contains the activation code.
    /// </summary>
    public class MapScanner : TileBehavior
    {
        private readonly BZone _behavior;
        private readonly TrafficGen _traffic;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapScanner" /> class.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="behavior">The behavior.</param>
        public MapScanner(Micropolis city, BZone behavior) : base(city)
        {
            _behavior = behavior;
            _traffic = new TrafficGen(city);
        }

        /// <summary>
        ///     Activate the tile identified by xpos and ypos properties.
        /// </summary>
        public override void Apply()
        {
            switch (_behavior)
            {
                case BZone.RESIDENTIAL:
                    DoResidential();
                    return;
                case BZone.HOSPITAL_CHURCH:
                    DoHospitalChurch();
                    return;
                case BZone.COMMERCIAL:
                    DoCommercial();
                    return;
                case BZone.INDUSTRIAL:
                    DoIndustrial();
                    return;
                case BZone.COAL:
                    DoCoalPower();
                    return;
                case BZone.NUCLEAR:
                    DoNuclearPower();
                    return;
                case BZone.FIRESTATION:
                    DoFireStation();
                    return;
                case BZone.POLICESTATION:
                    DoPoliceStation();
                    return;
                case BZone.STADIUM_EMPTY:
                    DoStadiumEmpty();
                    return;
                case BZone.STADIUM_FULL:
                    DoStadiumFull();
                    return;
                case BZone.AIRPORT:
                    DoAirport();
                    return;
                case BZone.SEAPORT:
                    DoSeaport();
                    return;
                    //assert false;
            }
        }

        /// <summary>
        ///     Checks the zone power to determine how many zones are with/without electricity.
        /// </summary>
        /// <returns></returns>
        private bool CheckZonePower()
        {
            bool zonePwrFlag = SetZonePower();

            if (zonePwrFlag)
            {
                City.PoweredZoneCount++;
            }
            else
            {
                City.UnpoweredZoneCount++;
            }

            return zonePwrFlag;
        }

        /// <summary>
        ///     Sets the zone power by comparing available power generated by power plants with power demand.
        /// </summary>
        /// <returns></returns>
        private bool SetZonePower()
        {
            bool oldPower = City.IsTilePowered(Xpos, Ypos);
            bool newPower = (
                Tile == TileConstants.NUCLEAR ||
                Tile == TileConstants.POWERPLANT ||
                City.HasPower(Xpos, Ypos)
                );

            if (newPower && !oldPower)
            {
                City.SetTilePower(Xpos, Ypos, true);
                City.PowerZone(Xpos, Ypos, TileConstants.GetZoneSizeFor(Tile));
            }
            else if (!newPower && oldPower)
            {
                City.SetTilePower(Xpos, Ypos, false);
                City.ShutdownZone(Xpos, Ypos, TileConstants.GetZoneSizeFor(Tile));
            }

            return newPower;
        }

        /// <summary>
        ///     Place a 3x3 zone on to the map, centered on the current location.
        ///     Note: nothing is done if part of this zone is off the edge
        ///     of the map or is being flooded or radioactive.
        /// </summary>
        /// <param name="baseTile">The "zone" tile value for this zone.</param>
        /// <returns>true iff the zone was actually placed.</returns>
        private bool ZonePlop(int baseTile)
        {
            //assert isZoneCenter(base);

            BuildingInfo bi = Tiles.Get(baseTile).GetBuildingInfo();
            //assert bi != null;
            if (bi == null)
                return false;

            for (int y = Ypos - 1; y < Ypos - 1 + bi.Height; y++)
            {
                for (int x = Xpos - 1; x < Xpos - 1 + bi.Width; x++)
                {
                    if (!City.TestBounds(x, y))
                    {
                        return false;
                    }
                    if (TileConstants.IsIndestructible2(City.GetTile(x, y)))
                    {
                        // radioactive, on fire, or flooded
                        return false;
                    }
                }
            }

            //assert bi.members.Length == bi.width * bi.height;
            int i = 0;
            for (int y = Ypos - 1; y < Ypos - 1 + bi.Height; y++)
            {
                for (int x = Xpos - 1; x < Xpos - 1 + bi.Width; x++)
                {
                    City.SetTile(x, y, (char) bi.Members[i]);
                    i++;
                }
            }

            // refresh own tile property 
            Tile = City.GetTile(Xpos, Ypos);

            SetZonePower();
            return true;
        }

        /// <summary>
        ///     Generates power via coal
        /// </summary>
        private void DoCoalPower()
        {
            bool powerOn = CheckZonePower();
            City.CoalCount++;
            if ((City.CityTime%8) == 0)
            {
                RepairZone((char) TileConstants.POWERPLANT, 4);
            }

            City.PowerPlants.Push(new CityLocation(Xpos, Ypos));
        }

        /// <summary>
        ///     Generates power via nuclear and initiates meltdown
        /// </summary>
        private void DoNuclearPower()
        {
            bool powerOn = CheckZonePower();
            if (!City.NoDisasters && PRNG.Next(City.MltdwnTab[City.GameLevel] + 1) == 0)
            {
                City.DoMeltdown(Xpos, Ypos);
                return;
            }

            City.NuclearCount++;
            if ((City.CityTime%8) == 0)
            {
                RepairZone((char) TileConstants.NUCLEAR, 4);
            }

            City.PowerPlants.Push(new CityLocation(Xpos, Ypos));
        }

        /// <summary>
        ///     Does the fire station.
        /// </summary>
        /// <remarks>If the fire brigade does not have power, its effect is halved.</remarks>
        private void DoFireStation()
        {
            bool powerOn = CheckZonePower();
            City.FireStationCount++;
            if ((City.CityTime%8) == 0)
            {
                RepairZone((char) TileConstants.FIRESTATION, 3);
            }

            int z;
            if (powerOn)
            {
                z = City.FireEffect; //if powered, get effect
            }
            else
            {
                z = City.FireEffect/2; // from the funding ratio
            }

            _traffic.MapX = Xpos;
            _traffic.MapY = Ypos;
            if (!_traffic.FindPerimeterRoad())
            {
                z /= 2;
            }

            City.FireStMap[Ypos/8][Xpos/8] += z;
        }

        /// <summary>
        ///     Does the police station.
        /// </summary>
        /// <remarks>If the police station does not have power, its effect is halved.</remarks>
        private void DoPoliceStation()
        {
            bool powerOn = CheckZonePower();
            City.PoliceCount++;
            if ((City.CityTime%8) == 0)
            {
                RepairZone((char) TileConstants.POLICESTATION, 3);
            }

            int z;
            if (powerOn)
            {
                z = City.PoliceEffect;
            }
            else
            {
                z = City.PoliceEffect/2;
            }

            _traffic.MapX = Xpos;
            _traffic.MapY = Ypos;
            if (!_traffic.FindPerimeterRoad())
            {
                z /= 2;
            }

            City.PoliceMap[Ypos/8][Xpos/8] += z;
        }

        /// <summary>
        ///     Does the stadium empty and schedules a game if power is provided.
        /// </summary>
        private void DoStadiumEmpty()
        {
            bool powerOn = CheckZonePower();
            City.StadiumCount++;
            if ((City.CityTime%16) == 0)
            {
                RepairZone((char) TileConstants.STADIUM, 4);
            }

            if (powerOn)
            {
                if (((City.CityTime + Xpos + Ypos)%32) == 0)
                {
                    DrawStadium((char) TileConstants.FULLSTADIUM);
                    City.SetTile(Xpos + 1, Ypos, (char) (TileConstants.FOOTBALLGAME1));
                    City.SetTile(Xpos + 1, Ypos + 1, (char) (TileConstants.FOOTBALLGAME2));
                }
            }
        }

        /// <summary>
        ///     Does the stadium full and will revert to an empty stadium after some time.
        /// </summary>
        private void DoStadiumFull()
        {
            bool powerOn = CheckZonePower();
            City.StadiumCount++;
            if (((City.CityTime + Xpos + Ypos)%8) == 0)
            {
                DrawStadium((char) TileConstants.STADIUM);
            }
        }

        /// <summary>
        ///     Does the airport and generates airplanes and helicopters if power is provided.
        /// </summary>
        private void DoAirport()
        {
            bool powerOn = CheckZonePower();
            City.AirportCount++;
            if ((City.CityTime%8) == 0)
            {
                RepairZone((char) TileConstants.AIRPORT, 6);
            }

            if (powerOn)
            {
                if (PRNG.Next(6) == 0)
                {
                    City.GeneratePlane(Xpos, Ypos);
                }

                if (PRNG.Next(13) == 0)
                {
                    City.GenerateCopter(Xpos, Ypos);
                }
            }
        }

        /// <summary>
        ///     Does the seaport and generates ships if power is provided.
        /// </summary>
        private void DoSeaport()
        {
            bool powerOn = CheckZonePower();
            City.SeaportCount++;
            if ((City.CityTime%16) == 0)
            {
                RepairZone((char) TileConstants.PORT, 4);
            }

            if (powerOn && !City.HasSprite(SpriteKinds.SpriteKind["SHI"]))
            {
                City.GenerateShip();
            }
        }

        /// <summary>
        ///     Place hospital or church if needed.
        /// </summary>
        private void MakeHospital()
        {
            if (City.NeedHospital > 0)
            {
                ZonePlop((char) TileConstants.HOSPITAL);
                City.NeedHospital = 0;
            }
            else if (City.NeedChurch > 0)
            {
                ZonePlop((char) TileConstants.CHURCH);
                City.NeedChurch = 0;
            }
        }

        /// <summary>
        ///     Called when the current tile is the key tile of a hospital or church.
        /// </summary>
        private void DoHospitalChurch()
        {
            bool powerOn = CheckZonePower();
            if (Tile == TileConstants.HOSPITAL)
            {
                City.HospitalCount++;

                if (City.CityTime%16 == 0)
                {
                    RepairZone((char) TileConstants.HOSPITAL, 3);
                }
                if (City.NeedHospital == -1) //too many hospitals
                {
                    if (PRNG.Next(21) == 0)
                    {
                        ZonePlop(TileConstants.RESCLR);
                    }
                }
            }
            else if (Tile == TileConstants.CHURCH)
            {
                City.ChurchCount++;

                if (City.CityTime%16 == 0)
                {
                    RepairZone((char) TileConstants.CHURCH, 3);
                }
                if (City.NeedChurch == -1) //too many churches
                {
                    if (PRNG.Next(21) == 0)
                    {
                        ZonePlop(TileConstants.RESCLR);
                    }
                }
            }
        }

        /// <summary>
        ///     Regenerate the tiles that make up the zone, repairing from fire, etc.
        ///     Only tiles that are not rubble, radioactive, flooded, or on fire will be regenerated.
        /// </summary>
        /// <param name="zoneCenter">the tile value for the "center" tile of the zone.</param>
        /// <param name="zoneSize">integer (3-6) indicating the width/height of the zone.</param>
        private void RepairZone(char zoneCenter, int zoneSize)
        {
            // from the given center tile, figure out what the
            // northwest tile should be
            int zoneBase = zoneCenter - 1 - zoneSize;

            for (int y = 0; y < zoneSize; y++)
            {
                for (int x = 0; x < zoneSize; x++, zoneBase++)
                {
                    int xx = Xpos - 1 + x;
                    int yy = Ypos - 1 + y;

                    if (City.TestBounds(xx, yy))
                    {
                        int thCh = City.GetTile(xx, yy);
                        if (TileConstants.IsZoneCenter(thCh))
                        {
                            continue;
                        }

                        if (TileConstants.IsAnimated(thCh))
                            continue;

                        if (!TileConstants.IsIndestructible(thCh))
                        {
                            //not rubble, radiactive, on fire or flooded

                            City.SetTile(xx, yy, (char) zoneBase);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the current tile is the key tile of a commercial zone.
        /// </summary>
        private void DoCommercial()
        {
            bool powerOn = CheckZonePower();
            City.ComZoneCount++;

            int tpop = TileConstants.CommercialZonePop(Tile);
            City.ComPop += tpop;

            int trafficGood;
            if (tpop > PRNG.Next(6))
            {
                trafficGood = MakeTraffic(ZoneType.COMMERCIAL);
            }
            else
            {
                trafficGood = 1;
            }

            if (trafficGood == -1)
            {
                int value = GetCrValue();
                DoCommercialOut(tpop, value);
                return;
            }

            if (PRNG.Next(8) == 0)
            {
                int locValve = EvalCommercial(trafficGood);
                int zscore = City.ComValve + locValve;

                if (!powerOn)
                    zscore = -500;

                if (trafficGood != 0 &&
                    zscore > -350 &&
                    zscore - 26380 > (PRNG.Next(0x10000) - 0x8000))
                {
                    int value = GetCrValue();
                    DoCommercialIn(tpop, value);
                    return;
                }

                if (zscore < 350 && zscore + 26380 < (PRNG.Next(0x10000) - 0x8000))
                {
                    int value = GetCrValue();
                    DoCommercialOut(tpop, value);
                }
            }
        }


        /// <summary>
        ///     Called when the current tile is the key tile of an industrial zone.
        /// </summary>
        private void DoIndustrial()
        {
            bool powerOn = CheckZonePower();
            City.IndZoneCount++;

            int tpop = TileConstants.IndustrialZonePop(Tile);
            City.IndPop += tpop;

            int trafficGood;
            if (tpop > PRNG.Next(6))
            {
                trafficGood = MakeTraffic(ZoneType.INDUSTRIAL);
            }
            else
            {
                trafficGood = 1;
            }

            if (trafficGood == -1)
            {
                DoIndustrialOut(tpop, PRNG.Next(2));
                return;
            }

            if (PRNG.Next(8) == 0)
            {
                int locValve = evalIndustrial(trafficGood);
                int zscore = City.IndValve + locValve;

                if (!powerOn)
                    zscore = -500;

                if (zscore > -350 &&
                    zscore - 26380 > (PRNG.Next(0x10000) - 0x8000))
                {
                    int value = PRNG.Next(2);
                    DoIndustrialIn(tpop, value);
                    return;
                }

                if (zscore < 350 && zscore + 26380 < (PRNG.Next(0x10000) - 0x8000))
                {
                    int value = PRNG.Next(2);
                    DoIndustrialOut(tpop, value);
                }
            }
        }

        /// <summary>
        ///     Called when the current tile is the key tile of a residential zone.
        /// </summary>
        private void DoResidential()
        {
            bool powerOn = CheckZonePower();
            City.ResZoneCount++;

            int tpop; //population of this zone
            if (Tile == TileConstants.RESCLR)
            {
                tpop = City.DoFreePop(Xpos, Ypos);
            }
            else
            {
                tpop = TileConstants.ResidentialZonePop(Tile);
            }

            City.ResPop += tpop;

            int trafficGood;
            if (tpop > PRNG.Next(36))
            {
                trafficGood = MakeTraffic(ZoneType.RESIDENTIAL);
            }
            else
            {
                trafficGood = 1;
            }

            if (trafficGood == -1)
            {
                int value = GetCrValue();
                DoResidentialOut(tpop, value);
                return;
            }

            if (Tile == TileConstants.RESCLR || PRNG.Next(8) == 0)
            {
                int locValve = EvalResidential(trafficGood);
                int zscore = City.ResValve + locValve;

                if (!powerOn)
                    zscore = -500;

                if (zscore > -350 && zscore - 26380 > (PRNG.Next(0x10000) - 0x8000))
                {
                    if (tpop == 0 && PRNG.Next(4) == 0)
                    {
                        MakeHospital();
                        return;
                    }

                    int value = GetCrValue();
                    DoResidentialIn(tpop, value);
                    return;
                }

                if (zscore < 350 && zscore + 26380 < (PRNG.Next(0x10000) - 0x8000))
                {
                    int value = GetCrValue();
                    DoResidentialOut(tpop, value);
                }
            }
        }


        /// <summary>
        ///     Consider the value of building a single-lot house at certain coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>positive number indicates good place for house to go; zero or a negative number indicates a bad place.</returns>
        private int EvalLot(int x, int y)
        {
            // test for clear lot
            int aTile = City.GetTile(x, y);
            if (aTile != TileConstants.DIRT && !TileConstants.IsResidentialClear(aTile))
            {
                return -1;
            }

            int score = 1;

            int[] dx = {0, 1, 0, -1};
            int[] dy = {-1, 0, 1, 0};
            for (int z = 0; z < 4; z++)
            {
                int xx = x + dx[z];
                int yy = y + dy[z];

                // look for road
                if (City.TestBounds(xx, yy))
                {
                    int tmp = City.GetTile(xx, yy);
                    if (TileConstants.IsRoadAny(tmp) || TileConstants.IsRail(tmp))
                    {
                        score++;
                    }
                }
            }

            return score;
        }

        /// <summary>
        ///     Build a single-lot house on the current residential zone.
        /// </summary>
        /// <param name="value">The value between 0 and 3.</param>
        private void BuildHouse(int value)
        {
            //assert value >= 0 && value <= 3;

            int[] zeX = {0, -1, 0, 1, -1, 1, -1, 0, 1};
            int[] zeY = {0, -1, -1, -1, 0, 0, 1, 1, 1};

            int bestLoc = 0;
            int hscore = 0;

            for (int z = 1; z < 9; z++)
            {
                int xx = Xpos + zeX[z];
                int yy = Ypos + zeY[z];

                if (City.TestBounds(xx, yy))
                {
                    int score = EvalLot(xx, yy);

                    if (score != 0)
                    {
                        if (score > hscore)
                        {
                            hscore = score;
                            bestLoc = z;
                        }

                        if ((score == hscore) && PRNG.Next(8) == 0)
                        {
                            bestLoc = z;
                        }
                    }
                }
            }

            if (bestLoc != 0)
            {
                int xx = Xpos + zeX[bestLoc];
                int yy = Ypos + zeY[bestLoc];
                int houseNumber = value*3 + PRNG.Next(3);
                //assert houseNumber >= 0 && houseNumber < 12;

                //assert city.testBounds(xx, yy);
                City.SetTile(xx, yy, (char) (TileConstants.HOUSE + houseNumber));
            }
        }

        /// <summary>
        ///     Commercial companies move in
        /// </summary>
        /// <param name="pop">The pop.</param>
        /// <param name="value">The value.</param>
        private void DoCommercialIn(int pop, int value)
        {
            int z = City.GetLandValue(Xpos, Ypos)/32;
            if (pop > z)
                return;

            if (pop < 5)
            {
                ComPlop(pop, value);
                AdjustRog(8);
            }
        }

        /// <summary>
        ///     Industry moves in
        /// </summary>
        /// <param name="pop">The pop.</param>
        /// <param name="value">The value.</param>
        private void DoIndustrialIn(int pop, int value)
        {
            if (pop < 4)
            {
                IndPlop(pop, value);
                AdjustRog(8);
            }
        }

        /// <summary>
        ///     Residents move in and smaller houses get converted to bigger ones
        /// </summary>
        /// <param name="pop">The pop.</param>
        /// <param name="value">The value.</param>
        private void DoResidentialIn(int pop, int value)
        {
            //assert value >= 0 && value <= 3;

            int z = City.PollutionMem[Ypos/2][Xpos/2];
            if (z > 128)
                return;

            if (Tile == TileConstants.RESCLR)
            {
                if (pop < 8)
                {
                    BuildHouse(value);
                    AdjustRog(1);
                    return;
                }

                if (City.GetPopulationDensity(Xpos, Ypos) > 64)
                {
                    ResidentialPlop(0, value);
                    AdjustRog(8);
                    return;
                }
                return;
            }

            if (pop < 40)
            {
                ResidentialPlop(pop/8 - 1, value);
                AdjustRog(8);
            }
        }

        private void ComPlop(int density, int value)
        {
            int baseTile = (value*5 + density)*9 + TileConstants.CZB;
            ZonePlop(baseTile);
        }

        private void IndPlop(int density, int value)
        {
            int baseTile = (value*4 + density)*9 + TileConstants.IZB;
            ZonePlop(baseTile);
        }

        private void ResidentialPlop(int density, int value)
        {
            int baseTile = (value*4 + density)*9 + TileConstants.RZB;
            ZonePlop(baseTile);
        }

        /// <summary>
        ///     Commercial companies move out
        /// </summary>
        /// <param name="pop">The pop.</param>
        /// <param name="value">The value.</param>
        private void DoCommercialOut(int pop, int value)
        {
            if (pop > 1)
            {
                ComPlop(pop - 2, value);
                AdjustRog(-8);
            }
            else if (pop == 1)
            {
                ZonePlop(TileConstants.COMCLR);
                AdjustRog(-8);
            }
        }

        /// <summary>
        ///     Industry moves out
        /// </summary>
        /// <param name="pop">The pop.</param>
        /// <param name="value">The value.</param>
        private void DoIndustrialOut(int pop, int value)
        {
            if (pop > 1)
            {
                IndPlop(pop - 2, value);
                AdjustRog(-8);
            }
            else if (pop == 1)
            {
                ZonePlop(TileConstants.INDCLR);
                AdjustRog(-8);
            }
        }

        /// <summary>
        ///     Residents move out, bigger houses get converted to smaller ones.
        /// </summary>
        /// <param name="pop">The pop.</param>
        /// <param name="value">The value.</param>
        private void DoResidentialOut(int pop, int value)
        {
            //assert value >= 0 && value < 4;

            int[] brdr = {0, 3, 6, 1, 4, 7, 2, 5, 8};

            if (pop == 0)
                return;

            if (pop > 16)
            {
                // downgrade to a lower-density full-size residential zone
                ResidentialPlop((pop - 24)/8, value);
                AdjustRog(-8);
                return;
            }

            if (pop == 16)
            {
                // downgrade from full-size zone to 8 little houses

                bool pwr = City.IsTilePowered(Xpos, Ypos);
                City.SetTile(Xpos,Ypos,TileConstants.RESCLR);
                City.SetTilePower(Xpos, Ypos, pwr);
                
                for (int x = Xpos - 1; x <= Xpos + 1; x++)
                {
                    for (int y = Ypos - 1; y <= Ypos + 1; y++)
                    {
                        if (City.TestBounds(x, y))
                        {
                            if (!(x == Xpos && y == Ypos))
                            {
                                // pick a random small house
                                int houseNumber = value*3 + PRNG.Next(3);
                                City.SetTile(x, y, (char) (TileConstants.HOUSE + houseNumber));
                            }
                        }
                    }
                }

                AdjustRog(-8);
                return;
            }

            if (pop < 16)
            {
                // Remove one little house
                AdjustRog(-1);
                int z = 0;

                for (int x = Xpos - 1; x <= Xpos + 1; x++)
                {
                    for (int y = Ypos - 1; y <= Ypos + 1; y++)
                    {
                        if (City.TestBounds(x, y))
                        {
                            int loc = City.Map[y][x] & TileConstants.LOMASK;
                            if (loc >= TileConstants.LHTHR && loc <= TileConstants.HHTHR)
                            {
                                //little house
                                City.SetTile(x, y, (char) (brdr[z] + TileConstants.RESCLR - 4));
                                return;
                            }
                        }
                        z++;
                    }
                }
            }
        }

        /// <summary>
        ///     Evaluates the zone value of the current commercial zone location.
        /// </summary>
        /// <param name="traf"></param>
        /// <returns>integer between -3000 and 3000</returns>
        /// <remarks>Same meaning as evalResidential</remarks>
        private int EvalCommercial(int traf)
        {
            if (traf < 0)
                return -3000;

            return City.ComRate[Ypos/8][Xpos/8];
        }


        /// <summary>
        ///     Evaluates the zone value of the current industrial zone location.
        /// </summary>
        /// <param name="traf">The traf.</param>
        /// <returns>integer between -3000 and 3000.</returns>
        /// <remarks>Same meaning as evalResidential.</remarks>
        private int evalIndustrial(int traf)
        {
            if (traf < 0)
                return -1000;
            return 0;
        }


        /// <summary>
        ///     Evaluates the zone value of the current residential zone location.
        /// </summary>
        /// <param name="traf">The traf.</param>
        /// <returns>
        ///     integer between -3000 and 3000. The higher the number, the more likely the zone is to GROW; the lower the
        ///     number, the more likely the zone is to SHRINK.
        /// </returns>
        private int EvalResidential(int traf)
        {
            if (traf < 0)
                return -3000;

            int value = City.GetLandValue(Xpos, Ypos);
            value -= City.PollutionMem[Ypos/2][Xpos/2];

            if (value < 0)
                value = 0; //cap at 0
            else
                value *= 32;

            if (value > 6000)
                value = 6000; //cap at 6000

            return value - 3000;
        }


        /// <summary>
        ///     Gets the land-value class (0-3) for the current residential or commercial zone location.
        /// </summary>
        /// <returns>integer from 0 to 3, 0 is the lowest-valued zone, and 3 is the highest-valued zone.</returns>
        private int GetCrValue()
        {
            int lval = City.GetLandValue(Xpos, Ypos);
            lval -= City.PollutionMem[Ypos/2][Xpos/2];

            if (lval < 30)
                return 0;

            if (lval < 80)
                return 1;

            if (lval < 150)
                return 2;

            return 3;
        }


        /// <summary>
        ///     Record a zone's population change to the rate-of-growth map
        ///     An adjustment of +/- 1 corresponds to one little house.
        ///     An adjustment of +/- 8 corresponds to a full-size zone.
        /// </summary>
        /// <param name="amount">the positive or negative adjustment to record.</param>
        private void AdjustRog(int amount)
        {
            City.RateOgMem[Ypos/8][Xpos/8] += 4*amount;
        }


        /// <summary>
        ///     Place tiles for a stadium (full or empty).
        /// </summary>
        /// <param name="zoneCenter">either STADIUM or FULLSTADIUM.</param>
        private void DrawStadium(int zoneCenter)
        {
            int zoneBase = zoneCenter - 1 - 4;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    City.SetTile(Xpos - 1 + x, Ypos - 1 + y, (char) zoneBase);
                    zoneBase++;
                }
            }
            City.SetTilePower(Xpos, Ypos, true);
        }


        /// <summary>
        ///     Makes the traffic.
        /// </summary>
        /// <param name="zoneType">Type of the zone.</param>
        /// <returns>1 if traffic "passed", 0 if traffic "failed", -1 if no roads found</returns>
        private int MakeTraffic(ZoneType zoneType)
        {
            _traffic.MapX = Xpos;
            _traffic.MapY = Ypos;
            _traffic.SourceZone = zoneType;
            return _traffic.MakeTraffic();
        }
    }
}