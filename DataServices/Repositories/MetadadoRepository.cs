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
    public class MetadadoRepository : RepositoryBase<METADADO>, IMetadadoRepository
    {
        public METADADO CheckExist(METADADO conta, Int32 idAss)
        {
            IQueryable<METADADO> query = Db.METADADO;
            query = query.Where(p => p.CLASSE.CLAS_SG_SIGLA == conta.CLASSE.CLAS_SG_SIGLA);
            query = query.Where(p => p.META_SG_SIGLA == conta.META_SG_SIGLA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public METADADO GetBySigla(String sigla)
        {
            IQueryable<METADADO> query = Db.METADADO.Where(p => p.META_IN_ATIVO == 1);
            query = query.Where(p => p.META_SG_SIGLA == sigla);
            return query.FirstOrDefault();
        }

        public METADADO GetItemById(Int32 id)
        {
            IQueryable<METADADO> query = Db.METADADO;
            query = query.Where(p => p.META_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<METADADO> GetAllItens(Int32 idAss)
        {
            IQueryable<METADADO> query = Db.METADADO.Where(p => p.META_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.META_SG_SIGLA);
            return query.ToList();
        }

        public List<METADADO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<METADADO> query = Db.METADADO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.META_SG_SIGLA);
            return query.ToList();
        }

        public List<METADADO> ExecuteFilter(Int32? tipo, String nome, String descricao, String sigla, Int32 idAss)
        {
            List<METADADO> lista = new List<METADADO>();
            IQueryable<METADADO> query = Db.METADADO;
            if (tipo != null)
            {
                query = query.Where(p => p.TIME_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.META_DS_DESCRICAO.Contains(descricao));
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.META_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(sigla))
            {
                query = query.Where(p => p.META_SG_SIGLA == sigla);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.META_SG_SIGLA);
                lista = query.ToList<METADADO>();
            }
            return lista;
        }
    }
}
 