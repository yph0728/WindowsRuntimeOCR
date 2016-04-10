using System;
using System.Collections.Generic;
using System.Text;

namespace OCR.ImageParser
{
    public class Scalability
    {
        public bool CanScale { get; set; }
        public double ScaleFactor { get; set; }

        public Scalability(bool canScale)
        {
            CanScale = canScale;
        }

    }
}
