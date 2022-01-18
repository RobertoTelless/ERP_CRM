using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaServicoService : IServiceBase<CATEGORIA_SERVICO>
    {
        Int32 Create(CATEGORIA_SERVICO item, LOG log);
        Int32 Create(CATEGORIA_SERVICO item);
        Int32 Edit(CATEGORIA_SERVICO item, LOG log);
        Int32 Edit(CATEGORIA_SERVICO item);
        Int32 Delete(CATEGORIA_SERVICO item, LOG log);
        CATEGORIA_SERVICO GetItemById(Int32 id);
        List<CATEGORIA_SERVICO> GetAllItens();
        List<CATEGORIA_SERVICO> GetAllItensAdm();
    }
}
