using PetShop.Models;

namespace PetShop.Services.Interfaces
{
    public interface IAddressService
    {
        Address? GetAddressById(int id);
        void AddAddress(Address address);
        void DeleteAddress(int id);
        void UpdateAddress(Address address);
        //Address? GetAddressByOrderId(int orderId);
    }
}
