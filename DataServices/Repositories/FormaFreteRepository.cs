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
using CrossCutting;

namespace DataServices.Repositories
{
    public class FormaFreteRepository : RepositoryBase<FORMA_FRETE>, IFormaFreteRepository
    {
        public List<FORMA_FRETE> GetAllItens(Int32 idAss)
        {
            IQueryable<FORMA_FRETE> query = Db.FORMA_FRETE.Where(x => x.FOFR_IN_ATIVO == 1);
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            return query.ToList<FORMA_FRETE>();
        }
    }
}
