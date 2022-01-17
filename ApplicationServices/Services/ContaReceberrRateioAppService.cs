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
    public class ContaReceberRateioAppService : AppServiceBase<CONTA_RECEBER_RATEIO>, IContaReceberRateioAppService
    {
        private readonly IContaReceberRateioService _baseService;


        public ContaReceberRateioAppService(IContaReceberRateioService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public CONTA_RECEBER_RATEIO CheckExist(CONTA_RECEBER_RATEIO conta)
        {
            CONTA_RECEBER_RATEIO item = _baseService.CheckExist(conta);
            return item;
        }

        public CONTA_RECEBER_RATEIO GetItemById(Int32 id)
        {
            CONTA_RECEBER_RATEIO item = _baseService.GetItemById(id);
            return item;
        }

        public List<CONTA_RECEBER_RATEIO> GetAllItens()
        {
            return _baseService.GetAllItens();
        }

        public Int32 ValidateCreate(CONTA_RECEBER_RATEIO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.CRRA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCRRA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_RECEBER_RATEIO>(item)
                };

                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONTA_RECEBER_RATEIO item)
        {
            try
            {
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CONTA_RECEBER_RATEIO item)
        {
            try
            {
                // Persiste
                return _baseService.Delete(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CONTA_RECEBER_RATEIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CRRA_IN_ATIVO = 1;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "ReatCARE",
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_RECEBER>(item)
                //};

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
