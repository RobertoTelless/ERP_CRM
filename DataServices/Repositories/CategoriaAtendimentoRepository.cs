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
    public class CategoriaAtendimentoRepository : RepositoryBase<CATEGORIA_ATENDIMENTO>, ICategoriaAtendimentoRepository
    {
        public CATEGORIA_ATENDIMENTO GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_ATENDIMENTO> query = Db.CATEGORIA_ATENDIMENTO;
            query = query.Where(p => p.CAAT_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_ATENDIMENTO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_ATENDIMENTO> query = Db.CATEGORIA_ATENDIMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CATEGORIA_ATENDIMENTO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_ATENDIMENTO> query = Db.CATEGORIA_ATENDIMENTO.Where(p => p.CAAT_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 