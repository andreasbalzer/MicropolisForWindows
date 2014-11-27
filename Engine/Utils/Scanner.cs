using System;

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
    ///     Scans the Tiles.RC file
    /// </summary>
    public class Scanner
    {
        private readonly String _str;
        private int _off;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Scanner" /> class.
        /// </summary>
        /// <param name="str">The string.</param>
        public Scanner(String str)
        {
            _str = str;
        }

        private void SkipWhitespace()
        {
            while (_off < _str.Length && Char.IsWhiteSpace(_str[_off]))
            {
                _off++;
            }
        }

        /// <summary>
        ///     Peeks a character.
        /// </summary>
        /// <returns></returns>
        public int PeekChar()
        {
            SkipWhitespace();
            if (_off < _str.Length)
            {
                return _str[_off];
            }
            return -1;
        }

        /// <summary>
        ///     Eats the character.
        /// </summary>
        /// <param name="ch">The ch.</param>
        public void EatChar(int ch)
        {
            SkipWhitespace();
            //assert str.charAt(off) == ch;
            _off++;
        }

        /// <summary>
        ///     Reads the attribute key.
        /// </summary>
        /// <returns></returns>
        public String ReadAttributeKey()
        {
            SkipWhitespace();

            int start = _off;
            while (_off < _str.Length && (_str[_off] == '-' || char.IsLetterOrDigit(_str[_off])))
            {
                _off++;
            }

            if (_off != start)
            {
                return _str.Substring(start, (_off - start));
            }
            return null;
        }

        /// <summary>
        ///     Reads the attribute value.
        /// </summary>
        /// <returns></returns>
        public String ReadAttributeValue()
        {
            return ReadString();
        }

        /// <summary>
        ///     Reads the image spec.
        /// </summary>
        /// <returns></returns>
        public String ReadImageSpec()
        {
            return ReadString();
        }

        /// <summary>
        ///     Reads the string.
        /// </summary>
        /// <returns></returns>
        protected String ReadString()
        {
            SkipWhitespace();

            int endQuote = 0; //any whitespace or certain punctuation
            if (PeekChar() == '"')
            {
                _off++;
                endQuote = '"';
            }

            int start = _off;
            while (_off < _str.Length)
            {
                int c = _str[_off];
                if (c == endQuote)
                {
                    int end = _off;
                    _off++;
                    return _str.Substring(start, (end - start));
                }
                if (endQuote == 0 && (char.IsWhiteSpace((char) c) || ((char) c) == ')' || ((char) c) == '|'))
                {
                    int end = _off;
                    return _str.Substring(start, (end - start));
                }
                _off++;
            }
            return _str.Substring(start);
        }

        /// <summary>
        ///     Determines whether this instance has more.
        /// </summary>
        /// <returns></returns>
        public bool HasMore()
        {
            return PeekChar() != -1;
        }
    }
}