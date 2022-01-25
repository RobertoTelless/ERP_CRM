using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITransportadoraRepository : IRepositoryBase<TRANSPORTADORA>
    {
        TRANSPORTADORA CheckExist(TRANSPORTADORA item, Int32 idAss);
        TRANSPORTADORA GetByEmail(String email, Int32 idAss);
        TRANSPORTADORA GetItemById(Int32 id);
        List<TRANSPORTADORA> GetAllItens(Int32 idAss);
        List<TRANSPORTADORA> GetAllItensAdm(Int32 idAss);
        List<TRANSPORTADORA> ExecuteFilter(Int32? veic, Int32? tran, String nome, String cnpj, String email, String cidade, String uf, Int32 idAss);
    }
}
