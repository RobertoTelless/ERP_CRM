using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IOrdemServicoService : IServiceBase<ORDEM_SERVICO>
    {
        Int32 Create(ORDEM_SERVICO perfil, LOG log);
        Int32 Create(ORDEM_SERVICO perfil);
        Int32 Edit(ORDEM_SERVICO perfil, LOG log);
        Int32 Edit(ORDEM_SERVICO perfil);
        Int32 Delete(ORDEM_SERVICO perfil, LOG log);

        ORDEM_SERVICO CheckExist(ORDEM_SERVICO item);
        ORDEM_SERVICO GetItemById(Int32 id);
        List<ORDEM_SERVICO> GetAllItens();
        List<ORDEM_SERVICO> GetAllItensAdm();
        List<ORDEM_SERVICO> ExecuteFilter(Int32? catOS, Int32? idClie, Int32? idUsu, DateTime? dtCriacao, Int32? status, Int32? idDept, Int32? idServ, Int32? idProd, Int32? idAten);
        ORDEM_SERVICO_ANEXO GetAnexoById(Int32 id);
    }
}
