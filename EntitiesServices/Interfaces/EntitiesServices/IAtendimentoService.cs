using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAtendimentoService : IServiceBase<ATENDIMENTO>
    {
        Int32 Create(ATENDIMENTO perfil, LOG log);
        Int32 Create(ATENDIMENTO perfil);
        Int32 Edit(ATENDIMENTO perfil, LOG log);
        Int32 Edit(ATENDIMENTO perfil);
        Int32 Delete(ATENDIMENTO perfil, LOG log);

        ATENDIMENTO CheckExist(ATENDIMENTO conta, Int32 idAss);
        ATENDIMENTO GetItemById(Int32 id);
        List<ATENDIMENTO> GetAllItens(Int32 idAss);
        List<ATENDIMENTO> GetAllItensAdm(Int32 idAss);
        List<ATENDIMENTO> GetByCliente(Int32 id, Int32 idAss);
        ATENDIMENTO_ANEXO GetAnexoById(Int32 id);
        List<CATEGORIA_ATENDIMENTO> GetAllTipos(Int32 idAss);

        List<ATENDIMENTO> ExecuteFilter(Int32? idCat, Int32? cliente, Int32? produto, DateTime? data, Int32? status, String descricao, Int32? depto, Int32? prioridade, Int32? idUsua, Int32? idServico, Int32? sla, Int32 idAss);
    }
}
