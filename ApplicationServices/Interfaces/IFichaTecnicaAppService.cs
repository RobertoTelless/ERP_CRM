using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFichaTecnicaAppService : IAppServiceBase<FICHA_TECNICA>
    {
        Int32 ValidateCreate(FICHA_TECNICA item, USUARIO usuario);
        Int32 ValidateCreateProduto(FICHA_TECNICA item, PRODUTO prod, USUARIO usuario);
        Int32 ValidateEdit(FICHA_TECNICA item, FICHA_TECNICA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(FICHA_TECNICA item, FICHA_TECNICA itemAntes);
        Int32 ValidateDelete(FICHA_TECNICA item, USUARIO usuario);
        Int32 ValidateReativar(FICHA_TECNICA item, USUARIO usuario);

        FICHA_TECNICA CheckExist(FICHA_TECNICA item, Int32 idAss);
        List<FICHA_TECNICA> GetAllItens(Int32 idAss);
        List<FICHA_TECNICA> GetAllItensAdm(Int32 idAss);
        FICHA_TECNICA GetItemById(Int32 id);
        FICHA_TECNICA_DETALHE GetDetalheById(Int32 id);
        Int32 ExecuteFilter(Int32? prodId, Int32? cat, String descricao, Int32 idAss, out List<FICHA_TECNICA> objeto);
        
        Int32 ValidateEditInsumo(FICHA_TECNICA_DETALHE item);
        Int32 ValidateCreateInsumo(FICHA_TECNICA_DETALHE item);
    }
}
