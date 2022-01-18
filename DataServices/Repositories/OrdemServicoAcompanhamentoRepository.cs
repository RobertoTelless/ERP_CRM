using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public class OrdemServicoAcompanhamentoRepository : RepositoryBase<ORDEM_SERVICO_ACOMPANHAMENTO>, IOrdemServicoAcompanhamentoRepository
    {
        public List<ORDEM_SERVICO_ACOMPANHAMENTO> GetByOs(ORDEM_SERVICO conta)
        {
            IQueryable<ORDEM_SERVICO_ACOMPANHAMENTO> query = Db.ORDEM_SERVICO_ACOMPANHAMENTO;
            query = query.Where(x => x.ORSE_CD_ID == conta.ORSE_CD_ID);
            return query.ToList<ORDEM_SERVICO_ACOMPANHAMENTO>();
        }
    }
}
