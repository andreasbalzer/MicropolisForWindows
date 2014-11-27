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
    ///     A micropolis tool that can be used to work with the map
    /// </summary>
    public class MicropolisTool
    {
        /// <summary>
        ///     The cost of this tool
        /// </summary>
        public int Cost;

        /// <summary>
        ///     The name of this tool
        /// </summary>
        public string Name;

        /// <summary>
        ///     The size of this tool
        /// </summary>
        public int Size;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MicropolisTool" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="cost">The cost.</param>
        public MicropolisTool(string name, int size, int cost)
        {
            Name = name;
            Size = size;
            Cost = cost;
        }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <returns>width</returns>
        public int GetWidth()
        {
            return Size;
        }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <returns>height</returns>
        public int GetHeight()
        {
            return GetWidth();
        }

        /// <summary>
        ///     Begins the stroke.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public ToolStroke BeginStroke(Micropolis engine, int xpos, int ypos)
        {
            if (Name == "BULLDOZER")
            {
                return new Bulldozer(engine, xpos, ypos);
            }
            if (Name == "WIRE" ||
                Name == "ROADS" ||
                Name == "RAIL")
            {
                return new RoadLikeTool(engine, this, xpos, ypos);
            }
            return new ToolStroke(engine, this, xpos, ypos);
        }

        /// <summary>
        ///     Applies the specified tool to the coordinate.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="xpos">The xpos.</param>
        /// <param name="ypos">The ypos.</param>
        /// <returns></returns>
        public ToolResult Apply(Micropolis engine, int xpos, int ypos)
        {
            return BeginStroke(engine, xpos, ypos).Apply();
        }

        /// <summary>
        ///     Gets the tool cost.
        /// </summary>
        /// <returns>the cost in money</returns>
        /// <remarks>
        ///     This is the cost displayed in the GUI when the tool is selected.
        ///     It does not necessarily reflect the cost charged when a tool is
        ///     applied, as extra may be charged for clearing land or building
        ///     over or through water.
        /// </remarks>
        public int GetToolCost()
        {
            return Cost;
        }

        public override string ToString()
        {
            return Name + ", " + Cost + ", " + Size;
        }
    }
}