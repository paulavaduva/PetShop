using PetShop.Models;
using PetShop.Repositories.Interfaces;
using PetShop.Services.Interfaces;

namespace PetShop.Services
{
    public class AddressService : IAddressService
    {
        private readonly IRepositoryWrapper _repository;
        public AddressService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public Address? GetAddressById(int id)
        {
            return _repository.AddressRepository.FindByCondition(a => a.Id == id).FirstOrDefault();
        }

        public void AddAddress(Address address)
        {
            _repository.AddressRepository.Create(address);
            _repository.Save();
        }

        public void UpdateAddress(Address address)
        {
            _repository.AddressRepository.Update(address);
            _repository.Save();
        }

        public void DeleteAddress(int id)
        {
            var address = GetAddressById(id);
            if (address != null)
            {
                _repository.AddressRepository.Delete(address);
                _repository.Save();
            }
        }

        //public Address? GetAddressByOrderId(int orderId)
        //{
        //    return _repository.AddressRepository.FindByCondition(a => a.)
        //}
    }
}
