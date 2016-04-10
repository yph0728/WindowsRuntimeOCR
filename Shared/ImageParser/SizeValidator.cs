using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;

namespace OCR.ImageParser
{
    public class SizeValidator
    {
        private double _minimumThreshold;
        private double _maximumThreshold;


        public double MaxRatioForScale { get { return _maximumThreshold / _minimumThreshold; } }

        public SizeValidator(double minimum, double maximum)
        {
            _minimumThreshold = minimum;
            _maximumThreshold = maximum;
        }

        public bool WithinBoundaries(Size size) {
            return
                size.Width >= _minimumThreshold &&
                size.Height >= _minimumThreshold &&
                size.Width <= _maximumThreshold &&
                size.Height <= _maximumThreshold;

        }


        public bool BothOutSideBoundaries(Size size) {
            return (size.Width <= _minimumThreshold && size.Height >= _maximumThreshold) ||
                (size.Width >= _maximumThreshold && size.Height <= _minimumThreshold);
        }


        public Scalability GetScalingInformation(Size size)
        {
            var width = size.Width;
            var height = size.Height;

            if (width <= 1 || height <= 1)
                return new Scalability(false);

            //comparing width and height to see 
            var isValidRatio = width > height ? width / height <= MaxRatioForScale : height / width <= MaxRatioForScale;

            var scalability = new Scalability(isValidRatio);

            if (BothOutSideBoundaries(size) || size.IsEmpty)
                return scalability;


            if (height < _minimumThreshold || width < _minimumThreshold) {
                scalability.ScaleFactor = width < height ? _minimumThreshold / width : _minimumThreshold / height;
                return scalability;
            }
            if (height > _maximumThreshold || width > _maximumThreshold)
            {
                scalability.ScaleFactor = width > height ? _maximumThreshold / width : _maximumThreshold / height;
                return scalability;
            }


            return scalability;
        }

       
    }
}
