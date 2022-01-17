using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaReceberRepository : IRepositoryBase<CONTA_RECEBER>
    {
        CONTA_RECEBER GetItemById(Int32 id);
        List<CONTA_RECEBER> GetItensAtrasoCliente(Int32 idAss);
        Decimal GetTotalRecebimentosMes(DateTime mes, Int32 idAss);
        Decimal GetTotalAReceberMes(DateTime mes, Int32 idAss);
        List<CONTA_RECEBER> GetAllItens(Int32 idAss);
        List<CONTA_RECEBER> GetAllItensAdm(Int32 idAss);
        List<CONTA_RECEBER> GetVencimentoAtual(Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilter(Int32? cliId, Int32? ccId, DateTime? dtLanc, DateTime? data, DateTime? dataFinal, String descricao, Int32? aberto, Int32? conta, Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilterRecebimentoMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, DateTime? liqui, Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilterAReceberMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilterCRAtrasos(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss);
        List<CONTA_RECEBER> ExecuteFilterAtrasos(String nome, String cidade, Int32? uf, Int32 idAss);
        List<CONTA_RECEBER> GetRecebimentosMes(DateTime mes, Int32 idAss);
        List<CONTA_RECEBER> GetAReceberMes(DateTime mes, Int32 idAss);
    }
}
