using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITamanhoAppService : IAppServiceBase<TAMANHO>
    {
        Int32 ValidateCreate(TAMANHO item, USUARIO usuario);
        Int32 ValidateEdit(TAMANHO item, TAMANHO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TAMANHO item, USUARIO usuario);
        Int32 ValidateReativar(TAMANHO item, USUARIO usuario);

        TAMANHO CheckExist(TAMANHO conta, Int32 idAss);
        List<TAMANHO> GetAllItens(Int32 idAss);
        TAMANHO GetItemById(Int32 id);
        List<TAMANHO> GetAllItensAdm(Int32 idAss);
    }
}
