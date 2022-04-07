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
    public class FormularioRespostaAnexoRepository : RepositoryBase<FORMULARIO_RESPOSTA_ANEXO>, IFormularioRespostaAnexoRepository
    {
        public List<FORMULARIO_RESPOSTA_ANEXO> GetAllItens()
        {
            return Db.FORMULARIO_RESPOSTA_ANEXO.ToList();
        }

        public FORMULARIO_RESPOSTA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<FORMULARIO_RESPOSTA_ANEXO> query = Db.FORMULARIO_RESPOSTA_ANEXO.Where(p => p.FRAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 