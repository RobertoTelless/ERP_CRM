using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAtendimentoRepository : IRepositoryBase<ATENDIMENTO>
    {
        ATENDIMENTO CheckExist(ATENDIMENTO item, Int32 idAss);
        ATENDIMENTO GetItemById(Int32 id);
        List<ATENDIMENTO> GetAllItens(Int32 idAss);
        List<ATENDIMENTO> GetByCliente(Int32 id, Int32 idAss);
        List<ATENDIMENTO> GetAllItensAdm(Int32 idAss);
        List<ATENDIMENTO> ExecuteFilter(Int32? idCat, Int32? cliente, Int32? produto, DateTime? data, Int32? status, String descricao, Int32? depto, Int32? prioridade, Int32? idUsua, Int32? idServico, Int32? sla, Int32 idAss);
    }
}
