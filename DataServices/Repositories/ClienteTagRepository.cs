using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ClienteTagRepository : RepositoryBase<CLIENTE_TAG>, IClienteTagRepository
    {
        public List<CLIENTE_TAG> GetAllItens()
        {
            return Db.CLIENTE_TAG.ToList();
        }

        public CLIENTE_TAG GetItemById(Int32 id)
        {
            IQueryable<CLIENTE_TAG> query = Db.CLIENTE_TAG.Where(p => p.CLTA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 