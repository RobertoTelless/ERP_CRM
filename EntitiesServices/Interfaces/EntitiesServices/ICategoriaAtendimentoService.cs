using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaAtendimentoService : IServiceBase<CATEGORIA_ATENDIMENTO>
    {
        Int32 Create(CATEGORIA_ATENDIMENTO item, LOG log);
        Int32 Create(CATEGORIA_ATENDIMENTO item);
        Int32 Edit(CATEGORIA_ATENDIMENTO item, LOG log);
        Int32 Edit(CATEGORIA_ATENDIMENTO item);
        Int32 Delete(CATEGORIA_ATENDIMENTO item, LOG log);

        CATEGORIA_ATENDIMENTO CheckExist(CATEGORIA_ATENDIMENTO item, Int32 idAss);
        CATEGORIA_ATENDIMENTO GetItemById(Int32 id);
        List<CATEGORIA_ATENDIMENTO> GetAllItens(Int32 idAss);
        List<CATEGORIA_ATENDIMENTO> GetAllItensAdm(Int32 idAss);
    }
}
