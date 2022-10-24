using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data
{
    public class AddressService : IAddresService
    {
        private readonly OnlineShopDbContext dbContext;
        private readonly IUserService userService;

        public AddressService(OnlineShopDbContext dbContext,IUserService userService)
        {
            this.dbContext = dbContext;
            this.userService = userService;
        }


        public int AddAddressToUser(string username, Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            
            ShopUser user = this.userService.GetUserByUsername(username);

            if(user == null)
            {
                return 0;
            }
            
            user.Addresses.Add(address);
            return this.dbContext.SaveChanges();
        }

        public async Task<Address> CreateAddress(string street,string country,string description, string city, string postcode)
        {
            if (string.IsNullOrWhiteSpace(street) 
                || string.IsNullOrWhiteSpace(city) 
                || string.IsNullOrWhiteSpace(postcode)
                || string.IsNullOrWhiteSpace(country))
            {
                return null;
            }

            var cityDb = await this.GetOrCreateCity(city,postcode);

            Address address = new Address
            {
                City = cityDb,
                Country = country,
                Street = street,
                Description = description
            };

            await this.dbContext.Addresses.AddAsync(address);
            await this.dbContext.SaveChangesAsync();

            return address;
        }

        public IEnumerable<Address> GetAllUserAddress(string username)
        {
            List<Address> addressDb = this.dbContext.Addresses 
                                .Include(address => address.City)
                                .Where(address => address.ShopUser.UserName == username) // Check for N + 1 problem 
                                .ToList();

            return addressDb;
        }


        private async Task<City> GetOrCreateCity(string name,string postcode)
        {
            City cityDb = this.dbContext.Cities.FirstOrDefault(city => city.Name == name);

            if(cityDb == null)
            {
                cityDb = new City
                {
                    Name = name,
                    Postcode = postcode
                };

                await this.dbContext.Cities.AddAsync(cityDb);
                await this.dbContext.SaveChangesAsync();
                
            }

            return cityDb;
        }
    }
}
