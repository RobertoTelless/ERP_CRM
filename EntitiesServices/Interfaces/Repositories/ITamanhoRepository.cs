using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITamanhoRepository : IRepositoryBase<TAMANHO>
    {
        TAMANHO CheckExist(TAMANHO item, Int32 idAss);
        List<TAMANHO> GetAllItens(Int32 idAss);
        TAMANHO GetItemById(Int32 id);
        List<TAMANHO> GetAllItensAdm(Int32 idAss);
    }
}
