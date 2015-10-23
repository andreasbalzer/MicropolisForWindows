using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Engine.Controller;
using Engine.Model.Enums;

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
    ///     The main simulation engine for Micropolis. The front-end should call animate() periodically to move the simulation
    ///     forward in time.
    /// </summary>
    public class Micropolis
    {

        public int ScoreWait { get; set; }
        public Scenario ScoreType { get; set; }
        public int DisasterWait { get; set; }
        public Scenario DisasterEvent { get; set; }
        public Scenario Scenario { get; set; }

        /// <summary>
        ///     The random number generator used in the game
        /// </summary>
        public static readonly Random DEFAULT_PRNG = new Random();

        internal static readonly int DEFAULT_WIDTH = 120;
        internal static readonly int DEFAULT_HEIGHT = 100;
        private static readonly int VALVE_RATE = 2;
        public static readonly int CENSUS_RATE = 4;
        private static readonly int TAX_FREQ = 48;

        private static readonly int[] TAX_TABLE =
        {
            200, 150, 120, 100, 80, 50, 30, 0, -10, -40, -100,
            -150, -200, -250, -300, -350, -400, -450, -500, -550, -600
        };

        private static readonly double[] R_LEVELS = {0.7, 0.9, 1.2};


        /// <summary>
        /// Tax income multiplier, for various difficulty settings.
        /// </summary>
        private static readonly double[] F_LEVELS = {1.4, 1.2, 0.8};

        /// <summary>
        ///     Annual maintenance cost of each police station.
        /// </summary>
        private static readonly int POLICE_STATION_MAINTENANCE = 100;

        /// <summary>
        ///     Annual maintenance cost of each fire station.
        /// </summary>
        private static readonly int FIRE_STATION_MAINTENANCE = 100;

        /// <summary>
        ///     The budget available to the player
        /// </summary>
        public readonly CityBudget Budget;

        private readonly List<IEarthquakeListener> _earthquakeListeners = new List<IEarthquakeListener>();
        private readonly List<IListener> _listeners = new List<IListener>();
        private readonly List<IMapListener> _mapListeners = new List<IMapListener>();
        public int[] MltdwnTab = {30000, 20000, 10000};

        /// <summary>
        ///     random number generator used in this game instance
        /// </summary>
        public Random Prng { get; set; }

        private bool _autoBudget;

        /// <summary>
        ///     The automatic budget setting
        /// </summary>
        public bool AutoBudget
        {
            get { return _autoBudget; }
            set { _autoBudget = value; }
        }

        /// <summary>
        ///     The automatic bulldoze setting
        /// </summary>
        public bool AutoBulldoze = true;

        public bool AutoGo = true;

        /// <summary>
        ///     The center mass x
        /// </summary>
        public int CenterMassX;

        /// <summary>
        ///     The center mass y
        /// </summary>
        public int CenterMassY;

        /// <summary>
        ///     commerce demands airport,   caps comValve at 0
        /// </summary>
        public bool ComCap;

        /// <summary>
        ///     The commercial valve, ranges between -1500 and 1500
        /// </summary>
        public int ComValve;

        /// <summary>
        ///     The crash location, may be null
        /// </summary>
        public CityLocation CrashLocation;

        /// <summary>
        ///     The crime average
        /// </summary>
        public int CrimeAverage;

        /// <summary>
        ///     The crime maximum location x
        /// </summary>
        public int CrimeMaxLocationX;

        /// <summary>
        ///     The crime maximum location y
        /// </summary>
        public int CrimeMaxLocationY;

        /// <summary>
        ///     The crime ramp
        /// </summary>
        public int CrimeRamp;

        /// <summary>
        ///     The city evaluation
        /// </summary>
        public CityEval Evaluation;

        public List<FinancialHistory> FinancialHistory = new List<FinancialHistory>();

        /// <summary>
        ///     The game difficulty
        /// </summary>
        public int GameLevel;

        public History History = new History();

        /// <summary>
        ///     industry demands sea port,  caps indValve at 0
        /// </summary>
        public bool IndCap;

        /// <summary>
        ///     The industry valve, ranges between -1500 and 1500
        /// </summary>
        public int IndValve;

        /// <summary>
        ///     The land value average
        /// </summary>
        public int LandValueAverage;

        /// <summary>
        ///     The total population in the city in the last period
        /// </summary>
        public int LastCityPop;

        /// <summary>
        ///     The last fire station count
        /// </summary>
        /// <remarks>
        ///     used in generateBudget()
        /// </remarks>
        public int LastFireStationCount;

        /// <summary>
        ///     The last police count
        /// </summary>
        /// <remarks>
        ///     used in generateBudget()
        /// </remarks>
        public int LastPoliceCount;

        /// <summary>
        ///     The last rail total
        /// </summary>
        /// <remarks>
        ///     used in generateBudget()
        /// </remarks>
        public int LastRailTotal;

        /// <summary>
        ///     The last road total
        /// </summary>
        /// <remarks>
        ///     used in generateBudget()
        /// </remarks>
        public int LastRoadTotal;

        /// <summary>
        ///     The last total pop
        /// </summary>
        /// <remarks>
        ///     used in generateBudget()
        /// </remarks>
        public int LastTotalPop;

        /// <summary>
        ///     The meltdown location, may be null
        /// </summary>
        public CityLocation MeltdownLocation;

        /// <summary>
        ///     The need church, -1 too many already, 0 just right, 1 not enough
        /// </summary>
        public int NeedChurch;

        /// <summary>
        ///     The need hospital, -1 too many already, 0 just right, 1 not enough
        /// </summary>
        public int NeedHospital;

        /// <summary>
        ///     The no disasters setting
        /// </summary>
        public bool NoDisasters = false;

        /// <summary>
        ///     The pollute ramp
        /// </summary>
        public int PolluteRamp;

        /// <summary>
        ///     The pollution average
        /// </summary>
        public int PollutionAverage;

        /// <summary>
        ///     The pollution maximum location x
        /// </summary>
        public int PollutionMaxLocationX;

        /// <summary>
        ///     The pollution maximum location y
        /// </summary>
        public int PollutionMaxLocationY;

        public Stack<CityLocation> PowerPlants = new Stack<CityLocation>();

        /// <summary>
        ///     residents demand a stadium, caps resValve at 0
        /// </summary>
        public bool ResCap;

        /// <summary>
        ///     The residential valve, ranges between -2000 and 2000, updated by setValves
        /// </summary>
        public int ResValve;

        /// <summary>
        ///     The speed of the simulation
        /// </summary>
        public Speed SimSpeed = Speeds.Speed["NORMAL"];

        /// <summary>
        ///     The sprites to be used on map
        /// </summary>
        public List<Sprite> Sprites = new List<Sprite>();

        private Dictionary<String, TileBehavior> _tileBehaviors;

        /// <summary>
        ///     The total population in the city in the current period
        /// </summary>
        public int TotalPop;

        /// <summary>
        ///     The traffic average
        /// </summary>
        public int TrafficAverage;

        /// <summary>
        ///     The traffic maximum location x
        /// </summary>
        public int TrafficMaxLocationX;

        /// <summary>
        ///     The traffic maximum location y
        /// </summary>
        public int TrafficMaxLocationY;

        #region IListener Adders and Removers

        /// <summary>
        ///     Adds the listener to receive information upon change.
        /// </summary>
        /// <param name="l">The listener.</param>
        public void AddListener(IListener l)
        {
            _listeners.Add(l);
        }

        /// <summary>
        ///     Removes the listener so it is no longer notified upon change.
        /// </summary>
        /// <param name="l">The listener.</param>
        public void RemoveListener(IListener l)
        {
            _listeners.Remove(l);
        }

        /// <summary>
        ///     Adds the earthquake listener.
        /// </summary>
        /// <param name="l">The listener.</param>
        public void AddEarthquakeListener(IEarthquakeListener l)
        {
            _earthquakeListeners.Add(l);
        }

        /// <summary>
        ///     Removes the earthquake listener.
        /// </summary>
        /// <param name="l">The listener.</param>
        public void RemoveEarthquakeListener(IEarthquakeListener l)
        {
            _earthquakeListeners.Remove(l);
        }

        /// <summary>
        ///     Adds the map listener.
        /// </summary>
        /// <param name="l">The listener.</param>
        public void AddMapListener(IMapListener l)
        {
            _mapListeners.Add(l);
        }

        /// <summary>
        ///     Removes the map listener.
        /// </summary>
        /// <param name="l">The listener.</param>
        public void RemoveMapListener(IMapListener l)
        {
            _mapListeners.Remove(l);
        }

        #endregion

        #region budget stuff

        /// <summary>
        ///     net change in TotalFunds in previous year
        /// </summary>
        public int CashFlow;

        /// <summary>
        ///     The city tax, default is 7
        /// </summary>
        public int CityTax = 7;

        /// <summary>
        ///     The fire effect, default is 1000
        /// </summary>
        public int FireEffect = 1000;

        /// <summary>
        ///     The fire percent
        /// </summary>
        public double FirePercent = 1.0;

        /// <summary>
        ///     The police effect, default is 1000
        /// </summary>
        public int PoliceEffect = 1000;

        /// <summary>
        ///     The police percent
        /// </summary>
        public double PolicePercent = 1.0;

        /// <summary>
        ///     The road effect, default is 32
        /// </summary>
        public int RoadEffect = 32;

        /// <summary>
        ///     The road percent
        /// </summary>
        public double RoadPercent = 1.0;

        /// <summary>
        ///     The tax effect, default is 7
        /// </summary>
        public int TaxEffect = 7;

        #endregion

        #region power

        /// <summary>
        ///     The new power, possibly whether new power plants are required
        /// </summary>
        public bool NewPower;

        #endregion

        #region flood

        /// <summary>
        ///     number of turns the flood will last
        /// </summary>
        public int FloodCnt;

        /// <summary>
        ///     The flood x-coordinate
        /// </summary>
        public int FloodX;

        /// <summary>
        ///     The flood y-coordinate
        /// </summary>
        public int FloodY;

        #endregion

        #region time

        /// <summary>
        ///     animation cycle (mod 960)
        /// </summary>
        public int Acycle;

        /// <summary>
        ///     counts "weeks" (actually, 1/48'ths years)
        /// </summary>
        public int CityTime;

        /// <summary>
        ///     counts simulation steps (mod 1024)
        /// </summary>
        public int Fcycle;

        /// <summary>
        ///     same as cityTime, except mod 1024
        /// </summary>
        public int Scycle;

        #endregion

        #region census numbers

        /// <summary>
        ///     The airport count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int AirportCount;

        /// <summary>
        ///     The church count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int ChurchCount;

        /// <summary>
        ///     The coal count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int CoalCount;

        /// <summary>
        ///     The COM pop
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int ComPop;

        /// <summary>
        ///     The COM zone count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int ComZoneCount;

        /// <summary>
        ///     The fire pop
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int FirePop;

        /// <summary>
        ///     The fire station count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int FireStationCount;

        /// <summary>
        ///     The hospital count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int HospitalCount;

        /// <summary>
        ///     The ind pop
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int IndPop;

        /// <summary>
        ///     The ind zone count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int IndZoneCount;

        /// <summary>
        ///     The nuclear count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int NuclearCount;

        /// <summary>
        ///     The police count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int PoliceCount;

        /// <summary>
        ///     The powered zone count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int PoweredZoneCount;

        /// <summary>
        ///     The rail total
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int RailTotal;

        /// <summary>
        ///     The resource pop
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int ResPop;

        /// <summary>
        ///     The resource zone count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int ResZoneCount;

        /// <summary>
        ///     The road total
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int RoadTotal;

        /// <summary>
        ///     The seaport count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int SeaportCount;

        /// <summary>
        ///     The stadium count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int StadiumCount;

        /// <summary>
        ///     The unpowered zone count
        /// </summary>
        /// <remarks>
        ///     census numbers, reset in phase 0 of each cycle, summed during map scan
        /// </remarks>
        public int UnpoweredZoneCount;

        #endregion

        #region full size arrays

        /// <summary>
        ///     The map (full size array)
        /// </summary>
        public char[][] Map;

        /// <summary>
        ///     The power map (full size array)
        /// </summary>
        public bool[][] PowerMap;

        #endregion

        #region half-size arrays

        /// <summary>
        ///     For each 2x2 section of the city, the crime level of the city (0-250).
        ///     0 is no crime; 250 is maximum crime.
        ///     Updated each cycle by crimeScan(); affects land value.
        /// </summary>
        public int[][] CrimeMem;

        /// <summary>
        ///     For each 2x2 section of the city, the land value of the city (0-250).
        ///     0 is lowest land value; 250 is maximum land value.
        ///     Updated each cycle by ptlScan().
        /// </summary>
        private int[][] _landValueMem;

        /// <summary>
        ///     For each 2x2 section of the city, the pollution level of the city (0-255).
        ///     0 is no pollution; 255 is maximum pollution.
        ///     Updated each cycle by ptlScan(); affects land value.
        /// </summary>
        public int[][] PollutionMem;

        /// <summary>
        ///     For each 2x2 section of the city, the population density (0-?).
        ///     Used for map overlays and as a factor for crime rates.
        /// </summary>
        public int[][] PopDensity;

        /// <summary>
        ///     For each 2x2 section of the city, the traffic density (0-255).
        ///     If less than 64, no cars are animated.
        ///     If between 64 and 192, then the "light traffic" animation is used.
        ///     If 192 or higher, then the "heavy traffic" animation is used.
        /// </summary>
        private int[][] _trfDensity;

        #endregion

        #region quarter-size arrays

        /// <summary>
        ///     For each 4x4 section of the city, an integer representing the natural land features in the vicinity of this part of
        ///     the city.
        /// </summary>
        private int[][] _terrainMem;

        #endregion

        #region eighth-size arrays

        /// <summary>
        ///     For each 8x8 section of city, this is an integer between 0 and 64, with higher numbers being closer to the center
        ///     of the city.
        /// </summary>
        public int[][] ComRate;

        /// <summary>
        ///     firestations reach- used for overlay graphs
        /// </summary>
        public int[][] FireRate;

        /// <summary>
        ///     firestations- cleared and rebuilt each sim cycle
        /// </summary>
        public int[][] FireStMap;

        /// <summary>
        ///     police stations- cleared and rebuilt each sim cycle
        /// </summary>
        public int[][] PoliceMap;

        /// <summary>
        ///     police stations reach- used for overlay graphs
        /// </summary>
        public int[][] PoliceMapEffect;

        /// <summary>
        ///     For each 8x8 section of the city, the rate of growth. Capped to a number between -200 and 200. Used for reporting
        ///     purposes only; the number has no affect.
        /// </summary>
        /// <remarks>
        ///     rate of growth?
        /// </remarks>
        public int[][] RateOgMem;

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="Micropolis" /> class.
        /// </summary>
        public Micropolis()
            : this(DEFAULT_WIDTH, DEFAULT_HEIGHT)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Micropolis" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Micropolis(int width, int height)
        {
            ScoreType = Scenarios.Items[ScenarioENUM.SC_NONE];
            ScoreWait = 0;
            DisasterWait = 1;
            DisasterEvent = Scenarios.Items[ScenarioENUM.SC_NONE];
            Scenario = Scenarios.Items[ScenarioENUM.SC_NONE];
            Budget = new CityBudget(this);
            Prng = DEFAULT_PRNG;
            Evaluation = new CityEval(this);
            Init(width, height);
            InitTileBehaviors();
        }

        /// <summary>
        ///     Spends the specified amount of money.
        /// </summary>
        /// <param name="amount">The amount of money.</param>
        public void Spend(int amount)
        {
            Budget.TotalFunds -= amount;
            FireFundsChanged();
        }

        /// <summary>
        ///     Initializes the specified map with the specified size.
        /// </summary>
        /// <param name="width">The width of the map.</param>
        /// <param name="height">The height of the map.</param>
        protected void Init(int width, int height)
        {
            Map = new char[height][];
            for (int i = 0; i < height; i++)
            {
                Map[i] = new char[width];
            }
            PowerMap = new bool[height][];
            for (int i = 0; i < height; i++)
            {
                PowerMap[i] = new bool[width];
            }


            int hX = (width + 1)/2;
            int hY = (height + 1)/2;

            _landValueMem = new int[hY][];
            for (int i = 0; i < hY; i++)
            {
                _landValueMem[i] = new int[hX];
            }
            PollutionMem = new int[hY][];
            for (int i = 0; i < hY; i++)
            {
                PollutionMem[i] = new int[hX];
            }
            CrimeMem = new int[hY][];
            for (int i = 0; i < hY; i++)
            {
                CrimeMem[i] = new int[hX];
            }
            PopDensity = new int[hY][];
            for (int i = 0; i < hY; i++)
            {
                PopDensity[i] = new int[hX];
            }
            _trfDensity = new int[hY][];
            for (int i = 0; i < hY; i++)
            {
                _trfDensity[i] = new int[hX];
            }

            int qX = (width + 3)/4;
            int qY = (height + 3)/4;

            _terrainMem = new int[qY][];
            for (int i = 0; i < qY; i++)
            {
                _terrainMem[i] = new int[qX];
            }

            int smX = (width + 7)/8;
            int smY = (height + 7)/8;

            RateOgMem = new int[smY][];
            for (int i = 0; i < smY; i++)
            {
                RateOgMem[i] = new int[smX];
            }
            FireStMap = new int[smY][];
            for (int i = 0; i < smY; i++)
            {
                FireStMap[i] = new int[smX];
            }
            PoliceMap = new int[smY][];
            for (int i = 0; i < smY; i++)
            {
                PoliceMap[i] = new int[smX];
            }
            PoliceMapEffect = new int[smY][];
            for (int i = 0; i < smY; i++)
            {
                PoliceMapEffect[i] = new int[smX];
            }
            FireRate = new int[smY][];
            for (int i = 0; i < smY; i++)
            {
                FireRate[i] = new int[smX];
            }
            ComRate = new int[smY][];
            for (int i = 0; i < smY; i++)
            {
                ComRate[i] = new int[smX];
            }

            CenterMassX = hX;
            CenterMassY = hY;
        }

        /// <summary>
        ///     Gets the map width.
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return Map[0].Length;
        }

        /// <summary>
        ///     Gets the map height.
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return Map.Length;
        }

        /// <summary>
        ///     Gets the tile contents at x/y-coordinate.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public char GetTile(int xpos, int ypos)
        {
            return (char) (Map[ypos][xpos] & TileConstants.LOMASK);
        }

        /// <summary>
        ///     Gets the tile raw contents at x/y-coordinate.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public char GetTileRaw(int xpos, int ypos)
        {
            return Map[ypos][xpos];
        }

        /// <summary>
        ///     Determines whether the tile eff is dozeable.
        /// </summary>
        /// <param name="eff">The tile effect Ifc.</param>
        /// <returns></returns>
        public bool IsTileDozeable(IToolEffectIfc eff)
        {
            int myTile = eff.GetTile(0, 0);
            TileSpec ts = Tiles.Get(myTile);
            if (ts.CanBulldoze)
            {
                return true;
            }

            if (ts.Owner != null)
            {
                // part of a zone; only bulldozeable if the owner tile is
                // no longer intact.

                int baseTile = eff.GetTile(-ts.OwnerOffsetX, -ts.OwnerOffsetY);
                return ts.Owner.TileNumber != baseTile;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the tile at coordinates is dozeable.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public bool IsTileDozeable(int xpos, int ypos)
        {
            return IsTileDozeable(
                new ToolEffect(this, xpos, ypos)
                );
        }

        /// <summary>
        ///     Determines whether the tile at coordinates is powered.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public bool IsTilePowered(int xpos, int ypos)
        {
            return (GetTileRaw(xpos, ypos) & TileConstants.PWRBIT) == TileConstants.PWRBIT;
        }

        /// <summary>
        ///     Sets the tile at coordinates.
        /// </summary>
        /// <remarks>
        ///     Note: this method clears the PWRBIT of the given location.
        /// </remarks>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <param name="newTile">The new tile.</param>
        public void SetTile(int xpos, int ypos, int newTile)
        {
            if (Map[ypos][xpos] != newTile)
            {
                Map[ypos][xpos] = (char) newTile;
                FireTileChanged(xpos, ypos);
            }
        }
        /// <summary>
        /// Sets power for the specfic tile
        /// </summary>
        /// <param name="xpos">x-position</param>
        /// <param name="ypos">y-position</param>
        /// <param name="power">true, if power, otherwise false</param>
        public void SetTilePower(int xpos, int ypos, bool power) 
     	{
            Map[ypos][xpos] = (char)(Map[ypos][xpos] & (~TileConstants.PWRBIT) | (power ? TileConstants.PWRBIT : 0)); 
     	} 


        /// <summary>
        ///     Tests the coordinate for validity.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public bool TestBounds(int xpos, int ypos)
        {
            return xpos >= 0 && xpos < GetWidth() &&
                   ypos >= 0 && ypos < GetHeight();
        }

        /// <summary>
        ///     Determines whether the specified coordinate has power.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool HasPower(int x, int y)
        {
            return PowerMap[y][x];
        }


        /// <summary>
        ///     Checks whether the next call to animate() will collect taxes and process the budget.
        /// </summary>
        /// <returns></returns>
        public bool IsBudgetTime()
        {
            return (
                CityTime != 0 &&
                (CityTime%TAX_FREQ) == 0 &&
                ((Fcycle + 1)%16) == 10 &&
                ((Acycle + 1)%2) == 0
                );
        }

        /// <summary>
        ///     Advances the simulation by one step.
        /// </summary>
        private void Step()
        {
            Fcycle = (Fcycle + 1)%1024;
            Simulate(Fcycle%16);
        }

        /// <summary>
        ///     Clears the census.
        /// </summary>
        private void ClearCensus()
        {
            PoweredZoneCount = 0;
            UnpoweredZoneCount = 0;
            FirePop = 0;
            RoadTotal = 0;
            RailTotal = 0;
            ResPop = 0;
            ComPop = 0;
            IndPop = 0;
            ResZoneCount = 0;
            ComZoneCount = 0;
            IndZoneCount = 0;
            HospitalCount = 0;
            ChurchCount = 0;
            PoliceCount = 0;
            FireStationCount = 0;
            StadiumCount = 0;
            CoalCount = 0;
            NuclearCount = 0;
            SeaportCount = 0;
            AirportCount = 0;
            PowerPlants.Clear();

            for (int y = 0; y < FireStMap.Length; y++)
            {
                for (int x = 0; x < FireStMap[y].Length; x++)
                {
                    FireStMap[y][x] = 0;
                    PoliceMap[y][x] = 0;
                }
            }
        }

        /// <summary>
        ///     Simulates the specified mod16.
        /// </summary>
        /// <param name="mod16">The mod16.</param>
        /// <exception cref="Exception">unreachable</exception>
        private void Simulate(int mod16)
        {
            int band = GetWidth()/8;

            switch (mod16)
            {
                case 0:
                    Scycle = (Scycle + 1)%1024;
                    CityTime++;
                    if (Scycle%2 == 0)
                    {
                        SetValves();
                    }
                    ClearCensus();
                    break;

                case 1:
                    MapScan(0*band, 1*band);
                    break;

                case 2:
                    MapScan(1*band, 2*band);
                    break;

                case 3:
                    MapScan(2*band, 3*band);
                    break;

                case 4:
                    MapScan(3*band, 4*band);
                    break;

                case 5:
                    MapScan(4*band, 5*band);
                    break;

                case 6:
                    MapScan(5*band, 6*band);
                    break;

                case 7:
                    MapScan(6*band, 7*band);
                    break;

                case 8:
                    MapScan(7*band, GetWidth());
                    break;

                case 9:
                    if (CityTime%CENSUS_RATE == 0)
                    {
                        TakeCensus();

                        if (CityTime%(CENSUS_RATE*12) == 0)
                        {
                            TakeCensus2();
                        }

                        FireCensusChanged();
                    }

                    CollectTaxPartial();

                    if (CityTime%TAX_FREQ == 0)
                    {
                        CollectTax();
                        Evaluation.CityEvaluation();
                    }
                    break;

                case 10:
                    if (Scycle%5 == 0)
                    {
                        // every ~10 weeks
                        DecRogMem();
                    }
                    DecTrafficMem();
                    FireMapOverlayDataChanged(MapState.TRAFFIC_OVERLAY); //TDMAP
                    FireMapOverlayDataChanged(MapState.TRANSPORT); //RDMAP
                    FireMapOverlayDataChanged(MapState.ALL); //ALMAP
                    FireMapOverlayDataChanged(MapState.RESIDENTIAL); //REMAP
                    FireMapOverlayDataChanged(MapState.COMMERCIAL); //COMAP
                    FireMapOverlayDataChanged(MapState.INDUSTRIAL); //INMAP
                    DoMessages();
                    break;

                case 11:
                    PowerScan();
                    FireMapOverlayDataChanged(MapState.POWER_OVERLAY);
                    NewPower = true;
                    break;

                case 12:
                    PtlScan();
                    break;

                case 13:
                    CrimeScan();
                    break;

                case 14:
                    PopDenScan();
                    break;

                case 15:
                    FireAnalysis();
                    DoDisasters();
                    break;

                default:
                    throw new Exception("unreachable");
            }
        }

        /// <summary>
        ///     Computes the pop density.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="tile">The tile.</param>
        /// <returns>population density</returns>
        private int ComputePopDen(int x, int y, char tile)
        {
            if (tile == TileConstants.RESCLR)
                return DoFreePop(x, y);

            if (tile < TileConstants.COMBASE)
                return TileConstants.ResidentialZonePop(tile);

            if (tile < TileConstants.INDBASE)
                return TileConstants.CommercialZonePop(tile)*8;

            if (tile < TileConstants.PORTBASE)
                return TileConstants.IndustrialZonePop(tile)*8;

            return 0;
        }

        private static int[][] DoSmooth(int[][] tem)
        {
            int h = tem.Length;
            int w = tem[0].Length;
            var tem2 = new int[h][];
            for (int i = 0; i < tem2.Length; i++)
            {
                tem2[i] = new int[w];
            }

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int z = tem[y][x];
                    if (x > 0)
                        z += tem[y][x - 1];
                    if (x + 1 < w)
                        z += tem[y][x + 1];
                    if (y > 0)
                        z += tem[y - 1][x];
                    if (y + 1 < h)
                        z += tem[y + 1][x];
                    z /= 4;
                    if (z > 255)
                        z = 255;
                    tem2[y][x] = z;
                }
            }

            return tem2;
        }

        /// <summary>
        ///     Calculates the center mass via population density scan.
        /// </summary>
        public void CalculateCenterMass()
        {
            PopDenScan();
        }

        /// <summary>
        ///     Performs a population density scan
        /// </summary>
        private void PopDenScan()
        {
            int xtot = 0;
            int ytot = 0;
            int zoneCount = 0;
            int width = GetWidth();
            int height = GetHeight();
            var tem = new int[(height + 1)/2][];
            for (int i = 0; i < tem.Length; i++)
            {
                tem[i] = new int[(width + 1)/2];
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    char tile = GetTile(x, y);
                    if (TileConstants.IsZoneCenter(tile))
                    {
                        int den = ComputePopDen(x, y, tile)*8;
                        if (den > 254)
                            den = 254;
                        tem[y/2][x/2] = den;
                        xtot += x;
                        ytot += y;
                        zoneCount++;
                    }
                }
            }

            tem = DoSmooth(tem);
            tem = DoSmooth(tem);
            tem = DoSmooth(tem);

            for (int x = 0; x < (width + 1)/2; x++)
            {
                for (int y = 0; y < (height + 1)/2; y++)
                {
                    PopDensity[y][x] = 2*tem[y][x];
                }
            }

            DistIntMarket(); //set ComRate

            // find center of mass for city
            if (zoneCount != 0)
            {
                CenterMassX = xtot/zoneCount;
                CenterMassY = ytot/zoneCount;
            }
            else
            {
                CenterMassX = (width + 1)/2;
                CenterMassY = (height + 1)/2;
            }

            FireMapOverlayDataChanged(MapState.POPDEN_OVERLAY); //PDMAP
            FireMapOverlayDataChanged(MapState.GROWTHRATE_OVERLAY); //RGMAP
        }

        private void DistIntMarket()
        {
            for (int y = 0; y < ComRate.Length; y++)
            {
                for (int x = 0; x < ComRate[y].Length; x++)
                {
                    int z = GetDisCc(x*4, y*4);
                    z /= 4;
                    z = 64 - z;
                    ComRate[y][x] = z;
                }
            }
        }


        /// <summary>
        /// </summary>
        /// <remarks>
        ///     tends to empty RateOGMem[][]
        /// </remarks>
        private void DecRogMem()
        {
            for (int y = 0; y < RateOgMem.Length; y++)
            {
                for (int x = 0; x < RateOgMem[y].Length; x++)
                {
                    int z = RateOgMem[y][x];
                    if (z == 0)
                        continue;

                    if (z > 0)
                    {
                        RateOgMem[y][x]--;
                        if (z > 200)
                        {
                            RateOgMem[y][x] = 200; //prevent overflow?
                        }
                        continue;
                    }

                    if (z < 0)
                    {
                        RateOgMem[y][x]++;
                        if (z < -200)
                        {
                            RateOgMem[y][x] = -200;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// </summary>
        /// <remarks>
        ///     tends to empty trfDensity
        /// </remarks>
        private void DecTrafficMem()
        {
            for (int y = 0; y < _trfDensity.Length; y++)
            {
                for (int x = 0; x < _trfDensity[y].Length; x++)
                {
                    int z = _trfDensity[y][x];
                    if (z != 0)
                    {
                        if (z > 200)
                            _trfDensity[y][x] = z - 34;
                        else if (z > 24)
                            _trfDensity[y][x] = z - 24;
                        else
                            _trfDensity[y][x] = 0;
                    }
                }
            }
        }

        /// <summary>
        ///     Scans for crime
        /// </summary>
        private void CrimeScan()
        {
            PoliceMap = smoothFirePoliceMap(PoliceMap);
            PoliceMap = smoothFirePoliceMap(PoliceMap);
            PoliceMap = smoothFirePoliceMap(PoliceMap);

            for (int sy = 0; sy < PoliceMap.Length; sy++)
            {
                for (int sx = 0; sx < PoliceMap[sy].Length; sx++)
                {
                    PoliceMapEffect[sy][sx] = PoliceMap[sy][sx];
                }
            }

            int count = 0;
            int sum = 0;
            int cmax = 0;
            for (int hy = 0; hy < _landValueMem.Length; hy++)
            {
                for (int hx = 0; hx < _landValueMem[hy].Length; hx++)
                {
                    int val = _landValueMem[hy][hx];
                    if (val != 0)
                    {
                        count++;
                        int z = 128 - val + PopDensity[hy][hx];
                        z = Math.Min(300, z);
                        z -= PoliceMap[hy/4][hx/4];
                        z = Math.Min(250, z);
                        z = Math.Max(0, z);
                        CrimeMem[hy][hx] = z;

                        sum += z;
                        if (z > cmax || (z == cmax && Prng.Next(4) == 0))
                        {
                            cmax = z;
                            CrimeMaxLocationX = hx*2;
                            CrimeMaxLocationY = hy*2;
                        }
                    }
                    else
                    {
                        CrimeMem[hy][hx] = 0;
                    }
                }
            }

            if (count != 0)
                CrimeAverage = sum/count;
            else
                CrimeAverage = 0;

            FireMapOverlayDataChanged(MapState.POLICE_OVERLAY);
        }

        /// <summary>
        ///     Performs disasters
        /// </summary>
        private void DoDisasters()
        {
            if (FloodCnt > 0)
            {
                FloodCnt--;
            }

            if (DisasterEvent != Scenarios.Items[ScenarioENUM.SC_NONE])
            {
                ScenarioDisaster();
            }

            int[] disChance = {480, 240, 60};
            if (NoDisasters)
                return;

            if (Prng.Next(disChance[GameLevel] + 1) != 0)
                return;

            switch (Prng.Next(9))
            {
                case 0:
                case 1:
                    SetFire();
                    break;
                case 2:
                case 3:
                    MakeFlood();
                    break;
                case 4:
                    break;
                case 5:
                    MakeTornado();
                    break;
                case 6:
                    MakeEarthquake();
                    break;
                case 7:
                case 8:
                    if (PollutionAverage > 60)
                    {
                        MakeMonster();
                    }
                    break;
            }
        }

        /// <summary>
        ///     Smoothes the fire police map.
        /// </summary>
        /// <param name="omap">The omap.</param>
        /// <returns></returns>
        private int[][] smoothFirePoliceMap(int[][] omap)
        {
            int smX = omap[0].Length;
            int smY = omap.Length;
            var nmap = new int[smY][];
            for (int i = 0; i < nmap.Length; i++)
            {
                nmap[i] = new int[smX];
            }

            for (int sy = 0; sy < smY; sy++)
            {
                for (int sx = 0; sx < smX; sx++)
                {
                    int edge = 0;
                    if (sx > 0)
                    {
                        edge += omap[sy][sx - 1];
                    }
                    if (sx + 1 < smX)
                    {
                        edge += omap[sy][sx + 1];
                    }
                    if (sy > 0)
                    {
                        edge += omap[sy - 1][sx];
                    }
                    if (sy + 1 < smY)
                    {
                        edge += omap[sy + 1][sx];
                    }
                    edge = edge/4 + omap[sy][sx];
                    nmap[sy][sx] = edge/2;
                }
            }
            return nmap;
        }

        /// <summary>
        ///     Performs a fire analysis.
        /// </summary>
        private void FireAnalysis()
        {
            FireStMap = smoothFirePoliceMap(FireStMap);
            FireStMap = smoothFirePoliceMap(FireStMap);
            FireStMap = smoothFirePoliceMap(FireStMap);
            for (int sy = 0; sy < FireStMap.Length; sy++)
            {
                for (int sx = 0; sx < FireStMap[sy].Length; sx++)
                {
                    FireRate[sy][sx] = FireStMap[sy][sx];
                }
            }

            FireMapOverlayDataChanged(MapState.FIRE_OVERLAY);
        }

        private bool TestForCond(CityLocation loc, int dir)
        {
            int xsave = loc.X;
            int ysave = loc.Y;

            bool rv = false;
            if (MovePowerLocation(loc, dir))
            {
                char t = GetTile(loc.X, loc.Y);
                rv = (
                    TileConstants.IsConductive(t) &&
                    t != TileConstants.NUCLEAR &&
                    t != TileConstants.POWERPLANT &&
                    !HasPower(loc.X, loc.Y)
                    );
            }

            loc.X = xsave;
            loc.Y = ysave;
            return rv;
        }

        private bool MovePowerLocation(CityLocation loc, int dir)
        {
            switch (dir)
            {
                case 0:
                    if (loc.Y > 0)
                    {
                        loc.Y--;
                        return true;
                    }
                    return false;
                case 1:
                    if (loc.X + 1 < GetWidth())
                    {
                        loc.X++;
                        return true;
                    }
                    return false;
                case 2:
                    if (loc.Y + 1 < GetHeight())
                    {
                        loc.Y++;
                        return true;
                    }
                    return false;
                case 3:
                    if (loc.X > 0)
                    {
                        loc.X--;
                        return true;
                    }
                    return false;
                case 4:
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Performs a power scan
        /// </summary>
        private void PowerScan()
        {
            // clear powerMap
            for (int i = 0; i < PowerMap.Length; i++)
            {
                for (int ib = 0; ib < PowerMap[i].Length; ib++)
                    PowerMap[i][ib] = false;
            }

            //
            // Note: brownouts are based on total number of power plants, not the number
            // of powerplants connected to your city.
            //

            int maxPower = CoalCount*700 + NuclearCount*2000;
            int numPower = 0;

            // This is kind of odd algorithm, but I haven't the heart to rewrite it at
            // this time.

            while (PowerPlants.Any())
            {
                CityLocation loc = PowerPlants.Pop();

                int aDir = 4;
                int conNum;
                do
                {
                    if (++numPower > maxPower)
                    {
                        // trigger notification
                        SendMessage(MicropolisMessages.BROWNOUTS_REPORT);
                        return;
                    }
                    MovePowerLocation(loc, aDir);
                    PowerMap[loc.Y][loc.X] = true;

                    conNum = 0;
                    int dir = 0;
                    while (dir < 4 && conNum < 2)
                    {
                        if (TestForCond(loc, dir))
                        {
                            conNum++;
                            aDir = dir;
                        }
                        dir++;
                    }
                    if (conNum > 1)
                    {
                        PowerPlants.Push(new CityLocation(loc.X, loc.Y));
                    }
                } while (conNum != 0);
            }
        }


        /// <summary>
        ///     Increase the traffic-density measurement at a particular spot.
        /// </summary>
        /// <param name="mapX">The map x-coordinate.</param>
        /// <param name="mapY">The map y-coordinate.</param>
        /// <param name="traffic">the amount to Add to the density</param>
        public void AddTraffic(int mapX, int mapY, int traffic)
        {
            int z = _trfDensity[mapY/2][mapX/2];
            z += traffic;

            //FIXME- why is this only capped to 240
            // by random chance. why is there no cap
            // the rest of the time?

            if (z > 240 && Prng.Next(6) == 0)
            {
                z = 240;
                TrafficMaxLocationX = mapX;
                TrafficMaxLocationY = mapY;

                var copter = (HelicopterSprite) GetSprite(SpriteKinds.SpriteKind["COP"]);
                if (copter != null)
                {
                    copter.DestX = mapX;
                    copter.DestY = mapY;
                }
            }

            _trfDensity[mapY/2][mapX/2] = z;
        }


        /// <summary>
        ///     Gets the fire station coverage. Accessor method for fireRate[].
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public int GetFireStationCoverage(int xpos, int ypos)
        {
            return FireRate[ypos/8][xpos/8];
        }


        /// <summary>
        ///     Gets the land value. Accessor method for landValueMem overlay.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public int GetLandValue(int xpos, int ypos)
        {
            if (TestBounds(xpos, ypos))
            {
                return _landValueMem[ypos/2][xpos/2];
            }
            return 0;
        }

        /// <summary>
        ///     Gets the traffic density.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public int GetTrafficDensity(int xpos, int ypos)
        {
            if (TestBounds(xpos, ypos))
            {
                return _trfDensity[ypos/2][xpos/2];
            }
            return 0;
        }


        /// <summary>
        ///     power, terrain, land value
        /// </summary>
        private void PtlScan()
        {
            int qX = (GetWidth() + 3)/4;
            int qY = (GetHeight() + 3)/4;
            var qtem = new int[qY][];
            for (int i = 0; i < qtem.Length; i++)
            {
                qtem[i] = new int[qX];
            }

            int landValueTotal = 0;
            int landValueCount = 0;

            int hwldx = (GetWidth() + 1)/2;
            int hwldy = (GetHeight() + 1)/2;
            var tem = new int[hwldy][];
            for (int i = 0; i < tem.Length; i++)
            {
                tem[i] = new int[hwldx];
            }

            for (int x = 0; x < hwldx; x++)
            {
                for (int y = 0; y < hwldy; y++)
                {
                    int plevel = 0;
                    int lvflag = 0;
                    int zx = 2*x;
                    int zy = 2*y;

                    for (int mx = zx; mx <= zx + 1; mx++)
                    {
                        for (int my = zy; my <= zy + 1; my++)
                        {
                            int tile = GetTile(mx, my);
                            if (tile != TileConstants.DIRT)
                            {
                                if (tile < TileConstants.RUBBLE) //natural land features
                                {
                                    //inc terrainMem
                                    qtem[y/2][x/2] += 15;
                                    continue;
                                }
                                plevel += TileConstants.GetPollutionValue(tile);
                                if (TileConstants.IsConstructed(tile))
                                    lvflag++;
                            }
                        }
                    }

                    if (plevel < 0)
                        plevel = 250; //?

                    if (plevel > 255)
                        plevel = 255;

                    tem[y][x] = plevel;

                    if (lvflag != 0)
                    {
                        //land value equation


                        int dis = 34 - GetDisCc(x, y);
                        dis *= 4;
                        dis += _terrainMem[y/2][x/2];
                        dis -= PollutionMem[y][x];
                        if (CrimeMem[y][x] > 190)
                        {
                            dis -= 20;
                        }
                        if (dis > 250)
                            dis = 250;
                        if (dis < 1)
                            dis = 1;
                        _landValueMem[y][x] = dis;
                        landValueTotal += dis;
                        landValueCount++;
                    }
                    else
                    {
                        _landValueMem[y][x] = 0;
                    }
                }
            }

            LandValueAverage = landValueCount != 0 ? (landValueTotal/landValueCount) : 0;

            tem = DoSmooth(tem);
            tem = DoSmooth(tem);

            int pcount = 0;
            int ptotal = 0;
            int pmax = 0;
            for (int x = 0; x < hwldx; x++)
            {
                for (int y = 0; y < hwldy; y++)
                {
                    int z = tem[y][x];
                    PollutionMem[y][x] = z;

                    if (z != 0)
                    {
                        pcount++;
                        ptotal += z;

                        if (z > pmax ||
                            (z == pmax && Prng.Next(4) == 0))
                        {
                            pmax = z;
                            PollutionMaxLocationX = 2*x;
                            PollutionMaxLocationY = 2*y;
                        }
                    }
                }
            }

            PollutionAverage = pcount != 0 ? (ptotal/pcount) : 0;

            _terrainMem = smoothTerrain(qtem);

            FireMapOverlayDataChanged(MapState.POLLUTE_OVERLAY); //PLMAP
            FireMapOverlayDataChanged(MapState.LANDVALUE_OVERLAY); //LVMAP
        }

        /// <summary>
        ///     Gets the location of maximum pollution.
        /// </summary>
        /// <returns></returns>
        public CityLocation GetLocationOfMaxPollution()
        {
            return new CityLocation(PollutionMaxLocationX, PollutionMaxLocationY);
        }

        private void SetValves()
        {
            double normResPop = ResPop/8.0;
            TotalPop = (int) (normResPop + ComPop + IndPop);

            double employment;
            if (normResPop != 0.0)
            {
                employment = (History.Com[1] + History.Ind[1])/normResPop;
            }
            else
            {
                employment = 1;
            }

            double migration = normResPop*(employment - 1);
            const double birthRate = 0.02;
            double births = normResPop*birthRate;
            double projectedResPop = normResPop + migration + births;

            double temp = (History.Com[1] + History.Ind[1]);
            double laborBase;
            if (temp != 0.0)
            {
                laborBase = History.Res[1]/temp;
            }
            else
            {
                laborBase = 1;
            }

            // clamp laborBase to between 0.0 and 1.3
            laborBase = Math.Max(0.0, Math.Min(1.3, laborBase));

            double internalMarket = (normResPop + ComPop + IndPop)/3.7;
            double projectedComPop = internalMarket*laborBase;

            int z = GameLevel;
            temp = 1.0;
            switch (z)
            {
                case 0:
                    temp = 1.2;
                    break;
                case 1:
                    temp = 1.1;
                    break;
                case 2:
                    temp = 0.98;
                    break;
            }

            double projectedIndPop = IndPop*laborBase*temp;
            if (projectedIndPop < 5.0)
                projectedIndPop = 5.0;

            double resRatio;
            if (normResPop != 0)
            {
                resRatio = projectedResPop/normResPop;
            }
            else
            {
                resRatio = 1.3;
            }

            double comRatio;
            if (ComPop != 0)
                comRatio = projectedComPop/ComPop;
            else
                comRatio = projectedComPop;

            double indRatio;
            if (IndPop != 0)
                indRatio = projectedIndPop/IndPop;
            else
                indRatio = projectedIndPop;

            if (resRatio > 2.0)
                resRatio = 2.0;

            if (comRatio > 2.0)
                comRatio = 2.0;

            if (indRatio > 2.0)
                indRatio = 2.0;

            int z2 = TaxEffect + GameLevel;
            if (z2 > 20)
                z2 = 20;

            resRatio = (resRatio - 1)*600 + TAX_TABLE[z2];
            comRatio = (comRatio - 1)*600 + TAX_TABLE[z2];
            indRatio = (indRatio - 1)*600 + TAX_TABLE[z2];

            // ratios are velocity changes to valves
            ResValve += (int) resRatio;
            ComValve += (int) comRatio;
            IndValve += (int) indRatio;

            if (ResValve > 2000)
                ResValve = 2000;
            else if (ResValve < -2000)
                ResValve = -2000;

            if (ComValve > 1500)
                ComValve = 1500;
            else if (ComValve < -1500)
                ComValve = -1500;

            if (IndValve > 1500)
                IndValve = 1500;
            else if (IndValve < -1500)
                IndValve = -1500;


            if (ResCap && ResValve > 0)
            {
                // residents demand stadium
                ResValve = 0;
            }

            if (ComCap && ComValve > 0)
            {
                // commerce demands airport
                ComValve = 0;
            }

            if (IndCap && IndValve > 0)
            {
                // industry demands sea port
                IndValve = 0;
            }

            FireDemandChanged();
        }

        private int[][] smoothTerrain(int[][] qtem)
        {
            int qwx = qtem[0].Length;
            int qwy = qtem.Length;

            var mem = new int[qwy][];
            for (int i = 0; i < mem.Length; i++)
            {
                mem[i] = new int[qwx];
            }

            for (int y = 0; y < qwy; y++)
            {
                for (int x = 0; x < qwx; x++)
                {
                    int z = 0;
                    if (x > 0)
                        z += qtem[y][x - 1];
                    if (x + 1 < qwx)
                        z += qtem[y][x + 1];
                    if (y > 0)
                        z += qtem[y - 1][x];
                    if (y + 1 < qwy)
                        z += qtem[y + 1][x];
                    mem[y][x] = z/4 + qtem[y][x]/2;
                }
            }
            return mem;
        }

        // calculate manhatten distance (in 2-units) from center of city
        // capped at 32
        private int GetDisCc(int x, int y)
        {
            //assert x >= 0 && x <= getWidth()/2;
            //assert y >= 0 && y <= getHeight()/2;

            int xdis = Math.Abs(x - CenterMassX/2);
            int ydis = Math.Abs(y - CenterMassY/2);

            int z = (xdis + ydis);
            if (z > 32)
                return 32;
            return z;
        }

        private void InitTileBehaviors()
        {
            Dictionary<string, TileBehavior> bb = new Dictionary<string, TileBehavior>();

            bb.Add("FIRE", new TerrainBehavior(this, BTerrainBehavior.FIRE));
            bb.Add("FLOOD", new TerrainBehavior(this, BTerrainBehavior.FLOOD));
            bb.Add("RADIOACTIVE", new TerrainBehavior(this, BTerrainBehavior.RADIOACTIVE));
            bb.Add("ROAD", new TerrainBehavior(this, BTerrainBehavior.ROAD));
            bb.Add("RAIL", new TerrainBehavior(this, BTerrainBehavior.RAIL));
            bb.Add("EXPLOSION", new TerrainBehavior(this, BTerrainBehavior.EXPLOSION));
            bb.Add("RESIDENTIAL", new MapScanner(this, BZone.RESIDENTIAL));
            bb.Add("HOSPITAL_CHURCH", new MapScanner(this, BZone.HOSPITAL_CHURCH));
            bb.Add("COMMERCIAL", new MapScanner(this, BZone.COMMERCIAL));
            bb.Add("INDUSTRIAL", new MapScanner(this, BZone.INDUSTRIAL));
            bb.Add("COAL", new MapScanner(this, BZone.COAL));
            bb.Add("NUCLEAR", new MapScanner(this, BZone.NUCLEAR));
            bb.Add("FIRESTATION", new MapScanner(this, BZone.FIRESTATION));
            bb.Add("POLICESTATION", new MapScanner(this, BZone.POLICESTATION));
            bb.Add("STADIUM_EMPTY", new MapScanner(this, BZone.STADIUM_EMPTY));
            bb.Add("STADIUM_FULL", new MapScanner(this, BZone.STADIUM_FULL));
            bb.Add("AIRPORT", new MapScanner(this, BZone.AIRPORT));
            bb.Add("SEAPORT", new MapScanner(this, BZone.SEAPORT));

            _tileBehaviors = bb;
        }

        /// <summary>
        ///     Performs a map scan
        /// </summary>
        /// <param name="x0">The x0.</param>
        /// <param name="x1">The x1.</param>
        public void MapScan(int x0, int x1)
        {
            for (int x = x0; x < x1; x++)
            {
                for (int y = 0; y < GetHeight(); y++)
                {
                    MapScanTile(x, y);
                }
            }
        }

        public void MapScanTile(int xpos, int ypos)
        {
            int tile = GetTile(xpos, ypos);
            String behaviorStr = TileConstants.GetTileBehavior(tile);
            if (behaviorStr == null)
            {
                return; //nothing to do
            }

            TileBehavior b = _tileBehaviors[behaviorStr];
            if (b != null)
            {
                b.ProcessTile(xpos, ypos);
            }
            else
            {
                throw new Exception("Unknown behavior: " + behaviorStr);
            }
        }

        /// <summary>
        ///     Generates a ship.
        /// </summary>
        public void GenerateShip()
        {
            int edge = Prng.Next(4);

            if (edge == 0)
            {
                for (int x = 4; x < GetWidth() - 2; x++)
                {
                    if (GetTile(x, 0) == TileConstants.CHANNEL)
                    {
                        MakeShipAt(x, 0, ShipSprite.NORTH_EDGE);
                        return;
                    }
                }
            }
            else if (edge == 1)
            {
                for (int y = 1; y < GetHeight() - 2; y++)
                {
                    if (GetTile(0, y) == TileConstants.CHANNEL)
                    {
                        MakeShipAt(0, y, ShipSprite.EAST_EDGE);
                        return;
                    }
                }
            }
            else if (edge == 2)
            {
                for (int x = 4; x < GetWidth() - 2; x++)
                {
                    if (GetTile(x, GetHeight() - 1) == TileConstants.CHANNEL)
                    {
                        MakeShipAt(x, GetHeight() - 1, ShipSprite.SOUTH_EDGE);
                        return;
                    }
                }
            }
            else
            {
                for (int y = 1; y < GetHeight() - 2; y++)
                {
                    if (GetTile(GetWidth() - 1, y) == TileConstants.CHANNEL)
                    {
                        MakeShipAt(GetWidth() - 1, y, ShipSprite.EAST_EDGE);
                        return;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the sprite.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public Sprite GetSprite(SpriteKind kind)
        {
            return Sprites.FirstOrDefault(s => s.Kind == kind);
        }

        /// <summary>
        ///     Determines whether the specified kind has a sprite.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public bool HasSprite(SpriteKind kind)
        {
            return GetSprite(kind) != null;
        }

        /// <summary>
        ///     Makes the ship at coordinates.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <param name="edge">The edge.</param>
        public void MakeShipAt(int xpos, int ypos, int edge)
        {
            //assert !hasSprite(SpriteKind.SHI);

            Sprites.Add(new ShipSprite(this, xpos, ypos, edge));
        }

        /// <summary>
        ///     Generates the helicopter.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public void GenerateCopter(int xpos, int ypos)
        {
            if (!HasSprite(SpriteKinds.SpriteKind["COP"]))
            {
                Sprites.Add(new HelicopterSprite(this, xpos, ypos));
            }
        }

        /// <summary>
        ///     Generates the airplane.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public void GeneratePlane(int xpos, int ypos)
        {
            if (!HasSprite(SpriteKinds.SpriteKind["AIR"]))
            {
                Sprites.Add(new AirplaneSprite(this, xpos, ypos));
            }
        }

        /// <summary>
        ///     Generates the train.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public void GenerateTrain(int xpos, int ypos)
        {
            if (TotalPop > 20 &&
                !HasSprite(SpriteKinds.SpriteKind["TRA"]) &&
                Prng.Next(26) == 0)
            {
                Sprites.Add(new TrainSprite(this, xpos, ypos));
            }
        }


        /// <summary>
        ///     counts the population in a certain type of residential zone
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public int DoFreePop(int xpos, int ypos)
        {
            int count = 0;

            for (int x = xpos - 1; x <= xpos + 1; x++)
            {
                for (int y = ypos - 1; y <= ypos + 1; y++)
                {
                    if (TestBounds(x, y))
                    {
                        char loc = GetTile(x, y);
                        if (loc >= TileConstants.LHTHR && loc <= TileConstants.HHTHR)
                            count++;
                    }
                }
            }

            return count;
        }


        /// <summary>
        ///     called every several cycles; this takes the census data collected in this cycle and records it to the history
        /// </summary>
        private void TakeCensus()
        {
            int resMax = 0;
            int comMax = 0;
            int indMax = 0;

            for (int i = 118; i >= 0; i--)
            {
                if (History.Res[i] > resMax)
                    resMax = History.Res[i];
                if (History.Com[i] > comMax)
                    comMax = History.Res[i];
                if (History.Ind[i] > indMax)
                    indMax = History.Ind[i];

                History.Res[i + 1] = History.Res[i];
                History.Com[i + 1] = History.Com[i];
                History.Ind[i + 1] = History.Ind[i];
                History.Crime[i + 1] = History.Crime[i];
                History.Pollution[i + 1] = History.Pollution[i];
                History.Money[i + 1] = History.Money[i];
            }

            History.ResMax = resMax;
            History.ComMax = comMax;
            History.IndMax = indMax;

            //graph10max = Math.max(resMax, Math.max(comMax, indMax));

            History.Res[0] = ResPop/8;
            History.Com[0] = ComPop;
            History.Ind[0] = IndPop;

            CrimeRamp += (CrimeAverage - CrimeRamp)/4;
            History.Crime[0] = Math.Min(255, CrimeRamp);

            PolluteRamp += (PollutionAverage - PolluteRamp)/4;
            History.Pollution[0] = Math.Min(255, PolluteRamp);

            int moneyScaled = CashFlow/20 + 128;
            if (moneyScaled < 0)
                moneyScaled = 0;
            if (moneyScaled > 255)
                moneyScaled = 255;
            History.Money[0] = moneyScaled;

            History.CityTime = CityTime;

            if (HospitalCount < ResPop/256)
            {
                NeedHospital = 1;
            }
            else if (HospitalCount > ResPop/256)
            {
                NeedHospital = -1;
            }
            else
            {
                NeedHospital = 0;
            }

            if (ChurchCount < ResPop/256)
            {
                NeedChurch = 1;
            }
            else if (ChurchCount > ResPop/256)
            {
                NeedChurch = -1;
            }
            else
            {
                NeedChurch = 0;
            }
        }

        private void TakeCensus2()
        {
            // update long term graphs
            int resMax = 0;
            int comMax = 0;
            int indMax = 0;

            for (int i = 238; i >= 120; i--)
            {
                if (History.Res[i] > resMax)
                    resMax = History.Res[i];
                if (History.Com[i] > comMax)
                    comMax = History.Res[i];
                if (History.Ind[i] > indMax)
                    indMax = History.Ind[i];

                History.Res[i + 1] = History.Res[i];
                History.Com[i + 1] = History.Com[i];
                History.Ind[i + 1] = History.Ind[i];
                History.Crime[i + 1] = History.Crime[i];
                History.Pollution[i + 1] = History.Pollution[i];
                History.Money[i + 1] = History.Money[i];
            }

            History.Res[120] = ResPop/8;
            History.Com[120] = ComPop;
            History.Ind[120] = IndPop;
            History.Crime[120] = History.Crime[0];
            History.Pollution[120] = History.Pollution[0];
            History.Money[120] = History.Money[0];
        }

        /** Road/rail maintenance cost multiplier, for various difficulty settings.
         */

        private void CollectTaxPartial()
        {
            LastRoadTotal = RoadTotal;
            LastRailTotal = RailTotal;
            LastTotalPop = TotalPop;
            LastFireStationCount = FireStationCount;
            LastPoliceCount = PoliceCount;

            BudgetNumbers b = GenerateBudget();

            Budget.TaxFund += b.TaxIncome;
            Budget.RoadFundEscrow -= b.RoadFunded;
            Budget.FireFundEscrow -= b.FireFunded;
            Budget.PoliceFundEscrow -= b.PoliceFunded;

            TaxEffect = b.TaxRate;
            RoadEffect = b.RoadRequest != 0
                ? (int) Math.Floor(32.0*b.RoadFunded/b.RoadRequest)
                : 32;
            PoliceEffect = b.PoliceRequest != 0
                ? (int) Math.Floor(1000.0*b.PoliceFunded/b.PoliceRequest)
                : 1000;
            FireEffect = b.FireRequest != 0
                ? (int) Math.Floor(1000.0*b.FireFunded/b.FireRequest)
                : 1000;
        }


        private void CollectTax()
        {
            int revenue = Budget.TaxFund/TAX_FREQ;
            int expenses = -(Budget.RoadFundEscrow + Budget.FireFundEscrow + Budget.PoliceFundEscrow)/TAX_FREQ;

            var hist = new FinancialHistory {CityTime = CityTime, TaxIncome = revenue, OperatingExpenses = expenses};

            CashFlow = revenue - expenses;
            Spend(-CashFlow);

            hist.TotalFunds = Budget.TotalFunds;
            FinancialHistory.Insert(0, hist);

            Budget.TaxFund = 0;
            Budget.RoadFundEscrow = 0;
            Budget.FireFundEscrow = 0;
            Budget.PoliceFundEscrow = 0;
        }


        /// <summary>
        ///     Calculate the current budget numbers.
        /// </summary>
        /// <returns></returns>
        public BudgetNumbers GenerateBudget()
        {
            var b = new BudgetNumbers
            {
                TaxRate = Math.Max(0, CityTax),
                RoadPercent = Math.Max(0.0, RoadPercent),
                FirePercent = Math.Max(0.0, FirePercent),
                PolicePercent = Math.Max(0.0, PolicePercent),
                PreviousBalance = Budget.TotalFunds
            };

            b.TaxIncome = (int) Math.Round(LastTotalPop*LandValueAverage/120*b.TaxRate*F_LEVELS[GameLevel]);
            //assert b.taxIncome >= 0;

            b.RoadRequest = (int) Math.Round((LastRoadTotal + LastRailTotal*2)*R_LEVELS[GameLevel]);
            b.FireRequest = FIRE_STATION_MAINTENANCE*LastFireStationCount;
            b.PoliceRequest = POLICE_STATION_MAINTENANCE*LastPoliceCount;

            b.RoadFunded = (int) Math.Round(b.RoadRequest*b.RoadPercent);
            b.FireFunded = (int) Math.Round(b.FireRequest*b.FirePercent);
            b.PoliceFunded = (int) Math.Round(b.PoliceRequest*b.PolicePercent);

            int yumDuckets = Budget.TotalFunds + b.TaxIncome;
            //assert yumDuckets >= 0;

            if (yumDuckets >= b.RoadFunded)
            {
                yumDuckets -= b.RoadFunded;
                if (yumDuckets >= b.FireFunded)
                {
                    yumDuckets -= b.FireFunded;
                    if (yumDuckets >= b.PoliceFunded)
                    {
                        yumDuckets -= b.PoliceFunded;
                    }
                    else
                    {
                        //assert b.policeRequest != 0;

                        b.PoliceFunded = yumDuckets;
                        b.PolicePercent = b.PoliceFunded/(double) b.PoliceRequest;
                        yumDuckets = 0;
                    }
                }
                else
                {
                    //assert b.fireRequest != 0;

                    b.FireFunded = yumDuckets;
                    b.FirePercent = b.FireFunded/(double) b.FireRequest;
                    b.PoliceFunded = 0;
                    b.PolicePercent = 0.0;
                    yumDuckets = 0;
                }
            }
            else
            {
                //assert b.roadRequest != 0;

                b.RoadFunded = yumDuckets;
                b.RoadPercent = b.RoadFunded/(double) b.RoadRequest;
                b.FireFunded = 0;
                b.FirePercent = 0.0;
                b.PoliceFunded = 0;
                b.PolicePercent = 0.0;
            }

            b.OperatingExpenses = b.RoadFunded + b.FireFunded + b.PoliceFunded;
            b.NewBalance = b.PreviousBalance + b.TaxIncome - b.OperatingExpenses;

            return b;
        }

        /// <summary>
        ///     Gets the population density.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public int GetPopulationDensity(int xpos, int ypos)
        {
            return PopDensity[ypos/2][xpos/2];
        }

        /// <summary>
        ///     Does the meltdown.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public void DoMeltdown(int xpos, int ypos)
        {
            MeltdownLocation = new CityLocation(xpos, ypos);

            MakeExplosion(xpos - 1, ypos - 1);
            MakeExplosion(xpos - 1, ypos + 2);
            MakeExplosion(xpos + 2, ypos - 1);
            MakeExplosion(xpos + 2, ypos + 2);

            for (int x = xpos - 1; x < xpos + 3; x++)
            {
                for (int y = ypos - 1; y < ypos + 3; y++)
                {
                    SetTile(x, y, (char) (TileConstants.FIRE + Prng.Next(4)));
                }
            }

            for (int z = 0; z < 200; z++)
            {
                int x = xpos - 20 + Prng.Next(41);
                int y = ypos - 15 + Prng.Next(31);
                if (!TestBounds(x, y))
                    continue;

                int t = Map[y][x];
                if (TileConstants.IsZoneCenter(t))
                {
                    continue;
                }
                if (TileConstants.IsCombustible(t) || t == TileConstants.DIRT)
                {
                    SetTile(x, y, TileConstants.RADTILE);
                }
            }

            clearMes();
            SendMessageAt(MicropolisMessages.MELTDOWN_REPORT, xpos, ypos);
        }

        /// <summary>
        ///     Saves the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task Save(StorageFile file)
        {
            await MapSaver.Save(file, this);
        }

        /// <summary>
        ///     Saves the specified stream.
        /// </summary>
        /// <param name="outStream">The out stream.</param>
        public void Save(Stream outStream)
        {
            MapSaver.Save(outStream, this);
        }

        /// <summary>
        ///     Loads the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public async Task Load(StorageFile file)
        {
            await MapLoader.Load(file, this);
        }

        public async Task LoadScenario(ScenarioENUM scenario)
        {
            await MapLoader.LoadScenario(scenario, this);
        }

        /// <summary>
        ///     Loads the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public void LoadFile(Stream stream)
        {
            MapLoader.LoadFile(stream, this);
        }

        /// <summary>
        ///     Loads the specified stream.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        public void Load(Stream inStream)
        {
            MapLoader.Load(inStream,this);
        }

        /// <summary>
        ///     Adds the ordinal to the number.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        /// <remarks>
        ///     source http://stackoverflow.com/questions/20156/is-there-an-easy-way-to-create-ordinals-in-c
        /// </remarks>
        /// <author>
        ///     external
        /// </author>
        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num%100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num%10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }


        internal void CheckPowerMap()
        {
            CoalCount = 0;
            NuclearCount = 0;

            PowerPlants.Clear();
            for (int y = 0; y < Map.Length; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    int tile = GetTile(x, y);
                    if (tile == TileConstants.NUCLEAR)
                    {
                        NuclearCount++;
                        PowerPlants.Push(new CityLocation(x, y));
                    }
                    else if (tile == TileConstants.POWERPLANT)
                    {
                        CoalCount++;
                        PowerPlants.Push(new CityLocation(x, y));
                    }
                }
            }

            PowerScan();
            NewPower = true;
        }

        
        
        /// <summary>
        ///     Toggles the automatic budget.
        /// </summary>
        public void ToggleAutoBudget()
        {
            AutoBudget = !AutoBudget;
            FireOptionsChanged();
        }

        /// <summary>
        ///     Toggles the automatic bulldoze.
        /// </summary>
        public void ToggleAutoBulldoze()
        {
            AutoBulldoze = !AutoBulldoze;
            FireOptionsChanged();
        }

        /// <summary>
        /// Toggles the automatic go.
        /// </summary>
        public void ToggleAutoGo()
        {
            AutoGo = !AutoGo;
            FireOptionsChanged();
        }

        /// <summary>
        ///     Toggles the disasters.
        /// </summary>
        public void ToggleDisasters()
        {
            NoDisasters = !NoDisasters;
            FireOptionsChanged();
        }

        /// <summary>
        ///     Sets the speed.
        /// </summary>
        /// <param name="newSpeed">The new speed.</param>
        public void SetSpeed(Speed newSpeed)
        {
            SimSpeed = newSpeed;
            FireOptionsChanged();
        }

        /// <summary>
        ///     Animates the game map initiating redraws.
        /// </summary>
        public void Animate()
        {
            Acycle = (Acycle + 1)%960;
            if (Acycle%2 == 0)
            {
                Step();
            }
            MoveObjects();
            AnimateTiles();
        }

        /// <summary>
        ///     Returns an array of alls the sprites.
        /// </summary>
        /// <returns></returns>
        public Sprite[] AllSprites()
        {
            return Sprites.ToArray(); //(new Sprite[0]);
        }

        private void MoveObjects()
        {
            foreach (Sprite sprite in AllSprites())
            {
                sprite.Move();

                if (sprite.Frame == 0)
                {
                    Sprites.Remove(sprite);
                }
            }
        }

        private void AnimateTiles()
        {
            for (int y = 0; y < Map.Length; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    char tilevalue = Map[y][x];
                    TileSpec spec = Tiles.Get(tilevalue & TileConstants.LOMASK);
                    if (spec != null && spec.AnimNext != null)
                    {
                        int flags = tilevalue & TileConstants.ALLBITS;
                        SetTile(x, y, (char)
                            (spec.AnimNext.TileNumber | flags)
                            );
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the city population.
        /// </summary>
        /// <returns></returns>
        public int GetCityPopulation()
        {
            if (LastCityPop == 0)
            {
                CheckGrowth(true);
            }

            return LastCityPop;
        }

        /// <summary>
        ///     Makes the sound.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="sound">The sound.</param>
        public void MakeSound(int x, int y, Sound sound)
        {
            FireCitySound(sound, new CityLocation(x, y));
        }

        /// <summary>
        ///     Makes the earthquake.
        /// </summary>
        public void MakeEarthquake()
        {
            MakeSound(CenterMassX, CenterMassY, Sounds.Sound["EXPLOSION_LOW"]);
            FireEarthquakeStarted();

            SendMessageAt(MicropolisMessages.EARTHQUAKE_REPORT, CenterMassX, CenterMassY);
            int time = Prng.Next(701) + 300;
            for (int z = 0; z < time; z++)
            {
                int x = Prng.Next(GetWidth());
                int y = Prng.Next(GetHeight());
                //assert testBounds(x, y);

                if (TileConstants.IsVulnerable(GetTile(x, y)))
                {
                    if (Prng.Next(4) != 0)
                    {
                        SetTile(x, y, (char) (TileConstants.RUBBLE + Prng.Next(4)));
                    }
                    else
                    {
                        SetTile(x, y, (char) (TileConstants.FIRE + Prng.Next(8)));
                    }
                }
            }
        }

        private void SetFire()
        {
            int x = Prng.Next(GetWidth());
            int y = Prng.Next(GetHeight());
            int t = GetTile(x, y);

            if (TileConstants.IsArsonable(t))
            {
                SetTile(x, y, (char) (TileConstants.FIRE + Prng.Next(8)));
                CrashLocation = new CityLocation(x, y);
                SendMessageAt(MicropolisMessages.FIRE_REPORT, x, y);
            }
        }

        /// <summary>
        ///     Makes the fire.
        /// </summary>
        public void MakeFire()
        {
            // forty attempts at finding place to start fire
            for (int t = 0; t < 40; t++)
            {
                int x = Prng.Next(GetWidth());
                int y = Prng.Next(GetHeight());
                int tile = GetTile(x, y);
                if (!TileConstants.IsZoneCenter(tile) && TileConstants.IsCombustible(tile))
                {
                    if (tile > 21 && tile < TileConstants.LASTZONE)
                    {
                        SetTile(x, y, (char) (TileConstants.FIRE + Prng.Next(8)));
                        SendMessageAt(MicropolisMessages.FIRE_REPORT, x, y);
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Let disasters of the scenario happen.
        /// </summary>
        private void ScenarioDisaster()
        {
            if (DisasterEvent == Scenarios.Items[ScenarioENUM.SC_SAN_FRANCISCO]) {
                if (DisasterWait == 1)
                {
                    MakeEarthquake();
                }
            }

            if (DisasterEvent == Scenarios.Items[ScenarioENUM.SC_HAMBURG])
            {
                if (DisasterWait % 10 == 0)
                {
                    MakeFireBombs();
                }
            }
            if (DisasterEvent == Scenarios.Items[ScenarioENUM.SC_TOKYO])
            {
                if (DisasterWait == 1)
                {
                    MakeMonster();
                }
            }
            if (DisasterEvent == Scenarios.Items[ScenarioENUM.SC_BOSTON])
            {
                if (DisasterWait == 1)
                {
                    MakeMeltdown();
                }
            }
            if (DisasterEvent == Scenarios.Items[ScenarioENUM.SC_RIO])
            {
                if ((DisasterWait % 24) == 0)
                {
                    MakeFlood();
                }
            }
      

            if (DisasterWait > 0)
            {
                DisasterWait--;
            }
            else
            {
                DisasterEvent = Scenarios.Items[ScenarioENUM.SC_NONE];
            }
        }


        /// <summary>
        /// Let a fire bomb explode at a random location
        /// </summary>
        private void FireBomb()
        {
            int crashX = Prng.Next(Map.Length - 1);
            int crashY = Prng.Next(Map[0].Length - 1);
            MakeExplosion(crashX, crashY);
            SendMessageAt(MicropolisMessages.FIREBOMBING_REPORT, crashX, crashY);
        }

        /// <summary>
        /// Throw several bombs onto the city.
        /// </summary>
        private void MakeFireBombs()
        {
            int count = 2 + (Prng.Next(16) & 1);

            while (count > 0)
            {
                FireBomb();
                count--;
            }

            // TODO: Schedule periodic fire bombs over time, every few ticks.
        }


        /// <summary>
        ///     Makes the meltdown. Force a meltdown to occur.
        /// </summary>
        /// <returns>true if a metldown was initiated.</returns>
        public bool MakeMeltdown()
        {
            var candidates = new List<CityLocation>();
            for (int y = 0; y < Map.Length; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    if (GetTile(x, y) == TileConstants.NUCLEAR)
                    {
                        candidates.Add(new CityLocation(x, y));
                    }
                }
            }

            if (candidates.Count == 0)
            {
                // tell caller that no nuclear plants were found
                return false;
            }

            int i = Prng.Next(candidates.Count);
            CityLocation p = candidates[i];
            DoMeltdown(p.X, p.Y);
            return true;
        }

        /// <summary>
        ///     Makes the monster.
        /// </summary>
        public void MakeMonster()
        {
            var monster = (MonsterSprite) GetSprite(SpriteKinds.SpriteKind["GOD"]);
            if (monster != null)
            {
                // already have a monster in town
                monster.SoundCount = 1;
                monster.Count = 1000;
                monster.Flag = false;
                monster.DestX = PollutionMaxLocationX;
                monster.DestY = PollutionMaxLocationY;
                return;
            }

            // try to find a suitable starting spot for monster

            for (int i = 0; i < 300; i++)
            {
                int x = Prng.Next(GetWidth() - 19) + 10;
                int y = Prng.Next(GetHeight() - 9) + 5;
                int t = GetTile(x, y);
                if (t == TileConstants.RIVER)
                {
                    MakeMonsterAt(x, y);
                    return;
                }
            }

            // no "nice" location found, just start in center of map then
            MakeMonsterAt(GetWidth()/2, GetHeight()/2);
        }

        private void MakeMonsterAt(int xpos, int ypos)
        {
            //assert !hasSprite(SpriteKind.GOD);
            Sprites.Add(new MonsterSprite(this, xpos, ypos));
            SendMessageAt(MicropolisMessages.MONSTER_REPORT, xpos, ypos);
        }

        /// <summary>
        ///     Makes the tornado.
        /// </summary>
        public void MakeTornado()
        {
            var tornado = (TornadoSprite) GetSprite(SpriteKinds.SpriteKind["TOR"]);
            if (tornado != null)
            {
                // already have a tornado, so extend the Length of the
                // existing tornado
                tornado.Count = 200;
                return;
            }

            //FIXME- this is not exactly like the original code
            int xpos = Prng.Next(GetWidth() - 19) + 10;
            int ypos = Prng.Next(GetHeight() - 19) + 10;
            Sprites.Add(new TornadoSprite(this, xpos, ypos));
            SendMessageAt(MicropolisMessages.TORNADO_REPORT, xpos, ypos);
        }

        /// <summary>
        ///     Makes the flood.
        /// </summary>
        public void MakeFlood()
        {
            int[] dx = {0, 1, 0, -1};
            int[] dy = {-1, 0, 1, 0};

            for (int z = 0; z < 300; z++)
            {
                int x = Prng.Next(GetWidth());
                int y = Prng.Next(GetHeight());
                int tile = GetTile(x, y);
                if (TileConstants.IsRiverEdge(tile))
                {
                    for (int t = 0; t < 4; t++)
                    {
                        int xx = x + dx[t];
                        int yy = y + dy[t];
                        if (TestBounds(xx, yy))
                        {
                            int c = Map[yy][xx];
                            if (TileConstants.IsFloodable(c))
                            {
                                SetTile(xx, yy, TileConstants.FLOOD);
                                FloodCnt = 30;
                                SendMessageAt(MicropolisMessages.FLOOD_REPORT, xx, yy);
                                FloodX = xx;
                                FloodY = yy;
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Makes all component tiles of a zone bulldozable. Should be called whenever the key zone tile of a zone is
        ///     destroyed, since otherwise the user would no longer have a way of destroying the zone.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <param name="zoneTile">The zone tile.</param>
        /// <see cref="ShutdownZone" />
        public void KillZone(int xpos, int ypos, int zoneTile)
        {
            RateOgMem[ypos/8][xpos/8] -= 20;

            //assert isZoneCenter(zoneTile);
            CityDimension dim = TileConstants.GetZoneSizeFor(zoneTile);
            //assert dim != null;
            //assert dim.width >= 3;
            //assert dim.height >= 3;

            int zoneBase = (zoneTile & TileConstants.LOMASK) - 1 - dim.Width;

            // this will take care of stopping smoke animations
            ShutdownZone(xpos, ypos, dim);
        }


        /// <summary>
        ///     If a zone has a different image (animation) for when it is powered, switch to that different image here. Note:
        ///     pollution is not accumulated here; see ptlScan() instead.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <param name="zoneSize">Size of the zone.</param>
        /// <see cref="ShutdownZone" />
        public void PowerZone(int xpos, int ypos, CityDimension zoneSize)
        {
            //assert zoneSize.width >= 3;
            //assert zoneSize.height >= 3;

            for (int dx = 0; dx < zoneSize.Width; dx++)
            {
                for (int dy = 0; dy < zoneSize.Height; dy++)
                {
                    int x = xpos - 1 + dx;
                    int y = ypos - 1 + dy;
                    int tile = GetTileRaw(x, y);
                    TileSpec ts = Tiles.Get(tile & TileConstants.LOMASK);
                    if (ts != null && ts.OnPower != null)
                    {
                        SetTile(x, y,
                            (char) (ts.OnPower.TileNumber | (tile & TileConstants.ALLBITS))
                            );
                    }
                }
            }
        }


        /// <summary>
        ///     If a zone has a different image (animation) for when it is powered, switch back to the original image.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <param name="zoneSize">Size of the zone.</param>
        /// <see cref="PowerZone" />
        /// <see cref="KillZone" />
        public void ShutdownZone(int xpos, int ypos, CityDimension zoneSize)
        {
            //assert zoneSize.width >= 3;
            //assert zoneSize.height >= 3;

            for (int dx = 0; dx < zoneSize.Width; dx++)
            {
                for (int dy = 0; dy < zoneSize.Height; dy++)
                {
                    int x = xpos - 1 + dx;
                    int y = ypos - 1 + dy;
                    int tile = GetTileRaw(x, y);
                    TileSpec ts = Tiles.Get(tile & TileConstants.LOMASK);
                    if (ts != null && ts.OnShutdown != null)
                    {
                        SetTile(x, y,
                            (char) (ts.OnShutdown.TileNumber | (tile & TileConstants.ALLBITS))
                            );
                    }
                }
            }
        }

        /// <summary>
        ///     Makes an explosion.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public void MakeExplosion(int xpos, int ypos)
        {
            MakeExplosionAt(xpos*16 + 8, ypos*16 + 8);
        }


        /// <summary>
        ///     Makes the explosion at. Uses x,y coordinates as 1/16th-Length tiles.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void MakeExplosionAt(int x, int y)
        {
            Sprites.Add(new ExplosionSprite(this, x, y));
        }

        /// <summary>
        ///     Checks the growth to display growth related messages to the user.
        /// </summary>
        public void CheckGrowth(bool forceCheck=false)
        {
            if (CityTime%4 == 0 || forceCheck)
            {
                int newPop = (ResPop + ComPop*8 + IndPop*8)*20;
                if (LastCityPop != 0)
                {
                    MicropolisMessage z = null;
                    if (LastCityPop < 500000 && newPop >= 500000)
                    {
                        z = MicropolisMessages.POP_500K_REACHED;
                    }
                    else if (LastCityPop < 100000 && newPop >= 100000)
                    {
                        z = MicropolisMessages.POP_100K_REACHED;
                    }
                    else if (LastCityPop < 50000 && newPop >= 50000)
                    {
                        z = MicropolisMessages.POP_50K_REACHED;
                    }
                    else if (LastCityPop < 10000 && newPop >= 10000)
                    {
                        z = MicropolisMessages.POP_10K_REACHED;
                    }
                    else if (LastCityPop < 2000 && newPop >= 2000)
                    {
                        z = MicropolisMessages.POP_2K_REACHED;
                    }
                    if (z != null)
                    {
                        SendMessage(z);
                    }
                }
                LastCityPop = newPop;
            }
        }

        private void DoMessages()
        {
            // Running a scenario, and waiting it to 'end' so we can give a score
            if (Scenario != Scenarios.Items[ScenarioENUM.SC_NONE] && ScoreType != Scenarios.Items[ScenarioENUM.SC_NONE] && ScoreWait > 0)
            {
                ScoreWait--;
                if (ScoreWait == 0)
                {
                    DoScenarioScore(ScoreType);
                }
            }


            CheckGrowth();

            int totalZoneCount = ResZoneCount + ComZoneCount + IndZoneCount;
            int powerCount = NuclearCount + CoalCount;

            int z = CityTime%64;
            switch (z)
            {
                case 1:
                    if (totalZoneCount/4 >= ResZoneCount)
                    {
                        SendMessage(MicropolisMessages.NEED_RES);
                    }
                    break;
                case 5:
                    if (totalZoneCount/8 >= ComZoneCount)
                    {
                        SendMessage(MicropolisMessages.NEED_COM);
                    }
                    break;
                case 10:
                    if (totalZoneCount/8 >= IndZoneCount)
                    {
                        SendMessage(MicropolisMessages.NEED_IND);
                    }
                    break;
                case 14:
                    if (totalZoneCount > 10 && totalZoneCount*2 > RoadTotal)
                    {
                        SendMessage(MicropolisMessages.NEED_ROADS);
                    }
                    break;
                case 18:
                    if (totalZoneCount > 50 && totalZoneCount > RailTotal)
                    {
                        SendMessage(MicropolisMessages.NEED_RAILS);
                    }
                    break;
                case 22:
                    if (totalZoneCount > 10 && powerCount == 0)
                    {
                        SendMessage(MicropolisMessages.NEED_POWER);
                    }
                    break;
                case 26:
                    ResCap = (ResPop > 500 && StadiumCount == 0);
                    if (ResCap)
                    {
                        SendMessage(MicropolisMessages.NEED_STADIUM);
                    }
                    break;
                case 28:
                    IndCap = (IndPop > 70 && SeaportCount == 0);
                    if (IndCap)
                    {
                        SendMessage(MicropolisMessages.NEED_SEAPORT);
                    }
                    break;
                case 30:
                    ComCap = (ComPop > 100 && AirportCount == 0);
                    if (ComCap)
                    {
                        SendMessage(MicropolisMessages.NEED_AIRPORT);
                    }
                    break;
                case 32:
                    int tm = UnpoweredZoneCount + PoweredZoneCount;
                    if (tm != 0)
                    {
                        if (PoweredZoneCount/(double) tm < 0.7)
                        {
                            SendMessage(MicropolisMessages.BLACKOUTS);
                        }
                    }
                    break;
                case 35:
                    if (PollutionAverage > 60)
                    {
                        // FIXME, consider changing threshold to 80
                        SendMessage(MicropolisMessages.HIGH_POLLUTION);
                    }
                    break;
                case 42:
                    if (CrimeAverage > 100)
                    {
                        SendMessage(MicropolisMessages.HIGH_CRIME);
                    }
                    break;
                case 45:
                    if (TotalPop > 60 && FireStationCount == 0)
                    {
                        SendMessage(MicropolisMessages.NEED_FIRESTATION);
                    }
                    break;
                case 48:
                    if (TotalPop > 60 && PoliceCount == 0)
                    {
                        SendMessage(MicropolisMessages.NEED_POLICE);
                    }
                    break;
                case 51:
                    if (CityTax > 12)
                    {
                        SendMessage(MicropolisMessages.HIGH_TAXES);
                    }
                    break;
                case 54:
                    if (RoadEffect < 20 && RoadTotal > 30)
                    {
                        SendMessage(MicropolisMessages.ROADS_NEED_FUNDING);
                    }
                    break;
                case 57:
                    if (FireEffect < 700 && TotalPop > 20)
                    {
                        SendMessage(MicropolisMessages.FIRE_NEED_FUNDING);
                    }
                    break;
                case 60:
                    if (PoliceEffect < 700 && TotalPop > 20)
                    {
                        SendMessage(MicropolisMessages.POLICE_NEED_FUNDING);
                    }
                    break;
                case 63:
                    if (TrafficAverage > 60)
                    {
                        SendMessage(MicropolisMessages.HIGH_TRAFFIC);
                    }
                    break;
                    //nothing
            }
        }

        /// <summary>
        /// Compute score for each scenario
        /// </summary>
        /// <remarks>Parameter type may not be SC_NONE</remarks>
        /// <param name="type">Scenario used</param>
        private void DoScenarioScore(Scenario type)
        {
            MicropolisMessage z = MicropolisMessages.SCENARIO_LOST;     /* you lose */

            if (type == Scenarios.Items[ScenarioENUM.SC_DULLSVILLE])
            {
                if (Evaluation.CityClass >= CC_METROPOLIS)
                {
                    z = MicropolisMessages.SCENARIO_WON;
                }
            }

            if (type == Scenarios.Items[ScenarioENUM.SC_SAN_FRANCISCO])
            {
                if (Evaluation.CityClass >= CC_METROPOLIS)
                {
                    z = MicropolisMessages.SCENARIO_WON;
                }
            }

            if (type == Scenarios.Items[ScenarioENUM.SC_HAMBURG])
            {
                if (Evaluation.CityClass >= CC_METROPOLIS)
                {
                    z = MicropolisMessages.SCENARIO_WON;
                }
            }

            if (type == Scenarios.Items[ScenarioENUM.SC_BERN])
            {
                if (TrafficAverage < 80)
                {
                    z = MicropolisMessages.SCENARIO_WON;
                }
            }

            if (type == Scenarios.Items[ScenarioENUM.SC_TOKYO])
            {
                if (Evaluation.CityScore > 500)
                {
                    z = MicropolisMessages.SCENARIO_WON;
                }
            }

            if (type == Scenarios.Items[ScenarioENUM.SC_DETROIT])
            {
                if (CrimeAverage < 60)
                {
                    z = MicropolisMessages.SCENARIO_WON;
                }
            }

            if (type == Scenarios.Items[ScenarioENUM.SC_BOSTON])
            {
                if (Evaluation.CityScore > 500)
                {
                    z = MicropolisMessages.SCENARIO_WON;
                }
            }

            if (type == Scenarios.Items[ScenarioENUM.SC_RIO])
            {
                if (Evaluation.CityScore > 500)
                {
                    z = MicropolisMessages.SCENARIO_WON;
                }
            }
            
            SendMessage(z);

            if (z == MicropolisMessages.SCENARIO_LOST)
            {
                DoLoseGame();
            }
        }

        private void DoLoseGame()
        {
            //ToDo: freeze game and go to main menu
        }

        private void clearMes()
        {
            //TODO.
            // What this does in the original code is clears the 'last message'
            // properties, ensuring that the next message will be delivered even
            // if it is a repeat.
        }

        private void SendMessage(MicropolisMessage message)
        {
            FireCityMessage(message, null);
        }

        /// <summary>
        ///     Sends the message at coordinate.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SendMessageAt(MicropolisMessage message, int x, int y)
        {
            FireCityMessage(message, new CityLocation(x, y));
        }

        /// <summary>
        ///     Queries the zone status at coordinate.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public ZoneStatus QueryZoneStatus(int xpos, int ypos)
        {
            var zs = new ZoneStatus {Building = TileConstants.GetDescriptionNumber(GetTile(xpos, ypos))};

            int z;
            z = (PopDensity[ypos/2][xpos/2]/64)%4;
            zs.PopDensity = z + 1;

            z = _landValueMem[ypos/2][xpos/2];
            z = z < 30 ? 4 : z < 80 ? 5 : z < 150 ? 6 : 7;
            zs.LandValue = z + 1;

            z = ((CrimeMem[ypos/2][xpos/2]/64)%4) + 8;
            zs.CrimeLevel = z + 1;

            z = Math.Max(13, ((PollutionMem[ypos/2][xpos/2]/64)%4) + 12);
            zs.Pollution = z + 1;

            z = RateOgMem[ypos/8][xpos/8];
            z = z < 0 ? 16 : z == 0 ? 17 : z <= 100 ? 18 : 19;
            zs.GrowthRate = z + 1;

            return zs;
        }

        /// <summary>
        ///     Gets the residential valve.
        /// </summary>
        /// <returns></returns>
        public int GetResValve()
        {
            return ResValve;
        }

        /// <summary>
        ///     Gets the commercial valve.
        /// </summary>
        /// <returns></returns>
        public int GetComValve()
        {
            return ComValve;
        }

        /// <summary>
        ///     Gets the industry valve.
        /// </summary>
        /// <returns></returns>
        public int GetIndValve()
        {
            return IndValve;
        }

        /// <summary>
        ///     Sets the game difficulty.
        /// </summary>
        /// <param name="newLevel">The new level.</param>
        public void SetGameLevel(int newLevel)
        {
            //assert GameLevel.isValid(newLevel);

            GameLevel = newLevel;
            FireOptionsChanged();
        }

        /// <summary>
        ///     Sets the funds.
        /// </summary>
        /// <param name="totalFunds">The total funds.</param>
        public void SetFunds(int totalFunds)
        {
            Budget.TotalFunds = totalFunds;
        }

        #region listener firing

        /// <summary>
        ///     Fires the census changed event.
        /// </summary>
        public void FireCensusChanged()
        {
            foreach (IListener l in _listeners)
            {
                l.CensusChanged();
            }
        }

        /// <summary>
        ///     Fires the city message event.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="loc">The location.</param>
        public void FireCityMessage(MicropolisMessage message, CityLocation loc)
        {
            foreach (IListener l in _listeners)
            {
                l.CityMessage(message, loc);
            }
        }

        /// <summary>
        ///     Fires the city sound event.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="loc">The location.</param>
        public void FireCitySound(Sound sound, CityLocation loc)
        {
            foreach (IListener l in _listeners)
            {
                l.CitySound(sound, loc);
            }
        }

        /// <summary>
        ///     Fires the demand changed event.
        /// </summary>
        public void FireDemandChanged()
        {
            foreach (IListener l in _listeners)
            {
                l.DemandChanged();
            }
        }

        /// <summary>
        ///     Fires the earthquake started event.
        /// </summary>
        public void FireEarthquakeStarted()
        {
            foreach (IEarthquakeListener l in _earthquakeListeners)
            {
                l.EarthquakeStarted();
            }
        }

        /// <summary>
        ///     Fires the evaluation changed event.
        /// </summary>
        public void FireEvaluationChanged()
        {
            foreach (IListener l in _listeners)
            {
                l.EvaluationChanged();
            }
        }

        /// <summary>
        ///     Fires the funds changed event.
        /// </summary>
        public void FireFundsChanged()
        {
            foreach (IListener l in _listeners)
            {
                l.FundsChanged();
            }
        }

        /// <summary>
        ///     Fires the map overlay data changed event.
        /// </summary>
        /// <param name="overlayDataType">Type of the overlay data.</param>
        public void FireMapOverlayDataChanged(MapState overlayDataType)
        {
            foreach (IMapListener l in _mapListeners)
            {
                l.MapOverlayDataChanged(overlayDataType);
            }
        }

        /// <summary>
        ///     Fires the options changed event.
        /// </summary>
        public void FireOptionsChanged()
        {
            foreach (IListener l in _listeners)
            {
                l.OptionsChanged();
            }
        }

        /// <summary>
        ///     Fires the sprite moved event.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void FireSpriteMoved(Sprite sprite)
        {
            foreach (IMapListener l in _mapListeners)
            {
                l.SpriteMoved(sprite);
            }
        }

        /// <summary>
        ///     Fires the tile changed event.
        /// </summary>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        public void FireTileChanged(int xpos, int ypos)
        {
            foreach (IMapListener l in _mapListeners)
            {
                l.TileChanged(xpos, ypos);
            }
        }

        /// <summary>
        ///     Fires the whole map changed event.
        /// </summary>
        public void FireWholeMapChanged()
        {
            foreach (IMapListener l in _mapListeners)
            {
                l.WholeMapChanged();
            }
        }

        #endregion

        // Disaster delay table for each scenario
        public static List<int> DisasterWaitTable = new List<int>
        {
            0,          // No scenario (free playing)
            2,          // Dullsville (boredom)
            10,         // San francisco (earth quake)
            4 * 10,     // Hamburg (fire bombs)
            20,         // Bern (traffic)
            3,          // Tokyo (scary monster)
            5,          // Detroit (crime)
            5,          // Boston (nuclear meltdown)
            2 * 48,     // Rio (flooding)
        };

        // Time to wait before score calculation for each scenario
        public static List<int> ScoreWaitTable = new List<int>
        {
            0,          // No scenario (free playing)
            30 * 48,    // Dullsville (boredom)
            5 * 48,     // San francisco (earth quake)
            5 * 48,     // Hamburg (fire bombs)
            10 * 48,    // Bern (traffic)
            5 * 48,     // Tokyo (scary monster)
            10 * 48,    // Detroit (crime)
            5 * 48,     // Boston (nuclear meltdown)
            10 * 48,    // Rio (flooding)
        };

        private readonly int CC_METROPOLIS = 4;
    }
}