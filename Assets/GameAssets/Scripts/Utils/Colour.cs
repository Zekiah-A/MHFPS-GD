using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Utils.Colour //TODO: Use a record!
{
    public class Colour
    {
        ///<summary>Red colour value.</summary>
        public byte R;
        ///<summary>Green colour value.</summary>
        public byte G;
        ///<summary>Blue colour value.</summary>
        public byte B;
        ///<summary>Alpha value.</summary>
        public byte A;

        public Colour(byte r, byte g, byte b, byte a)// => new Colour(_r, _g, _b, _a);
        {
            //Colour Colour = new Colour(_r, _g, _b, _a);
            //assign rgba instead
            R = r; G = g; B = b; A = a;
        }
    }
}
