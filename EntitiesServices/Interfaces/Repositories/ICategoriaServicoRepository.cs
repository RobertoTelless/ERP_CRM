using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaServicoRepository : IRepositoryBase<CATEGORIA_SERVICO>
    {
        CATEGORIA_SERVICO CheckExist(CATEGORIA_SERVICO item, Int32 idAss);
        List<CATEGORIA_SERVICO> GetAllItens(Int32 idAss);
        CATEGORIA_SERVICO GetItemById(Int32 id);
        List<CATEGORIA_SERVICO> GetAllItensAdm(Int32 idAss);
    }
}
