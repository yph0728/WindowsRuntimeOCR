using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace OCR.ImageParser
{
    public class ImageResizer
    {
        private readonly SizeValidator _sizeValidator;
        private bool _ensureSuccessfullScale;





        public ImageResizer(SizeValidator sizeValidator)
        {
            _sizeValidator = sizeValidator;
        }

        public void EnsureSuccesfullScale() {
            _ensureSuccessfullScale = true;
        }

        public async Task<ImageResult> ResizeAsync(IStorageFile file) {
            var imageProperties = await ((StorageFile)file).Properties.GetImagePropertiesAsync();
            var oldSize = new Size(imageProperties.Width, imageProperties.Height);
            if(_sizeValidator != null) {
                var scalability = _sizeValidator.GetScalingInformation(oldSize);

                if (_sizeValidator.WithinBoundaries(oldSize)) {
                    return new ImageResult(oldSize, file as StorageFile);
                }

                if (scalability.CanScale)
                {
                    var newHeight = imageProperties.Height * scalability.ScaleFactor;
                    var newWidth = imageProperties.Width * scalability.ScaleFactor;
                    var newSize = new Size(newWidth, newHeight);
                    return await ResizeAsync(file, oldSize, newSize);
                }

                else {
                    if (_ensureSuccessfullScale)
                        throw new Exception("image can not be resized");
                }
            }

            return new ImageResult(oldSize, file as StorageFile);
        }

        private async Task<ImageResult> ResizeAsync(IStorageFile file, Size oldSize, Size newSize)
        {
            var newFile = await CreateNewFile();
            try
            {
                using (IRandomAccessStream source = await file.OpenAsync(FileAccessMode.Read)) {
                    var decoder = await BitmapDecoder.CreateAsync(source);
                    var transform = new BitmapTransform {
                        ScaledHeight = (uint)newSize.Height,
                        ScaledWidth = (uint)newSize.Width
                    };
                    var pixelData = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

                    using (IRandomAccessStream destinationStream = await newFile.OpenAsync(FileAccessMode.ReadWrite)) {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destinationStream);
                        encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, (uint)newSize.Width,
                            (uint)newSize.Height, 96, 96, pixelData.DetachPixelData());
                        await encoder.FlushAsync();

                    }



                }


            }
            catch (Exception ex) {
                if (_ensureSuccessfullScale)
                    throw new Exception("image can not be resized123");

            }
            return new ImageResult(oldSize, newFile);
        }

        private async Task<StorageFile> CreateNewFile()
        {
            
            string tempFileName = string.Format("{0}.jpg", Guid.NewGuid());
            //await ApplicationData.Current.LocalFolder.CreateFileAsync(tempFileName, CreationCollisionOption.ReplaceExisting);
            //await FileIO.WriteBytesAsync(file);
            return await ApplicationData.Current.TemporaryFolder.CreateFileAsync(tempFileName, CreationCollisionOption.ReplaceExisting);
        }
    }
}
