using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data.Contracts
{
    public interface IImageService
    {
        void UploadImage(IFormFile image, string toDirectory);

        Task<IEnumerable<string>> UploadImages(IList<IFormFile> images, int count, string template, string productId);
    }
}
