using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IServicoTabelaPrecoService : IServiceBase<SERVICO_TABELA_PRECO>
    {
        Int32 Create(SERVICO_TABELA_PRECO item, Int32 idAss);
        Int32 Edit(SERVICO_TABELA_PRECO item, Int32 id);

        SERVICO_TABELA_PRECO GetByServFilial(Int32 id, Int32 fili, Int32 idAss);
    }
}
