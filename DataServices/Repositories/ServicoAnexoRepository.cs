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
    public class ServicoAnexoRepository : RepositoryBase<SERVICO_ANEXO>, IServicoAnexoRepository
    {
        public List<SERVICO_ANEXO> GetAllItens()
        {
            return Db.SERVICO_ANEXO.ToList();
        }

        public SERVICO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<SERVICO_ANEXO> query = Db.SERVICO_ANEXO.Where(p => p.SEAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 