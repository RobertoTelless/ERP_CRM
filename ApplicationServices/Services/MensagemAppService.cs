using System;
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
    public class MensagemAppService : AppServiceBase<MENSAGENS>, IMensagemAppService
    {
        private readonly IMensagemService _baseService;
        private readonly IConfiguracaoService _confService;

        public MensagemAppService(IMensagemService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<MENSAGENS> GetAllItens(Int32 idAss)
        {
            List<MENSAGENS> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public MENSAGEM_ANEXO GetAnexoById(Int32 id)
        {
            return _baseService.GetAnexoById(id);
        }

        public MENSAGENS_DESTINOS GetDestinoById(Int32 id)
        {
            MENSAGENS_DESTINOS lista = _baseService.GetDestinoById(id);
            return lista;
        }

        public List<MENSAGENS> GetAllItensAdm(Int32 idAss)
        {
            List<MENSAGENS> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MENSAGENS GetItemById(Int32 id)
        {
            MENSAGENS item = _baseService.GetItemById(id);
            return item;
        }

        public MENSAGENS CheckExist(MENSAGENS conta, Int32 idAss)
        {
            MENSAGENS item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_CLIENTE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<TEMPLATE_SMS> GetAllTemplatesSMS(Int32 idAss)
        {
            List<TEMPLATE_SMS> lista = _baseService.GetAllTemplatesSMS(idAss);
            return lista;
        }

        public Int32 ExecuteFilterSMS(DateTime? envio, Int32 cliente, String texto, Int32 idAss, out List<MENSAGENS> objeto)
        {
            try
            {
                objeto = new List<MENSAGENS>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterSMS(envio, cliente, texto, idAss);
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

        public Int32 ExecuteFilterEMail(DateTime? envio, Int32 cliente, String texto, Int32 idAss, out List<MENSAGENS> objeto)
        {
            try
            {
                objeto = new List<MENSAGENS>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterSMS(envio, cliente, texto, idAss);
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

        public Int32 ValidateCreate(MENSAGENS item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.MENS_IN_ATIVO = 1;
                item.MENS_DT_CRIACAO = DateTime.Now;
                item.USUA_CD_ID = usuario.USUA_CD_ID;
                if (item.MENS_NM_LINK != null)
                {
                    if (!item.MENS_NM_LINK.Contains("www."))
                    {
                        item.MENS_NM_LINK = "www." + item.MENS_NM_LINK;
                    }
                    if (!item.MENS_NM_LINK.Contains("http://"))
                    {
                        item.MENS_NM_LINK = "http://" + item.MENS_NM_LINK;
                    }
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddMENS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MENSAGENS>(item)
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

        public Int32 ValidateEdit(MENSAGENS item, MENSAGENS itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditMENS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MENSAGENS>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<MENSAGENS>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(MENSAGENS item, MENSAGENS itemAntes)
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

        public Int32 ValidateDelete(MENSAGENS item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MENS_IN_ATIVO = 0;
                item.ASSINANTE = null;
                item.USUARIO = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelMENS",
                    LOG_TX_REGISTRO = item.MENS_NM_CAMPANHA + "|" + item.MENS_DT_CRIACAO.Value.ToShortDateString() + "|" + item.MENS_IN_TIPO.ToString()
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(MENSAGENS item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MENS_IN_ATIVO = 1;
                item.ASSINANTE = null;
                item.USUARIO = null;                

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatMENS",
                    LOG_TX_REGISTRO = item.MENS_NM_CAMPANHA + "|" + item.MENS_DT_CRIACAO.Value.ToShortDateString() + "|" + item.MENS_IN_TIPO.ToString()
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditDestino(MENSAGENS_DESTINOS item)
        {
            try
            {
                // Persiste
                return _baseService.EditDestino(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
