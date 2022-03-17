using EntitiesServices.Model;
using ModelServices.Interfaces.EntitiesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaOrdemServicoService : IServiceBase<CATEGORIA_ORDEM_SERVICO>
    {
        Int32 Create(CATEGORIA_ORDEM_SERVICO perfil, LOG log);
        Int32 Create(CATEGORIA_ORDEM_SERVICO perfil);
        Int32 Edit(CATEGORIA_ORDEM_SERVICO perfil, LOG log);
        Int32 Edit(CATEGORIA_ORDEM_SERVICO perfil);
        Int32 Delete(CATEGORIA_ORDEM_SERVICO perfil, LOG log);

        CATEGORIA_ORDEM_SERVICO CheckExist(CATEGORIA_ORDEM_SERVICO item, Int32 idAss);
        CATEGORIA_ORDEM_SERVICO GetItemById(Int32 id);
        List<CATEGORIA_ORDEM_SERVICO> GetAllItens(Int32 idAss);
        List<CATEGORIA_ORDEM_SERVICO> GetAllItensAdm(Int32 idAss);
    }
}
