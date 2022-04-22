using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITemplatePropostaRepository : IRepositoryBase<TEMPLATE_PROPOSTA>
    {
        TEMPLATE_PROPOSTA GetByCode(String code, Int32 idAss);
        List<TEMPLATE_PROPOSTA> GetAllItens(Int32 idAss);
        TEMPLATE_PROPOSTA GetItemById(Int32 id);
        List<TEMPLATE_PROPOSTA> GetAllItensAdm(Int32 idAss);
        List<TEMPLATE_PROPOSTA> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss);
        TEMPLATE_PROPOSTA CheckExist(TEMPLATE_PROPOSTA item, Int32 idAss);
    }
}
