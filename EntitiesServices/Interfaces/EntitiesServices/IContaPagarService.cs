using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContaPagarService : IServiceBase<CONTA_PAGAR>
    {
        Int32 Create(CONTA_PAGAR item, LOG log);
        Int32 Create(CONTA_PAGAR item);
        Int32 Edit(CONTA_PAGAR item, LOG log);
        Int32 Edit(CONTA_PAGAR item);
        Int32 Delete(CONTA_PAGAR item, LOG log);

        CONTA_PAGAR GetItemById(Int32 id);
        CONTA_PAGAR_ANEXO GetAnexoById(Int32 id);
        List<CONTA_PAGAR> GetItensAtraso(Int32 idAss);
        List<CONTA_PAGAR> GetAllItens(Int32 idAss);
        List<CONTA_PAGAR> GetAllItensAdm(Int32 idAss);
        Decimal GetTotalPagoMes(DateTime mes, Int32 idAss);
        Decimal GetTotalAPagarMes(DateTime mes, Int32 idAss);
        List<CONTA_PAGAR> GetPagamentosMes(DateTime mes, Int32 idAss);
        List<CONTA_PAGAR> GetAPagarMes(DateTime mes, Int32 idAss);
        List<CONTA_PAGAR> GetItensAtrasoFornecedor(Int32 idAss);

        CONTA_PAGAR_PARCELA GetParcelaById(Int32 id);
        CONFIGURACAO CarregaConfiguracao(Int32 assinante);
        USUARIO GetResponsavelById(Int32 id);
        USUARIO GetResponsavelByUser(Int32 id);
        TEMPLATE GetTemplateBySigla(String sigla);
        List<CONTA_PAGAR> ExecuteFilter(Int32? forId, Int32? ccId, DateTime? data, String descricao, Int32? aberto, DateTime? vencimento, DateTime? vencFinal, DateTime? quitacao, Int32? atraso, Int32? conta, Int32 idAss);
        List<CONTA_PAGAR> ExecuteFilterAtraso(String nome, DateTime? vencimento, Int32 idAss);

    }
}
