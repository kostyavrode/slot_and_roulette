using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mkey
{
    public static class SortingOrder
    {
        #region sorting orders
        public static int Base  // bkg
        {
            get
            {
                return 1;
            }
        }

        public static int Units
        {
            get
            {
                return Base + 3;
            }
        }

        public static int Symbols
        {
            get
            {
                return Base + 6;
            }
        }

        public static int SymbolsBlur
        {
            get
            {
                return Base + 8;
            }
        }

        public static int SymbolsToFront
        {
            get
            {
                return Base + 10;
            }
        }

        public static int Lines
        {
            get
            {
                return Base + 12;
            }
        }

        public static int LinesButtonShelf
        {
            get
            {
                return Base + 14;
            }
        }

        public static int LinesButton
        {
            get
            {
                return Base + 16;
            }
        }

        #endregion sorting orders
    }
}