using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.CustomerRepository
{
    public interface ICustomerRepository : IRepository<CustomerEntity>
    {
    }
}
