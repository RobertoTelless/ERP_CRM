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
    public class ContaPagarParcelaAppService : AppServiceBase<CONTA_PAGAR_PARCELA>, IContaPagarParcelaAppService
    {
        private readonly IContaPagarParcelaService _baseService;
        private readonly IContaBancariaService _cbService;
        private readonly INotificacaoService _notiService;
        private readonly IFormaPagamentoAppService _fpService;
        private readonly IContaPagarService _cpService;
        private readonly IUsuarioService _usuService;

        public ContaPagarParcelaAppService(IContaPagarParcelaService baseService, IContaBancariaService cbService, INotificacaoService notiService, IFormaPagamentoAppService fpService, IContaPagarService cpService, IUsuarioService usuService): base(baseService)
        {
            _baseService = baseService;
            _cbService = cbService;
            _notiService = notiService;
            _fpService = fpService;
            _cpService = cpService;
            _usuService = usuService;
        }

        public CONTA_PAGAR_PARCELA GetItemById(Int32 id)
        {
            CONTA_PAGAR_PARCELA item = _baseService.GetItemById(id);
            return item;
        }

        public List<CONTA_PAGAR_PARCELA> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public Int32 ValidateCreate(CONTA_PAGAR_PARCELA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CPPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCPPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_PAGAR_PARCELA>(item)
                };

                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONTA_PAGAR_PARCELA item, CONTA_PAGAR_PARCELA itemAntes, USUARIO usuario)
        {
            try
            {
                //  ****** Se for Liquidação
                if (item.CPPA_IN_QUITADA == 0 & item.CPPA_DT_QUITACAO != null)
                {
                    // Checa data
                    if (item.CPPA_DT_QUITACAO > DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Valor
                    Decimal soma = item.CPPA_VL_VALOR.Value + item.CPPA_VL_TAXAS.Value + item.CPPA_VL_JUROS.Value - item.CPPA_VL_DESCONTO.Value;
                    if (soma != item.CPPA_VL_VALOR_PAGO)
                    {
                        return 2;
                    }

                    // Acerta objeto
                    item.CPPA_IN_ATIVO = 1;
                    item.CPPA_IN_QUITADA = 1;

                    // Monta lançamento bancário
                    CONTA_PAGAR cp = _cpService.GetItemById(item.CAPA_CD_ID);
                    FORMA_PAGAMENTO forma = _fpService.GetItemById(cp.FOPA_CD_ID.Value);
                    CONTA_BANCO conta = _cbService.GetItemById(forma.COBA_CD_ID.Value);
                    conta.COBA_VL_SALDO_ATUAL -= item.CPPA_VL_VALOR_PAGO;
                    CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                    lanc.COBA_CD_ID = forma.COBA_CD_ID.Value;
                    lanc.CBLA_DS_DESCRICAO = item.CPPA_DS_DESCRICAO;
                    lanc.CBLA_DT_LANCAMENTO = item.CPPA_DT_QUITACAO.Value;
                    lanc.CBLA_IN_ATIVO = 1;
                    lanc.CBLA_IN_ORIGEM = 0;
                    lanc.CBLA_IN_TIPO = 2;
                    lanc.CBLA_NR_NUMERO = item.CPPA_NR_PARCELA;
                    lanc.CBLA_VL_VALOR = item.CPPA_VL_VALOR_PAGO.Value;
                    conta.CONTA_BANCO_LANCAMENTO.Add(lanc);

                    // Gera Notificação
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Contas a Pagar - Liquidação de Parcela";
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_TX_TEXTO = "A parcela " + item.CPPA_NR_PARCELA + " do lançamento " + item.CONTA_PAGAR.CAPA_DS_DESCRICAO + " foi liquidada em " + DateTime.Today.Date.ToLongDateString();
                    noti.USUA_CD_ID = usuario.USUA_CD_ID;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.CANO_CD_ID = 1;
                    noti.NOTI_IN_STATUS = 0;

                    // Envia notificação
                    Int32 volta = _notiService.Create(noti);

                    // Persiste lancamento Bancario
                    volta = _cbService.Edit(conta);

                    // Persiste Lancamento Parcela CR
                    item.CONTA_PAGAR = null;
                    volta = _baseService.Edit(item);

                    // Recupera template e-mail
                    String header = _usuService.GetTemplate("PAGCPAG").TEMP_TX_CABECALHO;
                    String body = _usuService.GetTemplate("PAGCPAG").TEMP_TX_CORPO;
                    String footer = _usuService.GetTemplate("PAGCPAG").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    header = header.Replace("{Nome}", item.CONTA_PAGAR.USUARIO.USUA_NM_NOME);
                    body = body.Replace("{Emissor}", usuario.ASSINANTE.ASSI_NM_NOME);
                    footer = footer.Replace("{DataLanc}", item.CONTA_PAGAR.CAPA_DT_LANCAMENTO.Value.ToLongDateString());
                    footer = footer.Replace("{Valor}", item.CPPA_VL_VALOR.ToString());
                    footer = footer.Replace("{DataVenc}", item.CPPA_DT_VENCIMENTO.Value.ToLongDateString());
                    footer = footer.Replace("{DataPag}", item.CPPA_DT_QUITACAO.Value.ToLongDateString());
                    footer = footer.Replace("{Desc}", item.CPPA_DS_DESCRICAO);
                    footer = footer.Replace("{Numero}", item.CAPA_CD_ID.ToString());

                    // Concatena
                    String emailBody = header + body + footer;

                    // Monta e-mail
                    CONFIGURACAO conf = _cpService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Pagamento de Lançamento - Conta a Pagar";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = item.CONTA_PAGAR.USUARIO.USUA_NM_EMAIL;
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

                    // Monta token
                    String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                    byte[] textBytes = Encoding.UTF8.GetBytes(text);
                    String token = Convert.ToBase64String(textBytes);
                    String auth = "Basic " + token;

                    // Prepara texto
                    String texto = _usuService.GetTemplate("SMSCPAG").TEMP_TX_CORPO; ;
                    texto = texto.Replace("{Nome}", item.CONTA_PAGAR.USUARIO.USUA_NM_NOME);
                    texto = texto.Replace("{Numero}", item.CAPA_CD_ID.ToString());
                    texto = texto.Replace("{Emissor}", usuario.ASSINANTE.ASSI_NM_NOME);
                    String smsBody = texto;
                    String erro = null;

                    // inicia processo
                    String resposta = String.Empty;

                    // Monta destinatarios
                    try
                    {
                        String listaDest = "55" + Regex.Replace(item.CONTA_PAGAR.USUARIO.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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

                    // Acerta saldo
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CONTA_PAGAR_PARCELA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CPPA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCPPC",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_PAGAR_PARCELA>(item)
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CONTA_PAGAR_PARCELA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CPPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCPPC",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONTA_PAGAR_PARCELA>(item)
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
