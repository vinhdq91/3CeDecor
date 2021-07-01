using DAL.Models.Application;
using DAL.Repositories.CustomerRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.CustomerService
{
    public class CustomerService : ICustomerService
    {
        ICustomerRepository _customerRepos;
        public CustomerService(ICustomerRepository customerRepos)
        {
            _customerRepos = customerRepos;
        }
        public async Task AddCustomerAsync(CustomerEntity customerEntity)
        {
            await _customerRepos.AddAsync(customerEntity);
        }

        public async Task<CustomerEntity> GetCustomerAsync(int id)
        {
            CustomerEntity customerEntity = await _customerRepos.GetSingleOrDefaultAsync(x => x.Id == id);
            return customerEntity;
        }

        public async Task<IQueryable<CustomerEntity>> GetListAllCustomersAsync()
        {
            IQueryable<CustomerEntity> customerEntities = await _customerRepos.GetAllAsync();
            return customerEntities;
        }

        public async Task RemoveAsync(CustomerEntity customerEntity)
        {
            await _customerRepos.RemoveAsync(customerEntity);
        }

        public async Task UpdateCustomerAsync(CustomerEntity customerEntity)
        {
            await _customerRepos.UpdateAsync(customerEntity);
        }
    }
}
