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
    public class TamanhoAppService : AppServiceBase<TAMANHO>, ITamanhoAppService
    {
        private readonly ITamanhoService _baseService;

        public TamanhoAppService(ITamanhoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public TAMANHO CheckExist(TAMANHO conta, Int32 idAss)
        {
            TAMANHO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TAMANHO> GetAllItens(Int32 idAss)
        {
            List<TAMANHO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TAMANHO> GetAllItensAdm(Int32 idAss)
        {
            List<TAMANHO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TAMANHO GetItemById(Int32 id)
        {
            TAMANHO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(TAMANHO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTAMA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TAMANHO>(item)
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

        public Int32 ValidateEdit(TAMANHO item, TAMANHO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTAMA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TAMANHO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TAMANHO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TAMANHO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PRODUTO_GRADE.Count > 0)
                {
                    return 1;
                }              
                
                // Acerta campos
                item.TAMA_IN_ATIVO = 0;
                item.ASSINANTE = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTAMA",
                    LOG_TX_REGISTRO = "Tamanho: " + item.TAMA_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TAMANHO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TAMA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTAMA",
                    LOG_TX_REGISTRO = "Tamanho: " + item.TAMA_NM_NOME
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
