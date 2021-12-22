using System;
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
    public class PlanoAppService : AppServiceBase<PLANO>, IPlanoAppService
    {
        private readonly IPlanoService _baseService;
        private readonly IConfiguracaoService _confService;

        public PlanoAppService(IPlanoService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<PLANO> GetAllItens()
        {
            List<PLANO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<PLANO> GetAllItensAdm()
        {
            List<PLANO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public List<PLANO> GetAllValidos()
        {
            List<PLANO> lista = _baseService.GetAllValidos();
            return lista;
        }

        public List<PLANO_PERIODICIDADE> GetAllPeriodicidades()
        {
            List<PLANO_PERIODICIDADE> lista = _baseService.GetAllPeriodicidades();
            return lista;
        }

        public PLANO GetItemById(Int32 id)
        {
            PLANO item = _baseService.GetItemById(id);
            return item;
        }

        public PLANO CheckExist(PLANO conta)
        {
            PLANO item = _baseService.CheckExist(conta);
            return item;
        }

        public Int32 ExecuteFilter(String nome, String descricao, out List<PLANO> objeto)
        {
            try
            {
                objeto = new List<PLANO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, descricao);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(PLANO item, USUARIO usuario)
        {
            try
            {
                var conf = usuario.USUA_CD_ID;
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PLAN_IN_ATIVO = 1;
                item.PLAN_DT_CRIACAO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPLAN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLANO>(item)
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

        public Int32 ValidateEdit(PLANO item, PLANO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPLAN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLANO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PLANO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PLANO item, PLANO itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PLANO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ASSINANTE_PLANO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.PLAN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPLAN",
                    LOG_TX_REGISTRO = item.PLAN_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PLANO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PLAN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPLAN",
                    LOG_TX_REGISTRO = item.PLAN_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
