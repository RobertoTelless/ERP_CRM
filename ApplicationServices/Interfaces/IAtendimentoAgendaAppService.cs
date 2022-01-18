using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAtendimentoAgendaAppService : IAppServiceBase<ATENDIMENTO_AGENDA>
    {
        Int32 ValidateCreate(ATENDIMENTO_AGENDA item, USUARIO usu);

        List<ATENDIMENTO_AGENDA> GetAgendaByAtendimento(ATENDIMENTO item);
    }
}
