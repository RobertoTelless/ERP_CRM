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
    public class SubcategoriaProdutoAppService : AppServiceBase<SUBCATEGORIA_PRODUTO>, ISubcategoriaProdutoAppService
    {
        private readonly ISubcategoriaProdutoService _baseService;

        public SubcategoriaProdutoAppService(ISubcategoriaProdutoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public SUBCATEGORIA_PRODUTO CheckExist(SUBCATEGORIA_PRODUTO conta, Int32 idAss)
        {
            SUBCATEGORIA_PRODUTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllItens(Int32 idAss)
        {
            List<SUBCATEGORIA_PRODUTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            List<SUBCATEGORIA_PRODUTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public SUBCATEGORIA_PRODUTO GetItemById(Int32 id)
        {
            SUBCATEGORIA_PRODUTO item = _baseService.GetItemById(id);
            return item;
        }

        public List<CATEGORIA_PRODUTO> GetAllCategorias(Int32 idAss)
        {
            List<CATEGORIA_PRODUTO> lista = _baseService.GetAllCategorias(idAss);
            return lista;
        }

        public Int32 ValidateCreate(SUBCATEGORIA_PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.SCPR_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddSCPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<SUBCATEGORIA_PRODUTO>(item)
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

        public Int32 ValidateEdit(SUBCATEGORIA_PRODUTO item, SUBCATEGORIA_PRODUTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditSCPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<SUBCATEGORIA_PRODUTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<SUBCATEGORIA_PRODUTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(SUBCATEGORIA_PRODUTO item, SUBCATEGORIA_PRODUTO itemAntes)
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

        public Int32 ValidateDelete(SUBCATEGORIA_PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PRODUTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.SCPR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleSCPR",
                    LOG_TX_REGISTRO = "Subcategoria: " + item.SCPR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(SUBCATEGORIA_PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.SCPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatSCPR",
                    LOG_TX_REGISTRO = "Subcategoria: " + item.SCPR_NM_NOME
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
