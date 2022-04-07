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
    public class FormularioRespostaAcaoRepository : RepositoryBase<FORMULARIO_RESPOSTA_ACAO>, IFormularioRespostaAcaoRepository
    {
        public List<FORMULARIO_RESPOSTA_ACAO> GetAllItens()
        {
            IQueryable<FORMULARIO_RESPOSTA_ACAO> query = Db.FORMULARIO_RESPOSTA_ACAO.Where(p => p.FRAC_IN_STATUS == 1);
            return query.ToList();
        }

        public FORMULARIO_RESPOSTA_ACAO GetItemById(Int32 id)
        {
            IQueryable<FORMULARIO_RESPOSTA_ACAO> query = Db.FORMULARIO_RESPOSTA_ACAO.Where(p => p.FRAC_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 