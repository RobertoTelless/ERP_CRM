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
    public class DocumentoRepository : RepositoryBase<DOCUMENTO>, IDocumentoRepository
    {
        public DOCUMENTO GetItemById(Int32 id)
        {
            IQueryable<DOCUMENTO> query = Db.DOCUMENTO;
            query = query.Where(p => p.DOCU_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<DOCUMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<DOCUMENTO> query = Db.DOCUMENTO.Where(p => p.DOCU_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.DOCU_NM_NOME);
            return query.ToList();
        }

        public List<DOCUMENTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<DOCUMENTO> query = Db.DOCUMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.DOCU_NM_NOME);
            return query.ToList();
        }

        public List<DOCUMENTO> ExecuteFilter(Int32? grupo, Int32? classe, Int32? tipo, Int32? cliente, Int32? forn, String nome, String descricao, String numero, DateTime? data, Int32 idAss)
        {
            List<DOCUMENTO> lista = new List<DOCUMENTO>();
            IQueryable<DOCUMENTO> query = Db.DOCUMENTO;
            if (grupo != null)
            {
                query = query.Where(p => p.GRDC_CD_ID == grupo);
            }
            if (classe != null)
            {
                query = query.Where(p => p.CLAS_CD_ID == classe);
            }
            if (tipo != null)
            {
                query = query.Where(p => p.TIDO_CD_ID == tipo);
            }
            if (cliente != null)
            {
                query = query.Where(p => p.CLIE_CD_ID == cliente);
            }
            if (forn != null)
            {
                query = query.Where(p => p.FORN_CD_ID == forn);
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.DOCU_DS_DESCRICAO.Contains(descricao));
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.DOCU_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.DOCU_NM_NOME == numero);
            }
            if (data != null)
            {
                query = query.Where(p => p.DOCU_DT_DATA == data);
            }

            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.DOCU_NM_NOME);
                lista = query.ToList<DOCUMENTO>();
            }
            return lista;
        }
    }
}
 