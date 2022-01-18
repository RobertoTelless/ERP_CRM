using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IServicoTabelaPrecoAppService : IAppServiceBase<SERVICO_TABELA_PRECO>
    {
        Int32 ValidateCreateLista(List<SERVICO_TABELA_PRECO> lista, Int32 idAss);
        Int32 ValidateCreate(SERVICO_TABELA_PRECO item, Int32 idAss);
        Int32 ValidateEdit(SERVICO_TABELA_PRECO item, Int32 id);
        Int32 ValidateDelete(SERVICO_TABELA_PRECO item, Int32 id);
        Int32 ValidateReativar(SERVICO_TABELA_PRECO item, Int32 id);

        SERVICO_TABELA_PRECO GetByServFilial(Int32 id, Int32 fili, Int32 idAss);
    }
}
