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
        CATEGORIA_ATENDIMENTO CheckExist(CATEGORIA_ATENDIMENTO item, Int32 idAss);
        List<CATEGORIA_ATENDIMENTO> GetAllItens(Int32 idAss);
        CATEGORIA_ATENDIMENTO GetItemById(Int32 id);
        List<CATEGORIA_ATENDIMENTO> GetAllItensAdm(Int32 idAss);
    }
}
