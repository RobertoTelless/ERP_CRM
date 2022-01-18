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
        SERVICO CheckExist(SERVICO item);
        SERVICO GetItemById(Int32 id);
        List<SERVICO> GetAllItens();
        List<SERVICO> GetAllItensAdm();
        List<SERVICO> ExecuteFilter(Int32? catId, String nome, String descricao, String referencia);
    }
}
