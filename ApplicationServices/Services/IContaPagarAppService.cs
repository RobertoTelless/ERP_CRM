using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IContaPagarAppService : IAppServiceBase<CONTA_PAGAR>
    {
        Int32 ValidateCreate(CONTA_PAGAR item, Int32 recorrencia, DateTime? data, USUARIO usuario);
        Int32 ValidateEdit(CONTA_PAGAR item, CONTA_PAGAR itemAntes, USUARIO usuario, Int32 liquida, Int32 eParcela);
        Int32 ValidateEditPagar(CONTA_PAGAR item, CONTA_PAGAR itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CONTA_PAGAR item, USUARIO usuario);
        Int32 ValidateReativar(CONTA_PAGAR item, USUARIO usuario);

        CONTA_PAGAR GetItemById(Int32 id);
        CONTA_PAGAR_ANEXO GetAnexoById(Int32 id);
        List<CONTA_PAGAR> GetItensAtraso(Int32 idAss);
        List<CONTA_PAGAR> GetAllItens(Int32 idAss);
        List<CONTA_PAGAR> GetAllItensAdm(Int32 idAss);
        CONTA_PAGAR_PARCELA GetParcelaById(Int32 id);
        List<CONTA_PAGAR> GetPagamentosMes(DateTime mes, Int32 idAss);
        List<CONTA_PAGAR> GetAPagarMes(DateTime mes, Int32 idAss);
        List<CONTA_PAGAR> GetItensAtrasoFornecedor(Int32 idAss);

        Decimal GetTotalPagoMes(DateTime mes, Int32 idAss);
        Decimal GetTotalAPagarMes(DateTime mes, Int32 idAss);
        Int32 ExecuteFilter(Int32? forId, Int32? ccId, DateTime? data, String descricao, Int32? aberto, DateTime? vencimento, DateTime? vencFinal, DateTime? quitacao, Int32? atraso, Int32? conta, Int32 idAss, out List<CONTA_PAGAR> objeto);
        Int32 ExecuteFilterAtraso(String nome, DateTime? vencimento, Int32 idAss, out List<CONTA_PAGAR> objeto);
        Int32 IncluirRateioCC(CONTA_PAGAR item, Int32? cc, Int32? perc, USUARIO usuario);

    }
}
