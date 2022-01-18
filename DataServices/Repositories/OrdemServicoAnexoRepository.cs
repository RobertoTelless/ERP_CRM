using EntitiesServices.Interfaces.Repositories;
using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public class OrdemServicoAnexoRepository : RepositoryBase<ORDEM_SERVICO_ANEXO>, IOrdemServicoAnexoRepository
    {
        public List<ORDEM_SERVICO_ANEXO> GetAllItens()
        {
            return Db.ORDEM_SERVICO_ANEXO.ToList();
        }

        public ORDEM_SERVICO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<ORDEM_SERVICO_ANEXO> query = Db.ORDEM_SERVICO_ANEXO.Where(p => p.ORSX_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
