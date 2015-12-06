using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SII
{
    class InfoLocation
    {
        public int idTask;
        public int idSelection;

        //signleton
        private static InfoLocation curInfoLocation;

        public InfoLocation()
        {
        }

        public static InfoLocation CurInfoLocation
        {
            get
            {
                return curInfoLocation;
            }
            set
            {
                curInfoLocation = value;
            }
        }
    }
}
