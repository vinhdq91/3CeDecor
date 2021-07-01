using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.CustomerRepository
{
    public class CustomerRepository : Repository<CustomerEntity>, ICustomerRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public CustomerRepository(ApplicationDbContext applicationDbContext): base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
    }
}
