using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMovimentoEstoqueProdutoService : IServiceBase<MOVIMENTO_ESTOQUE_PRODUTO>
    {
        Int32 Create(MOVIMENTO_ESTOQUE_PRODUTO perfil, LOG log);
        Int32 Create(MOVIMENTO_ESTOQUE_PRODUTO perfil);
        Int32 Edit(MOVIMENTO_ESTOQUE_PRODUTO item, LOG log);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens();
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensAdm();
        MOVIMENTO_ESTOQUE_PRODUTO GetByProdId(Int32 prod, Int32 fili);
        MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensEntrada();
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensSaida();
        List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilter(Int32? catId, Int32? subCatId, String nome, String barcode, Int32? filiId, DateTime? dtMov);
        List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilterAvulso(Int32? operacao, Int32? tipoMovimento, DateTime? dtInicial, DateTime? dtFinal, Int32? filial, Int32? prod);
    }
}
