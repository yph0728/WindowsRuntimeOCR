using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.Storage;

namespace OCR.ImageParser
{
    public class ImageResult
    {
        private readonly IStorageFile _file;
        private readonly Size _size;

        public Size OriginalSize { get { return _size;  } }

        public IStorageFile File { get { return _file;  } }


        public ImageResult(Size original, IStorageFile file) {
            _size = original;
            _file = file;

        }

    }
}
