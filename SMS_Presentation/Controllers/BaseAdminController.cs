﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SMS_Presentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_CRM_Solution.ViewModels;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using EntitiesServices.WorkClasses;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using CrossCutting;

namespace ERP_CRM_Solution.Controllers
{
    public class BaseAdminController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly INoticiaAppService notiApp;
        private readonly ILogAppService logApp;
        private readonly ITarefaAppService tarApp;
        private readonly INotificacaoAppService notfApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IAgendaAppService ageApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITipoPessoaAppService tpApp;
        //private readonly IClienteAppService cliApp;
        private readonly ITelefoneAppService telApp;
        private readonly IFormularioRespostaAppService frApp;
        private readonly ICRMOrigemAppService origApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        FORMULARIO_RESPOSTA objetoFR = new FORMULARIO_RESPOSTA();
        FORMULARIO_RESPOSTA objetoFRAntes = new FORMULARIO_RESPOSTA();
        List<FORMULARIO_RESPOSTA> listaMasterFR = new List<FORMULARIO_RESPOSTA>();
        String extensao;

        public BaseAdminController(IUsuarioAppService baseApps, ILogAppService logApps, INoticiaAppService notApps, ITarefaAppService tarApps, INotificacaoAppService notfApps, IUsuarioAppService usuApps, IAgendaAppService ageApps, IConfiguracaoAppService confApps, ITipoPessoaAppService tpApps, ITelefoneAppService telApps, IFormularioRespostaAppService frApps, ICRMOrigemAppService origApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notApps;
            tarApp = tarApps;
            notfApp = notfApps;
            usuApp = usuApps;
            ageApp = ageApps;
            confApp = confApps;
            tpApp = tpApps;
            telApp = telApps;
            //cliApp = cliApps;
            frApp = frApps;
            origApp = origApps;
        }

        public ActionResult CarregarAdmin()
        {
            Int32? idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = baseApp.GetAllUsuarios(idAss.Value).Count;
            ViewBag.Logs = logApp.GetAllItens(idAss.Value).Count;
            ViewBag.UsuariosLista = baseApp.GetAllUsuarios(idAss.Value);
            ViewBag.LogsLista = logApp.GetAllItens(idAss.Value);
            return View();

        }

        public ActionResult CarregarLandingPage()
        {
            return View();
        }

        public JsonResult GetRefreshTime()
        {
            return Json(confApp.GetById(1).CONF_NR_REFRESH_DASH);
        }

        public JsonResult GetConfigNotificacoes()
        {
            Int32? idAss = (Int32)Session["IdAssinante"];
            bool hasNotf;
            var hash = new Hashtable();
            USUARIO usu = (USUARIO)Session["Usuario"];
            CONFIGURACAO conf = confApp.GetById(1);

            if (baseApp.GetAllItensUser(usu.USUA_CD_ID, idAss.Value).Count > 0)
            {
                hasNotf = true;
            }
            else
            {
                hasNotf = false;
            }

            hash.Add("CONF_NM_ARQUIVO_ALARME", conf.CONF_NM_ARQUIVO_ALARME);
            hash.Add("NOTIFICACAO", hasNotf);
            return Json(hash);
        }

        [HttpGet]
        public ActionResult IncluirContato()
        {
            // Prepara view
            ViewBag.UF = new SelectList(frApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            FORMULARIO_RESPOSTA item = new FORMULARIO_RESPOSTA();
            FormularioRespostaViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA, FormularioRespostaViewModel>(item);
            vm.FORE_IN_ATIVO = 1;
            vm.FORE_IN_STATUS = 1;
            vm.FORE_DT_CADASTRO = DateTime.Today.Date;
            vm.FORE_IN_ESTRELA = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(FormularioRespostaViewModel vm)
        {
            ViewBag.UF = new SelectList(frApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            if (ModelState.IsValid)
            {
                try
                {
                    // Completa campos
                    ViewBag.Usuarios = new SelectList(usuApp.GetAllSistema().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                    vm.FORE_NM_PROCESSO = "Processo " + vm.FORE_NM_NOME;
                    vm.FORE_DS_DESCRICAO = "Processo aberto via formulário on-line";
                    vm.USUA_CD_ID = usuApp.GetAllSistema().OrderBy(p => p.USUA_CD_ID).FirstOrDefault().USUA_CD_ID;

                    // Executa a operação
                    FORMULARIO_RESPOSTA item = Mapper.Map<FormularioRespostaViewModel, FORMULARIO_RESPOSTA>(vm);
                    Int32 volta = frApp.ValidateCreate(item, 1);

                    // Verifica retorno
                    return RedirectToAction("CarregarLandingPage");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult CarregarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["Login"] == 1)
            {
                Session["Perfis"] = baseApp.GetAllPerfis();
                Session["Usuarios"] = usuApp.GetAllUsuarios(idAss);
                Session["TiposPessoas"] = tpApp.GetAllItens();
                Session["UFs"] = telApp.GetAllUF();
            }

            Session["MensTarefa"] = 0;
            Session["MensNoticia"] = 0;
            Session["MensNotificacao"] = 0;
            Session["MensUsuario"] = 0;
            Session["MensLog"] = 0;
            Session["MensUsuarioAdm"] = 0;
            Session["MensAgenda"] = 0;
            Session["MensTemplate"] = 0;
            Session["MensConfiguracao"] = 0;
            Session["MensTelefone"] = 0;
            Session["MensBanco"] = 0;
            Session["MensFilial"] = 0;
            Session["MensCC"] = 0;
            Session["MensGrupo"] = 0;
            Session["MensFornecedor"] = 0;
            Session["MensCliente"] = 0;
            Session["MensSMSCliente"] = 0;
            Session["MensEquipamento"] = 0;
            Session["MensSMSTrans"] = 0;
            Session["MensTransportadora"] = 0;
            Session["MensProduto"] = 0;
            Session["MensEstoque"] = 0;
            Session["MensFormaPag"] = 0;
            Session["MensCP"] = 0;
            Session["MensCR"] = 0;
            Session["MensCompra"] = 0;
            Session["MensVenda"] = 0;
            Session["ErroSoma"] = 0;
            Session["IdCP"] = 0;
            Session["VoltaCP"] = 0;
            Session["VoltaCompra"] = 0;
            Session["MensvencimentoCR"] = 0;
            Session["VoltaCR"] = 0;
            Session["IdVoltaTab"] = 0;
            Session["SMSEMailEnvio"] = 0;
            Session["MensTemplateSMS"] = 0;
            Session["MensPermissao"] = 0;
            Session["MensCRM"] = 0;
            Session["MensCatCliente"] = 0;
            Session["MensCargo"] = 0;
            Session["MensOrigem"] = 0;
            Session["MensMotCancelamento"] = 0;
            Session["MensMotEncerramento"] = 0;
            Session["MensTipoAcao"] = 0;
            Session["MensAtendimento"] = 0;
            Session["MensServico"] = 0;
            Session["MensCR"] = 0;
            Session["MensCP"] = 0;
            Session["MensOrdemServico"] = 0;
            Session["IdOrdemServico"] = 0;
            Session["VoltaNotificacao"] = 3;
            Session["VoltaNoticia"] = 1;
            Session["AgendaCorp"] = 0;
            Session["VoltaMensagem"] = 0;
            Session["VoltaCRM"] = 0;
            Session["ListaCRM"] = null;
            Session["CRM"] = null;
            Session["FiltroCRM"] = null;
            Session["ListaITPC"] = null;

            USUARIO usu = new USUARIO();
            UsuarioViewModel vm = new UsuarioViewModel();
            List<NOTIFICACAO> noti = new List<NOTIFICACAO>();

            ObjectCache cache = MemoryCache.Default;
            USUARIO usuContent = cache["usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID] as USUARIO;

            if (usuContent == null)
            {
                usu = usuApp.GetItemById(((USUARIO)Session["UserCredentials"]).USUA_CD_ID);
                vm = Mapper.Map<USUARIO, UsuarioViewModel>(usu);
                noti = notfApp.GetAllItens(idAss);
                DateTime expiration = DateTime.Now.AddDays(15);
                cache.Set("usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, usu, expiration);
                cache.Set("vm" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, vm, expiration);
                cache.Set("noti" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, noti, expiration);
            }

            usu = cache.Get("usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as USUARIO;
            vm = cache.Get("vm" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as UsuarioViewModel;
            noti = cache.Get("noti" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as List<NOTIFICACAO>;
            ViewBag.Perfil = usu.PERFIL.PERF_SG_SIGLA;

            noti = notfApp.GetAllItensUser(usu.USUA_CD_ID, usu.ASSI_CD_ID);
            Session["Notificacoes"] = noti; 
            Session["ListaNovas"] = noti.Where(p => p.NOTI_IN_VISTA == 0).ToList().Take(5).OrderByDescending(p => p.NOTI_DT_EMISSAO).ToList();
            Session["NovasNotificacoes"] = noti.Where(p => p.NOTI_IN_VISTA == 0).Count();
            Session["Nome"] = usu.USUA_NM_NOME;

            Session["Noticias"] = notiApp.GetAllItensValidos(idAss);
            Session["NoticiasNumero"] = ((List<NOTICIA>)Session["Noticias"]).Count;

            Session["ListaPendentes"] = tarApp.GetTarefaStatus(usu.USUA_CD_ID, 1);
            Session["TarefasPendentes"] = ((List<TAREFA>)Session["ListaPendentes"]).Count;
            Session["TarefasLista"] = tarApp.GetByUser(usu.USUA_CD_ID);
            Session["Tarefas"] = ((List<TAREFA>)Session["TarefasLista"]).Count;

            Session["Agendas"] = ageApp.GetByUser(usu.USUA_CD_ID, usu.ASSI_CD_ID);
            Session["NumAgendas"] = ((List<AGENDA>)Session["Agendas"]).Count;
            Session["AgendasHoje"] = ((List<AGENDA>)Session["Agendas"]).Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).ToList();
            Session["NumAgendasHoje"] = ((List<AGENDA>)Session["AgendasHoje"]).Count;

            Session["Telefones"] = telApp.GetAllItens(usu.ASSI_CD_ID);
            Session["NumTelefones"] = ((List<TELEFONE>)Session["Telefones"]).Count;
            Session["Logs"] = usu.LOG.Count;

            String frase = String.Empty;
            String nome = usu.USUA_NM_NOME.Substring(0, usu.USUA_NM_NOME.IndexOf(" "));
            Session["NomeGreeting"] = nome;
            if (DateTime.Now.Hour <= 12)
            {
                frase = "Bom dia, " + nome;
            }
            else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
            {
                frase = "Boa tarde, " + nome;
            }
            else
            {
                frase = "Boa noite, " + nome;
            }
            Session["Greeting"] = frase;
            Session["Foto"] = usu.USUA_AQ_FOTO;

            // Mensagens
            if ((Int32)Session["MensPermissao"] == 2)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            Session["MensPermissao"] = 0;
            ViewBag.Especial = vm.USUA_IN_ESPECIAL;
            return View(vm);
        }

        public ActionResult CarregarDesenvolvimento()
        {
            return View();
        }

        public ActionResult VoltarDashboard()
        {
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult MontarFaleConosco()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MontarTelaCRMFR()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CRM>)Session["ListaCRMFR"] == null)
            {
                listaMasterFR = frApp.GetAllItens();
                Session["ListaCRMFR"] = listaMasterFR;
            }
            Session["CRMFR"] = null;
            ViewBag.Listas = (List<FORMULARIO_RESPOSTA>)Session["ListaCRMFR"];
            ViewBag.Title = "CRMFR";
            ViewBag.Origem = new SelectList(origApp.GetAllItens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> visao = new List<SelectListItem>();
            visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
            visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
            ViewBag.Visao = new SelectList(visao, "Value", "Text");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            ViewBag.UF = new SelectList(frApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            Session["IncluirCRMFR"] = 0;
            Session["CRMVoltaAtendimento"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 30)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 31)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 60)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 61)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 62)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 63)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 51)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture));
                }

            }

            // Abre view
            Session["MensCRM"] = 0;
            Session["VoltaCRMFR"] = 1;
            Session["IncluirCliente"] = 0;
            objetoFR = new FORMULARIO_RESPOSTA();
            if (Session["FiltroCRMFR"] != null)
            {
                objetoFR = (FORMULARIO_RESPOSTA)Session["FiltroCRMFR"];
            }
            return View(objetoFR);
        }

        [HttpGet]
        public ActionResult MontarTelaKanbanCRMFR()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CRM>)Session["ListaCRMFR"] == null)
            {
                listaMasterFR = frApp.GetAllItens();
                Session["ListaCRMFR"] = listaMasterFR;
            }
            Session["CRMFR"] = null;
            ViewBag.Listas = (List<FORMULARIO_RESPOSTA>)Session["ListaCRMFR"];
            ViewBag.Title = "CRMFR";
            ViewBag.Origem = new SelectList(origApp.GetAllItens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> visao = new List<SelectListItem>();
            visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
            visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
            ViewBag.Visao = new SelectList(visao, "Value", "Text");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            Session["IncluirCRMFR"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 30)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 31)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 60)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 61)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 62)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 63)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCRMFR"] = 1;
            Session["IncluirCliente"] = 0;
            objetoFR = new FORMULARIO_RESPOSTA();
            if (Session["FiltroCRMFR"] != null)
            {
                objetoFR = (FORMULARIO_RESPOSTA)Session["FiltroCRMFR"];
            }
            return View(objetoFR);
        }

        public ActionResult RetirarFiltroCRMFR()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCRMFR"] = null;
            Session["FiltroCRMFR"] = null;
            return RedirectToAction("MontarTelaCRMFR");
        }

        [HttpPost]
        public ActionResult FiltrarCRMFR(FORMULARIO_RESPOSTA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<FORMULARIO_RESPOSTA> listaObj = new List<FORMULARIO_RESPOSTA>();
                Session["FiltroCRMFR"] = item;
                Int32 volta = frApp.ExecuteFilter(item.FORE_IN_STATUS, item.FORE_NM_NOME, item.FORE_NM_EMAIL, item.FORE_NR_CELULAR, item.FORE_NM_CIDADE, item.UF_CD_ID, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCRM"] = 1;
                }

                // Sucesso
                listaMasterFR = listaObj;
                Session["ListaCRMFR"] = listaObj;
                return RedirectToAction("MontarTelaCRMFR");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCRMFR");
            }
        }

        public ActionResult VoltarBaseCRMFR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCRMFR"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRMFR");
            }
            if ((Int32)Session["VoltaCRMFR"] == 11)
            {
                return RedirectToAction("MontarTelaCRMFR", "BaseAdmin");
            }
            Session["VoltaCRMFR"] = 0;
            return RedirectToAction("MontarTelaCRMFR");
        }

        [HttpGet]
        public ActionResult ExcluirProcessoFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRMFR"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);
            objetoFRAntes = (FORMULARIO_RESPOSTA)Session["CRMFR"];
            item.FORE_IN_ATIVO = 2;
            Int32 volta = frApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCRMFR"] = 4;
                return RedirectToAction("MontarTelaCRMFR");
            }
            Session["ListaCRMFR"] = null;
            return RedirectToAction("MontarTelaCRMFR");
        }

        [HttpGet]
        public ActionResult ReativarProcessoFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);
            objetoFRAntes = (FORMULARIO_RESPOSTA)Session["CRMFR"];
            item.FORE_IN_ATIVO = 1;
            Int32 volta = frApp.ValidateReativar(item, usuario);
            Session["ListaCRMFR"] = null;
            return RedirectToAction("MontarTelaCRMFR");
        }

        [HttpGet]
        public ActionResult EstrelaSimFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);
            objetoFRAntes = (FORMULARIO_RESPOSTA)Session["CRMFR"];
            item.FORE_IN_ESTRELA = 1;
            Int32 volta = frApp.ValidateEdit(item, item);
            Session["ListaCRMFR"] = null;
            return RedirectToAction("MontarTelaCRMFR");
        }

        [HttpGet]
        public ActionResult EstrelaNaoFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);
            objetoFRAntes = (FORMULARIO_RESPOSTA)Session["CRMFR"];
            item.FORE_IN_ESTRELA = 0;
            Int32 volta = frApp.ValidateEdit(item, item);
            Session["ListaCRMFR"] = null;
            return RedirectToAction("MontarTelaCRMFR");
        }

        [HttpGet]
        public ActionResult EncerrarAcaoFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            FORMULARIO_RESPOSTA_ACAO item = frApp.GetAcaoById(id);
            item.FRAC_IN_STATUS = 3;
            Int32 volta = frApp.ValidateEditAcao(item);
            return RedirectToAction("AcompanhamentoProcessoCRMFR");
        }

        [HttpGet]
        public ActionResult IncluirProcessoCRMFR()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Usuarios = new SelectList(usuApp.GetAllSistema().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Origem = new SelectList(origApp.GetAllItens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            ViewBag.UF = new SelectList(frApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            Session["IncluirCRMFR"] = 0;
            Session["CRMFR"] = null;

            // Prepara view
            Session["CRMNovo"] = 0;
            Session["VoltaCliente"] = 8;
            FORMULARIO_RESPOSTA item = new FORMULARIO_RESPOSTA();
            FormularioRespostaViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA, FormularioRespostaViewModel>(item);
            vm.FORE_DT_CADASTRO = DateTime.Today.Date;
            vm.FORE_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirProcessoCRMFR(FormularioRespostaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Usuarios = new SelectList(usuApp.GetAllSistema().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Origem = new SelectList(origApp.GetAllItens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMULARIO_RESPOSTA item = Mapper.Map<FormularioRespostaViewModel, FORMULARIO_RESPOSTA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = frApp.ValidateCreate(item, 2);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRMFR"] = 3;
                        return RedirectToAction("MontarTelaCRMFR");
                    }

                    // Cria pasta
                    String caminho = "/Imagens/" + idAss.ToString() + "/FormResposta/" + item.FORE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Listas
                    listaMasterFR = new List<FORMULARIO_RESPOSTA>();
                    Session["ListaCRMFR"] = null;
                    Session["IncluirCRMFR"] = 1;
                    Session["CRMNovoFR"] = item.FORE_CD_ID;
                    Session["IdCRMFR"] = item.FORE_CD_ID;

                    // Processa Anexos
                    if (Session["FileQueueCRMFR"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCRMFR"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueCRMFR(file);
                            }
                        }
                        Session["FileQueueCRMFR"] = null;
                    }

                    // Processa voltas
                    if ((Int32)Session["VoltaCRMFR"] == 3)
                    {
                        Session["VoltaCRMFR"] = 0;
                        Session["CRMAtendimento"] = 0;
                        return RedirectToAction("IncluirProcessoCRMFR", "BaseAdmin");
                    }
                    return RedirectToAction("MontarTelaCRMFR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();
            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }
                queue.Add(f);
            }
            Session["FileQueueCRMFR"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueCRMFR(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                return RedirectToAction("VoltarAnexoCRMFR");
            }

            FORMULARIO_RESPOSTA item = frApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                return RedirectToAction("VoltarAnexoCRMFR");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/FormResposta/" + item.FORE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            FORMULARIO_RESPOSTA_ANEXO foto = new FORMULARIO_RESPOSTA_ANEXO();
            foto.FRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.FRAN_DT_ANEXO = DateTime.Today;
            foto.FRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.FRAN_IN_TIPO = tipo;
            foto.FRAN_NM_TITULO = fileName;
            foto.FORE_CD_ID = item.FORE_CD_ID;

            item.FORMULARIO_RESPOSTA_ANEXO.Add(foto);
            objetoFRAntes = item;
            Int32 volta = frApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAnexoCRMFR");
        }

        [HttpPost]
        public ActionResult UploadFileCRMFR(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                return RedirectToAction("VoltarAnexoCRMFR");
            }

            FORMULARIO_RESPOSTA item = frApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                return RedirectToAction("VoltarAnexoCRMFR");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/FormResposta/" + item.FORE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            FORMULARIO_RESPOSTA_ANEXO foto = new FORMULARIO_RESPOSTA_ANEXO();
            foto.FRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.FRAN_DT_ANEXO = DateTime.Today;
            foto.FRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.FRAN_IN_TIPO = tipo;
            foto.FRAN_NM_TITULO = fileName;
            foto.FORE_CD_ID = item.FORE_CD_ID;

            item.FORMULARIO_RESPOSTA_ANEXO.Add(foto);
            objetoFRAntes = item;
            Int32 volta = frApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAnexoCRMFR");
        }

        public ActionResult VoltarAnexoCRMFR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRMFR");
            }
            return RedirectToAction("EditarProcessoCRMFR", new { id = (Int32)Session["IdCRMFR"] });
        }

        public ActionResult VoltarAcompanhamentoCRMFR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("AcompanhamentoProcessoCRMFR", new { id = (Int32)Session["IdCRMFR"] });
        }


        [HttpGet]
        public ActionResult VerAnexoCRMFR(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            FORMULARIO_RESPOSTA_ANEXO item = frApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadCRMFR(Int32 id)
        {
            FORMULARIO_RESPOSTA_ANEXO item = frApp.GetAnexoById(id);
            String arquivo = item.FRAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpGet]
        public ActionResult CancelarProcessoCRMFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            Session["IncluirCRMFR"] = 0;
            Session["CRMFR"] = null;

            // Recupera
            Session["CRMNovoFR"] = 0;
            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);

            // Checa ações
            Session["TemAcaoFR"] = 0;
            if (item.FORMULARIO_RESPOSTA_ACAO.Where(p => p.FRAC_IN_ATIVO == 1).ToList().Count > 0)
            {
                Session["TemAcaoFR"] = 1;
            }

            // Prepara view
            FormularioRespostaViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA, FormularioRespostaViewModel>(item);
            vm.FORE_DT_CANCELAMENTO = DateTime.Today.Date;
            vm.FORE_IN_ATIVO = 3;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CancelarProcessoCRMFR(FormularioRespostaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMULARIO_RESPOSTA item = Mapper.Map<FormularioRespostaViewModel, FORMULARIO_RESPOSTA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = frApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 30;
                        return RedirectToAction("MontarTelaCRMFR");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 31;
                        return RedirectToAction("MontarTelaCRMFR");
                    }

                    // Listas
                    listaMasterFR = new List<FORMULARIO_RESPOSTA>();
                    Session["ListaCRMFR"] = null;
                    Session["IncluirCRMFR"] = 1;
                    Session["CRMNovoFR"] = item.FORE_CD_ID;
                    Session["IdCRMFR"] = item.FORE_CD_ID;
                    return RedirectToAction("MontarTelaCRMFR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EncerrarProcessoCRMFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            Session["IncluirCRMFR"] = 0;
            Session["CRMFR"] = null;

            // Recupera
            Session["CRMNovoFR"] = 0;
            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);

            // Checa ações
            Session["TemAcaoFR"] = 0;
            if (item.FORMULARIO_RESPOSTA_ACAO.Where(p => p.FRAC_IN_ATIVO == 1).ToList().Count > 0)
            {
                Session["TemAcaoFR"] = 1;
            }

            // Prepara view
            FormularioRespostaViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA, FormularioRespostaViewModel>(item);
            vm.FORE_DT_ENCERRAMENTO = DateTime.Today.Date;
            vm.FORE_IN_ATIVO = 5;
            vm.FORE_IN_STATUS = 5;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EncerrarProcessoCRMFR(FormularioRespostaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMULARIO_RESPOSTA item = Mapper.Map<FormularioRespostaViewModel, FORMULARIO_RESPOSTA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = frApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("MontarTelaCRMFR");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("MontarTelaCRMFR");
                    }

                    // Listas
                    listaMasterFR = new List<FORMULARIO_RESPOSTA>();
                    Session["ListaCRMFR"] = null;
                    Session["IncluirCRMFR"] = 1;
                    Session["CRMNovoFR"] = item.FORE_CD_ID;
                    Session["IdCRMFR"] = item.FORE_CD_ID;
                    return RedirectToAction("MontarTelaCRMFR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarProcessoCRMFR(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Monta listas
            ViewBag.Origem = new SelectList(origApp.GetAllItens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");

            // Recupera
            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);
            Session["CRMFR"] = item;
            ViewBag.Incluir = (Int32)Session["IncluirCRMFR"];

            // Mensagens
            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 11)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0039", CultureInfo.CurrentCulture));
                }
            }

            // Monta view
            Session["VoltaCRMFR"] = 1;
            objetoFRAntes = item;
            Session["IdCRMFR"] = id;
            FormularioRespostaViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA, FormularioRespostaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarProcessoCRMFR(FormularioRespostaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Origem = new SelectList(origApp.GetAllItens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");

            // Indicadores
            ViewBag.Incluir = (Int32)Session["IncluirCRMFR"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    FORMULARIO_RESPOSTA item = Mapper.Map<FormularioRespostaViewModel, FORMULARIO_RESPOSTA>(vm);
                    Int32 volta = frApp.ValidateEdit(item, (FORMULARIO_RESPOSTA)Session["CRMFR"], usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("MontarTelaCRMFR");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("MontarTelaCRMFR");
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 62;
                        return RedirectToAction("MontarTelaCRMFR");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 63;
                        return RedirectToAction("MontarTelaCRMFR");
                    }

                    // Verificar se já tem pasta






                    // Sucesso
                    listaMasterFR = new List<FORMULARIO_RESPOSTA>();
                    Session["ListaCRMFR"] = null;
                    Session["IncluirCRMFR"] = 0;

                    if (Session["FiltroCRMFR"] != null)
                    {
                        FiltrarCRMFR((FORMULARIO_RESPOSTA)Session["FiltroCRMFR"]);
                    }
                    return RedirectToAction("VoltarBaseCRMFR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VisualizarProcessoCRMFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            Session["IdCRMFR"] = id;
            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);
            FormularioRespostaViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA, FormularioRespostaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult AcompanhamentoProcessoCRMFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Mensagens
            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 42)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 43)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 44)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture));
                }
            }

            // Processa...
            Session["IdCRMFR"] = id;
            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);
            FormularioRespostaViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA, FormularioRespostaViewModel>(item);
            List<FORMULARIO_RESPOSTA_ACAO> acoes = item.FORMULARIO_RESPOSTA_ACAO.ToList().OrderByDescending(p => p.FRAC_DT_CRIACAO).ToList();
            FORMULARIO_RESPOSTA_ACAO acao = acoes.Where(p => p.FRAC_IN_STATUS == 1).FirstOrDefault();
            Session["AcoesFR"] = acoes;
            Session["CRMFR"] = item;
            Session["VoltaCRM"] = 11;
            ViewBag.Acoes = acoes;
            ViewBag.Acao = acao;
            return View(vm);
        }

        [HttpGet]
        public ActionResult EnviarSMSClienteFR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 crm = (Int32)Session["IdCRMFR"];
            FORMULARIO_RESPOSTA item = frApp.GetItemById(crm);
            //CLIENTE cont = cliApp.GetItemById(id);
            //Session["Cliente"] = cont;
            ViewBag.Cliente = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.FORE_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.FORE_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSClienteFR(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSCliente(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRMFR", new { id = idNot });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult IncluirComentarioCRMFR()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdCRM"];
            FORMULARIO_RESPOSTA item = frApp.GetItemById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            FORMULARIO_RESPOSTA_COMENTARIO coment = new FORMULARIO_RESPOSTA_COMENTARIO();
            FormularioRespostaComentarioViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA_COMENTARIO, FormularioRespostaComentarioViewModel>(coment);
            vm.FRCO_DT_COMENTARIO = DateTime.Now;
            vm.FRCO_IN_ATIVO = 1;
            vm.FORE_CD_ID = item.FORE_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentarioCRMFR(FormularioRespostaComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMULARIO_RESPOSTA_COMENTARIO item = Mapper.Map<FormularioRespostaComentarioViewModel, FORMULARIO_RESPOSTA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FORMULARIO_RESPOSTA not = frApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.FORMULARIO_RESPOSTA_COMENTARIO.Add(item);
                    objetoFRAntes = not;
                    Int32 volta = frApp.ValidateEdit(not, objetoFRAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRMFR", new { id = idNot });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAcaoFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode editar ação
            FORMULARIO_RESPOSTA_ACAO item = frApp.GetAcaoById(id);
            if (item.FRAC_IN_STATUS > 2)
            {
                Session["MensCRM"] = 43;
                return RedirectToAction("VoltarAcompanhamentoCRMFR");
            }

            // Prepara view
            ViewBag.Usuarios = new SelectList(usuApp.GetAllSistema().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            // Monta Status
            List<SelectListItem> status = new List<SelectListItem>();
            if (item.FRAC_IN_STATUS == 1)
            {
                status.Add(new SelectListItem() { Text = "Pendente", Value = "2" });
                status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
            }
            else if (item.FRAC_IN_STATUS == 2)
            {
                status.Add(new SelectListItem() { Text = "Ativa", Value = "1" });
                status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
            }

            // Processa
            objetoFRAntes = (FORMULARIO_RESPOSTA)Session["CRMFR"];
            FormularioRespostaAcaoViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA_ACAO, FormularioRespostaAcaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAcaoFR(FormularioRespostaAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = new SelectList(usuApp.GetAllSistema().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMULARIO_RESPOSTA_ACAO item = Mapper.Map<FormularioRespostaAcaoViewModel, FORMULARIO_RESPOSTA_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = frApp.ValidateEditAcao(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAcompanhamentoCRMFR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAcaoFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            FORMULARIO_RESPOSTA_ACAO item = frApp.GetAcaoById(id);
            objetoFRAntes = (FORMULARIO_RESPOSTA)Session["CRMFR"];
            item.FRAC_IN_ATIVO = 0;
            item.FRAC_IN_STATUS = 4;
            Int32 volta = frApp.ValidateEditAcao(item);
            return RedirectToAction("VoltarAcompanhamentoCRMFR");
        }

        [HttpGet]
        public ActionResult ReativarAcaoFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode reativar ação
            List<FORMULARIO_RESPOSTA_ACAO> acoes = (List<FORMULARIO_RESPOSTA_ACAO>)Session["AcoesFR"];
            if (acoes.Where(p => p.FRAC_IN_STATUS == 1).ToList().Count > 0)
            {
                Session["MensCRM"] = 44;
                return RedirectToAction("VoltarAcompanhamentoCRMFR");
            }

            // Processa
            FORMULARIO_RESPOSTA_ACAO item = frApp.GetAcaoById(id);
            objetoFRAntes = (FORMULARIO_RESPOSTA)Session["CRMFR"];
            item.FRAC_IN_ATIVO = 1;
            item.FRAC_IN_STATUS = 1;
            Int32 volta = frApp.ValidateEditAcao(item);
            return RedirectToAction("VoltarAcompanhamentoCRMFR");
        }

        public ActionResult VerAcaoFR(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            FORMULARIO_RESPOSTA_ACAO item = frApp.GetAcaoById(id);
            objetoFRAntes = (FORMULARIO_RESPOSTA)Session["CRMFR"];
            FormularioRespostaAcaoViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA_ACAO, FormularioRespostaAcaoViewModel>(item);
            return View(vm);
        }

        public ActionResult VerAcoesUsuarioCRMFR()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            List<FORMULARIO_RESPOSTA_ACAO> lista = frApp.GetAllAcoes().Where(p => p.USUA_CD_ID2 == usuario.USUA_CD_ID).OrderByDescending(m => m.FRAC_DT_PREVISTA).ToList();
            ViewBag.Lista = lista;
            return View();
        }

        [HttpGet]
        public ActionResult IncluirAcaoFR()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.USUA_IN_SISTEMA == 0)
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMFR");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode inlcuir ação
            List<FORMULARIO_RESPOSTA_ACAO> acoes = (List<FORMULARIO_RESPOSTA_ACAO>)Session["AcoesFR"];
            if (acoes.Where(p => p.FRAC_IN_STATUS == 1).ToList().Count > 0)
            {
                Session["MensCRM"] = 42;
                return RedirectToAction("VoltarAcompanhamentoCRMFR");
            }

            // Prepara view
            ViewBag.Usuarios = new SelectList(usuApp.GetAllSistema().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> agenda = new List<SelectListItem>();
            agenda.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            agenda.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Agenda = new SelectList(agenda, "Value", "Text");

            FORMULARIO_RESPOSTA_ACAO item = new FORMULARIO_RESPOSTA_ACAO();
            FormularioRespostaAcaoViewModel vm = Mapper.Map<FORMULARIO_RESPOSTA_ACAO, FormularioRespostaAcaoViewModel>(item);
            vm.FORE_CD_ID = (Int32)Session["IdCRMFR"];
            vm.FRAC_IN_ATIVO = 1;
            vm.FRAC_DT_CRIACAO = DateTime.Now;
            vm.FRAC_IN_STATUS = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcaoFR(FormularioRespostaAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = new SelectList(usuApp.GetAllSistema().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMULARIO_RESPOSTA_ACAO item = Mapper.Map<FormularioRespostaAcaoViewModel, FORMULARIO_RESPOSTA_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = frApp.ValidateCreateAcao(item, usuarioLogado);

                    // Verifica retorno
                    return RedirectToAction("VoltarAcompanhamentoCRMFR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSCliente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa SMS
            CONFIGURACAO conf = confApp.GetAll().First();

            // Monta token
            String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = vm.MENS_TX_SMS;

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(vm.MENS_TX_SMS);
            if (!String.IsNullOrEmpty(vm.LINK))
            {
                if (!vm.LINK.Contains("www."))
                {
                    vm.LINK = "www." + vm.LINK;
                }
                if (!vm.LINK.Contains("http://"))
                {
                    vm.LINK = "http://" + vm.LINK;
                }
                str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                texto += "  " + vm.LINK;
            }
            String body = str.ToString();
            String smsBody = body;
            String erro = null;

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                String listaDest = "55" + Regex.Replace(vm.MODELO, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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
            return 0;
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult EnviarEMailClienteFR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 66)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
            }

            // recupera cliente e assinante
            FORMULARIO_RESPOSTA cli = frApp.GetItemById(id);
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            Session["ClienteFR"] = cli;

            // Prepara mensagem
            String header = "Prezado <b>" + cli.FORE_NM_NOME + "</b>";
            String body = String.Empty;
            String footer = "<b> ERPSys - ERP em Nuvem para Gestão Empresarial</b>";

            // Mota vm
            MensagemViewModel vm = new MensagemViewModel();
            vm.ASSI_CD_ID = idAss;
            vm.MENS_DT_CRIACAO = DateTime.Now;
            vm.MENS_IN_ATIVO = 1;
            vm.NOME = cli.FORE_NM_NOME;
            vm.ID = id;
            vm.MODELO = cli.FORE_NM_EMAIL;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.MENS_NM_CABECALHO = header;
            vm.MENS_NM_RODAPE = footer;
            vm.MENS_IN_TIPO = 1;
            vm.ID = cli.FORE_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarEMailClienteFR(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                Int32 idNot = (Int32)Session["IdCRMFR"];
                try
                {
                    // Checa corpo da mensagem
                    if (String.IsNullOrEmpty(vm.MENS_TX_TEXTO))
                    {
                        Session["MensMensagem"] = 66;
                        return RedirectToAction("EnviarEMailClienteFR");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailClienteFR(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRMFR", new { id = idNot });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailClienteFR(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera cliente
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetAll().First();

            // Prepara cabeçalho
            String cab = "Prezado <b>" + vm.NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b> ERPSys - ERP em Nuvem para Gestão Empresarial</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO;
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
            }
            String body = str.ToString();
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
            Email mensagem = new Email();
            mensagem.ASSUNTO = "Contato";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_DESTINO = vm.MODELO;
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
            return 0;
        }

        [HttpPost]
        public JsonResult GetProcessosFR()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            listaMasterFR = frApp.GetAllItens();
            var listaHash = new List<Hashtable>();
            foreach (var item in listaMasterFR)
            {
                var hash = new Hashtable();
                hash.Add("FORE_IN_STATUS", item.FORE_IN_STATUS);
                hash.Add("FORE_CD_ID", item.FORE_CD_ID);
                hash.Add("FORE_NM_PROCESSO", item.FORE_NM_PROCESSO);
                hash.Add("CRM1_DT_CRIACAO", item.FORE_DT_CADASTRO.Value.ToString("dd/MM/yyyy"));
                if (item.FORE_DT_ENCERRAMENTO != null)
                {
                    hash.Add("CRM1_DT_ENCERRAMENTO", item.FORE_DT_ENCERRAMENTO.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    hash.Add("CRM1_DT_ENCERRAMENTO", "-");
                }
                hash.Add("CRM1_NM_CLIENTE", item.FORE_NM_NOME);
                listaHash.Add(hash);
            }
            return Json(listaHash);
        }

        [HttpPost]
        public JsonResult EditarStatusCRMFR(Int32 id, Int32 status, DateTime? dtEnc)
        {
            FORMULARIO_RESPOSTA crm = frApp.GetById(id);
            crm.FORE_IN_STATUS = status;
            crm.FORE_DT_ENCERRAMENTO = dtEnc;
            crm.FORE_DS_MOTIVO_ENCERRAMENTO = "Processo Encerrado";

            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Int32 volta = frApp.ValidateEdit(crm, crm, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    return Json(PlatMensagens_Resources.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(PlatMensagens_Resources.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }

                Session["ListaCRMFR"] = null;
                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Json(ex.Message);
            }
        }




    }
}