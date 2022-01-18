using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFichaTecnicaRepository : IRepositoryBase<FICHA_TECNICA>
    {
        FICHA_TECNICA CheckExist(FICHA_TECNICA item, Int32 idAss);
        FICHA_TECNICA GetByNome(String nome, Int32 idAss);
        FICHA_TECNICA GetItemById(Int32 id);
        List<FICHA_TECNICA> GetAllItens(Int32 idAss);
        List<FICHA_TECNICA> GetAllItensAdm(Int32 idAss);
        List<FICHA_TECNICA> ExecuteFilter(Int32? prodId, Int32? cat, String descricao, Int32 idAss);
    }
}
