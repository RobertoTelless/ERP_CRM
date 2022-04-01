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
using System.Net;
using EntitiesServices.WorkClasses;
using System.Net.Mail;
using System.IO;

namespace ApplicationServices.Services
{
    public class CRMComercialAppService : AppServiceBase<CRM_COMERCIAL>, ICRMComercialAppService
    {
        private readonly ICRMComercialService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly IClienteService _cliService;
        private readonly IConfiguracaoService _confService;
        private readonly ITemplateService _tempService;
        private readonly IUsuarioService _usuService;
        private readonly IMovimentoEstoqueProdutoService _movService;
        private readonly IProdutoEstoqueFilialService _pefService;
        private readonly IProdutoTabelaPrecoService _ptpService;

        public CRMComercialAppService(ICRMComercialService baseService, INotificacaoService notiService, IClienteService cliService, IConfiguracaoService confService, ITemplateService tempService, IUsuarioService usuService, IMovimentoEstoqueProdutoService movService, IProdutoEstoqueFilialService pefService, IProdutoTabelaPrecoService ptpService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _cliService = cliService;
            _confService = confService;
            _tempService = tempService;
            _usuService = usuService;
            _movService = movService;
            _pefService = pefService;
            _ptpService = ptpService;
        }

        public List<CRM_COMERCIAL> GetAllItens(Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CRM_COMERCIAL> GetAllItensAdm(Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<CRM_COMERCIAL> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetTarefaStatus(tipo, idAss);
            return lista;
        }

        public List<CRM_COMERCIAL> GetByDate(DateTime data, Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetByDate(data, idAss);
            return lista;
        }

        public List<CRM_COMERCIAL> GetByUser(Int32 user)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetByUser(user);
            return lista;
        }

        public CRM_COMERCIAL GetItemById(Int32 id)
        {
            CRM_COMERCIAL item = _baseService.GetItemById(id);
            return item;
        }

        public USUARIO GetUserById(Int32 id)
        {
            USUARIO item = _baseService.GetUserById(id);
            return item;
        }

        public CRM_COMERCIAL_CONTATO GetContatoById(Int32 id)
        {
            CRM_COMERCIAL_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public CRM_COMERCIAL_ACAO GetAcaoById(Int32 id)
        {
            CRM_COMERCIAL_ACAO lista = _baseService.GetAcaoById(id);
            return lista;
        }

        public CRM_COMERCIAL CheckExist(CRM_COMERCIAL tarefa, Int32 idUsu, Int32 idAss)
        {
            CRM_COMERCIAL item = _baseService.CheckExist(tarefa, idUsu, idAss);
            return item;
        }

        public List<TIPO_CRM> GetAllTipos()
        {
            List<TIPO_CRM> lista = _baseService.GetAllTipos();
            return lista;
        }

        public List<CRM_COMERCIAL_ACAO> GetAllAcoes(Int32 idAss)
        {
            List<CRM_COMERCIAL_ACAO> lista = _baseService.GetAllAcoes(idAss);
            return lista;
        }

        public List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss)
        {
            List<TIPO_ACAO> lista = _baseService.GetAllTipoAcao(idAss);
            return lista;
        }

        public List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss)
        {
            List<MOTIVO_CANCELAMENTO> lista = _baseService.GetAllMotivoCancelamento(idAss);
            return lista;
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss)
        {
            List<MOTIVO_ENCERRAMENTO> lista = _baseService.GetAllMotivoEncerramento(idAss);
            return lista;
        }

        public List<CRM_ORIGEM> GetAllOrigens(Int32 idAss)
        {
            List<CRM_ORIGEM> lista = _baseService.GetAllOrigens(idAss);
            return lista;
        }

        public CRM_COMERCIAL_ANEXO GetAnexoById(Int32 id)
        {
            CRM_COMERCIAL_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public CRM_COMERCIAL_COMENTARIO_NOVA GetComentarioById(Int32 id)
        {
            CRM_COMERCIAL_COMENTARIO_NOVA lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? status, DateTime? inicio, DateTime? prevista, String numero, String nota, Int32? estrela, String nome, String busca, Int32 idAss, out List<CRM_COMERCIAL> objeto)
        {
            try
            {
                objeto = new List<CRM_COMERCIAL>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(status, inicio, prevista, numero, nota, estrela, nome, busca, idAss);
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

        public Int32 ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss, out List<CRM_COMERCIAL> objeto)
        {
            try
            {
                objeto = new List<CRM_COMERCIAL>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterDash(nmr, dtFinal, nome, usu, status, idAss);
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

        public Int32 ValidateCreate(CRM_COMERCIAL item, USUARIO usuario)
        {
            try
            {
                //Verifica Campos
                if (item.TIPO_CRM != null)
                {
                    item.TIPO_CRM = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }

                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.USUA_CD_ID, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.CRMC_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.USUA_CD_ID = usuario.USUA_CD_ID;
                item.CRMC_IN_STATUS = 1;

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRMC_DS_DESCRICAO + "|" + item.CRMC_DT_CRIACAO.ToShortDateString() + "|" + item.CRMC_IN_ATIVO.ToString() + "|" + item.CRMC_IN_STATUS.ToString() + "|" + item.CRMC_NM_NOME;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddCRMC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = serial
                };
                
                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Atribuição de Processo CRM - Comercial";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Processo CRM-Comercial " + item.CRMC_NM_NOME + " do cliente " + cli.CLIE_NM_NOME + " foi colocado sob sua responsabilidade em "  + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(CRM_COMERCIAL item, CRM_COMERCIAL itemAntes, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.CRMC_DT_ENCERRAMENTO != null)
                {
                    if (item.CRMC_DT_ENCERRAMENTO < item.CRMC_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRMC_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRMC_DT_CANCELAMENTO != null)
                {
                    if (item.CRMC_DT_CANCELAMENTO < item.CRMC_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRMC_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRMC_CD_ID.ToString() + "|" + item.CRMC_DS_DESCRICAO + "|" + item.CRMC_DT_CRIACAO.ToShortDateString() + "|" + item.CRMC_IN_ATIVO.ToString() + "|" + item.CRMC_IN_STATUS.ToString() + "|" + item.CRMC_NM_NOME + "|" + item.CRM_ORIGEM.CROR_NM_NOME;
                String antes = itemAntes.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + itemAntes.CRMC_CD_ID.ToString() + "|" + itemAntes.CRMC_DS_DESCRICAO + "|" + itemAntes.CRMC_DT_CRIACAO.ToShortDateString() + "|" + itemAntes.CRMC_IN_ATIVO.ToString() + "|" + itemAntes.CRMC_IN_STATUS.ToString() + "|" + itemAntes.CRMC_NM_NOME + "|" + itemAntes.CRM_ORIGEM.CROR_NM_NOME;

                // Monta Log
                LOG log = new LOG();
                if (item.CRMC_DT_CANCELAMENTO != null)
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "CancCRMC",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }
                else
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "EditCRMC",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = serial,
                        LOG_TX_REGISTRO_ANTES = antes
                    };
                }

                // Persiste
                item.CLIENTE = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item, log);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Alteração de Processo CRM - Comercial";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: O Processo CRM-Comercial " + item.CRMC_NM_NOME + " do cliente " + cli.CLIE_NM_NOME + " sob sua responsabilidade, foi alterado em " + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = item.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CRM_COMERCIAL item, CRM_COMERCIAL itemAntes)
        {
            try
            {
                // Verificação
                if (item.CRMC_DT_ENCERRAMENTO != null)
                {
                    if (item.CRMC_DT_ENCERRAMENTO < item.CRMC_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRMC_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRMC_DT_CANCELAMENTO != null)
                {
                    if (item.CRMC_DT_CANCELAMENTO < item.CRMC_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRMC_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Persiste
                item.CLIENTE = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CRM_COMERCIAL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                // Acerta campos
                item.CRMC_IN_ATIVO = 2;

                // Verifica integridade
                List<CRM_COMERCIAL_ACAO> acao = item.CRM_COMERCIAL_ACAO.Where(p => p.CRCA_DT_PREVISTA > DateTime.Today.Date).ToList();
                if (acao.Count > 0)
                {
                    return 1;
                }

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRMC_CD_ID.ToString() + "|" + item.CRMC_DS_DESCRICAO + "|" + item.CRMC_DT_CRIACAO.ToShortDateString() + "|" + item.CRMC_IN_ATIVO.ToString() + "|" + item.CRMC_IN_STATUS.ToString() + "|" + item.CRMC_NM_NOME + "|" + item.CRM_ORIGEM.CROR_NM_NOME;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCRMC",
                    LOG_TX_REGISTRO = serial
                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);

                // Notificações
                if (volta == 0 & usuario.USUA_CD_ID != item.USUA_CD_ID)
                {
                    // Notifica vendedor
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Exclusão de CRM - Comercial";
                    noti.NOTI_TX_TEXTO = "O Processo de CRM-Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO + " foi excluído por " + usuario.USUA_NM_NOME + " em " + DateTime.Today.Date.ToShortDateString();

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Configuracao
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Recupera template
                    String header = _tempService.GetByCode("EXCCRMCOM").TEMP_TX_CABECALHO;
                    String body = _tempService.GetByCode("EXCCRMCOM").TEMP_TX_CORPO;
                    String footer = _tempService.GetByCode("EXCCRMCOM").TEMP_TX_DADOS;

                    // Prepara campos
                    body = body.Replace("{processo}", item.CRMC_NM_NOME);
                    body = body.Replace("{numero}", item.CRMC_NR_NUMERO);
                    header = header.Replace("{nome}", item.USUARIO.USUA_NM_NOME);
                    String emailBody = header + "<br /><br />" + body + "<br /><br />" + footer;

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Processo CRM-Comercial - Exclusão";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = item.USUARIO.USUA_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
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
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CRM_COMERCIAL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CRMC_IN_ATIVO = 1;

                // Serializa registro
                CLIENTE cli = _cliService.GetItemById(item.CLIE_CD_ID);
                String serial = item.ASSI_CD_ID.ToString() + "|" + cli.CLIE_NM_NOME + "|" + item.CRMC_CD_ID.ToString() + "|" + item.CRMC_DS_DESCRICAO + "|" + item.CRMC_DT_CRIACAO.ToShortDateString() + "|" + item.CRMC_IN_ATIVO.ToString() + "|" + item.CRMC_IN_STATUS.ToString() + "|" + item.CRMC_NM_NOME + "|" + item.CRM_ORIGEM.CROR_NM_NOME;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCRMC",
                    LOG_TX_REGISTRO = serial
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                // Notificações
                if (volta == 0 & usuario.USUA_CD_ID != item.USUA_CD_ID)
                {
                    // Notifica vendedor
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Reativação de CRM - Comercial";
                    noti.NOTI_TX_TEXTO = "O Processo de CRM-Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO + " foi reativado por " + usuario.USUA_NM_NOME + " em " + DateTime.Today.Date.ToShortDateString();

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Configuracao
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Recupera template
                    String header = _tempService.GetByCode("REACRMCOM").TEMP_TX_CABECALHO;
                    String body = _tempService.GetByCode("REACRMCOM").TEMP_TX_CORPO;
                    String footer = _tempService.GetByCode("REACRMCOM").TEMP_TX_DADOS;

                    // Prepara campos
                    body = body.Replace("{processo}", item.CRMC_NM_NOME);
                    body = body.Replace("{numero}", item.CRMC_NR_NUMERO);
                    header = header.Replace("{nome}", item.USUARIO.USUA_NM_NOME);
                    String emailBody = header + "<br /><br />" + body + "<br /><br />" + footer;

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Processo CRM-Comercial - Reativação";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = item.USUARIO.USUA_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
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
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditContato(CRM_COMERCIAL_CONTATO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(CRM_COMERCIAL_CONTATO item)
        {
            try
            {
                item.CRCO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAcao(CRM_COMERCIAL_ACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAcao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAcao(CRM_COMERCIAL_ACAO item, USUARIO usuario)
        {
            try
            {
                item.CRCA_IN_ATIVO = 1;

                // Recupera CRM
                CRM_COMERCIAL crm = _baseService.GetItemById(item.CRMC_CD_ID);

                // Persiste
                Int32 volta = _baseService.CreateAcao(item);

                // Gera Notificação
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Atribuição de Ação de Processo CRM - Comercial";
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_TX_TEXTO = "ATENÇÃO: A Ação " + item.CRCA_NM_TITULO + " do processo CRM-Comercial " + crm.CRMC_NM_NOME + " foi colocada sob sua responsabilidade em " + DateTime.Today.Date.ToLongDateString() + ".";
                noti.USUA_CD_ID = usuario.USUA_CD_ID;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_NIVEL = 1;
                Int32 volta1 = _notiService.Create(noti);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<CRM_COMERCIAL> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetAllItensAdmUser(id, idAss);
            return lista;
        }

        public List<CRM_COMERCIAL> GetAtrasados(Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetAtrasados(idAss);
            return lista;
        }

        public List<CRM_COMERCIAL> GetEncerrados(Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetEncerrados(idAss);
            return lista;
        }

        public List<CRM_COMERCIAL> GetCancelados(Int32 idAss)
        {
            List<CRM_COMERCIAL> lista = _baseService.GetCancelados(idAss);
            return lista;
        }

        public List<USUARIO> GetAllUsers(Int32 idAss)
        {
            return _baseService.GetAllUsers(idAss);
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            List<FILIAL> lista = _baseService.GetAllFilial(idAss);
            return lista;
        }

        public Int32 ValidateCreateAcompanhamento(CRM_COMERCIAL_COMENTARIO_NOVA item)
        {
            try
            {
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                item.CRCC_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateAcompanhamento(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CRM_COMERCIAL_ITEM GetItemCRMById(Int32 id)
        {
            CRM_COMERCIAL_ITEM lista = _baseService.GetItemCRMById(id);
            return lista;
        }

        public Int32 ValidateEditItemCRM(CRM_COMERCIAL_ITEM item)
        {
            try
            {
                if (item.CRM_COMERCIAL != null)
                {
                    item.CRM_COMERCIAL = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                // Persiste
                return _baseService.EditItemCRM(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDeleteItemCRM(CRM_COMERCIAL_ITEM item)
        {
            try
            {
                if (item.CRM_COMERCIAL != null)
                {
                    item.CRM_COMERCIAL = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                // Acerta campos
                item.CRCI_IN_ATIVO = 0;

                // Persiste
                return _baseService.EditItemCRM(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativarItemCRM(CRM_COMERCIAL_ITEM item)
        {
            try
            {
                if (item.CRM_COMERCIAL != null)
                {
                    item.CRM_COMERCIAL = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                // Acerta campos
                item.CRCI_IN_ATIVO = 1;

                // Persiste
                return _baseService.EditItemCRM(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateItemCRM(CRM_COMERCIAL_ITEM item)
        {
            item.CRCI_IN_ATIVO = 1;

            try
            {
                // Persiste
                Int32 volta = _baseService.CreateItemCRM(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEnvioAprovacao(CRM_COMERCIAL item, List<AttachmentForn> anexo, String emailPersonalizado, USUARIO usuario, List<CLIENTE> listaForn)
        {
            try
            {
                // Preparação
                CRM_COMERCIAL ped = _baseService.GetItemById(item.CRMC_CD_ID);
                var lstFornecedores = listaForn;

                // Acerta campos
                item.CRMC_IN_STATUS = 2;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelENCO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM_COMERCIAL>(item)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                ped = _baseService.GetItemById(item.CRMC_CD_ID);

                if (volta == 0)
                {
                    // Notifica usuario
                    if (usuario.USUA_CD_ID != ped.USUA_CD_ID)
                    {
                        NOTIFICACAO noti = new NOTIFICACAO();
                        noti.CANO_CD_ID = 1;
                        noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        noti.NOTI_DT_EMISSAO = DateTime.Today;
                        noti.NOTI_IN_ATIVO = 1;
                        noti.NOTI_IN_STATUS = 1;
                        noti.USUA_CD_ID = item.USUA_CD_ID;
                        noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                        noti.NOTI_IN_NIVEL = 1;
                        noti.NOTI_IN_VISTA = 0;
                        noti.NOTI_NM_TITULO = "Aviso de CRM-Comercial";
                        noti.NOTI_TX_TEXTO = "O Processo CRM-Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO + " está em aprovação pelo cliente";

                        // Persiste notificação 
                        Int32 volta1 = _notiService.Create(noti);
                    }

                    // Envia proposta
                    foreach (var f in lstFornecedores)
                    {
                        // Recupera cliente
                        CLIENTE forn = f;

                        // Recupera template e-mail
                        String header = _tempService.GetByCode("CRMCPROP").TEMP_TX_CABECALHO;
                        String body = emailPersonalizado == "" || emailPersonalizado == null ? _tempService.GetByCode("CRMCPROP").TEMP_TX_CORPO : emailPersonalizado;
                        String footer = _tempService.GetByCode("CRMCPROP").TEMP_TX_DADOS;

                        //Prepara header
                        header = header.Replace("{NomeCliente}", forn.CLIE_NM_NOME);

                        // Prepara corpo do e-mail  
                        String frase = String.Empty;
                        body = body.Replace("{Nome}", item.CRMC_NM_NOME);
                        body = body.Replace("{Numero}", item.CRMC_NR_NUMERO);
                        body = body.Replace("{Frase}", "");

                        String table = "<table>"
                                + "<thead style=\"background-color:lightsteelblue\">"
                                + "<tr>"
                                + "<th style=\"width:30%\">Produto</th>"
                                + "<th style=\"width:60%\">Descrição</th>"
                                + "<th style=\"width: 10%;\">Quantidade</th>"
                                + "</tr>"
                                + "</thead>"
                                + "<tbody>";

                        String tableContent = String.Empty;
                        foreach (var ipc in ped.CRM_COMERCIAL_ITEM.Where(x => x.CRCI_IN_ATIVO == 1).ToList())
                        {
                            tableContent += "<tr>"
                            + "<td style=\"width:30%\">" + ipc.PRODUTO.PROD_NM_NOME + "</td>"
                            + "<td style=\"width:60%\">" + ipc.PRODUTO.PROD_DS_DESCRICAO + "</td>"
                            + "<td style=\"width: 10%\">" + ipc.CRCI_QN_QUANTIDADE + "</td>"
                            + "</tr>";
                        }
                        footer = table + tableContent + "</tbody>";

                        // Concatena
                        String emailBody = header + body + footer;
                        CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                        // Monta e-mail
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = "Solicitação de Aprovação";
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_DESTINO = forn.CLIE_NM_EMAIL;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.IS_HTML = true;
                        if (anexo.Count > 0)
                        {
                            mensagem.ATTACHMENT = new List<Attachment>();
                            mensagem.ATTACHMENT.Add(anexo.First(x => x.FORN_CD_ID == f.CLIE_CD_ID).ATTACHMENT);
                        }

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

        public Int32 ValidateEnvioAprovacao(CRM_COMERCIAL item, String emailPersonalizado, USUARIO usuario)
        {
            try
            {
                // Preparação
                CRM_COMERCIAL ped = _baseService.GetItemById(item.CRMC_CD_ID);

                // Acerta campos
                item.CRMC_IN_STATUS = 2;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ValENCO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM_COMERCIAL>(item)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                ped = _baseService.GetItemById(item.CRMC_CD_ID);
                ped.CLIENTE = _cliService.GetById(item.CLIE_CD_ID);

                if (volta == 0)
                {
                    if (usuario.USUA_CD_ID != ped.USUA_CD_ID)
                    {
                        NOTIFICACAO noti = new NOTIFICACAO();
                        noti.CANO_CD_ID = 1;
                        noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        noti.NOTI_DT_EMISSAO = DateTime.Today;
                        noti.NOTI_IN_ATIVO = 1;
                        noti.NOTI_IN_STATUS = 1;
                        noti.USUA_CD_ID = item.USUA_CD_ID;
                        noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                        noti.NOTI_IN_NIVEL = 1;
                        noti.NOTI_IN_VISTA = 0;
                        noti.NOTI_NM_TITULO = "Aviso de CRM-Comercial";
                        noti.NOTI_TX_TEXTO = "O Processo CRM-Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO + " está em aprovação pelo cliente";

                        // Persiste notificação 
                        Int32 volta1 = _notiService.Create(noti);
                    }

                    // Recupera template e-mail
                    String header = _tempService.GetByCode("CRMCPROP").TEMP_TX_CABECALHO;
                    String body = emailPersonalizado == "" || emailPersonalizado == null ? _tempService.GetByCode("CRMCPROP").TEMP_TX_CORPO : emailPersonalizado;
                    String footer = _tempService.GetByCode("CRMCPROP").TEMP_TX_DADOS;

                    //Prepara header
                    header = header.Replace("{NomeCliente}", ped.CLIENTE.CLIE_NM_NOME);

                    // Prepara corpo do e-mail  
                    String frase = String.Empty;
                    body = body.Replace("{Nome}", item.CRMC_NM_NOME);
                    body = body.Replace("{Numero}", item.CRMC_NR_NUMERO);

                    String table = "<table>"
                            + "<thead style=\"background-color:lightsteelblue\">"
                            + "<tr>"
                            + "<th style=\"width:30%\">Produto</th>"
                            + "<th style=\"width:60%\">Descrição</th>"
                            + "<th style=\"width: 10%;\">Quantidade</th>"
                            + "</tr>"
                            + "</thead>"
                            + "<tbody>";
                    String tableContent = String.Empty;

                    //Prepara dados
                    foreach (var ipc in ped.CRM_COMERCIAL_ITEM.Where(x => x.CRCI_IN_ATIVO == 1).ToList())
                    {
                        tableContent += "<tr>"
                        + "<td style=\"width:30%\">" + ipc.PRODUTO.PROD_NM_NOME + "</td>"
                        + "<td style=\"width:60%\">" + ipc.PRODUTO.PROD_DS_DESCRICAO + "</td>"
                        + "<td style=\"width: 10%\">" + ipc.CRCI_QN_QUANTIDADE + "</td>"
                        + "</tr>";
                    }
                    footer = table + tableContent + "</tbody>";

                    // Concatena
                    String emailBody = header + body + footer;
                    CONFIGURACAO conf = _confService.GetItemById(1);

                    // Gera emails
                    CLIENTE forn = _cliService.GetItemById((Int32)ped.CLIE_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Solicitação de Aprovação";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = forn.CLIE_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;

                    // Envia mensagem
                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public String ValidateCreateMensagem(CLIENTE item, USUARIO usuario, CRM_COMERCIAL ped, Int32? idAss)
        {
            try
            {
                CLIENTE forn = _cliService.GetById(item.CLIE_CD_ID);

                // Verifica existencia prévia
                if (forn == null)
                {
                    return "1";
                }

                // Criticas
                if (forn.CLIE_NR_CELULAR == null)
                {
                    return "2";
                }
                CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);
                
                // Monta token
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Monta texto
                String texto = _tempService.GetByCode("SMSAPROVACAO").TEMP_TX_CORPO;
                texto = texto.Replace("{Nome}", forn.CLIE_NM_NOME);
                texto = texto.Replace("{Numero}", ped.CRMC_NR_NUMERO);
                texto = texto.Replace("{Emissor}", "ERPSys");

                // inicia processo
                String smsBody = texto;
                String erro = null;
                String resposta = String.Empty;

                // Monta destinatarios
                try
                {
                    String listaDest = "55" + Regex.Replace(forn.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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
                return resposta;
            }
            catch (Exception ex)
            {
                return "3";
            }
        }

        public Int32 ValidateEditItemCRMAprovacao(CRM_COMERCIAL_ITEM item)
        {
            try
            {
                // Persiste
                return _baseService.EditItemCRM(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateAprovacao(CRM_COMERCIAL item)
        {
            try
            {
                // Preparação
                CRM_COMERCIAL ped = _baseService.GetItemById(item.CRMC_CD_ID);

                // Verificação
                List<CRM_COMERCIAL_ITEM> ipc = ped.CRM_COMERCIAL_ITEM.Where(a => a.CRCI_IN_ATIVO == 1).ToList();
                if (ipc.Where(a => a.CRCI_VL_VALOR == 0 || a.CRCI_VL_VALOR == null).Count() > 0)
                {
                    return 1;
                }
                if (ipc.Where(a => a.CRCI_QN_QUANTIDADE_REVISADA == 0).Count() > 0)
                {
                    return 2;
                }
                if (ipc.Where(a => a.CRCI_DT_APROVACAO == null).Count() > 0)
                {
                    return 3;
                }

                // Acerta campos
                item.CRMC_IN_STATUS = 4;
                item.CRMC_DT_APROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta =  _baseService.Edit(item);

                if (volta == 0)
                {
                    // Notifica vendedor
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de CRM - Comercial";
                    noti.NOTI_TX_TEXTO = "O Processo de CRM-Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO + " foi aprovado pelo cliente";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Configuracao
                    CONFIGURACAO conf = _confService.GetItemById(item.ASSI_CD_ID);

                    // Recupera template
                    String header = _tempService.GetByCode("CRMCOMAPROV").TEMP_TX_CABECALHO;
                    String body = _tempService.GetByCode("CRMCOMAPROV").TEMP_TX_CORPO;
                    String footer = _tempService.GetByCode("CRMCOMAPROV").TEMP_TX_DADOS;

                    // Prepara campos
                    body = body.Replace("{processo}", item.CRMC_NM_NOME);
                    body = body.Replace("{numero}", item.CRMC_NR_NUMERO);
                    header = header.Replace("{nome}", item.USUARIO.USUA_NM_NOME);
                    String emailBody = header + "<br /><br />" + body + "<br /><br />" + footer;

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Processo CRM-Comercial - Aprovação";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = item.USUARIO.USUA_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = "ErpSys";
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
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReprovacao(CRM_COMERCIAL item)
        {
            try
            {
                // Preparação
                CRM_COMERCIAL ped = _baseService.GetItemById(item.CRMC_CD_ID);

                // Acerta campos
                item.CRMC_IN_STATUS = 5;
                item.CRMC_DT_REPROVACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);

                if (volta == 0)
                {
                    // Notifica comprador
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de CRM - Comercial";
                    noti.NOTI_TX_TEXTO = "O Processo de CRM-Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO + " foi reprovado pelo cliente em " + DateTime.Today.Date.ToShortDateString();

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Configuracao
                    CONFIGURACAO conf = _confService.GetItemById(item.ASSI_CD_ID);

                    // Recupera template
                    String header = _tempService.GetByCode("CRMCOMREP").TEMP_TX_CABECALHO;
                    String body = _tempService.GetByCode("CRMCOMREP").TEMP_TX_CORPO;
                    String footer = _tempService.GetByCode("CRMCOMREP").TEMP_TX_DADOS;

                    // Prepara campos
                    body = body.Replace("{processo}", item.CRMC_NM_NOME);
                    body = body.Replace("{numero}", item.CRMC_NR_NUMERO);
                    header = header.Replace("{nome}", item.USUARIO.USUA_NM_NOME);
                    String emailBody = header + "<br /><br />" + body + "<br /><br />" + footer;

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Processo CRM-Comercial - Reprovação";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = item.USUARIO.USUA_NM_EMAIL;
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
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       public Int32 ValidateEfetuarVenda(CRM_COMERCIAL item)
        {
            try
            {
                // Preparação
                CRM_COMERCIAL ped = _baseService.GetItemById(item.CRMC_CD_ID);

                // Acerta campos
                item.CRMC_IN_STATUS = 6;
                item.CRMC_DT_ENCERRAMENTO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);

                // Notifica usuario
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.CANO_CD_ID = 1;
                noti.ASSI_CD_ID = item.ASSI_CD_ID;
                noti.NOTI_DT_EMISSAO = DateTime.Today;
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_IN_STATUS = 1;
                noti.USUA_CD_ID = item.USUA_CD_ID;
                noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                noti.NOTI_IN_NIVEL = 1;
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "Aviso de CRM - Comercial";
                noti.NOTI_TX_TEXTO = "O Processo de CRM-Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO + " foi finalizado em " + DateTime.Today.Date.ToShortDateString();

                // Persiste notificação 
                Int32 volta1 = _notiService.Create(noti);

                // Configuracao
                CONFIGURACAO conf = _confService.GetItemById(item.ASSI_CD_ID);

                // Recupera template
                USUARIO usu = _usuService.GetItemById(item.USUA_CD_ID);
                String header = _tempService.GetByCode("CRMCOMENC").TEMP_TX_CABECALHO;
                String body = _tempService.GetByCode("CRMCOMENC").TEMP_TX_CORPO;
                String footer = _tempService.GetByCode("CRMCOMENC").TEMP_TX_DADOS;

                // Prepara campos
                body = body.Replace("{processo}", item.CRMC_NM_NOME);
                body = body.Replace("{numero}", item.CRMC_NR_NUMERO);
                header = header.Replace("{nome}", usu.USUA_NM_NOME);
                String emailBody = header + "<br /><br />" + body + "<br /><br />" + footer;

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                Email mensagem = new Email();
                mensagem.ASSUNTO = "Processo CRM-Comercial - Encerramento";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_DESTINO = usu.USUA_NM_EMAIL;
                mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = usu.ASSINANTE.ASSI_NM_NOME;
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

        public Int32 ValidateEntreguePorItem(CRM_COMERCIAL item)
        {
            try
            {
                item.CRMC_IN_STATUS = 6;
                item.CRMC_DT_ENCERRAMENTO = DateTime.Today.Date;
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEncerrar(CRM_COMERCIAL item, USUARIO usuario)
        {
            try
            {
                CRM_COMERCIAL ped = _baseService.GetItemById(item.CRMC_CD_ID);
                Int32 idAss = usuario.ASSI_CD_ID;

                // Acerta campos
                item.CRMC_IN_STATUS = 7;
                item.CRMC_DT_FATURAMENTO = DateTime.Today.Date;

                // Acerta estoque
                foreach (CRM_COMERCIAL_ITEM itpc in ped.CRM_COMERCIAL_ITEM.Where(x => x.CRCI_IN_ATIVO == 1 && x.CRCI_QN_QUANTIDADE == null).ToList())
                {
                    itpc.CRCI_QN_QUANTIDADE = itpc.CRCI_QN_QUANTIDADE_REVISADA;
                    if (itpc.CRCI_QN_QUANTIDADE == null)
                    {
                        itpc.CRCI_QN_QUANTIDADE = itpc.CRCI_QN_QUANTIDADE_REVISADA == null ? itpc.CRCI_QN_QUANTIDADE : itpc.CRCI_QN_QUANTIDADE_REVISADA;
                    }

                    // Grava movimento
                    MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                    mov.ASSI_CD_ID = item.ASSI_CD_ID;
                    mov.FILI_CD_ID = item.FILI_CD_ID;
                    mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                    mov.MOEP_IN_ATIVO = 1;
                    mov.MOEP_IN_CHAVE_ORIGEM = 3;
                    mov.MOEP_IN_ORIGEM = "Venda";
                    mov.MOEP_IN_OPERACAO = 2;
                    mov.MOEP_IN_TIPO_MOVIMENTO = 2;
                    mov.MOEP_QN_QUANTIDADE = (Int32)itpc.CRCI_QN_QUANTIDADE;
                    mov.PROD_CD_ID = (Int32)itpc.PROD_CD_ID;
                    mov.USUA_CD_ID = ped.USUA_CD_ID;
                    Int32 volta2 = _movService.Create(mov);

                    // Acerta estoque
                    PRODUTO_ESTOQUE_FILIAL pef = new PRODUTO_ESTOQUE_FILIAL();
                    pef.FILI_CD_ID = ped.FILI_CD_ID == null ? usuario.FILI_CD_ID.Value : (Int32)ped.FILI_CD_ID;
                    pef.PROD_CD_ID = (Int32)itpc.PROD_CD_ID;

                    if (_pefService.CheckExist(pef, item.ASSI_CD_ID) != null)
                    {
                        pef.PREF_CD_ID = _pefService.CheckExist(pef, item.ASSI_CD_ID).PREF_CD_ID;
                        pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                        pef.PREF_IN_ATIVO = 1;
                        if (_pefService.CheckExist(pef, item.ASSI_CD_ID).PREF_QN_ESTOQUE == null)
                        {
                            pef.PREF_QN_ESTOQUE = (Int32)itpc.CRCI_QN_QUANTIDADE;
                        }
                        else
                        {
                            pef.PREF_QN_ESTOQUE = _pefService.CheckExist(pef, item.ASSI_CD_ID).PREF_QN_ESTOQUE - (Int32)itpc.CRCI_QN_QUANTIDADE;
                        }
                        Int32 voltaPef = _pefService.Edit(pef);
                    }
                    else
                    {
                        pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                        pef.PREF_IN_ATIVO = 1;
                        pef.PREF_QN_ESTOQUE = (Int32)itpc.CRCI_QN_QUANTIDADE;

                        Int32 voltaPef = _pefService.Create(pef);
                    }
                }

                // Persiste
                Int32 volta = _baseService.Edit(item);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateItemEntregue(CRM_COMERCIAL_ITEM item, USUARIO usuario)
        {
            try
            {
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                CRM_COMERCIAL ped = GetItemById(item.CRMC_CD_ID);
                CRM_COMERCIAL_ITEM itpc = _baseService.GetItemCRMById(item.CRCI_CD_ID);

                // Acerta estoque
                MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                mov.ASSI_CD_ID = item.CRM_COMERCIAL.ASSI_CD_ID;
                mov.FILI_CD_ID = ped.FILI_CD_ID;
                mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                mov.MOEP_IN_ATIVO = 1;
                mov.MOEP_IN_CHAVE_ORIGEM = 3;
                mov.MOEP_IN_OPERACAO = 2;
                mov.MOEP_IN_TIPO_MOVIMENTO = 0;
                mov.MOEP_QN_QUANTIDADE = (Int32)item.CRCI_QN_QUANTIDADE;
                mov.PROD_CD_ID = (Int32)item.PROD_CD_ID;
                mov.USUA_CD_ID = ped.USUA_CD_ID;
                Int32 volta2 = _movService.Create(mov);

                PRODUTO_ESTOQUE_FILIAL pef = new PRODUTO_ESTOQUE_FILIAL();
                pef.FILI_CD_ID = ped.FILI_CD_ID == null ? usuario.FILI_CD_ID.Value : (Int32)ped.FILI_CD_ID;
                pef.PROD_CD_ID = (Int32)item.PROD_CD_ID;

                if (_pefService.CheckExist(pef, item.CRM_COMERCIAL.ASSI_CD_ID) != null)
                {
                    pef.PREF_CD_ID = _pefService.CheckExist(pef, item.CRM_COMERCIAL.ASSI_CD_ID).PREF_CD_ID;
                    pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                    pef.PREF_IN_ATIVO = 1;
                    if (_pefService.CheckExist(pef, item.CRM_COMERCIAL.ASSI_CD_ID).PREF_QN_ESTOQUE == null)
                    {
                        pef.PREF_QN_ESTOQUE = (Int32)item.CRCI_QN_QUANTIDADE;
                    }
                    else
                    {
                        pef.PREF_QN_ESTOQUE = _pefService.CheckExist(pef, item.CRM_COMERCIAL.ASSI_CD_ID).PREF_QN_ESTOQUE - (Int32)item.CRCI_QN_QUANTIDADE;
                    }

                    Int32 voltaPef = _pefService.Edit(pef);
                }
                else
                {
                    pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                    pef.PREF_IN_ATIVO = 1;
                    pef.PREF_QN_ESTOQUE = (Int32)item.CRCI_QN_QUANTIDADE;

                    Int32 voltaPef = _pefService.Create(pef);
                }

                PRODUTO_TABELA_PRECO ptp = new PRODUTO_TABELA_PRECO();
                ptp.PROD_CD_ID = (Int32)itpc.PROD_CD_ID;
                ptp.FILI_CD_ID = ped.FILI_CD_ID;
                PRODUTO_TABELA_PRECO ptpAntes = _ptpService.CheckExist(ptp, usuario.ASSI_CD_ID);

                if (ptpAntes == null)
                {
                    ptp.PRTP_IN_ATIVO = 1;
                    ptp.PRTP_VL_CUSTO = itpc.CRCI_VL_VALOR;
                    Int32 voltaPtp = _ptpService.Create(ptp);
                }
                else
                {
                    ptp.PRTP_IN_ATIVO = 1;
                    ptp.PRTP_VL_CUSTO = itpc.CRCI_VL_VALOR;
                    ptp.PRTP_VL_PRECO = ptpAntes.PRTP_VL_PRECO;
                    ptp.PRTP_VL_PRECO_PROMOCAO = ptp.PRTP_VL_PRECO_PROMOCAO;
                    ptp.PRTP_VL_DESCONTO_MAXIMO = ptpAntes.PRTP_VL_DESCONTO_MAXIMO;
                    ptp.PRTP_DT_DATA_REAJUSTE = ptpAntes.PRTP_DT_DATA_REAJUSTE;
                    ptp.PRTP_NR_MARKUP = ptpAntes.PRTP_NR_MARKUP;
                    ptp.PRTP_CD_ID = ptpAntes.PRTP_CD_ID;

                    Int32 voltaPtp = _ptpService.Edit(ptp);
                }

                // Persiste
                Int32 volta = _baseService.EditItemCRM(item);
                Int32 conta = ped.CRM_COMERCIAL_ITEM.Where(x => x.CRCI_IN_ATIVO == 1 && x.CRCI_QN_QUANTIDADE != null || x.CRCI_CD_ID == item.CRCI_CD_ID).Count();

                if (ped.CRM_COMERCIAL_ITEM.Where(x => x.CRCI_IN_ATIVO == 1).Count() == conta)
                {
                    Int32 voltaItemR = ValidateEntreguePorItem(ped);
                    return 2;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCancelamento(CRM_COMERCIAL item)
        {
            try
            {
                // Critica
                if (String.IsNullOrEmpty(item.CRMC_DS_JUSTIFICATIVA_CANCELAMENTO))
                {
                    return 1;
                }
                if (item.CRMC_DT_CANCELAMENTO > DateTime.Today.Date)
                {
                    return 2;
                }
                if (item.CRMC_DT_CANCELAMENTO < item.CRMC_DT_CRIACAO)
                {
                    return 3;
                }

                // Acerta campos
                item.CRMC_IN_STATUS = 8;
                item.CRMC_DT_CANCELAMENTO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                if (volta == 0)
                {
                    // Notifica vendedor
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = item.ASSI_CD_ID;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.USUARIO.USUA_CD_ID;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de CRM - Comercial";
                    noti.NOTI_TX_TEXTO = "O Processo de CRM-Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO + " foi cancelado em " + DateTime.Today.Date.ToShortDateString() + ". Justificativa: " + item.CRMC_DS_JUSTIFICATIVA_CANCELAMENTO;

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);

                    // Configuracao
                    CONFIGURACAO conf = _confService.GetItemById(item.ASSI_CD_ID);

                    // Recupera template
                    String header = _tempService.GetByCode("CRMCOMCANC").TEMP_TX_CABECALHO;
                    String body = _tempService.GetByCode("CRMCOMCANC").TEMP_TX_CORPO;
                    String footer = _tempService.GetByCode("CRMCOMCANC").TEMP_TX_DADOS;

                    // Prepara campos
                    body = body.Replace("{processo}", item.CRMC_NM_NOME);
                    body = body.Replace("{numero}", item.CRMC_NR_NUMERO);
                    header = header.Replace("{nome}", item.USUARIO.USUA_NM_NOME);
                    String emailBody = header + "<br /><br />" + body + "<br /><br />" + footer;

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Processo CRM-Comercial - Cancelamento";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = item.USUARIO.USUA_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = item.ASSINANTE.ASSI_NM_NOME;
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
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
