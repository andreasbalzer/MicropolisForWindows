using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    public static class DebugMapDumper
    {
        private static void ToDebug(char[][] map)
        {
            for (int iy = 0; iy < map.Length; iy++)
            {
                string outi = "";
                char last = ' ';
                for (int ix = 0; ix < map[0].Length; ix++)
                {
                    if (last != map[iy][ix])
                    {
                        outi += ".";
                        last = map[iy][ix];
                    }
                    else
                    {
                        outi += " ";
                    }
                }
                Debug.WriteLine(outi);
            }
            Debug.WriteLine("-----------------------");
            for (int iy = 0; iy < map.Length; iy++)
            {
                string outi = "";
                char last = ' ';
                for (int ix = 0; ix < map[0].Length; ix++)
                {
                    outi += (int)map[iy][ix];

                    if (ix < map[0].Length - 1)
                    {
                        outi += ",";
                    }
                }
                Debug.WriteLine(outi);
            }
            Debug.WriteLine("-----------------------");
        }
    }
}
