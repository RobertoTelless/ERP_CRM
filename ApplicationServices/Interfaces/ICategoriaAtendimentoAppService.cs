using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaAtendimentoAppService : IAppServiceBase<CATEGORIA_ATENDIMENTO>
    {
        Int32 ValidateCreate(CATEGORIA_ATENDIMENTO item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_ATENDIMENTO item, CATEGORIA_ATENDIMENTO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_ATENDIMENTO item, CATEGORIA_ATENDIMENTO itemAntes);
        Int32 ValidateDelete(CATEGORIA_ATENDIMENTO item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_ATENDIMENTO item, USUARIO usuario);
        List<CATEGORIA_ATENDIMENTO> GetAllItens();
        List<CATEGORIA_ATENDIMENTO> GetAllItensAdm();
        CATEGORIA_ATENDIMENTO GetItemById(Int32 id);
    }
}
