using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaEquipamentoAppService : IAppServiceBase<CATEGORIA_EQUIPAMENTO>
    {
        Int32 ValidateCreate(CATEGORIA_EQUIPAMENTO item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_EQUIPAMENTO item, CATEGORIA_EQUIPAMENTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CATEGORIA_EQUIPAMENTO item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_EQUIPAMENTO item, USUARIO usuario);

        CATEGORIA_EQUIPAMENTO CheckExist(CATEGORIA_EQUIPAMENTO item, Int32 idAss);
        List<CATEGORIA_EQUIPAMENTO> GetAllItens(Int32 idAss);
        CATEGORIA_EQUIPAMENTO GetItemById(Int32 id);
        List<CATEGORIA_EQUIPAMENTO> GetAllItensAdm(Int32 idAss);

    }
}
