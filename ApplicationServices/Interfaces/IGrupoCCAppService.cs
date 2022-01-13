using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IGrupoCCAppService : IAppServiceBase<GRUPO_CC>
    {
        Int32 ValidateCreate(GRUPO_CC item, USUARIO usuario);
        Int32 ValidateEdit(GRUPO_CC item, GRUPO_CC itemAntes, USUARIO usuario);
        Int32 ValidateEdit(GRUPO_CC item, GRUPO_CC itemAntes);
        Int32 ValidateDelete(GRUPO_CC item, USUARIO usuario);
        Int32 ValidateReativar(GRUPO_CC item, USUARIO usuario);

        GRUPO_CC CheckExist(GRUPO_CC obj, Int32 idAss);
        List<GRUPO_CC> GetAllItens(Int32 idAss);
        List<GRUPO_CC> GetAllItensAdm(Int32 idAss);
        GRUPO_CC GetItemById(Int32 id);
    }
}
