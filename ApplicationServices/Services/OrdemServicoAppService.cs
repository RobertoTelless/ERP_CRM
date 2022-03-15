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
using System.IO;
using EntitiesServices.Interfaces.Services;

namespace ApplicationServices.Services
{
    public class OrdemServicoAppService : AppServiceBase<ORDEM_SERVICO>, IOrdemServicoAppService
    {
        private readonly IOrdemServicoService _baseService;
        private readonly IOrdemServicoAcompanhamentoService _acomService;
        private readonly IOrdemServicoComentarioService _commentService;
        private readonly ITemplateAppService _usuService;
        private readonly IClienteService _cliService;
        private readonly IConfiguracaoService _confService;

        public OrdemServicoAppService(IOrdemServicoService baseService, IOrdemServicoAcompanhamentoService acomService, IOrdemServicoComentarioService commentService, ITemplateAppService usuService, IClienteService cliService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _acomService = acomService;
            _commentService = commentService;
            _usuService = usuService;
            _cliService = cliService;
            _confService = confService;
        }

        public ORDEM_SERVICO CheckExist(ORDEM_SERVICO conta, Int32 idAss)
        {
            return _baseService.CheckExist(conta, idAss);
        }

        public ORDEM_SERVICO GetItemById(Int32 id)
        {
            return _baseService.GetItemById(id);
        }
        public List<ORDEM_SERVICO> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public List<ORDEM_SERVICO> GetAllItensAdm(Int32 idAss)
        {
            return _baseService.GetAllItensAdm(idAss);
        }

        public ORDEM_SERVICO_ANEXO GetAnexoById(Int32 id)
        {
            ORDEM_SERVICO_ANEXO item = _baseService.GetAnexoById(id);
            return item;
        }

        public Int32 ExecuteFilter(Int32? catOS, Int32? idClie, Int32? idUsu, DateTime? dtCriacao, Int32? status, Int32? idDept, Int32? idServ, Int32? idProd, Int32? idAten, Int32? idAss, out List<ORDEM_SERVICO> objeto)
        {
            try
            {
                objeto = new List<ORDEM_SERVICO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catOS, idClie, idUsu, dtCriacao, status, idDept, idServ, idProd, idAten, idAss);
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

        public Int32 ValidateCreate(ORDEM_SERVICO item, USUARIO usuario)
        {
            try
            {
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ATENDIMENTO != null)
                {
                    item.ATENDIMENTO = null;
                }

                item.ORSE_IN_ATIVO = 1;

                Int32 volta = _baseService.Create(item);
                item.ORSE_NR_NUMERO = item.ORSE_CD_ID.ToString();

                // Recupera template e-mail
                String header = _usuService.GetByCode("OSCRIACAO").TEMP_TX_CABECALHO;
                String body = _usuService.GetByCode("OSCRIACAO").TEMP_TX_CORPO;
                String footer = _usuService.GetByCode("OSCRIACAO").TEMP_TX_DADOS;

                // Prepara corpo do e-mail  
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                String frase = String.Empty;
                body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
                body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
                body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
                body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
                body = body.Replace("{frase}", "");
                header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

                // Concatena
                String emailBody = header + body;
                CONFIGURACAO conf = _confService.GetItemById(1);

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                Email mensagem = new Email();
                mensagem.ASSUNTO = "Ordem Serviço - Cliente";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
                mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.NETWORK_CREDENTIAL = net;

                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(ORDEM_SERVICO item, ORDEM_SERVICO itemAntes, USUARIO usuario)
        {
            if (item.ATENDIMENTO != null)
            { 
                item.ATENDIMENTO = null; 
            }
            if (item.ASSINANTE != null)
            { 
                item.ASSINANTE = null; 
            }
            if (item.CATEGORIA_ORDEM_SERVICO != null)
            {
                item.CATEGORIA_ORDEM_SERVICO = null; 
            }
            if (item.CLIENTE != null)
            {
                item.CLIENTE = null; 
            }
            if (item.DEPARTAMENTO != null)
            {
                item.DEPARTAMENTO = null; 
            }
            if (item.PRODUTO != null)
            {
                item.PRODUTO = null; 
            }
            if (item.SERVICO != null)
            {
                item.SERVICO = null; 
            }
            if (item.USUARIO != null)
            {
                item.USUARIO = null; 
            }
            if (item.ORDEM_SERVICO_ANEXO != null)
            {
                item.ORDEM_SERVICO_ANEXO = null;
            }
            if (item.FILIAL != null)
            {
                item.FILIAL = null;
            }

            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditORSE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ORDEM_SERVICO>(item),
                };

                Int32 volta = _baseService.Edit(item, log);

                if (item.ORSE_IN_STATUS == 3)
                {
                    // Recupera template e-mail
                    String header = _usuService.GetByCode("OSENCERRAR").TEMP_TX_CABECALHO;
                    String body = _usuService.GetByCode("OSENCERRAR").TEMP_TX_CORPO;
                    String footer = _usuService.GetByCode("OSENCERRAR").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                    String frase = String.Empty;
                    body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
                    body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
                    body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
                    body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
                    body = body.Replace("{frase}", "");
                    header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(1);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Ordem Serviço - Cliente";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;

                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                }
                if (item.ORSE_IN_STATUS == 5 || item.ORSE_IN_STATUS == 6)
                {
                    // Recupera template e-mail
                    String header = _usuService.GetByCode("OSENVAPROV").TEMP_TX_CABECALHO;
                    String body = _usuService.GetByCode("OSENVAPROV").TEMP_TX_CORPO;
                    String footer = _usuService.GetByCode("OSENVAPROV").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                    String frase = String.Empty;
                    body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
                    body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
                    body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
                    body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
                    body = body.Replace("{frase}", "");
                    header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(1);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Ordem Serviço - Cliente";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;

                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                }
                if (item.ORSE_IN_STATUS == 6)
                {
                    // Recupera template e-mail
                    String header = _usuService.GetByCode("OSAPROV").TEMP_TX_CABECALHO;
                    String body = _usuService.GetByCode("OSAPROV").TEMP_TX_CORPO;
                    String footer = _usuService.GetByCode("OSAPROV").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                    String frase = String.Empty;
                    body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
                    body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
                    body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
                    body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
                    body = body.Replace("{frase}", "");
                    header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(1);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Ordem Serviço - Cliente";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;

                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                }
                if (item.ORSE_IN_STATUS == 7)
                {
                    // Recupera template e-mail
                    String header = _usuService.GetByCode("OSRECUSA").TEMP_TX_CABECALHO;
                    String body = _usuService.GetByCode("OSRECUSA").TEMP_TX_CORPO;
                    String footer = _usuService.GetByCode("OSRECUSA").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                    String frase = String.Empty;
                    body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
                    body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
                    body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
                    body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
                    body = body.Replace("{frase}", "");
                    header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(1);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Ordem Serviço - Cliente";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;

                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                }

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public Int32 ValidateEdit(ORDEM_SERVICO item, ORDEM_SERVICO itemAntes)
        //{
        //    if (item.ATENDIMENTO != null)
        //    {
        //        item.ATENDIMENTO = null;
        //    }
        //    if (item.ASSINANTE != null)
        //    {
        //        item.ASSINANTE = null;
        //    }
        //    if (item.CATEGORIA_ORDEM_SERVICO != null)
        //    {
        //        item.CATEGORIA_ORDEM_SERVICO = null;
        //    }
        //    if (item.CLIENTE != null)
        //    {
        //        item.CLIENTE = null;
        //    }
        //    if (item.DEPARTAMENTO != null)
        //    {
        //        item.DEPARTAMENTO = null;
        //    }
        //    if (item.PRODUTO != null)
        //    {
        //        item.PRODUTO = null;
        //    }
        //    if (item.SERVICO != null)
        //    {
        //        item.SERVICO = null;
        //    }
        //    if (item.USUARIO != null)
        //    {
        //        item.USUARIO = null;
        //    }
        //    if (item.FILIAL != null)
        //    {
        //        item.FILIAL = null;
        //    }

        //    try
        //    {
        //        Int32 volta = _baseService.Edit(item);

        //        if (item.ORSE_IN_STATUS == 3)
        //        {
        //            // Recupera template e-mail
        //            String header = _usuService.GetByCode("OSENCERRAR").TEMP_TX_CABECALHO;
        //            String body = _usuService.GetByCode("OSENCERRAR").TEMP_TX_CORPO;
        //            String footer = _usuService.GetByCode("OSENCERRAR").TEMP_TX_DADOS;

        //            // Prepara corpo do e-mail  
        //            CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
        //            String frase = String.Empty;
        //            body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
        //            body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
        //            body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
        //            body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
        //            body = body.Replace("{frase}", "");
        //            header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

        //            // Concatena
        //            String emailBody = header + body;
        //            CONFIGURACAO conf = _confService.GetItemById(1);

        //            // Monta e-mail
        //            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
        //            Email mensagem = new Email();
        //            mensagem.ASSUNTO = "Ordem Serviço - Cliente";
        //            mensagem.CORPO = emailBody;
        //            mensagem.DEFAULT_CREDENTIALS = false;
        //            mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
        //            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
        //            mensagem.ENABLE_SSL = true;
        //            mensagem.NOME_EMISSOR = usu.USUA_NM_NOME;
        //            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
        //            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
        //            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
        //            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
        //            mensagem.NETWORK_CREDENTIAL = net;

        //            Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
        //        }
        //        if (item.ORSE_IN_STATUS == 5 || item.ORSE_IN_STATUS == 6)
        //        {
        //            // Recupera template e-mail
        //            String header = _usuService.GetByCode("OSENVAPROV").TEMP_TX_CABECALHO;
        //            String body = _usuService.GetByCode("OSENVAPROV").TEMP_TX_CORPO;
        //            String footer = _usuService.GetByCode("OSENVAPROV").TEMP_TX_DADOS;

        //            // Prepara corpo do e-mail  
        //            CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
        //            String frase = String.Empty;
        //            body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
        //            body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
        //            body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
        //            body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
        //            body = body.Replace("{frase}", "");
        //            header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

        //            // Concatena
        //            String emailBody = header + body;
        //            CONFIGURACAO conf = _confService.GetItemById(1);

        //            // Monta e-mail
        //            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
        //            Email mensagem = new Email();
        //            mensagem.ASSUNTO = "Ordem Serviço - Cliente";
        //            mensagem.CORPO = emailBody;
        //            mensagem.DEFAULT_CREDENTIALS = false;
        //            mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
        //            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
        //            mensagem.ENABLE_SSL = true;
        //            mensagem.NOME_EMISSOR = SessionMocks.UserCredentials.USUA_NM_NOME;
        //            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
        //            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
        //            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
        //            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
        //            mensagem.NETWORK_CREDENTIAL = net;

        //            Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
        //        }
        //        if (item.ORSE_IN_STATUS == 6)
        //        {
        //            // Recupera template e-mail
        //            String header = _usuService.GetByCode("OSAPROV").TEMP_TX_CABECALHO;
        //            String body = _usuService.GetByCode("OSAPROV").TEMP_TX_CORPO;
        //            String footer = _usuService.GetByCode("OSAPROV").TEMP_TX_DADOS;

        //            // Prepara corpo do e-mail  
        //            CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
        //            String frase = String.Empty;
        //            body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
        //            body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
        //            body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
        //            body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
        //            body = body.Replace("{frase}", "");
        //            header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

        //            // Concatena
        //            String emailBody = header + body;
        //            CONFIGURACAO conf = _confService.GetItemById(1);

        //            // Monta e-mail
        //            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
        //            Email mensagem = new Email();
        //            mensagem.ASSUNTO = "Ordem Serviço - Cliente";
        //            mensagem.CORPO = emailBody;
        //            mensagem.DEFAULT_CREDENTIALS = false;
        //            mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
        //            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
        //            mensagem.ENABLE_SSL = true;
        //            mensagem.NOME_EMISSOR = SessionMocks.UserCredentials.USUA_NM_NOME;
        //            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
        //            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
        //            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
        //            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
        //            mensagem.NETWORK_CREDENTIAL = net;

        //            Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
        //        }
        //        if (item.ORSE_IN_STATUS == 7)
        //        {
        //            // Recupera template e-mail
        //            String header = _usuService.GetByCode("OSRECUSA").TEMP_TX_CABECALHO;
        //            String body = _usuService.GetByCode("OSRECUSA").TEMP_TX_CORPO;
        //            String footer = _usuService.GetByCode("OSRECUSA").TEMP_TX_DADOS;

        //            // Prepara corpo do e-mail  
        //            CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
        //            String frase = String.Empty;
        //            body = body.Replace("{numero}", item.ORSE_NR_NUMERO.ToString());
        //            body = body.Replace("{data}", item.ORSE_DT_INICIO.Value.ToShortDateString());
        //            body = body.Replace("{Assunto}", item.ORSE_DS_DESCRICAO);
        //            body = body.Replace("{DataPrevista}", item.ORSE_DT_PREVISTA.Value.ToShortDateString());
        //            body = body.Replace("{frase}", "");
        //            header = header.Replace("{Nome}", cli.CLIE_NM_NOME);

        //            // Concatena
        //            String emailBody = header + body;
        //            CONFIGURACAO conf = _confService.GetItemById(1);

        //            // Monta e-mail
        //            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
        //            Email mensagem = new Email();
        //            mensagem.ASSUNTO = "Ordem Serviço - Cliente";
        //            mensagem.CORPO = emailBody;
        //            mensagem.DEFAULT_CREDENTIALS = false;
        //            mensagem.EMAIL_DESTINO = cli.CLIE_NM_EMAIL;
        //            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
        //            mensagem.ENABLE_SSL = true;
        //            mensagem.NOME_EMISSOR = SessionMocks.UserCredentials.USUA_NM_NOME;
        //            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
        //            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
        //            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
        //            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
        //            mensagem.NETWORK_CREDENTIAL = net;

        //            Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
        //        }

        //        return volta;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public Int32 ValidateDelete(ORDEM_SERVICO item, USUARIO usuario)
        {
            try
            {
                if (item.ATENDIMENTO != null)
                {
                    item.ATENDIMENTO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CATEGORIA_ORDEM_SERVICO != null)
                {
                    item.CATEGORIA_ORDEM_SERVICO = null;
                }
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.DEPARTAMENTO != null)
                {
                    item.DEPARTAMENTO = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }
                if (item.SERVICO != null)
                {
                    item.SERVICO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ORDEM_SERVICO_ANEXO != null)
                {
                    item.ORDEM_SERVICO_ANEXO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }

                // Acerta campos
                item.ORSE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelORSE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ORDEM_SERVICO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(ORDEM_SERVICO item, USUARIO usuario)
        {
            try
            {
                if (item.ATENDIMENTO != null)
                {
                    item.ATENDIMENTO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CATEGORIA_ORDEM_SERVICO != null)
                {
                    item.CATEGORIA_ORDEM_SERVICO = null;
                }
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.DEPARTAMENTO != null)
                {
                    item.DEPARTAMENTO = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }
                if (item.SERVICO != null)
                {
                    item.SERVICO = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ORDEM_SERVICO_ANEXO != null)
                {
                    item.ORDEM_SERVICO_ANEXO = null;
                }
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }

                // Acerta campos
                item.ORSE_IN_ATIVO = 1;
                item.ORSE_IN_STATUS = 1;

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatORSE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ORDEM_SERVICO>(item)
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAcompanhamento(ORDEM_SERVICO_ACOMPANHAMENTO item)
        {
            try
            {
                item.ORSA_IN_ATIVO = 1;

                Int32 volta = _acomService.Create(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ORDEM_SERVICO_ACOMPANHAMENTO> GetAcompanhamentoByOs(ORDEM_SERVICO item)
        {
            return _acomService.GetByOs(item);
        }

        public Int32 ValidateCreateComentario(ORDEM_SERVICO_COMENTARIOS item)
        {
            try
            {
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }

                Int32 volta = _commentService.Create(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ORDEM_SERVICO_COMENTARIOS> GetComentarioByOs(ORDEM_SERVICO item)
        {
            return _commentService.GetByOs(item);
        }
    }
}
