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
    ///     Tile Specification describing a tile
    /// </summary>
    public class TileSpec
    {
        private readonly Dictionary<String, String> _attributes;
        private readonly List<String> _images;

        /// <summary>
        ///     The next animation
        /// </summary>
        public TileSpec AnimNext;

        private BuildingInfo _buildingInfo;

        /// <summary>
        ///     Whether tile can be bulldozed
        /// </summary>
        public bool CanBulldoze;

        /// <summary>
        ///     Whether tile can be burned in fire
        /// </summary>
        public bool CanBurn;

        /// <summary>
        ///     Whether tile can conduct electricity (?!)
        /// </summary>
        public bool CanConduct;

        /// <summary>
        ///     What to do when powered
        /// </summary>
        public TileSpec OnPower;

        /// <summary>
        ///     What to do when destroyed
        /// </summary>
        public TileSpec OnShutdown;

        /// <summary>
        ///     Whether tile can be placed over water
        /// </summary>
        public bool OverWater;

        /// <summary>
        ///     The owner of this tile
        /// </summary>
        public TileSpec Owner;

        /// <summary>
        ///     The offset x relative to its owner
        /// </summary>
        public int OwnerOffsetX;

        /// <summary>
        ///     The offset y relative to its owner
        /// </summary>
        public int OwnerOffsetY;

        /// <summary>
        ///     The tile number
        /// </summary>
        public int TileNumber;

        /// <summary>
        ///     Whether tile is a zone
        /// </summary>
        public bool Zone;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TileSpec" /> class.
        /// </summary>
        /// <param name="tileNumber">The tile number.</param>
        protected TileSpec(int tileNumber)
        {
            TileNumber = tileNumber;
            _attributes = new Dictionary<String, String>();
            _images = new List<String>();
        }

        /// <summary>
        ///     Parses the specified tile number.
        /// </summary>
        /// <param name="tileNumber">The tile number.</param>
        /// <param name="inStr">The in string.</param>
        /// <param name="tilesRc">The tiles rc.</param>
        /// <returns>The tile specification</returns>
        public static TileSpec Parse(int tileNumber, String inStr, IList<string> tilesRc)
        {
            var ts = new TileSpec(tileNumber);
            ts.Load(inStr, tilesRc);
            return ts;
        }

        /// <summary>
        ///     Gets the attribute value of the attribute identified by key.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <returns>the attribute value</returns>
        public String GetAttribute(String key)
        {
            if (!_attributes.ContainsKey(key))
            {
                return null;
            }

            return _attributes[key];
        }

        /// <summary>
        ///     Gets the value of the attribute identified by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>value of the attribute</returns>
        public bool GetboolAttribute(String key)
        {
            String v = GetAttribute(key);
            return (v != null && v.Equals("true"));
        }


        /// <summary>
        ///     Gets the building information.
        /// </summary>
        /// <returns></returns>
        public BuildingInfo GetBuildingInfo()
        {
            return _buildingInfo;
        }

        /// <summary>
        ///     Resolves the building information.
        /// </summary>
        /// <param name="tileMap">The tile map.</param>
        private void ResolveBuildingInfo(Dictionary<String, TileSpec> tileMap)
        {
            String tmp = GetAttribute("building");
            if (tmp == null)
            {
                return;
            }

            var bi = new BuildingInfo();

            String[] p2 = tmp.Split('x');
            bi.Width = Convert.ToInt32(p2[0]);
            bi.Height = Convert.ToInt32(p2[1]);

            bi.Members = new short[bi.Width*bi.Height];
            int startTile = TileNumber;
            if (bi.Width >= 3)
            {
                startTile--;
            }
            if (bi.Height >= 3)
            {
                startTile -= bi.Width;
            }

            for (int row = 0; row < bi.Height; row++)
            {
                for (int col = 0; col < bi.Width; col++)
                {
                    bi.Members[row*bi.Width + col] = (short) startTile;
                    startTile++;
                }
            }

            _buildingInfo = bi;
        }

        /// <summary>
        ///     Gets the size of the building.
        /// </summary>
        /// <returns></returns>
        public CityDimension GetBuildingSize()
        {
            if (_buildingInfo != null)
            {
                return new CityDimension(
                    _buildingInfo.Width,
                    _buildingInfo.Height
                    );
            }
            return null;
        }

        /// <summary>
        ///     Gets the description number.
        /// </summary>
        /// <returns></returns>
        public int GetDescriptionNumber()
        {
            String v = GetAttribute("description");
            if (v != null && v.StartsWith("#"))
            {
                return Convert.ToInt32(v.Substring(1));
            }
            if (Owner != null)
            {
                return Owner.GetDescriptionNumber();
            }
            return -1;
        }

        /// <summary>
        ///     Gets the images.
        /// </summary>
        /// <returns></returns>
        public String[] GetImages()
        {
            return _images.ToArray();
        }

        /// <summary>
        ///     Gets the pollution value.
        /// </summary>
        /// <returns></returns>
        public int GetPollutionValue()
        {
            String v = GetAttribute("pollution");
            if (v != null)
            {
                return Convert.ToInt32(v);
            }
            if (Owner != null)
            {
                // pollution inherits from building tile
                return Owner.GetPollutionValue();
            }
            return 0;
        }

        /// <summary>
        ///     Gets the population.
        /// </summary>
        /// <returns></returns>
        public int GetPopulation()
        {
            String v = GetAttribute("population");
            if (v != null)
            {
                return Convert.ToInt32(v);
            }
            return 0;
        }

        /// <summary>
        ///     Loads the specified in string.
        /// </summary>
        /// <param name="inStr">The in string.</param>
        /// <param name="tilesRc">The tiles rc.</param>
        protected void Load(String inStr, IList<string> tilesRc)
        {
            var inData = new Scanner(inStr);

            while (inData.HasMore())
            {
                if (inData.PeekChar() == '(')
                {
                    inData.EatChar('(');
                    String k = inData.ReadAttributeKey();
                    String v = "true";
                    if (inData.PeekChar() == '=')
                    {
                        inData.EatChar('=');
                        v = inData.ReadAttributeValue();
                    }
                    inData.EatChar(')');

                    if (!_attributes.ContainsKey(k))
                    {
                        _attributes.Add(k, v);
                        String sup = tilesRc.FirstOrDefault(s => s.StartsWith(k));
                        if (sup != null)
                        {
                            Load(sup, tilesRc);
                        }
                    }
                    else
                    {
                        _attributes.Add(k, v);
                    }
                }

                else if (inData.PeekChar() == '|' || inData.PeekChar() == ',')
                {
                    inData.EatChar(inData.PeekChar());
                }

                else
                {
                    String v = inData.ReadImageSpec();
                    _images.Add(v);
                }
            }

            CanBulldoze = GetboolAttribute("bulldozable");
            CanBurn = !GetboolAttribute("noburn");
            CanConduct = GetboolAttribute("conducts");
            OverWater = GetboolAttribute("overwater");
            Zone = GetboolAttribute("zone");
        }


        /// <summary>
        ///     Returns the TileSpec as a string
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "{tile#" + TileNumber + "}";
        }

        /// <summary>
        ///     Resolves the references.
        /// </summary>
        /// <param name="tileMap">The tile map.</param>
        public void ResolveReferences(Dictionary<String, TileSpec> tileMap)
        {
            String tmp = GetAttribute("becomes");
            if (tmp != null)
            {
                if (!tileMap.ContainsKey(tmp))
                {
                    AnimNext = null;
                }
                else
                {
                    AnimNext = tileMap[tmp];
                }
            }
            tmp = GetAttribute("onpower");
            if (tmp != null)
            {
                OnPower = tileMap[tmp];
            }
            tmp = GetAttribute("onshutdown");
            if (tmp != null)
            {
                OnShutdown = tileMap[tmp];
            }
            tmp = GetAttribute("building-part");
            if (tmp != null)
            {
                HandleBuildingPart(tmp, tileMap);
            }

            ResolveBuildingInfo(tileMap);
        }

        /// <summary>
        ///     Handles the building part.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="tileMap">The tile map.</param>
        /// <exception cref="Exception">Invalid building-part specification</exception>
        private void HandleBuildingPart(String text, Dictionary<String, TileSpec> tileMap)
        {
            String[] parts = text.Split(',');
            if (parts.Length != 3)
            {
                throw new Exception("Invalid building-part specification");
            }

            Owner = tileMap[parts[0]];
            OwnerOffsetX = Convert.ToInt32(parts[1]);
            OwnerOffsetY = Convert.ToInt32(parts[2]);

            //assert this.owner != null;
            //assert this.ownerOffsetX != 0 || this.ownerOffsetY != 0;
        }

        /*
        public static String[] generateTileNames(Properties recipe)
        {
            int ntiles = recipe.Size();
            String[] tileNames = new String[ntiles];
            ntiles = 0;
            for (int i = 0; recipe.ContainsKey(i.ToString()); i++)
            {
                tileNames[ntiles++] = i.ToString();
            }
            int naturalNumberTiles = ntiles;

            foreach (Object n_obj in recipe.keySet())
            {
                String n = (String) n_obj;
                if (Regex.IsMatch(n, "^\\d+$"))
                {
                    int x = Convert.ToInt32(n);
                    if (x >= 0 && x < naturalNumberTiles)
                    {
                        //assert tileNames[x].Equals(n);
                        continue;
                    }
                }
                //assert ntiles < tileNames.Length;
                tileNames[ntiles++] = n;
            }
            //assert ntiles == tileNames.Length;
            return tileNames;
        }*/
    }
}