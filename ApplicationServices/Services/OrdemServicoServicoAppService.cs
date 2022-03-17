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
    public class OrdemServicoServicoAppService : AppServiceBase<ORDEM_SERVICO_SERVICO>, IOrdemServicoServicoAppService
    {
        private readonly IOrdemServicoServicoService _baseService;

        public OrdemServicoServicoAppService(IOrdemServicoServicoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public ORDEM_SERVICO_SERVICO CheckExist(ORDEM_SERVICO_SERVICO item)
        {
            return _baseService.CheckExist(item);
        }

        public List<ORDEM_SERVICO_SERVICO> GetAllByOs(Int32 id)
        {
            return _baseService.GetAllByOs(id);
        }

        public Int32 ValidateCreate(ORDEM_SERVICO_SERVICO item)
        {
            try
            {
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                item.OSSE_IN_ATIVO = 1;

                Int32 volta = _baseService.Create(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(ORDEM_SERVICO_SERVICO item, ORDEM_SERVICO_SERVICO itemAntes)
        {
            try
            {
                if (item.ORDEM_SERVICO != null)
                {
                    item.ORDEM_SERVICO = null;
                }

                if (item.SERVICO != null)
                {
                    item.SERVICO = null;
                }

                // Acerta campos
                item.OSSE_IN_ATIVO = 0;

                // Persiste
                return _baseService.Edit(item, itemAntes);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(ORDEM_SERVICO_SERVICO item, ORDEM_SERVICO_SERVICO itemAntes)
        {
            try
            {
                if (item.ORDEM_SERVICO != null)
                {
                    item.ORDEM_SERVICO = null;
                }

                if (item.SERVICO != null)
                {
                    item.SERVICO = null;
                }

                // Acerta campos
                item.OSSE_IN_ATIVO = 1;

                // Persiste
                return _baseService.Edit(item, itemAntes);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
