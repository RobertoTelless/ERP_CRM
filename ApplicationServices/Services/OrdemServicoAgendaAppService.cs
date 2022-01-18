using ApplicationServices.Interfaces;
using EntitiesServices.Interfaces.Services;
using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.Services
{
    public class OrdemServicoAgendaAppService : AppServiceBase<ORDEM_SERVICO_AGENDA>, IOrdemServicoAgendaAppService
    {
        private readonly IOrdemServicoAgendaService _baseService;

        public OrdemServicoAgendaAppService(IOrdemServicoAgendaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<ORDEM_SERVICO_AGENDA> GetAgendaByOs(ORDEM_SERVICO item)
        {
            List<ORDEM_SERVICO_AGENDA> lista = _baseService.GetAgendaByOs(item);
            return lista;
        }

        public Int32 ValidateCreate(ORDEM_SERVICO_AGENDA item)
        {
            try
            {
                item.OSAG_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
