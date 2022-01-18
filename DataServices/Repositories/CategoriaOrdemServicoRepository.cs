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
    public class CategoriaOrdemServicoRepository : RepositoryBase<CATEGORIA_ORDEM_SERVICO>, ICategoriaOrdemServicoRepository
    {
        public CATEGORIA_ORDEM_SERVICO CheckExist(CATEGORIA_ORDEM_SERVICO item)
        {
            Int32 idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_ORDEM_SERVICO> query = Db.CATEGORIA_ORDEM_SERVICO;
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            query = query.Where(x => x.CAOS_NM_NOME == item.CAOS_NM_NOME);
            return query.FirstOrDefault();
        }

        public CATEGORIA_ORDEM_SERVICO GetItemById(Int32 id)
        {
            Int32 idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_ORDEM_SERVICO> query = Db.CATEGORIA_ORDEM_SERVICO;
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            query = query.Where(x => x.CAOS_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_ORDEM_SERVICO> GetAllItens()
        {
            Int32 idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_ORDEM_SERVICO> query = Db.CATEGORIA_ORDEM_SERVICO.Where(x => x.CAOS_IN_ATIVO == 1);
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            return query.ToList<CATEGORIA_ORDEM_SERVICO>();
        }

        public List<CATEGORIA_ORDEM_SERVICO> GetAllItensAdm()
        {
            Int32 idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_ORDEM_SERVICO> query = Db.CATEGORIA_ORDEM_SERVICO;
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            return query.ToList<CATEGORIA_ORDEM_SERVICO>();
        }
    }
}
