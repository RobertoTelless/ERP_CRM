using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface IClasseRepository : IRepositoryBase<CLASSE>
    {
        CLASSE CheckExist(CLASSE item, Int32 idAss);
        CLASSE GetBySigla(String sigla);
        CLASSE GetItemById(Int32 id);
        List<CLASSE> GetAllItens(Int32 idAss);
        List<CLASSE> GetAllItensAdm(Int32 idAss);
        List<CLASSE> ExecuteFilter(Int32? grupo, Int32? seg, String nome, String descricao, String sigla, Int32 idAss);
    }
}
