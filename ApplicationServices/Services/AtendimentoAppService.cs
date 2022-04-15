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

namespace ApplicationServices.Services
{
    public class AtendimentoAppService : AppServiceBase<ATENDIMENTO>, IAtendimentoAppService
    {
        private readonly IAtendimentoService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly IAgendaAppService _ageService;
        private readonly ITemplateAppService _usuService;
        private readonly IConfiguracaoService _confService;
        private readonly IClienteService _cliService;
        private readonly IUsuarioService _usuaService;
        private readonly IServicoService _serService;

        public AtendimentoAppService(IAtendimentoService baseService, INotificacaoService notiService, IAgendaAppService ageService, ITemplateAppService usuService, IConfiguracaoService confService, IClienteService cliService, IUsuarioService usuaService, IServicoService serService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _ageService = ageService;
            _usuService = usuService;
            _confService = confService;
            _cliService = cliService;
            _usuaService = usuaService;
            _serService = serService;
        }
         
        public List<ATENDIMENTO> GetAllItens(Int32 idAss)
        {
            List<ATENDIMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<ATENDIMENTO> GetAllItensAdm(Int32 idAss)
        {
            List<ATENDIMENTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public ATENDIMENTO_ANEXO GetAnexoById(Int32 id)
        {
            ATENDIMENTO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public ATENDIMENTO GetItemById(Int32 id)
        {
            ATENDIMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public List<ATENDIMENTO> GetByCliente(Int32 id, Int32 idAss)
        {
            List<ATENDIMENTO> lista = _baseService.GetByCliente(id, idAss);
            return lista;
        }

        public List<CATEGORIA_ATENDIMENTO> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_ATENDIMENTO> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }
        
        public ATENDIMENTO CheckExist(ATENDIMENTO conta, Int32 idAss)
        {
            ATENDIMENTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ExecuteFilter(Int32? idCat, Int32? cliente, Int32? produto, DateTime? data, Int32? status, String descricao, Int32? depto, Int32? prioridade, Int32? idUsua, Int32? idServico, Int32? sla, Int32 idAss, out List<ATENDIMENTO> objeto)
        {
            try
            {
                objeto = new List<ATENDIMENTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(idCat, cliente, produto, data, status, descricao, depto, prioridade, idUsua, idServico, sla, idAss);
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

        public Int32 ValidateCreate(ATENDIMENTO item, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.USUA_CD_ID == null & item.DEPT_CD_ID == null)
                {
                    return 2;
                }
                if (item.ATEN_DT_PREVISTA < item.ATEN_DT_INICIO)
                {
                    return 3;
                }
                
                // Completa objeto
                item.ATEN_IN_ATIVO = 1;
                item.ATEN_IN_STATUS = 1;
                item.ATEN_IN_DESTINO = usuario.USUA_CD_ID;
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddATEN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ATENDIMENTO>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);

                if (item.USUA_CD_ID != null)
                {
                    // Gera Notificação
                    NOTIFICACAO noti2 = new NOTIFICACAO();
                    noti2.NOTI_DT_EMISSAO = DateTime.Now;
                    noti2.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti2.NOTI_IN_NIVEL = 1;
                    noti2.NOTI_IN_VISTA = 0;
                    noti2.NOTI_NM_TITULO = "Atendimento - Atribuição";
                    noti2.NOTI_IN_ATIVO = 1;
                    noti2.NOTI_TX_TEXTO = "O atendimento " + item.ATEN_CD_ID.ToString() + " foi atribuído a você em " + DateTime.Today.Date.ToLongDateString() + " e está sob sua responsabilidade. Assunto: " + item.ATEN_NM_ASSUNTO;
                    noti2.USUA_CD_ID = item.USUA_CD_ID.Value;
                    noti2.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti2.CANO_CD_ID = 1;
                    noti2.NOTI_IN_STATUS = 0;
                    noti2.ATEN_CD_ID = item.ATEN_CD_ID;

                    // Persiste notificação
                    Int32 volta2 = _notiService.Create(noti2);

                    // Gera Agenda
                    CLIENTE clie = _cliService.GetById(item.CLIE_CD_ID);
                    AGENDA age = new AGENDA();
                    age.AGEN_CD_USUARIO = usuario.USUA_CD_ID;
                    age.AGEN_DS_DESCRICAO = "Atendimento " + item.ATEN_CD_ID.ToString() + ". Assunto: " + item.ATEN_NM_ASSUNTO;
                    age.AGEN_DT_DATA = item.ATEN_DT_PREVISTA.Value;
                    age.AGEN_HR_HORA = TimeSpan.Parse("16:00");
                    age.AGEN_IN_ATIVO = 1;
                    age.AGEN_IN_STATUS = 1;
                    age.AGEN_NM_TITULO = "Atendimento " + item.ATEN_CD_ID.ToString() + " - " + clie.CLIE_NM_NOME;
                    age.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    age.CAAG_CD_ID = 4;
                    age.USUA_CD_ID = usuario.USUA_CD_ID;

                    // Persiste agenda
                    Int32 volta1 = _ageService.ValidateCreate(age, usuario);

                    // Vincula Agenda   
                    ATENDIMENTO_AGENDA ag = new ATENDIMENTO_AGENDA();
                    ag.AGEN_CD_ID = age.AGEN_CD_ID;
                    ag.ATEN_CD_ID = item.ATEN_CD_ID;
                    ag.ATAG_IN_ATIVO = 1;

                    ATENDIMENTO itemEdit = _baseService.GetItemById(item.ATEN_CD_ID);
                    itemEdit.ATENDIMENTO_AGENDA.Add(ag);
                    Int32 volta5 = _baseService.Edit(itemEdit);

                    // Recupera template e-mail
                    String header = _usuService.GetByCode("ATENCLI").TEMP_TX_CABECALHO;
                    String body = _usuService.GetByCode("ATENCLI").TEMP_TX_CORPO;
                    String footer = _usuService.GetByCode("ATENCLI").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    String frase = String.Empty;
                    body = body.Replace("{numero}", item.ATEN_CD_ID.ToString());
                    body = body.Replace("{data}", item.ATEN_DT_INICIO.Value.ToShortDateString());
                    body = body.Replace("{Assunto}", item.ATEN_NM_ASSUNTO);
                    body = body.Replace("{DataPrevista}", item.ATEN_DT_PREVISTA.Value.ToShortDateString());
                    body = body.Replace("{frase}", "");
                    header = header.Replace("{NomeCliente}", cli.CLIE_NM_NOME);

                    // Concatena
                    String emailBody = header + body;

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Atendimento - Cliente";
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
                    mensagem.IS_HTML = true;
                    mensagem.NETWORK_CREDENTIAL = net;

                    // Envia mensagem
                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);

                    // Envia SMS Cliente
                    String voltaSMS = ValidateCreateMensagem(clie, usuario, item, usuario.ASSI_CD_ID);

                    if (usuario.USUA_NM_EMAIL != null)
                    {
                        String headerUsu = _usuService.GetByCode("ATENUSU").TEMP_TX_CABECALHO;
                        String bodyUsu = _usuService.GetByCode("ATENUSU").TEMP_TX_CORPO;

                        bodyUsu = bodyUsu.Replace("{numero}", item.ATEN_CD_ID.ToString());
                        bodyUsu = bodyUsu.Replace("{data}", item.ATEN_DT_INICIO.Value.ToShortDateString());
                        bodyUsu = bodyUsu.Replace("{NomeCliente}", cli.CLIE_NM_NOME);
                        bodyUsu = bodyUsu.Replace("{DataPrevista}", item.ATEN_DT_PREVISTA.Value.ToShortDateString());
                        bodyUsu = bodyUsu.Replace("{Assunto}", item.ATEN_NM_ASSUNTO);
                        headerUsu = headerUsu.Replace("{NomeUsuario}", usuario.USUA_NM_NOME);

                        Email mensagemUsuario = new Email();
                        mensagemUsuario.ASSUNTO = "Atendimento - Cliente";
                        mensagemUsuario.CORPO = headerUsu + bodyUsu;
                        mensagemUsuario.DEFAULT_CREDENTIALS = false;
                        mensagemUsuario.EMAIL_DESTINO = usuario.USUA_NM_EMAIL;
                        mensagemUsuario.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagemUsuario.ENABLE_SSL = true;
                        mensagemUsuario.NOME_EMISSOR = usuario.USUA_NM_NOME;
                        mensagemUsuario.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagemUsuario.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagemUsuario.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagemUsuario.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagemUsuario.IS_HTML = true;
                        mensagemUsuario.NETWORK_CREDENTIAL = net;
                        Int32 voltaMailUser = CommunicationPackage.SendEmail(mensagemUsuario);
                    }
                    return volta;
                }

                if (item.DEPT_CD_ID != null)
                {
                    List<USUARIO> lista = _usuaService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.DEPT_CD_ID == item.DEPT_CD_ID).ToList();
                    foreach (USUARIO usu in lista)
                    {
                        // Gera Notificação
                        NOTIFICACAO noti2 = new NOTIFICACAO();
                        noti2.NOTI_DT_EMISSAO = DateTime.Now;
                        noti2.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                        noti2.NOTI_IN_NIVEL = 1;
                        noti2.NOTI_IN_VISTA = 0;
                        noti2.NOTI_NM_TITULO = "Atendimento - Atribuição";
                        noti2.NOTI_IN_ATIVO = 1;
                        noti2.NOTI_TX_TEXTO = "O atendimento " + item.ATEN_CD_ID.ToString() + " foi atribuído ao seu departamento em " + DateTime.Today.Date.ToLongDateString() + " Você poderá assumi-lo. Assunto: " + item.ATEN_NM_ASSUNTO;
                        noti2.USUA_CD_ID = usu.USUA_CD_ID;
                        noti2.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        noti2.CANO_CD_ID = 1;
                        noti2.NOTI_IN_STATUS = 0;
                        noti2.ATEN_CD_ID = item.ATEN_CD_ID;

                        // Persiste notificação
                        Int32 volta2 = _notiService.Create(noti2);

                        if (usu.USUA_NM_EMAIL != null)
                        {
                            String headerUsu = _usuService.GetByCode("ATENUSU").TEMP_TX_CABECALHO;
                            String bodyUsu = _usuService.GetByCode("ATENUSU").TEMP_TX_CORPO;

                            bodyUsu = bodyUsu.Replace("{numero}", item.ATEN_CD_ID.ToString());
                            bodyUsu = bodyUsu.Replace("{data}", item.ATEN_DT_INICIO.Value.ToShortDateString());
                            bodyUsu = bodyUsu.Replace("{NomeCliente}", cli.CLIE_NM_NOME);
                            bodyUsu = bodyUsu.Replace("{DataPrevista}", item.ATEN_DT_PREVISTA.Value.ToShortDateString());
                            bodyUsu = bodyUsu.Replace("{Assunto}", item.ATEN_NM_ASSUNTO);
                            headerUsu = headerUsu.Replace("{NomeUsuario}", usu.USUA_NM_NOME);

                            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                            Email mensagemUsuario = new Email();
                            mensagemUsuario.ASSUNTO = "Atendimento - Cliente";
                            mensagemUsuario.CORPO = headerUsu + bodyUsu;
                            mensagemUsuario.DEFAULT_CREDENTIALS = false;
                            mensagemUsuario.EMAIL_DESTINO = usuario.USUA_NM_EMAIL;
                            mensagemUsuario.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                            mensagemUsuario.ENABLE_SSL = true;
                            mensagemUsuario.NOME_EMISSOR = usuario.USUA_NM_NOME;
                            mensagemUsuario.PORTA = conf.CONF_NM_PORTA_SMTP;
                            mensagemUsuario.PRIORIDADE = System.Net.Mail.MailPriority.High;
                            mensagemUsuario.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                            mensagemUsuario.SMTP = conf.CONF_NM_HOST_SMTP;
                            mensagemUsuario.IS_HTML = true;
                            mensagemUsuario.NETWORK_CREDENTIAL = net;
                            Int32 voltaMailUser = CommunicationPackage.SendEmail(mensagemUsuario);
                        }
                    }
                    return volta;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public String ValidateCreateMensagem(CLIENTE item, USUARIO usuario, ATENDIMENTO aten, Int32? idAss)
        {
            try
            {
                // Verifica existencia prévia
                if (item == null)
                {
                    return "1";
                }

                // Criticas
                if (item.CLIE_NR_CELULAR == null)
                {
                    return "2";
                }

                // Monta token
                CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Monta routing
                String routing = "1";

                // Monta texto
                String texto = _usuaService.GetTemplate("ATENSMSCLI").TEMP_TX_CORPO;
                texto = texto.Replace("{numero}", aten.ATEN_CD_ID.ToString());
                texto = texto.Replace("{data}", aten.ATEN_DT_INICIO.Value.ToShortDateString());
                texto = texto.Replace("{NomeCliente}", item.CLIE_NM_NOME);
                texto = texto.Replace("{DataPrevista}", aten.ATEN_DT_PREVISTA.Value.ToShortDateString());
                texto = texto.Replace("{Assunto}", aten.ATEN_NM_ASSUNTO);
                String smsBody = texto;
                String erro = null;

                // inicia processo
                String resposta = String.Empty;

                // Monta destinatarios
                try
                {
                    String listaDest = "55" + Regex.Replace(item.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    String customId = Cryptography.GenerateRandomPassword(8);
                    String data = String.Empty;
                    String json = String.Empty;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                        streamWriter.Write(json);
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        resposta = result;
                    }
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                }
                return "0";
            }
            catch (Exception ex)
            {
                return "3";
            }
        }

        public Int32 ValidateEdit(ATENDIMENTO item, ATENDIMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.USUA_CD_ID == null & item.DEPT_CD_ID == null)
                {
                    return 2;
                }
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CATEGORIA_ATENDIMENTO != null)
                {
                    item.CATEGORIA_ATENDIMENTO = null;
                }
                if (item.DEPARTAMENTO != null)
                {
                    item.DEPARTAMENTO = null;
                }
                if (item.PEDIDO_VENDA != null)
                {
                    item.PEDIDO_VENDA = null;
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
                if (item.USUARIO1 != null)
                {
                    item.USUARIO1 = null;
                }

                // Monta Log

                // Persiste
                Int32 volta = _baseService.Edit(item);

                if (item.ATEN_IN_STATUS == 3)
                {
                    // Recupera template e-mail
                    String header = _usuService.GetByCode("ATENCANC").TEMP_TX_CABECALHO;
                    String body = _usuService.GetByCode("ATENCANC").TEMP_TX_CORPO;
                    String footer = _usuService.GetByCode("ATENCANC").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                    String frase = String.Empty;
                    body = body.Replace("{numero}", item.ATEN_CD_ID.ToString());
                    body = body.Replace("{data}", item.ATEN_DT_CANCELAMENTO.Value.ToShortDateString());
                    body = body.Replace("{Assunto}", item.ATEN_NM_ASSUNTO);
                    body = body.Replace("{Cancelamento}", item.ATEN_DS_CANCELAMENTO);
                    header = header.Replace("{NomeCliente}", cli.CLIE_NM_NOME);

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Atendimento - Cancelamento";
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
                    mensagem.IS_HTML = true;
                    mensagem.NETWORK_CREDENTIAL = net;

                    // Envia mensagem
                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                }
                if (item.ATEN_IN_STATUS == 5)
                {
                    // Recupera template e-mail
                    String header = _usuService.GetByCode("ATENENC").TEMP_TX_CABECALHO;
                    String body = _usuService.GetByCode("ATENENC").TEMP_TX_CORPO;
                    String footer = _usuService.GetByCode("ATENENC").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    if (item.CLIE_CD_ID != null)
                    {
                        CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID.Value);
                        String frase = String.Empty;
                        body = body.Replace("{numero}", item.ATEN_CD_ID.ToString());
                        body = body.Replace("{data}", item.ATEN_DT_ENCERRAMENTO.Value.ToShortDateString());
                        body = body.Replace("{Assunto}", item.ATEN_NM_ASSUNTO);
                        body = body.Replace("{Encerrameento}", item.ATEN_DS_ENCERRAMENTO);
                        header = header.Replace("{NomeCliente}", cli.CLIE_NM_NOME);

                        // Concatena
                        String emailBody = header + body;
                        CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                        // Monta e-mail
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = "Atendimento - Encerramento";
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
                        mensagem.IS_HTML = true;
                        mensagem.NETWORK_CREDENTIAL = net;

                        // Envia mensagem
                        Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                    }
                }

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(ATENDIMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ORDEM_SERVICO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.ATEN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelATEN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ATENDIMENTO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(ATENDIMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.ATEN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatATEN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ATENDIMENTO>(item)
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
