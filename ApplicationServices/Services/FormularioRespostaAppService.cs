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
using System.Net;

namespace ApplicationServices.Services
{
    public class FormularioRespostaAppService : AppServiceBase<FORMULARIO_RESPOSTA>, IFormularioRespostaAppService
    {
        private readonly IFormularioRespostaService _baseService;
        private readonly IConfiguracaoService _confService;
        private readonly ITemplateAppService _tempService;

        public FormularioRespostaAppService(IFormularioRespostaService baseService, IConfiguracaoService confService, ITemplateAppService tempService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
            _tempService = tempService;
        }

        public List<FORMULARIO_RESPOSTA> GetAllItens()
        {
            List<FORMULARIO_RESPOSTA> lista = _baseService.GetAllItens();
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
        public FORMULARIO_RESPOSTA GetItemById(Int32 id)
        {
            FORMULARIO_RESPOSTA item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ExecuteFilter(String nome, String email, String celular, String cidade, Int32? uf, out List<FORMULARIO_RESPOSTA> objeto)
        {
            try
            {
                objeto = new List<FORMULARIO_RESPOSTA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, email, celular, cidade, uf);
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

        public Int32 ValidateCreate(FORMULARIO_RESPOSTA item)
        {
            try
            {
                // Completa objeto
                item.FORE_IN_ATIVO = 1;
                item.FORE_IN_STATUS = 1;
 
                // Persiste
                Int32 volta = _baseService.Create(item);

                // Envia e-mail de confirmação
                CONFIGURACAO conf = _confService.GetAll().First();

                // Recupera template
                String header = _tempService.GetByCode("RESPFORM").TEMP_TX_CABECALHO;
                String body = _tempService.GetByCode("RESPFORM").TEMP_TX_CORPO;
                String footer = _tempService.GetByCode("RESPFORM").TEMP_TX_DADOS;

                // Prepara campos
                header = header.Replace("{nome}", item.FORE_NM_NOME);
                body = body.Replace("{data}", DateTime.Today.ToLongDateString());
                String emailBody = header + "<br /><br />" + body + "<br /><br />" + footer;

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                Email mensagem = new Email();
                mensagem.ASSUNTO = "Solicitação de Informações - ERPSys";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_DESTINO = item.FORE_NM_EMAIL;
                mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = "ERPSys";
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.IS_HTML = true;
                mensagem.NETWORK_CREDENTIAL = net;

                // Envia mensagem
                try
                {
                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                }
                catch (Exception ex)
                {
                    String erro = ex.Message;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FORMULARIO_RESPOSTA item, FORMULARIO_RESPOSTA itemAntes, USUARIO usuario)
        {
            try
            {
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FORMULARIO_RESPOSTA item, FORMULARIO_RESPOSTA itemAntes)
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

        public Int32 ValidateDelete(FORMULARIO_RESPOSTA item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.FORE_IN_ATIVO = 0;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FORMULARIO_RESPOSTA item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.FORE_IN_ATIVO = 1;

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
