using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Engine.Controller
{
    public static class MapSaver
    {
        /// <summary>
        ///     Saves the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static async Task Save(StorageFile file, Micropolis engine)
        {
            Stream stream = await file.OpenStreamForWriteAsync();
            Save(stream, engine);
        }

        /// <summary>
        ///     Saves the specified stream.
        /// </summary>
        /// <param name="outStream">The out stream.</param>
        public static void Save(Stream outStream, Micropolis engine)
        {
            writeHistoryArray(engine.History.Res, outStream);
            writeHistoryArray(engine.History.Com, outStream);
            writeHistoryArray(engine.History.Ind, outStream);
            writeHistoryArray(engine.History.Crime, outStream);
            writeHistoryArray(engine.History.Pollution, outStream);
            writeHistoryArray(engine.History.Money, outStream);
            WriteMisc(outStream, engine);
            WriteMap(outStream, engine);
            outStream.Flush();
            outStream.Dispose();
        }

        private static void writeHistoryArray(int[] array, Stream outStream)
        {
            var writer = new MyBinaryWriter(outStream);
            for (int i = 0; i < 240; i++)
            {
                writer.WriteShort((short) array[i]);
            }
        }

        private static void WriteMisc(Stream outStream, Micropolis engine)
        {
            var writer = new MyBinaryWriter(outStream);
            writer.WriteShort((short) 0);
            writer.WriteShort((short) 0);
            writer.WriteShort((short) engine.ResPop);
            writer.WriteShort((short) engine.ComPop);
            writer.WriteShort((short) engine.IndPop);
            writer.WriteShort((short) engine.ResValve);
            writer.WriteShort((short) engine.ComValve);
            writer.WriteShort((short) engine.IndValve);
            //8
            writer.WriteInt32(engine.CityTime);
            writer.WriteShort((short) engine.CrimeRamp);
            writer.WriteShort((short) engine.PolluteRamp);
            //12
            writer.WriteShort((short) engine.LandValueAverage);
            writer.WriteShort((short) engine.CrimeAverage);
            writer.WriteShort((short) engine.PollutionAverage);
            writer.WriteShort((short) engine.GameLevel);
            //16
            writer.WriteShort((short) engine.Evaluation.CityClass);
            writer.WriteShort((short) engine.Evaluation.CityScore);
            //18
            for (int i = 18; i < 50; i++)
            {
                writer.WriteShort((short) 0);
            }
            //50
            writer.WriteInt32(engine.Budget.TotalFunds);
            writer.WriteShort((short) (engine.AutoBulldoze ? 1 : 0));
            writer.WriteShort((short) (engine.AutoBudget ? 1 : 0));
            //54
            writer.WriteShort((short) (engine.AutoGo ? 1 : 0));
            writer.WriteShort((short) 1); //userSoundOn
            writer.WriteShort((short) engine.CityTax);

            int speedKeyToWrite = 0;
            foreach (string key in Speeds.Speed.Keys)
            {
                if (Speeds.Speed[key].SimStepsPerUpdate == engine.SimSpeed.SimStepsPerUpdate)
                {
                    break;
                }
                speedKeyToWrite++;
            }
            writer.WriteShort((short) (speedKeyToWrite));

            //58
            writer.WriteInt32((int) (engine.PolicePercent*65536));
            writer.WriteInt32((int) (engine.FirePercent*65536));
            writer.WriteInt32((int) (engine.RoadPercent*65536));

            //64
            for (int i = 64; i < 120; i++)
            {
                writer.WriteShort((short) 0);
            }
        }

        private static void WriteMap(Stream outStream, Micropolis engine)
        {
            var writer = new MyBinaryWriter(outStream);
            for (int x = 0; x < Micropolis.DEFAULT_WIDTH; x++)
            {
                for (int y = 0; y < Micropolis.DEFAULT_HEIGHT; y++)
                {
                    int z = engine.Map[y][x];
                    if (TileConstants.IsConductive(z & TileConstants.LOMASK))
                    {
                        z |= 16384; //synthesize CONDBIT on export
                    }
                    if (TileConstants.IsCombustible(z & TileConstants.LOMASK))
                    {
                        z |= 8192; //synthesize BURNBIT on export
                    }
                    if (engine.IsTileDozeable(x, y))
                    {
                        z |= 4096; //synthesize BULLBIT on export
                    }
                    if (TileConstants.IsAnimated(z & TileConstants.LOMASK))
                    {
                        z |= 2048; //synthesize ANIMBIT on export
                    }
                    if (TileConstants.IsZoneCenter(z & TileConstants.LOMASK))
                    {
                        z |= 1024; //synthesize ZONEBIT
                    }
                    writer.WriteShort((short) z);
                }
            }
        }
    }
}