using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IUnidadeAppService : IAppServiceBase<UNIDADE>
    {
        Int32 ValidateCreate(UNIDADE item, USUARIO usuario);
        Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes, USUARIO usuario);
        Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes);
        Int32 ValidateDelete(UNIDADE item, USUARIO usuario);
        Int32 ValidateReativar(UNIDADE item, USUARIO usuario);

        UNIDADE CheckExist(UNIDADE conta, Int32 idAss);
        List<UNIDADE> GetAllItens(Int32 idAss);
        List<UNIDADE> GetAllItensAdm(Int32 idAss);
        UNIDADE GetItemById(Int32 id);
    }
}
