using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPeriodicidadeTarefaRepository : IRepositoryBase<PERIODICIDADE_TAREFA>
    {
        List<PERIODICIDADE_TAREFA> GetAllItens(Int32 idAss);
        PERIODICIDADE_TAREFA GetItemById(Int32 id);
        List<PERIODICIDADE_TAREFA> GetAllItensAdm(Int32 idAss);
    }
}
