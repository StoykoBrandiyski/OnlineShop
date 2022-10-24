using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data.Contracts
{
    public interface IAddresService
    {
        Task<Address> CreateAddress(string street,string country, string description, string city, string postcode);

        int AddAddressToUser(string username, Address address);

        IEnumerable<Address> GetAllUserAddress(string username);
    }
}
