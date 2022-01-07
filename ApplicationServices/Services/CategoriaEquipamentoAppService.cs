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
    public class CategoriaEquipamentoAppService : AppServiceBase<CATEGORIA_EQUIPAMENTO>, ICategoriaEquipamentoAppService
    {
        private readonly ICategoriaEquipamentoService _baseService;

        public CategoriaEquipamentoAppService(ICategoriaEquipamentoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<CATEGORIA_EQUIPAMENTO> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_EQUIPAMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_EQUIPAMENTO> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_EQUIPAMENTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_EQUIPAMENTO GetItemById(Int32 id)
        {
            CATEGORIA_EQUIPAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public CATEGORIA_EQUIPAMENTO CheckExist(CATEGORIA_EQUIPAMENTO conta, Int32 idAss)
        {
            CATEGORIA_EQUIPAMENTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_EQUIPAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.CAEQ_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAEQ",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_EQUIPAMENTO>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(CATEGORIA_EQUIPAMENTO item, CATEGORIA_EQUIPAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCAEQ",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_EQUIPAMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_EQUIPAMENTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CATEGORIA_EQUIPAMENTO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.EQUIPAMENTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CAEQ_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCAEQ",
                    LOG_TX_REGISTRO = item.CAEQ_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_EQUIPAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAEQ_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCAEQ",
                    LOG_TX_REGISTRO = item.CAEQ_NM_NOME
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
