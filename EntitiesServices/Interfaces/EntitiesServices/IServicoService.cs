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

        SERVICO CheckExist(SERVICO conta);
        SERVICO GetItemById(Int32 id);
        List<SERVICO> GetAllItens();
        List<SERVICO> GetAllItensAdm();
        List<SERVICO> ExecuteFilter(Int32? catId, String nome, String descricao, String referencia);

        List<CATEGORIA_SERVICO> GetAllTipos();
        List<NOMENCLATURA_BRAS_SERVICOS> GetAllNBSE();
        SERVICO_ANEXO GetAnexoById(Int32 id);
    }
}
