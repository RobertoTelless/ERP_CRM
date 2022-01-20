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
    public class AtendimentoAnexoRepository : RepositoryBase<ATENDIMENTO_ANEXO>, IAtendimentoAnexoRepository
    {
        public List<ATENDIMENTO_ANEXO> GetAllItens()
        {
            return Db.ATENDIMENTO_ANEXO.ToList();
        }

        public ATENDIMENTO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<ATENDIMENTO_ANEXO> query = Db.ATENDIMENTO_ANEXO.Where(p => p.ATAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 