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
    public class UnidadeRepository : RepositoryBase<UNIDADE>, IUnidadeRepository
    {
        public UNIDADE CheckExist(UNIDADE conta, Int32 idAss)
        {
            IQueryable<UNIDADE> query = Db.UNIDADE;
            query = query.Where(p => p.UNID_NM_NOME == conta.UNID_NM_NOME);
            query = query.Where(p => p.UNID_IN_TIPO_UNIDADE == conta.UNID_IN_TIPO_UNIDADE);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public UNIDADE GetItemById(Int32 id)
        {
            IQueryable<UNIDADE> query = Db.UNIDADE;
            query = query.Where(p => p.UNID_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<UNIDADE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<UNIDADE> query = Db.UNIDADE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<UNIDADE> GetAllItens(Int32 idAss)
        {
            IQueryable<UNIDADE> query = Db.UNIDADE.Where(p => p.UNID_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 