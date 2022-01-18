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
    public class ServicoRepository : RepositoryBase<SERVICO>, IServicoRepository
    {
        public SERVICO CheckExist(SERVICO conta, Int32 idAss)
        {
            IQueryable<SERVICO> query = Db.SERVICO;
            query = query.Where(p => p.SERV_NM_NOME == conta.SERV_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public SERVICO GetItemById(Int32 id)
        {
            IQueryable<SERVICO> query = Db.SERVICO;
            query = query.Where(p => p.SERV_CD_ID == id);
            query = query.Include(p => p.SERVICO_ANEXO);
            return query.FirstOrDefault();
        }

        public List<SERVICO> GetAllItens(Int32 idAss)
        {
            IQueryable<SERVICO> query = Db.SERVICO.Where(p => p.SERV_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<SERVICO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<SERVICO> query = Db.SERVICO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<SERVICO> ExecuteFilter(Int32? catId, String nome, String descricao, String referencia, Int32 idAss)
        {
            List<SERVICO> lista = new List<SERVICO>();
            IQueryable<SERVICO> query = Db.SERVICO;
            if (catId != null)
            {
                query = query.Where(p => p.CATEGORIA_SERVICO.CASE_CD_ID == catId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.SERV_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.SERV_DS_DESCRICAO.Contains(descricao));
            }
            if (!String.IsNullOrEmpty(referencia))
            {
                query = query.Where(p => p.SERV_TX_OBSERVACOES.Contains(referencia));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.SERV_NM_NOME);
                lista = query.ToList<SERVICO>();
            }
            return lista;
        }
    }
}
 