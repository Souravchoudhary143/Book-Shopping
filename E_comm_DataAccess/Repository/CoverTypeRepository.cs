using E_comm_DataAccess.Data;
using E_comm_DataAccess.Repository.IRepository;
using E_comm_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_comm_DataAccess.Repository
{

    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        public readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

