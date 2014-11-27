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

    public class History
    {
        public int CityTime;
        public int[] Com = new int[240];
        public int ComMax;
        public int[] Crime = new int[240];
        public int[] Ind = new int[240];
        public int IndMax;
        public int[] Money = new int[240];
        public int[] Pollution = new int[240];
        public int[] Res = new int[240];
        public int ResMax;
    }
}