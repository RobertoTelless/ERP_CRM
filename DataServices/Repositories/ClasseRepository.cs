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
    public class ClasseRepository : RepositoryBase<CLASSE>, IClasseRepository
    {
        public CLASSE CheckExist(CLASSE conta, Int32 idAss)
        {
            IQueryable<CLASSE> query = Db.CLASSE;
            query = query.Where(p => p.CLAS_SG_SIGLA == conta.CLAS_SG_SIGLA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CLASSE GetBySigla(String sigla)
        {
            IQueryable<CLASSE> query = Db.CLASSE.Where(p => p.CLAS_IN_ATIVO == 1);
            query = query.Where(p => p.CLAS_SG_SIGLA == sigla);
            return query.FirstOrDefault();
        }

        public CLASSE GetItemById(Int32 id)
        {
            IQueryable<CLASSE> query = Db.CLASSE;
            query = query.Where(p => p.CLAS_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CLASSE> GetAllItens(Int32 idAss)
        {
            IQueryable<CLASSE> query = Db.CLASSE.Where(p => p.CLAS_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CLAS_SG_SIGLA);
            return query.ToList();
        }

        public List<CLASSE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CLASSE> query = Db.CLASSE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CLAS_SG_SIGLA);
            return query.ToList();
        }

        public List<CLASSE> ExecuteFilter(Int32? grupo, Int32? seg, String nome, String descricao, String sigla, Int32 idAss)
        {
            List<CLASSE> lista = new List<CLASSE>();
            IQueryable<CLASSE> query = Db.CLASSE;
            if (grupo != null)
            {
                query = query.Where(p => p.GRDC_CD_ID == grupo);
            }
            if (seg != null)
            {
                query = query.Where(p => p.NISE_CD_ID == seg);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.CLAS_DS_DESCRICAO.Contains(descricao));
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CLAS_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(sigla))
            {
                query = query.Where(p => p.CLAS_SG_SIGLA == sigla);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CLAS_SG_SIGLA);
                lista = query.ToList<CLASSE>();
            }
            return lista;
        }
    }
}
 