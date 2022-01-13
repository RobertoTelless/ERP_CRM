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
    public class GrupoCCAppService : AppServiceBase<GRUPO_CC>, IGrupoCCAppService
    {
        private readonly IGrupoCCService _baseService;

        public GrupoCCAppService(IGrupoCCService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<GRUPO_CC> GetAllItens(Int32 idAss)
        {
            List<GRUPO_CC> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public GRUPO_CC CheckExist(GRUPO_CC obj, Int32 idAss)
        {
            GRUPO_CC item = _baseService.CheckExist(obj, idAss);
            return item;
        }

        public List<GRUPO_CC> GetAllItensAdm(Int32 idAss)
        {
            List<GRUPO_CC> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public GRUPO_CC GetItemById(Int32 id)
        {
            GRUPO_CC item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(GRUPO_CC item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.GRCC_CD_ID = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddGRUP",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<GRUPO_CC>(item)
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

        public Int32 ValidateEdit(GRUPO_CC item, GRUPO_CC itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditGRUP",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<GRUPO_CC>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<GRUPO_CC>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(GRUPO_CC item, GRUPO_CC itemAntes)
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

        public Int32 ValidateDelete(GRUPO_CC item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CENTRO_CUSTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.GRCC_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleGRUP",
                    LOG_TX_REGISTRO = "Grupo: " + item.GRCC_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(GRUPO_CC item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.GRCC_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatGRUP",
                    LOG_TX_REGISTRO = "Grupo: " + item.GRCC_NM_NOME
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
