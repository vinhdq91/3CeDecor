using DAL.EntityFrameworkCore.Application;
using Microsoft.AspNetCore.Http;
using DAL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class HttpUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public HttpUnitOfWork(ApplicationDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            context.CurrentUserId = accessor.HttpContext?.User.FindFirst(ClaimConstants.Subject)?.Value?.Trim();
        }
    }
}
