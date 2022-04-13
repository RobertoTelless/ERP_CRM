using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormularioRespostaRepository : IRepositoryBase<FORMULARIO_RESPOSTA>
    {
        List<FORMULARIO_RESPOSTA> GetAllItens();
        List<FORMULARIO_RESPOSTA> GetAllItensTodos();
        FORMULARIO_RESPOSTA GetItemById(Int32 id);
        List<FORMULARIO_RESPOSTA> ExecuteFilter(Int32? status, String nome, String email, String celular, String cidade, Int32? uf);
    }
}
