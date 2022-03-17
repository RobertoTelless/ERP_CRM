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
    public class OrdemServicoProdutoAppService : AppServiceBase<ORDEM_SERVICO_PRODUTO>, IOrdemServicoProdutoAppService
    {
        private readonly IOrdemServicoProdutoService _baseService;

        public OrdemServicoProdutoAppService(IOrdemServicoProdutoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public ORDEM_SERVICO_PRODUTO CheckExist(ORDEM_SERVICO_PRODUTO item)
        {
            return _baseService.CheckExist(item);
        }

        public List<ORDEM_SERVICO_PRODUTO> GetAllByOs(Int32 id)
        {
            return _baseService.GetAllByOs(id);
        }

        public Int32 ValidateCreate(ORDEM_SERVICO_PRODUTO item)
        {
            try
            {
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                item.OSPR_IN_ATIVO = 1;

                Int32 volta = _baseService.Create(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(ORDEM_SERVICO_PRODUTO item, ORDEM_SERVICO_PRODUTO itemAntes)
        {
            try
            {
                if (item.ORDEM_SERVICO != null)
                {
                    item.ORDEM_SERVICO = null;
                }

                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                // Acerta campos
                item.OSPR_IN_ATIVO = 0;

                // Persiste
                return _baseService.Edit(item, itemAntes);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(ORDEM_SERVICO_PRODUTO item, ORDEM_SERVICO_PRODUTO itemAntes)
        {
            try
            {
                if (item.ORDEM_SERVICO != null)
                {
                    item.ORDEM_SERVICO = null;
                }

                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                // Acerta campos
                item.OSPR_IN_ATIVO = 1;

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
