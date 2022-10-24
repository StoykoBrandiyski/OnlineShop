using Common;
using Microsoft.AspNetCore.Http;
using OnlineShop.Service.Data.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data
{
    public class ImageService : IImageService
    {

        public async void UploadImage(IFormFile image, string toDirectory)
        {
            using (var stream = new FileStream(toDirectory,FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
        }

        public async Task<IEnumerable<string>> UploadImages(IList<IFormFile> images, int existingImages, string template, string productId)
        {
            
            List<string> imageUrls = new List<string>();

            for (int i = 0; i < images.Count; i++)
            {
                string urlName = $"Id{productId}_{existingImages + i}";
                string imagePath = string.Format(template, urlName);

                using (var stream = new FileStream(imagePath,FileMode.Create))
                {
                    await images[i].CopyToAsync(stream);
                }

                string imageRoot = imagePath.Replace(GlobalConstants.WWWROOT, "");
                imageUrls.Add(imageRoot);

            }
            return imageUrls;
        }
    }
}
