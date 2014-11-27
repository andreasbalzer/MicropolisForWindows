using System.IO;

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
    ///     Binary reader with endian conversion
    /// </summary>
    /// <remarks>
    ///     source
    ///     http://stackoverflow.com/questions/12017244/readint16-in-c-sharp-returns-something-different-than-what-writeshort-in-java-wr
    /// </remarks>
    /// <author>
    ///     external
    /// </author>
    public class MyBinaryReader : BinaryReader
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MyBinaryReader" /> class.
        /// </summary>
        /// <param name="s">The s.</param>
        public MyBinaryReader(Stream s)
            : base(s)
        {
        }

        /// <summary>
        ///     Reads an int.
        /// </summary>
        /// <returns></returns>
        public override int ReadInt32()
        {
            return HostToNetworkOrder(base.ReadInt32());
        }

        /// <summary>
        ///     Reads an int.
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            return ReadInt32();
        }

        /// <summary>
        ///     Reads a short.
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            return ReadInt16();
        }

        /// <summary>
        ///     Reads a two-byte-natural number with sign of the stream and increases the current position by two bytes.
        /// </summary>
        /// <returns>
        ///     A whole number made up of two bytes with sign from the stream.
        /// </returns>
        public override short ReadInt16()
        {
            return HostToNetworkOrder(base.ReadInt16());
        }

        /// <summary>
        ///     Host to network order.
        /// </summary>
        /// <param name="host">The host ordered number.</param>
        /// <returns>network ordered number</returns>
        /// <remarks>source http://stackoverflow.com/questions/11798356/ipaddress-hosttonetworkorder-equivalent-in-winrt</remarks>
        /// <author>external</author>
        public static short HostToNetworkOrder(short host)
        {
            return (short) (((host & 0xff) << 8) | ((host >> 8) & 0xff));
        }

        /// <summary>
        ///     Host to network order.
        /// </summary>
        /// <param name="host">The host ordered number.</param>
        /// <returns>network ordered number</returns>
        /// <remarks>source http://stackoverflow.com/questions/11798356/ipaddress-hosttonetworkorder-equivalent-in-winrt</remarks>
        /// <author>external</author>
        public static int HostToNetworkOrder(int host)
        {
            return (((HostToNetworkOrder((short) host) & 0xffff) << 0x10) |
                    (HostToNetworkOrder((short) (host >> 0x10)) & 0xffff));
        }
    }
}