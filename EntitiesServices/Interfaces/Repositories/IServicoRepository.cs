using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IServicoRepository : IRepositoryBase<SERVICO>
    {
        SERVICO CheckExist(SERVICO item, Int32 idAss);
        SERVICO GetItemById(Int32 id);
        List<SERVICO> GetAllItens(Int32 idAss);
        List<SERVICO> GetAllItensAdm(Int32 idAss);
        List<SERVICO> ExecuteFilter(Int32? catId, String nome, String descricao, String referencia, Int32 idAss);
    }
}
