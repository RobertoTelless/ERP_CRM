using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IServicoTabelaPrecoRepository : IRepositoryBase<SERVICO_TABELA_PRECO>
    {
        SERVICO_TABELA_PRECO CheckExist(Int32 fili, Int32 servico, Int32 idAss);
        SERVICO_TABELA_PRECO GetByServFilial(Int32 id, Int32 fili, Int32 idAss);
    }
}
