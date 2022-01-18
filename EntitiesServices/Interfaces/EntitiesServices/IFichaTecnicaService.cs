using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFichaTecnicaService : IServiceBase<FICHA_TECNICA>
    {
        Int32 Create(FICHA_TECNICA item, LOG log);
        Int32 Create(FICHA_TECNICA item);
        Int32 Edit(FICHA_TECNICA item, LOG log);
        Int32 Edit(FICHA_TECNICA item);
        Int32 Delete(FICHA_TECNICA item, LOG log);
        FICHA_TECNICA GetItemById(Int32 id);
        List<FICHA_TECNICA> GetAllItens();
        List<FICHA_TECNICA> GetAllItensAdm();
        FICHA_TECNICA_DETALHE GetDetalheById(Int32 id);
        List<FICHA_TECNICA> ExecuteFilter(Int32? prodId, Int32? cat, String descricao);
        Int32 EditInsumo(FICHA_TECNICA_DETALHE item);
        Int32 CreateInsumo(FICHA_TECNICA_DETALHE item);
    }
}
