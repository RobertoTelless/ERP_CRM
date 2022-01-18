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
        List<CATEGORIA_SERVICO> GetAllItens();
        CATEGORIA_SERVICO GetItemById(Int32 id);
        List<CATEGORIA_SERVICO> GetAllItensAdm();
    }
}
