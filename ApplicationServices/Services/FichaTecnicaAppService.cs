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
    public class FichaTecnicaAppService : AppServiceBase<FICHA_TECNICA>, IFichaTecnicaAppService
    {
        private readonly IFichaTecnicaService _baseService;

        public FichaTecnicaAppService(IFichaTecnicaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public FICHA_TECNICA CheckExist(FICHA_TECNICA conta, Int32 idAss)
        {
            FICHA_TECNICA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<FICHA_TECNICA> GetAllItens(Int32 idAss)
        {
            List<FICHA_TECNICA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<FICHA_TECNICA> GetAllItensAdm(Int32 idAss)
        {
            List<FICHA_TECNICA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public FICHA_TECNICA GetItemById(Int32 id)
        {
            FICHA_TECNICA item = _baseService.GetItemById(id);
            return item;
        }

        public FICHA_TECNICA_DETALHE GetDetalheById(Int32 id)
        {
            FICHA_TECNICA_DETALHE lista = _baseService.GetDetalheById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? prodId, Int32? cat, String descricao, Int32 idAss, out List<FICHA_TECNICA> objeto)
        {
            try
            {
                objeto = new List<FICHA_TECNICA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(prodId, cat, descricao, idAss);
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

        public Int32 ValidateCreate(FICHA_TECNICA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.FITE_IN_ATIVO = 1;
                item.FITE_DT_CADASTRO = DateTime.Today.Date;
               
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddFITE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FICHA_TECNICA>(item)
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

        public Int32 ValidateCreateProduto(FICHA_TECNICA item, PRODUTO prod, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.FITE_IN_ATIVO = 1;
                item.FITE_DT_CADASTRO = DateTime.Today;
                item.PROD_CD_ID = prod.PROD_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddFITE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FICHA_TECNICA>(item)
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

        public Int32 ValidateEdit(FICHA_TECNICA item, FICHA_TECNICA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditFITE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FICHA_TECNICA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<FICHA_TECNICA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FICHA_TECNICA item, FICHA_TECNICA itemAntes)
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

        public Int32 ValidateDelete(FICHA_TECNICA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
               
                // Acerta campos
                item.FITE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelFITE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FICHA_TECNICA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FICHA_TECNICA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FITE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatFITE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FICHA_TECNICA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditInsumo(FICHA_TECNICA_DETALHE item)
        {
            try
            {
                // Persiste
                return _baseService.EditInsumo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateInsumo(FICHA_TECNICA_DETALHE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateInsumo(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
