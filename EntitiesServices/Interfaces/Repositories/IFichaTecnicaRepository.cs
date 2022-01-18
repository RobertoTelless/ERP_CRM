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
        FICHA_TECNICA GetByNome(String nome);
        FICHA_TECNICA GetItemById(Int32 id);
        List<FICHA_TECNICA> GetAllItens();
        List<FICHA_TECNICA> GetAllItensAdm();
        List<FICHA_TECNICA> ExecuteFilter(Int32? prodId, Int32? cat, String descricao);
    }
}
