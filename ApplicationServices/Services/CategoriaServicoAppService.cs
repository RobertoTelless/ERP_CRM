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
    public class CategoriaServicoAppService : AppServiceBase<CATEGORIA_SERVICO>, ICategoriaServicoAppService
    {
        private readonly ICategoriaServicoService _baseService;

        public CategoriaServicoAppService(ICategoriaServicoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public CATEGORIA_SERVICO CheckExist(CATEGORIA_SERVICO conta, Int32 idAss)
        {
            CATEGORIA_SERVICO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_SERVICO> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_SERVICO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_SERVICO> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_SERVICO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_SERVICO GetItemById(Int32 id)
        {
            CATEGORIA_SERVICO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_SERVICO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CASE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCASE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_SERVICO>(item)
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

        public Int32 ValidateEdit(CATEGORIA_SERVICO item, CATEGORIA_SERVICO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCASE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_SERVICO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_SERVICO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CATEGORIA_SERVICO item, CATEGORIA_SERVICO itemAntes)
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

        public Int32 ValidateDelete(CATEGORIA_SERVICO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.SERVICO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CASE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCASE",
                    LOG_TX_REGISTRO = "Categoria: " + item.CASE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_SERVICO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CASE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCASE",
                    LOG_TX_REGISTRO = "Categoria: " + item.CASE_NM_NOME
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
