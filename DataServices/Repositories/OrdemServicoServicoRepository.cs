using EntitiesServices.Interfaces.Repositories;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public class OrdemServicoServicoRepository : RepositoryBase<ORDEM_SERVICO_SERVICO>, IOrdemServicoServicoRepository
    {
        public ORDEM_SERVICO_SERVICO CheckExist(ORDEM_SERVICO_SERVICO item)
        {
            IQueryable<ORDEM_SERVICO_SERVICO> query = Db.ORDEM_SERVICO_SERVICO;
            query = query.Where(x => x.ORSE_CD_ID == item.ORSE_CD_ID);
            query = query.Where(x => x.SERV_CD_ID == item.SERV_CD_ID);
            return query.FirstOrDefault();
        }
        
        public List<ORDEM_SERVICO_SERVICO> GetAllbyOs(Int32 id)
        {
            IQueryable<ORDEM_SERVICO_SERVICO> query = Db.ORDEM_SERVICO_SERVICO.Where(x => x.OSSE_IN_ATIVO == 1);
            query = query.Where(x => x.ORSE_CD_ID == id);
            return query.ToList();
        }
    }
}
