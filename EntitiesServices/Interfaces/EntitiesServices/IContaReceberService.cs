using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContaReceberService : IServiceBase<CONTA_RECEBER>
    {
        Int32 Create(CONTA_RECEBER item, LOG log);
        Int32 Create(CONTA_RECEBER item);
        Int32 Edit(CONTA_RECEBER item, LOG log);
        Int32 Edit(CONTA_RECEBER item);
        Int32 Delete(CONTA_RECEBER item, LOG log);

        CONTA_RECEBER GetItemById(Int32 id);
        List<CONTA_RECEBER> GetAllItens(Int32 idAss);
        List<CONTA_RECEBER> GetAllItensAdm(Int32 idAss);
        List<CONTA_RECEBER> GetVencimentoAtual(Int32 idAss);
        Decimal GetTotalRecebimentosMes(DateTime mes, Int32 idAss);
        Decimal GetTotalAReceberMes(DateTime mes, Int32 idAss);
        List<CONTA_RECEBER> GetRecebimentosMes(DateTime mes, Int32 idAss);
        List<CONTA_RECEBER> GetAReceberMes(DateTime mes, Int32 idAss);
        List<CONTA_RECEBER> GetItensAtrasoCliente(Int32 idAss);

        //List<TIPO_TAG> GetAllTags();
        CONTA_RECEBER_ANEXO GetAnexoById(Int32 id);
        CONTA_RECEBER_PARCELA GetParcelaById(Int32 id);
        CONFIGURACAO CarregaConfiguracao(Int32 assinante);
        USUARIO GetResponsavelById(Int32 id);
        USUARIO GetResponsavelByUser(Int32 id);
        TEMPLATE GetTemplateBySigla(String sigla);

        List<CONTA_RECEBER> ExecuteFilter(Int32? cliId, Int32? ccId, DateTime? dtLanc, DateTime? data, DateTime? dataFinal, String descricao, Int32? aberto, Int32? conta, Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilterRecebimentoMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, DateTime? liqui, Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilterAReceberMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilterCRAtrasos(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilterAtrasos(String nome, String cidade, Int32? uf, Int32 idAss);
    }
}
