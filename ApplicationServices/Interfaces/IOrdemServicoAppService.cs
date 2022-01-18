using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace ApplicationServices.Interfaces
{
    public interface IOrdemServicoAppService : IAppServiceBase<ORDEM_SERVICO>
    {
        Int32 ValidateCreate(ORDEM_SERVICO item, USUARIO usuario);
        Int32 ValidateEdit(ORDEM_SERVICO item, ORDEM_SERVICO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(ORDEM_SERVICO item, ORDEM_SERVICO itemAntes);
        Int32 ValidateDelete(ORDEM_SERVICO item, USUARIO usuario);
        Int32 ValidateReativar(ORDEM_SERVICO item, USUARIO usuario);

        Int32 ValidateCreateAcompanhamento(ORDEM_SERVICO_ACOMPANHAMENTO item);
        Int32 ValidateCreateComentario(ORDEM_SERVICO_COMENTARIOS item);

        ORDEM_SERVICO CheckExist(ORDEM_SERVICO conta);
        ORDEM_SERVICO GetItemById(Int32 id);
        List<ORDEM_SERVICO> GetAllItens();
        List<ORDEM_SERVICO> GetAllItensAdm();
        Int32 ExecuteFilter(Int32? catOS, Int32? idClie, Int32? idUsu, DateTime? dtCriacao, Int32? status, Int32? idDept, Int32? idServ, Int32? idProd, Int32? idAten, out List<ORDEM_SERVICO> objeto);
        ORDEM_SERVICO_ANEXO GetAnexoById(Int32 id);
        List<ORDEM_SERVICO_ACOMPANHAMENTO> GetAcompanhamentoByOs(ORDEM_SERVICO item);
        List<ORDEM_SERVICO_COMENTARIOS> GetComentarioByOs(ORDEM_SERVICO item);
    }
}
