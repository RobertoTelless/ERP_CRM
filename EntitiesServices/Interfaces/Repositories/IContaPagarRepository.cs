using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaPagarRepository : IRepositoryBase<CONTA_PAGAR>
    {
        CONTA_PAGAR GetItemById(Int32 id);
        List<CONTA_PAGAR> GetItensAtraso(Int32 idAss);
        List<CONTA_PAGAR> GetAllItens(Int32 idAss);
        List<CONTA_PAGAR> GetAllItensAdm(Int32 idAss);
        Decimal GetTotalPagoMes(DateTime mes, Int32 idAss);
        Decimal GetTotalAPagarMes(DateTime mes, Int32 idAss);
        List<CONTA_PAGAR> ExecuteFilter(Int32? forId, Int32? ccId, DateTime? data, String descricao, Int32? aberto, DateTime? vencimento, DateTime? vencFinal, DateTime? quitacao, Int32? atraso, Int32? conta, Int32 idAss);
        List<CONTA_PAGAR> ExecuteFilterAtraso(String nome, DateTime? vencimento, Int32 idAss);
        List<CONTA_PAGAR> GetPagamentosMes(DateTime mes, Int32 idAss);
        List<CONTA_PAGAR> GetAPagarMes(DateTime mes, Int32 idAss);
        List<CONTA_PAGAR> GetItensAtrasoFornecedor(Int32 idAss);

    }
}
