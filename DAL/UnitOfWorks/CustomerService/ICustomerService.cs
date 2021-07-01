using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.CustomerService
{
    public interface ICustomerService
    {
        Task<IQueryable<CustomerEntity>> GetListAllCustomersAsync();
        Task<CustomerEntity> GetCustomerAsync(int id);
        Task AddCustomerAsync(CustomerEntity customerEntity);
        Task RemoveAsync(CustomerEntity customerEntity);
        Task UpdateCustomerAsync(CustomerEntity customerEntity);
    }
}
