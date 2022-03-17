using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaOrdemServicoRepository : IRepositoryBase<CATEGORIA_ORDEM_SERVICO>
    {
        CATEGORIA_ORDEM_SERVICO CheckExist(CATEGORIA_ORDEM_SERVICO item, Int32 idAss);
        CATEGORIA_ORDEM_SERVICO GetItemById(Int32 id);
        List<CATEGORIA_ORDEM_SERVICO> GetAllItens(Int32 idAss);
        List<CATEGORIA_ORDEM_SERVICO> GetAllItensAdm(Int32 idAss);

    }
}
