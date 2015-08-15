using System;
using System.Collections.Generic;
using System.Linq;

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
    ///     City Evaluation. Contains the code for performing a city evaluation.
    /// </summary>
    public class CityEval
    {
        /// <summary>
        ///     The random number generator
        /// </summary>
        private readonly Random _prng;

        /// <summary>
        ///     The reference to the engine associated to this object
        /// </summary>
        private readonly Micropolis _engine;

        /// <summary>
        ///     City assessment value.
        /// </summary>
        public int CityAssValue { get; set; }

        /// <summary>
        ///     Classification of city size. 0==village, 1==town, etc.
        /// </summary>
        public int CityClass { get; set; } // 0..5

        /// <summary>
        ///     Percentage of population "disapproving" the mayor. Derived from cityScore.
        /// </summary>
        public int CityNo { get; set; }

        /// <summary>
        ///     City population as of current evaluation.
        /// </summary>
        public int CityPop { get; set; }

        /// <summary>
        ///     Player's score, 0-1000.
        /// </summary>
        public int CityScore { get; set; }

        /// <summary>
        ///     Percentage of population "approving" the mayor. Derived from cityScore.
        /// </summary>
        public int CityYes { get; set; }

        /// <summary>
        ///     Change in cityPopulation since last evaluation.
        /// </summary>
        public int DeltaCityPop { get; set; }

        /// <summary>
        ///     Change in cityScore since last evaluation.
        /// </summary>
        public int DeltaCityScore { get; set; }

        /// <summary>
        ///     City's top 4 (or fewer) problems as reported by citizens.
        /// </summary>
        public CityProblem[] ProblemOrder = new CityProblem[0];

        /// <summary>
        ///     Score for various problems.
        /// </summary>
        public Dictionary<CityProblem, int> ProblemTable = new Dictionary<CityProblem, int>();

        /// <summary>
        ///     Number of votes given for the various problems identified by problemOrder[].
        /// </summary>
        public Dictionary<CityProblem, int> ProblemVotes = new Dictionary<CityProblem, int>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="CityEval" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public CityEval(Micropolis engine)
        {
            _engine = engine;
            _prng = engine.Prng;

            //assert PRNG != null;
        }

        /// <summary>
        ///     Perform an evaluation.
        /// </summary>
        public void CityEvaluation()
        {
            if (_engine.TotalPop != 0)
            {
                CalculateAssValue();
                DoPopNum();
                DoProblems();
                CalculateScore();
                DoVotes();
            }
            else
            {
                EvalInit();
            }
            _engine.FireEvaluationChanged();
        }

        /// <summary>
        ///     Evaluate an empty city.
        /// </summary>
        private void EvalInit()
        {
            CityYes = 0;
            CityNo = 0;
            CityAssValue = 0;
            CityClass = 0;
            CityScore = 500;
            DeltaCityScore = 0;
            ProblemVotes.Clear();
            ProblemOrder = new CityProblem[0];
        }

        /// <summary>
        ///     Calculates the assesment value.
        /// </summary>
        private void CalculateAssValue()
        {
            int z = 0;
            z += _engine.RoadTotal*5;
            z += _engine.RailTotal*10;
            z += _engine.PoliceCount*1000;
            z += _engine.FireStationCount*1000;
            z += _engine.HospitalCount*400;
            z += _engine.StadiumCount*3000;
            z += _engine.SeaportCount*5000;
            z += _engine.AirportCount*10000;
            z += _engine.CoalCount*3000;
            z += _engine.NuclearCount*6000;
            CityAssValue = z*1000;
        }

        /// <summary>
        ///     Determines the city class
        /// </summary>
        /// <remarks>
        ///     <=   2000 = village
        ///     >    2000 = Town
        ///     >  10.000 = City
        ///     >  50.000 = Capital
        ///     > 100.000 = Metropolis
        ///     > 500.000 = Megalopolis
        /// </remarks>
        private void DoPopNum()
        {
            int oldCityPop = CityPop;
            CityPop = _engine.GetCityPopulation();
            DeltaCityPop = CityPop - oldCityPop;

            CityClass =
                CityPop > 500000
                    ? 5
                    : //megalopolis
                    CityPop > 100000
                        ? 4
                        : //metropolis
                        CityPop > 50000
                            ? 3
                            : //capital
                            CityPop > 10000
                                ? 2
                                : //city
                                CityPop > 2000
                                    ? 1
                                    : //town
                                    0; //village
        }


        /// <summary>
        ///     Compares the problem votes of two city problems.
        /// </summary>
        /// <param name="a">city problem a.</param>
        /// <param name="b">city problem b.</param>
        /// <returns></returns>
        public int CompareProbOrder(CityProblem a, CityProblem b)
        {
            return -(ProblemVotes[a].CompareTo(ProblemVotes[b]));
        }

        /// <summary>
        ///     Adds the various problems to the problemTable
        /// </summary>
        private void DoProblems()
        {
            ProblemTable.Clear();
            ProblemTable.Add(CityProblem.CRIME, _engine.CrimeAverage);
            ProblemTable.Add(CityProblem.POLLUTION, _engine.PollutionAverage);
            ProblemTable.Add(CityProblem.HOUSING, (int) Math.Round(_engine.LandValueAverage*0.7));
            ProblemTable.Add(CityProblem.TAXES, _engine.CityTax*10);
            ProblemTable.Add(CityProblem.TRAFFIC, AverageTrf());
            ProblemTable.Add(CityProblem.UNEMPLOYMENT, GetUnemployment());
            ProblemTable.Add(CityProblem.FIRE, GetFire());

            ProblemVotes = VoteProblems(ProblemTable);

            List<CityProblem> valuesAsList = Enum.GetValues(typeof (CityProblem)).Cast<CityProblem>().ToList();
            valuesAsList.Sort(CompareProbOrder);
            CityProblem[] probOrder = valuesAsList.ToArray();


            int c = 0;
            while (c < probOrder.Length &&
                   ProblemVotes[probOrder[c]] != 0 &&
                   c < 4)
                c++;

            ProblemOrder = new CityProblem[c];
            for (int i = 0; i < c; i++)
            {
                ProblemOrder[i] = probOrder[i];
            }
        }

        /// <summary>
        ///     Calculates votes of the various city problems.
        /// </summary>
        /// <param name="probTab">The prob tab.</param>
        /// <returns></returns>
        private Dictionary<CityProblem, int> VoteProblems(Dictionary<CityProblem, int> probTab)
        {
            List<CityProblem> valuesAsList = Enum.GetValues(typeof (CityProblem)).Cast<CityProblem>().ToList();

            CityProblem[] pp = valuesAsList.ToArray();

            var votes = new int[pp.Length];

            int countVotes = 0;
            for (int i = 0; i < 600; i++)
            {
                if (_prng.Next(301) < probTab[pp[i%pp.Length]])
                {
                    votes[i%pp.Length]++;
                    countVotes++;
                    if (countVotes >= 100)
                        break;
                }
            }

            var rv = new Dictionary<CityProblem, int>();
            
            for (int i = 0; i < pp.Length; i++)
            {
                rv.Add(pp[i], votes[i]);
            }
            return rv;
        }

        /// <summary>
        ///     Determines traffic average.
        /// </summary>
        /// <returns></returns>
        private int AverageTrf()
        {
            int count = 1;
            int total = 0;

            for (int y = 0; y < _engine.GetHeight(); y++)
            {
                for (int x = 0; x < _engine.GetWidth(); x++)
                {
                    // only consider tiles that have nonzero landvalue
                    if (_engine.GetLandValue(x, y) != 0)
                    {
                        total += _engine.GetTrafficDensity(x, y);
                        count++;
                    }
                }
            }

            _engine.TrafficAverage = (int) Math.Round((total/(double) count)*2.4);
            return _engine.TrafficAverage;
        }

        /// <summary>
        ///     Determines the unemployment.
        /// </summary>
        /// <returns></returns>
        private int GetUnemployment()
        {
            int b = (_engine.ComPop + _engine.IndPop)*8;
            if (b == 0)
                return 0;

            double r = _engine.ResPop/(double) b;
            b = (int) Math.Floor((r - 1.0)*255);
            if (b > 255)
            {
                b = 255;
            }
            return b;
        }

        /// <summary>
        ///     Determines amount of fires.
        /// </summary>
        /// <returns></returns>
        private int GetFire()
        {
            int z = _engine.FirePop*5;
            return Math.Min(255, z);
        }

        /// <summary>
        ///     Clamps by Max(min, Math.Min(max, x))
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        private static double Clamp(double x, double min, double max)
        {
            return Math.Max(min, Math.Min(max, x));
        }

        /// <summary>
        ///     Calculates the city score.
        /// </summary>
        private void CalculateScore()
        {
            int oldCityScore = CityScore;

            int x = ProblemTable.Values.Sum();

            x /= 3;
            x = Math.Min(256, x);

            double ztemp = Clamp((256 - x)*4, 0, 1000);

            if (_engine.ResCap)
            {
                ztemp = 0.85*ztemp;
            }
            if (_engine.ComCap)
            {
                ztemp = 0.85*ztemp;
            }
            if (_engine.IndCap)
            {
                ztemp = 0.85*ztemp;
            }
            if (_engine.RoadEffect < 32)
            {
                ztemp -= (32 - _engine.RoadEffect);
            }
            if (_engine.PoliceEffect < 1000)
            {
                ztemp *= (0.9 + (_engine.PoliceEffect/10000.1));
            }
            if (_engine.FireEffect < 1000)
            {
                ztemp *= (0.9 + (_engine.FireEffect/10000.1));
            }
            if (_engine.ResValve < -1000)
            {
                ztemp *= 0.85;
            }
            if (_engine.ComValve < -1000)
            {
                ztemp *= 0.85;
            }
            if (_engine.IndValve < -1000)
            {
                ztemp *= 0.85;
            }

            double sm = 1.0;
            if (CityPop == 0 && DeltaCityPop == 0)
            {
                sm = 1.0;
            }
            else if (DeltaCityPop == CityPop)
            {
                sm = 1.0;
            }
            else if (DeltaCityPop > 0)
            {
                sm = DeltaCityPop/(double) CityPop + 1.0;
            }
            else if (DeltaCityPop < 0)
            {
                sm = 0.95 + (DeltaCityPop/(double) (CityPop - DeltaCityPop));
            }
            ztemp *= sm;
            ztemp -= GetFire();
            ztemp -= _engine.CityTax;

            int tm = _engine.UnpoweredZoneCount + _engine.PoweredZoneCount;
            sm = tm != 0 ? (_engine.PoweredZoneCount/(double) tm) : 1.0;
            ztemp *= sm;

            ztemp = Clamp(ztemp, 0, 1000);

            CityScore = (int) Math.Round((CityScore + ztemp)/2.0);
            DeltaCityScore = CityScore - oldCityScore;
        }

        /// <summary>
        ///     Calculates the votes.
        /// </summary>
        private void DoVotes()
        {
            CityYes = CityNo = 0;
            for (int i = 0; i < 100; i++)
            {
                if (_prng.Next(1001) < CityScore)
                {
                    CityYes++;
                }
                else
                {
                    CityNo++;
                }
            }
        }
    }
}