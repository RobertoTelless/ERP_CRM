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
    public class OrdemServicoComentarioRepository : RepositoryBase<ORDEM_SERVICO_COMENTARIOS>, IOrdemServicoComentarioRepository
    {
        public List<ORDEM_SERVICO_COMENTARIOS> GetByOs(ORDEM_SERVICO item)
        {
            IQueryable<ORDEM_SERVICO_COMENTARIOS> query = Db.ORDEM_SERVICO_COMENTARIOS;
            query = query.Where(x => x.ORSE_CD_ID == item.ORSE_CD_ID);
            return query.ToList<ORDEM_SERVICO_COMENTARIOS>();
        }
    }
}
