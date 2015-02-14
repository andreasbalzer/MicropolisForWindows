using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Engine.Controller
{
    public static class MapLoader
    {
        /// <summary>
        ///     Loads the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public static async Task Load(StorageFile file, Micropolis engine)
        {
            Stream stream = await file.OpenStreamForReadAsync();
            LoadFile(stream, engine);
        }

        /// <summary>
        ///     Loads the specified stream.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        public static async Task Load(Stream inStream, Micropolis engine)
        {
            Stream dis = inStream;
            loadHistoryArray(engine.History.Res, dis);
            loadHistoryArray(engine.History.Com, dis);
            loadHistoryArray(engine.History.Ind, dis);
            loadHistoryArray(engine.History.Crime, dis);
            loadHistoryArray(engine.History.Pollution, dis);
            loadHistoryArray(engine.History.Money, dis);
            LoadMisc(dis, engine);
            LoadMap(dis, engine);
            dis.Flush();
            dis.Dispose();

            engine.CheckPowerMap();

            engine.FireWholeMapChanged();
            engine.FireDemandChanged();
            engine.FireFundsChanged();
        }

        /// <summary>
        ///     Loads the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static void LoadFile(Stream stream, Micropolis engine)
        {
            if (stream.Length == 0)
            {
                throw new ArgumentException("File empty");   
            }
            else if (stream.Length > 27120)
            {
                // some editions of the classic Simcity game
                // start the file off with a 128-byte header,
                // but otherwise use the same format as us,
                // so read in that 128-byte header and continue
                // as before.
                var bbHeader = new byte[128];
                stream.Read(bbHeader, 0, 128);
            }
            Load(stream, engine);
        }

        private static void LoadMap(Stream dis, Micropolis engine)
        {
            var reader = new MyBinaryReader(dis);
            for (int x = 0; x < Micropolis.DEFAULT_WIDTH; x++)
            {
                for (int y = 0; y < Micropolis.DEFAULT_HEIGHT; y++)
                {
                    int z = reader.ReadInt16();
                    z &= ~(1024 | 2048 | 4096 | 8192 | 16384);
                    // clear ZONEBIT,ANIMBIT,BULLBIT,BURNBIT,CONDBIT on import
                    engine.Map[y][x] = (char) z;
                }
            }
        }

        private static void loadHistoryArray(int[] array, Stream dis)
        {
            var reader = new MyBinaryReader(dis); //changed
            for (int i = 0; i < 240; i++)
            {
                array[i] = reader.ReadInt16();
            }
        }


        private static void LoadMisc(Stream dis, Micropolis engine)
        {
            var reader = new MyBinaryReader(dis);
            reader.ReadInt16(); //[0]... ignored?
            reader.ReadInt16(); //[1] externalMarket, ignored
            engine.ResPop = reader.ReadInt16(); //[2-4] populations
            engine.ComPop = reader.ReadInt16();
            engine.IndPop = reader.ReadInt16();
            engine.ResValve = reader.ReadInt16(); //[5-7] valves
            engine.ComValve = reader.ReadInt16();
            engine.IndValve = reader.ReadInt16();
            engine.CityTime = reader.ReadInt32(); //[8-9] city time
            engine.CrimeRamp = reader.ReadInt16(); //[10]
            engine.PolluteRamp = reader.ReadInt16();
            engine.LandValueAverage = reader.ReadInt16(); //[12]
            engine.CrimeAverage = reader.ReadInt16();
            engine.PollutionAverage = reader.ReadInt16(); //[14]
            engine.GameLevel = reader.ReadInt16();
            engine.Evaluation.CityClass = reader.ReadInt16(); //[16]
            engine.Evaluation.CityScore = reader.ReadInt16();

            for (int i = 18; i < 50; i++)
            {
                reader.ReadInt16();
            }

            engine.Budget.TotalFunds = reader.ReadInt32(); //[50-51] total funds
            engine.AutoBulldoze = reader.ReadInt16() != 0; //52
            engine.AutoBudget = reader.ReadInt16() != 0;
            engine.AutoGo = reader.ReadInt16() != 0; //54
            reader.ReadInt16(); // userSoundOn (this setting not saved to game file
            // in this edition of the game)
            engine.CityTax = reader.ReadInt16(); //56
            engine.TaxEffect = engine.CityTax;
            int simSpeedAsInt = reader.ReadInt16();
            if (simSpeedAsInt >= 0 && simSpeedAsInt <= 4)
                engine.SimSpeed =
                    Speeds.Speed.FirstOrDefault(s => s.Key == Speeds.Speed.Keys.ToList()[simSpeedAsInt]).Value;
            else
                engine.SimSpeed = Speeds.Speed["NORMAL"];

            // read budget numbers, convert them to percentages
            //
            long n = reader.ReadInt32(); //58,59... police percent
            engine.PolicePercent = n/65536.0;
            n = reader.ReadInt32(); //60,61... fire percent
            engine.FirePercent = n/65536.0;
            n = reader.ReadInt32(); //62,63... road percent
            engine.RoadPercent = n/65536.0;

            for (int i = 64; i < 120; i++)
            {
                reader.ReadInt16();
            }

            if (engine.CityTime < 0)
            {
                engine.CityTime = 0;
            }
            if (engine.CityTax < 0 || engine.CityTax > 20)
            {
                engine.CityTax = 7;
            }
            if (engine.GameLevel < 0 || engine.GameLevel > 2)
            {
                engine.GameLevel = 0;
            }
            if (engine.Evaluation.CityClass < 0 || engine.Evaluation.CityClass > 5)
            {
                engine.Evaluation.CityClass = 0;
            }
            if (engine.Evaluation.CityScore < 1 || engine.Evaluation.CityScore > 999)
            {
                engine.Evaluation.CityScore = 500;
            }

            engine.ResCap = false;
            engine.ComCap = false;
            engine.IndCap = false;
        }
    }
}