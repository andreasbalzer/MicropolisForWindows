using System;
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
    ///     Binary writer with endian conversion
    /// </summary>
    /// <remarks>
    ///     source http://dan.clarke.name/2011/07/converting-floatsingle-to-network-order-and-back-in-c/
    /// </remarks>
    /// <author>
    ///     external
    /// </author>
    public class MyBinaryWriter : BinaryWriter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MyBinaryReader" /> class.
        /// </summary>
        /// <param name="s">The s.</param>
        public MyBinaryWriter(Stream s)
            : base(s)
        {
        }

        /// <summary>
        ///     Reads an int.
        /// </summary>
        /// <returns></returns>
        public void WriteInt32(int value)
        {
            base.Write(NetworkToHostOrder(value));
        }

        /// <summary>
        ///     Reads an int.
        /// </summary>
        /// <returns></returns>
        public void WriteInt(int value)
        {
            WriteInt32(value);
        }

        /// <summary>
        ///     Reads a short.
        /// </summary>
        /// <returns></returns>
        public void WriteShort(short value)
        {
            WriteInt16(value);
        }

        /// <summary>
        ///     Reads a two-byte-natural number with sign of the stream and increases the current position by two bytes.
        /// </summary>
        /// <returns>
        ///     A whole number made up of two bytes with sign from the stream.
        /// </returns>
        public void WriteInt16(short value)
        {
            base.Write(NetworkToHostOrder(value));
        }

        /// <summary>
        ///     Convert a float to host order
        /// </summary>
        /// <param name="network">Float to convert</param>
        /// <returns>Float in host order</returns>
        /// <remarks>http://dan.clarke.name/2011/07/converting-floatsingle-to-network-order-and-back-in-c/</remarks>
        /// <author>external</author>
        public static int NetworkToHostOrder(int network)
        {
            byte[] bytes = BitConverter.GetBytes(network);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return (int) BitConverter.ToInt32(bytes, 0);
        }


        /// <summary>
        ///     Convert a float to host order
        /// </summary>
        /// <param name="network">Float to convert</param>
        /// <returns>Float in host order</returns>
        /// <remarks>http://dan.clarke.name/2011/07/converting-floatsingle-to-network-order-and-back-in-c/</remarks>
        /// <author>external</author>
        public static short NetworkToHostOrder(short network)
        {
            byte[] bytes = BitConverter.GetBytes(network);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return (short) BitConverter.ToInt16(bytes, 0);
        }

        public new void Write(object value)
        {
            throw new NotSupportedException("Values have to be converted and there is no default converter available. Use value specific methods.");
        }
    }
}