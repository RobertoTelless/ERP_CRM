using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.DTO;

namespace ApplicationServices.Interfaces
{
    public interface IContaReceberAppService : IAppServiceBase<CONTA_RECEBER>
    {
        Int32 ValidateCreate(CONTA_RECEBER item, Int32 recorrente, DateTime? data, USUARIO usuario);
        Int32 ValidateEdit(CONTA_RECEBER item, CONTA_RECEBER itemAntes, USUARIO usuario, DTO_CR dto);
        Int32 ValidateEditReceber(CONTA_RECEBER item, CONTA_RECEBER itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CONTA_RECEBER item, USUARIO usuario);
        Int32 ValidateReativar(CONTA_RECEBER item, USUARIO usuario);
        Int32 ValidateEditSimples(CONTA_RECEBER item);

        CONTA_RECEBER GetItemById(Int32 id);
        List<CONTA_RECEBER> GetAllItens(Int32 idAss);
        List<CONTA_RECEBER> GetAllItensAdm(Int32 idAss);
        List<CONTA_RECEBER> GetVencimentoAtual(Int32 idAss);
        List<CONTA_RECEBER> GetItensAtrasoCliente(Int32 idAss);
        Decimal GetTotalRecebimentosMes(DateTime mes, Int32 idAss);
        Decimal GetTotalAReceberMes(DateTime mes, Int32 idAss);
        List<CONTA_RECEBER> GetRecebimentosMes(DateTime mes, Int32 idAss);
        List<CONTA_RECEBER> GetAReceberMes(DateTime mes, Int32 idAss);

        CONTA_RECEBER_ANEXO GetAnexoById(Int32 id);
        CONTA_RECEBER_PARCELA GetParcelaById(Int32 id);

        Int32 ExecuteFilter(Int32? cliId, Int32? ccId, DateTime? dtLanc, DateTime? data, DateTime? dataFinal, String descricao, Int32? aberto, Int32? conta, Int32 idAss, out List<CONTA_RECEBER> objeto);
        Int32 ExecuteFilterRecebimentoMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, DateTime? liqui, Int32 idAss, out List<CONTA_RECEBER> objeto);
        Int32 ExecuteFilterAReceberMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss, out List<CONTA_RECEBER> objeto);
        Int32 ExecuteFilterCRAtrasos(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss, out List<CONTA_RECEBER> objeto);
        Int32 ExecuteFilterAtrasos(String nome, String cidade, Int32? uf, Int32 idAss, out List<CONTA_RECEBER> objeto);
        Int32 IncluirRateioCC(CONTA_RECEBER item, Int32? cc, Int32? perc, USUARIO usuario);
    }
}
