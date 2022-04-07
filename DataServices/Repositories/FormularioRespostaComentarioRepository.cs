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
    public class FormularioRespostaComentarioRepository : RepositoryBase<FORMULARIO_RESPOSTA_COMENTARIO>, IFormularioRespostaComentarioRepository
    {
        public List<FORMULARIO_RESPOSTA_COMENTARIO> GetAllItens()
        {
            return Db.FORMULARIO_RESPOSTA_COMENTARIO.ToList();
        }

        public FORMULARIO_RESPOSTA_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<FORMULARIO_RESPOSTA_COMENTARIO> query = Db.FORMULARIO_RESPOSTA_COMENTARIO.Where(p => p.FRCO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 