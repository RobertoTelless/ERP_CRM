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
    public class CategoriaProdutoAppService : AppServiceBase<CATEGORIA_PRODUTO>, ICategoriaProdutoAppService
    {
        private readonly ICategoriaProdutoService _baseService;

        public CategoriaProdutoAppService(ICategoriaProdutoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public CATEGORIA_PRODUTO CheckExist(CATEGORIA_PRODUTO conta, Int32 idAss)
        {
            CATEGORIA_PRODUTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_PRODUTO> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_PRODUTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_PRODUTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_PRODUTO GetItemById(Int32 id)
        {
            CATEGORIA_PRODUTO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_PRODUTO item, USUARIO usuario)
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
                item.CAPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_PRODUTO>(item)
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

        public Int32 ValidateEdit(CATEGORIA_PRODUTO item, CATEGORIA_PRODUTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCAPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_PRODUTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_PRODUTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CATEGORIA_PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PRODUTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CAPR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCAPR",
                    LOG_TX_REGISTRO = "Categoria: " + item.CAPR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCAPR",
                    LOG_TX_REGISTRO = "Categoria: " + item.CAPR_NM_NOME
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
