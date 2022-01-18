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
    public class ServicoAppService : AppServiceBase<SERVICO>, IServicoAppService
    {
        private readonly IServicoService _baseService;

        public ServicoAppService(IServicoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<SERVICO> GetAllItens(Int32 idAss)
        {
            List<SERVICO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<SERVICO> GetAllItensAdm(Int32 idAss)
        {
            List<SERVICO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public SERVICO GetItemById(Int32 id)
        {
            SERVICO item = _baseService.GetItemById(id);
            return item;
        }

        public SERVICO CheckExist(SERVICO conta, Int32 idAss)
        {
            SERVICO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_SERVICO> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_SERVICO> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<NOMENCLATURA_BRAS_SERVICOS> GetAllNBSE()
        {
            List<NOMENCLATURA_BRAS_SERVICOS> lista = _baseService.GetAllNBSE();
            return lista;
        }


        public SERVICO_ANEXO GetAnexoById(Int32 id)
        {
            SERVICO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? catId, String nome, String descricao, String referencia, Int32 idAss, out List<SERVICO> objeto)
        {
            try
            {
                objeto = new List<SERVICO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, nome, descricao, referencia, idAss);
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

        public Int32 ValidateCreate(SERVICO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.SERV_IN_ATIVO = 1;


                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddSERV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<SERVICO>(item)
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

        public Int32 ValidateEdit(SERVICO item, SERVICO itemAntes, USUARIO usuario)
        {
            try
            {

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditSERV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<SERVICO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<SERVICO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(SERVICO item, SERVICO itemAntes)
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

        public Int32 ValidateDelete(SERVICO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ATENDIMENTO.Count > 0)
                {
                    return 1;
                }
                if (item.ORDEM_SERVICO.Count > 0)
                {
                    return 1;
                }

                // Ajusta objetos
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CATEGORIA_SERVICO != null)
                {
                    item.CATEGORIA_SERVICO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                if (item.NOMENCLATURA_BRAS_SERVICOS!= null)
                {
                    item.NOMENCLATURA_BRAS_SERVICOS = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                // Acerta campos
                item.SERV_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelSERV",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<SERVICO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(SERVICO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.SERV_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatSERV",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<SERVICO>(item)
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
