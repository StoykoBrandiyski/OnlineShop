using Microsoft.EntityFrameworkCore;
using OnlineShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Tests.Services.Common
{
    public static class OnlineShopDbContextInMemoryFactory
    {

        public static OnlineShopDbContext InitializeContext()
        {
            var options = new DbContextOptionsBuilder<OnlineShopDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

            return new OnlineShopDbContext(options);
        }

    
       
    }
}
