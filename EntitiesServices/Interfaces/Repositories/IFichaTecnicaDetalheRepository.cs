using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFichaTecnicaDetalheRepository : IRepositoryBase<FICHA_TECNICA_DETALHE>
    {
        List<FICHA_TECNICA_DETALHE> GetAllItens();
        FICHA_TECNICA_DETALHE GetItemById(Int32 id);
    }
}
