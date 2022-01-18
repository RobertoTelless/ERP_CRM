using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IServicoService : IServiceBase<SERVICO>
    {
        Int32 Create(SERVICO perfil, LOG log);
        Int32 Create(SERVICO perfil);
        Int32 Edit(SERVICO perfil, LOG log);
        Int32 Edit(SERVICO perfil);
        Int32 Delete(SERVICO perfil, LOG log);

        SERVICO CheckExist(SERVICO conta, Int32 idAss);
        SERVICO GetItemById(Int32 id);
        List<SERVICO> GetAllItens(Int32 idAss);
        List<SERVICO> GetAllItensAdm(Int32 idAss);
        List<SERVICO> ExecuteFilter(Int32? catId, String nome, String descricao, String referencia, Int32 idAss);

        List<CATEGORIA_SERVICO> GetAllTipos(Int32 idAss);
        List<NOMENCLATURA_BRAS_SERVICOS> GetAllNBSE();
        SERVICO_ANEXO GetAnexoById(Int32 id);
    }
}
