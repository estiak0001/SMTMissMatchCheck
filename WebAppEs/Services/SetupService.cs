using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.Data;

namespace WebAppEs.Services
{
    public class SetupService : ISetupService
    {
        private readonly ApplicationDbContext _context;

        public SetupService(ApplicationDbContext context)
        {
            _context = context;
        }

    }
}
