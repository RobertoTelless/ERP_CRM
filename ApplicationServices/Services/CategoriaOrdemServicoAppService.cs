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
using System.Net;
using System.IO;
using EntitiesServices.Interfaces.Services;

namespace ApplicationServices.Services
{
    public class CategoriaOrdemServicoAppService : AppServiceBase<CATEGORIA_ORDEM_SERVICO>, ICategoriaOrdemServicoAppService
    {
        private readonly ICategoriaOrdemServicoService _baseService;

        public CategoriaOrdemServicoAppService(ICategoriaOrdemServicoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public CATEGORIA_ORDEM_SERVICO CheckExist(CATEGORIA_ORDEM_SERVICO item)
        {
            return _baseService.CheckExist(item);
        }

        public CATEGORIA_ORDEM_SERVICO GetItemById(Int32 id)
        {
            return _baseService.GetItemById(id);
        }

        public List<CATEGORIA_ORDEM_SERVICO> GetAllItens()
        {
            return _baseService.GetAllItens();
        }

        public List<CATEGORIA_ORDEM_SERVICO> GetAllItensAdm()
        {
            return _baseService.GetAllItensAdm();
        }

        public Int32 ValidateCreate(CATEGORIA_ORDEM_SERVICO item, USUARIO usuario)
        {
            try
            {
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                item.CAOS_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAOS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_ORDEM_SERVICO>(item)
                };

                Int32 volta = _baseService.Create(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CATEGORIA_ORDEM_SERVICO item, CATEGORIA_ORDEM_SERVICO itemAntes, USUARIO usuario)
        {
            if (item.ASSINANTE != null)
            {
                item.ASSINANTE = null;
            } 
            if (item.ORDEM_SERVICO != null)
            {
                item.ORDEM_SERVICO = null;
            }

            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCAOS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_ORDEM_SERVICO>(item),
                };

                Int32 volta = _baseService.Edit(item, log);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CATEGORIA_ORDEM_SERVICO item, CATEGORIA_ORDEM_SERVICO itemAntes)
        {
            if (item.ASSINANTE != null)
            {
                item.ASSINANTE = null;
            }
            if (item.ORDEM_SERVICO != null)
            {
                item.ORDEM_SERVICO = null;
            }

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

        public Int32 ValidateDelete(CATEGORIA_ORDEM_SERVICO item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.CAOS_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCAOS",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_ORDEM_SERVICO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_ORDEM_SERVICO item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.CAOS_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCAOS",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_ORDEM_SERVICO>(item)
                };

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
