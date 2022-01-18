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
    public class CategoriaAtendimentoAppService : AppServiceBase<CATEGORIA_ATENDIMENTO>, ICategoriaAtendimentoAppService
    {
        private readonly ICategoriaAtendimentoService _baseService;

        public CategoriaAtendimentoAppService(ICategoriaAtendimentoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<CATEGORIA_ATENDIMENTO> GetAllItens()
        {
            List<CATEGORIA_ATENDIMENTO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<CATEGORIA_ATENDIMENTO> GetAllItensAdm()
        {
            List<CATEGORIA_ATENDIMENTO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public CATEGORIA_ATENDIMENTO GetItemById(Int32 id)
        {
            CATEGORIA_ATENDIMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_ATENDIMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CAAT_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAAT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_ATENDIMENTO>(item)
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

        public Int32 ValidateEdit(CATEGORIA_ATENDIMENTO item, CATEGORIA_ATENDIMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCAAT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_ATENDIMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_ATENDIMENTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CATEGORIA_ATENDIMENTO item, CATEGORIA_ATENDIMENTO itemAntes)
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

        public Int32 ValidateDelete(CATEGORIA_ATENDIMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ATENDIMENTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CAAT_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleCAAT",
                    LOG_TX_REGISTRO = "Categoria: " + item.CAAT_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_ATENDIMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAAT_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCAAT",
                    LOG_TX_REGISTRO = "Categoria: " + item.CAAT_NM_NOME
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
