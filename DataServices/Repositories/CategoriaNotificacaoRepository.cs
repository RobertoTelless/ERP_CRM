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
    public class CategoriaNotificacaoRepository : RepositoryBase<CATEGORIA_NOTIFICACAO>, ICategoriaNotificacaoRepository
    {
        public CATEGORIA_NOTIFICACAO GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_NOTIFICACAO> query = Db.CATEGORIA_NOTIFICACAO;
            query = query.Where(p => p.CANO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllItensAdm()
        {
            IQueryable<CATEGORIA_NOTIFICACAO> query = Db.CATEGORIA_NOTIFICACAO;
            return query.ToList();
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllItens()
        {
            IQueryable<CATEGORIA_NOTIFICACAO> query = Db.CATEGORIA_NOTIFICACAO.Where(p => p.CANO_IN_ATIVO == 1);
            return query.ToList();
        }

    }
}
 