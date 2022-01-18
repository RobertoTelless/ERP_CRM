using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAtendimentoAppService : IAppServiceBase<ATENDIMENTO>
    {
        Int32 ValidateCreate(ATENDIMENTO perfil, USUARIO usuario);
        Int32 ValidateEdit(ATENDIMENTO perfil, ATENDIMENTO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(ATENDIMENTO item, ATENDIMENTO itemAntes);
        Int32 ValidateDelete(ATENDIMENTO perfil, USUARIO usuario);
        Int32 ValidateReativar(ATENDIMENTO perfil, USUARIO usuario);
        String ValidateCreateMensagem(CLIENTE item, USUARIO usuario, Int32? idAss);

        List<ATENDIMENTO> GetAllItens();
        List<ATENDIMENTO> GetAllItensAdm();
        ATENDIMENTO GetItemById(Int32 id);
        List<ATENDIMENTO> GetByCliente(Int32 id);
        ATENDIMENTO_ANEXO GetAnexoById(Int32 id);
        ATENDIMENTO CheckExist(ATENDIMENTO conta);
        List<CATEGORIA_ATENDIMENTO> GetAllTipos();

        Int32 ExecuteFilter(Int32? idCat, Int32? cliente, Int32? produto, DateTime? data, Int32? status, String descricao, Int32? depto, Int32? prioridade, Int32? idUsua, Int32? idServico, Int32? sla, out List<ATENDIMENTO> objeto);
    }
}
