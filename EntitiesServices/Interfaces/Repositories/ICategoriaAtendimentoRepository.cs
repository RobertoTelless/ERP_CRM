using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaAtendimentoRepository : IRepositoryBase<CATEGORIA_ATENDIMENTO>
    {
        List<CATEGORIA_ATENDIMENTO> GetAllItens();
        CATEGORIA_ATENDIMENTO GetItemById(Int32 id);
        List<CATEGORIA_ATENDIMENTO> GetAllItensAdm();
    }
}
