using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class AtendimentoAgendaAppService : AppServiceBase<ATENDIMENTO_AGENDA>, IAtendimentoAgendaAppService
    {
        private readonly IAtendimentoAgendaService _baseService;

        public AtendimentoAgendaAppService(IAtendimentoAgendaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<ATENDIMENTO_AGENDA> GetAgendaByAtendimento(ATENDIMENTO item)
        {
            List<ATENDIMENTO_AGENDA> lista = _baseService.GetAgendaByAtendimento(item);
            return lista;
        }

        public Int32 ValidateCreate(ATENDIMENTO_AGENDA item, USUARIO usu)
        {
            try
            {
                // Verifica existencia prévia
                // Completa objeto
                item.ATAG_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddATENAGEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ATENDIMENTO_AGENDA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
