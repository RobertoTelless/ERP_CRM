using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorRepository : IRepositoryBase<PRESTADOR>
    {
        PRESTADOR CheckExist(PRESTADOR item, Int32 idUsu, Int32 idAss);
        PRESTADOR GetItemById(Int32 id);
        List<PRESTADOR> GetAllItens(Int32 idAss);
        List<PRESTADOR> GetAllItensAdm(Int32 idAss);
        List<PRESTADOR> ExecuteFilter(String nome, String razao, String cnpj, String email, String cidade, Int32? uf, Int32 idAss);
    }
}
