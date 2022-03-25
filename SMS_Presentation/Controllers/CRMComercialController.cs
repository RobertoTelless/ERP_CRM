using System;
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
using Correios.Net;
using Canducci.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EntitiesServices.Attributes;
using OfficeOpenXml.Table;
using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using CrossCutting;
using System.Net.Mail;
using System.Net.Http;

namespace ERP_CRM_Solution.Controllers
{
    public class CRMComercialController : Controller
    {
        private readonly ICRMComercialAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IMensagemAppService menApp;
        private readonly IAgendaAppService ageApp;
        private readonly IClienteAppService cliApp;
        private readonly IAtendimentoAppService ateApp;
        private readonly IProdutoAppService proApp;
        private readonly IContaBancariaAppService cbApp;

        private String msg;
        private Exception exception;
        CRM_COMERCIAL objeto = new CRM_COMERCIAL();
        CRM_COMERCIAL objetoAntes = new CRM_COMERCIAL();
        List<CRM_COMERCIAL> listaMaster = new List<CRM_COMERCIAL>();
        String extensao;

        public CRMComercialController(ICRMComercialAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IMensagemAppService menApps, IAgendaAppService ageApps, IClienteAppService cliApps, IAtendimentoAppService ateApps, IProdutoAppService proApps, IContaBancariaAppService cbApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            menApp = menApps;
            ageApp = ageApps;
            cliApp = cliApps;
            ateApp = ateApps;
            proApp = proApps;
            cbApp = cbApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult GetPrecoProduto(Int32 id, Int32? fili)
        {
            var result = new Hashtable();
            if (fili != null)
            {
                var prod = proApp.GetById(id).PRODUTO_TABELA_PRECO.First(x => x.FILI_CD_ID == fili);
                result.Add("custo", prod.PRTP_VL_PRECO == null ? 0 : prod.PRTP_VL_PRECO);
                result.Add("markup", prod.PRTP_NR_MARKUP == null ? 0 : prod.PRTP_NR_MARKUP);
                result.Add("unidade", prod.PRODUTO.UNIDADE.UNID_NM_NOME);
            }
            else
            {
                var prod = proApp.GetById(id);
                result.Add("custo", prod.PRTP_VL_PRECO == null ? 0 : prod.PRTP_VL_PRECO);
                result.Add("markup", prod.PROD_VL_MARKUP_PADRAO == null ? 0 : prod.PROD_VL_MARKUP_PADRAO);
                result.Add("unidade", prod.UNIDADE == null ? "" : prod.UNIDADE.UNID_NM_NOME);
            }
            return Json(result);
        }

        public FileResult DownloadCRM(Int32 id)
        {
            CRM_COMERCIAL_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.CRCA_AQ_ARQUIVO;
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
        public ActionResult MontarTelaCRMComercial()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            if ((List<CRM_COMERCIAL>)Session["ListaCRM"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCRM"] = listaMaster;
            }
            Session["CRM"] = null;
            List<CRM_COMERCIAL> lista = (List<CRM_COMERCIAL>)Session["ListaCRM"];
            ViewBag.Listas = (List<CRM_COMERCIAL>)Session["ListaCRM"];
            ViewBag.Title = "CRM";
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> visao = new List<SelectListItem>();
            visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
            visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
            ViewBag.Visao = new SelectList(visao, "Value", "Text");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Oportunidade", Value = "1" });
            status.Add(new SelectListItem() { Text = "Proposta", Value = "2" });
            status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovado", Value = "4" });
            status.Add(new SelectListItem() { Text = "Reprovado", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelado", Value = "8" });
            status.Add(new SelectListItem() { Text = "Vendido", Value = "6" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "7" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            Session["IncluirCRM"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Pedidos = lista.Count;
            ViewBag.Encerradas = lista.Count(p => p.CRMC_IN_STATUS == 7);
            ViewBag.Canceladas = lista.Count(p => p.CRMC_IN_STATUS == 8);
            ViewBag.Atrasadas = lista.Count(p => p.CRMC_DT_PREVISTA < DateTime.Today.Date && p.CRMC_IN_STATUS != 7 && p.CRMC_IN_STATUS != 8);
            ViewBag.EncerradasLista = lista.Where(p => p.CRMC_IN_STATUS == 7).ToList();
            ViewBag.CanceladasLista = lista.Where(p => p.CRMC_IN_STATUS == 8).ToList();
            ViewBag.AtrasadasLista = lista.Where(p => p.CRMC_DT_PREVISTA < DateTime.Today.Date && p.CRMC_IN_STATUS != 7 && p.CRMC_IN_STATUS != 8).ToList();
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.CotacaoEnvioSucesso = (Int32)Session["SMSEmailEnvio"];

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
                if ((Int32)Session["MensCRM"] == 70)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0119", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 71)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 72)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensCRM"] = 0;
            Session["VoltaCRM"] = 1;
            Session["IncluirCliente"] = 0;
            objeto = new CRM_COMERCIAL();
            if (Session["FiltroCRM"] != null)
            {
                objeto = (CRM_COMERCIAL)Session["FiltroCRM"];
            }
            return View(objeto);
        }

        [HttpGet]
        public ActionResult MontarTelaKanbanCRMComercial()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            if ((List<CRM_COMERCIAL>)Session["ListaCRM"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCRM"] = listaMaster;
            }
            Session["CRM"] = null;
            ViewBag.Listas = (List<CRM_COMERCIAL>)Session["ListaCRM"];
            ViewBag.Title = "CRM";
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> visao = new List<SelectListItem>();
            visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
            visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
            ViewBag.Visao = new SelectList(visao, "Value", "Text");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Oportunidade", Value = "1" });
            status.Add(new SelectListItem() { Text = "Proposta", Value = "2" });
            status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovado", Value = "4" });
            status.Add(new SelectListItem() { Text = "Reprovado", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelado", Value = "8" });
            status.Add(new SelectListItem() { Text = "Vendido", Value = "6" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "7" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            Session["IncluirCRM"] = 0;

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
            Session["VoltaCRM"] = 1;
            Session["IncluirCliente"] = 0;
            objeto = new CRM_COMERCIAL();
            if (Session["FiltroCRM"] != null)
            {
                objeto = (CRM_COMERCIAL)Session["FiltroCRM"];
            }
            return View(objeto);
        }
        
        public ActionResult RetirarFiltroCRMComercial()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCRM"] = null;
            Session["FiltroCRM"] = null;
            return RedirectToAction("MontarTelaCRMComercial");
        }

        public ActionResult MostrarTudoCRMComercial()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaCRM"] = listaMaster;
            return RedirectToAction("MontarTelaCRMComercial");
        }

        [HttpPost]
        public ActionResult FiltrarCRMComercial(CRM_COMERCIAL item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<CRM_COMERCIAL> listaObj = new List<CRM_COMERCIAL>();
                Session["FiltroCRM"] = item;
                Int32 volta = baseApp.ExecuteFilter(item.CRMC_IN_STATUS, item.CRMC_DT_CRIACAO, item.CRMC_DT_CANCELAMENTO, item.CROR_CD_ID, item.CRMC_IN_ATIVO, item.CRMC_NM_NOME, item.CRMC_DS_DESCRICAO, item.CRMC_IN_ESTRELA, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCRM"] = 1;
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaCRM"] = listaObj;
                return RedirectToAction("MontarTelaCRMComercial");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCRMComercial");
            }
        }

        public ActionResult VoltarBaseCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRMComercial");
            }
            if ((Int32)Session["VoltaCRM"] == 11)
            {
                return RedirectToAction("MontarTelaCRMComercial", "CRM");
            }
            if ((Int32)Session["VoltaCRM"] == 12)
            {
                return RedirectToAction("VoltarAnexoAtendimento", "Atendimento");
            }
            if ((Int32)Session["VoltaCRM"] == 30)
            {
                return RedirectToAction("VoltarAnexoAtendimento", "Atendimento");
            }
            Session["VoltaCRM"] = 0;
            return RedirectToAction("MontarTelaCRMComercial");
        }

        [HttpGet]
        public ActionResult ExcluirProcessoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_COMERCIAL item = baseApp.GetItemById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            item.CRMC_IN_ATIVO = 2;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCRM"] = 4;
                return RedirectToAction("MontarTelaCRMComercial");
            }
            Session["ListaCRM"] = null;
            return RedirectToAction("MontarTelaCRMComercial");
        }

        [HttpGet]
        public ActionResult ReativarProcessoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumVendas"] <= num)
            {
                Session["MensCRM"] = 50;
                return RedirectToAction("MontarTelaCRMComercial", "CRM");
            }

            CRM_COMERCIAL item = baseApp.GetItemById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            item.CRMC_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaCRM"] = null;
            return RedirectToAction("MontarTelaCRMComercial");
        }

        [HttpGet]
        public ActionResult EstrelaSim(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_COMERCIAL item = baseApp.GetItemById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            item.CRMC_IN_ESTRELA = 1;
            Int32 volta = baseApp.ValidateEdit(item, item);
            Session["ListaCRM"] = null;
            return RedirectToAction("MontarTelaCRMComercial");
        }

        [HttpGet]
        public ActionResult EstrelaNao(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_COMERCIAL item = baseApp.GetItemById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            item.CRMC_IN_ESTRELA = 0;
            Int32 volta = baseApp.ValidateEdit(item, item);
            Session["ListaCRM"] = null;
            return RedirectToAction("MontarTelaCRMComercial");
        }

        [HttpGet]
        public ActionResult EncerrarAcao(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_COMERCIAL_ACAO item = baseApp.GetAcaoById(id);
            item.CRCA_IN_STATUS = 3;
            Int32 volta = baseApp.ValidateEditAcao(item);
            return RedirectToAction("AcompanhamentoProcessoCRMComercial");
        }

        public ActionResult GerarRelatorioListaCRM()
        {            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ProcessosComerciaisLista" + "_" + data + ".pdf";
            List<CRM_COMERCIAL> lista = (List<CRM_COMERCIAL>)Session["ListaCRM"];
            CRM_COMERCIAL filtro = (CRM_COMERCIAL)Session["FiltroCRM"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontO = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.ORANGE);
            Font meuFontP = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
            Font meuFontE = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
            Font meuFontD = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);
            Font meuFontS = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/favicon_SystemBR.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Processos Comerciais - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(new float[] { 70f, 150f, 160f, 80f, 150f, 80f, 80f, 80f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Processos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Favorito", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Status", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Próxima Ação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data Prevista", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Origem", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Situação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CRM_COMERCIAL item in lista)
            {
                if (item.CRMC_IN_ESTRELA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Sim", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_ESTRELA == 0)
                {
                    cell = new PdfPCell(new Paragraph("Não", meuFontO))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.CRMC_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                if (item.CRMC_IN_STATUS == 1)
                {
                    cell = new PdfPCell(new Paragraph("Oportunidade", meuFontO))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_STATUS == 2)
                {
                    cell = new PdfPCell(new Paragraph("Proposta", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_STATUS == 3)
                {
                    cell = new PdfPCell(new Paragraph("Em Aprovação", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_STATUS == 4)
                {
                    cell = new PdfPCell(new Paragraph("Aprovado", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_STATUS == 5)
                {
                    cell = new PdfPCell(new Paragraph("Reprovado", meuFontD))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_STATUS == 6)
                {
                    cell = new PdfPCell(new Paragraph("Vendido", meuFontP))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_STATUS == 7)
                {
                    cell = new PdfPCell(new Paragraph("Encerrado", meuFontP))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_STATUS == 8)
                {
                    cell = new PdfPCell(new Paragraph("Cancelado", meuFontO))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);

                if (item.CRM_COMERCIAL_ACAO.Count > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CRM_COMERCIAL_ACAO.Where(p => p.CRCA_IN_ATIVO == 1).OrderByDescending(m => m.CRCA_DT_PREVISTA).FirstOrDefault().CRCA_DS_DESCRICAO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFontP))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);

                if (item.CRM_COMERCIAL_ACAO.Count > 0)
                {
                    if (item.CRM_COMERCIAL_ACAO.Where(p => p.CRCA_IN_ATIVO == 1).OrderByDescending(m => m.CRCA_DT_PREVISTA).FirstOrDefault().CRCA_DT_PREVISTA.Date >= DateTime.Today.Date)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRM_COMERCIAL_ACAO.Where(p => p.CRCA_IN_ATIVO == 1).OrderByDescending(m => m.CRCA_DT_PREVISTA).FirstOrDefault().CRCA_DT_PREVISTA.ToShortDateString(), meuFontE))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph(item.CRM_COMERCIAL_ACAO.Where(p => p.CRCA_IN_ATIVO == 1).OrderByDescending(m => m.CRCA_DT_PREVISTA).FirstOrDefault().CRCA_DT_PREVISTA.ToShortDateString(), meuFontD))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFontP))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.CRM_ORIGEM.CROR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                if (item.CRMC_IN_ATIVO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Ativo", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRMC_IN_ATIVO == 2)
                {
                    cell = new PdfPCell(new Paragraph("Arquivado", meuFontO))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CRMC_IN_STATUS > 0)
                {
                    if (filtro.CRMC_IN_STATUS == 1)
                    {
                        parametros += "Status: Oportunidade";
                    }
                    else if (filtro.CRMC_IN_STATUS == 2)
                    {
                        parametros += "Status: Proposta";
                    }
                    else if (filtro.CRMC_IN_STATUS == 3)
                    {
                        parametros += "Status: Em Aprovação";
                    }
                    else if (filtro.CRMC_IN_STATUS == 4)
                    {
                        parametros += "Status: Aprovado";
                    }
                    else if (filtro.CRMC_IN_STATUS == 5)
                    {
                        parametros += "Status: Reprovado";
                    }
                    else if (filtro.CRMC_IN_STATUS == 6)
                    {
                        parametros += "Status: Vendido";
                    }
                    else if (filtro.CRMC_IN_STATUS == 7)
                    {
                        parametros += "Status: Encerrado";
                    }
                    else if (filtro.CRMC_IN_STATUS == 8)
                    {
                        parametros += "Status: Camcelado";
                    }
                    ja = 1;
                }

                if (filtro.CRMC_DT_CRIACAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data Início: " + filtro.CRMC_DT_CRIACAO.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += "e Data Início: " + filtro.CRMC_DT_CRIACAO.ToShortDateString();
                    }
                }

                if (filtro.CRMC_DT_CANCELAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data Final: " + filtro.CRMC_DT_CANCELAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += "e Data Final: " + filtro.CRMC_DT_CANCELAMENTO.Value.ToShortDateString();
                    }
                }

                if (filtro.CROR_CD_ID > 0)
                {
                    if (ja == 0)
                    {
                        parametros += "Origem: " + filtro.CRM_ORIGEM.CROR_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Origem: " + filtro.CRM_ORIGEM.CROR_NM_NOME;
                    }
                }

                if (filtro.CRMC_IN_ATIVO > 0)
                {
                    if (ja == 0)
                    {
                        if (filtro.CRMC_IN_ATIVO == 1)
                        {
                            parametros += "Situação: Ativo";
                        }
                        else if (filtro.CRMC_IN_ATIVO == 2)
                        {
                            parametros += "Situação: Arquivado";
                        }
                        ja = 1;
                    }
                    else
                    {
                        if (filtro.CRMC_IN_ATIVO == 1)
                        {
                            parametros += "e Situação: Ativo";
                        }
                        else if (filtro.CRMC_IN_ATIVO == 2)
                        {
                            parametros += "e Situação: Arquivado";
                        }
                    }
                }

                if (filtro.CRMC_IN_ESTRELA > 0)
                {
                    if (ja == 0)
                    {
                        if (filtro.CRMC_IN_ESTRELA == 1)
                        {
                            parametros += "Favorito: Sim";
                        }
                        else if (filtro.CRMC_IN_ESTRELA == 0)
                        {
                            parametros += "Favorito: Não";
                        }
                        ja = 1;
                    }
                    else
                    {
                        if (filtro.CRMC_IN_ESTRELA == 1)
                        {
                            parametros += "e Favorito: Sim";
                        }
                        else if (filtro.CRMC_IN_ESTRELA == 0)
                        {
                            parametros += "e Favorito: Não";
                        }
                    }
                }

                if (filtro.CRMC_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Título: " + filtro.CRMC_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Título: " + filtro.CRMC_NM_NOME;
                    }
                }

                if (filtro.CRMC_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Geral: " + filtro.CRMC_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Geral: " + filtro.CRMC_DS_DESCRICAO;
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("MontarTelaCRMComercial");
        }

        [HttpGet]
        public ActionResult IncluirProcessoCRMComercial()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial", "CRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumVendas"] <= num)
            {
                Session["MensCRM"] = 50;
                return RedirectToAction("MontarTelaCRMComercial", "CRMComercial");
            }

            // Prepara listas
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            List<SelectListItem> tipo = new List<SelectListItem>();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");

            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Prepara view
            Session["CRMNovo"] = 0;
            Session["VoltaCliente"] = 8;
            CRM_COMERCIAL item = new CRM_COMERCIAL();
            CRMComercialViewModel vm = Mapper.Map<CRM_COMERCIAL, CRMComercialViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CRMC_DT_CRIACAO = DateTime.Today.Date;
            vm.CRMC_IN_ATIVO = 1;
            vm.CRMC_IN_STATUS = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.CRMC_DT_PREVISTA = DateTime.Today.Date.AddDays(30);
            vm.CRMC_DT_ENCERRAMENTO = DateTime.MinValue;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirProcessoCRMComercial(CRMComercialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ListaITPC"] == null)
                    {
                        ModelState.AddModelError("", "Nenhum Item de Pedido cadastrado no pedido");
                        return View(vm);
                    }

                    // Executa a operação
                    CRM_COMERCIAL item = Mapper.Map<CRMComercialViewModel, CRM_COMERCIAL>(vm);
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 3;
                        return RedirectToAction("MontarTelaCRMComercial");
                    }

                    // Carrega foto e processa alteracao
                    item.CRMC_AQ_IMAGEM = "~/Images/icone_imagem.jpg";
                    volta = baseApp.ValidateEdit(item, item);

                    // Cria pasta
                    String caminho = "/Imagens/" + idAss.ToString() + "/CRM-Comercial/" + item.CRMC_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/CRM-Comercial/" + item.CRMC_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Acerta numero do pedido
                    item.CRMC_NR_NUMERO = item.CRMC_CD_ID.ToString();
                    volta = baseApp.ValidateEdit(item, item, usuario);

                    // Listas
                    listaMaster = new List<CRM_COMERCIAL>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRMC_CD_ID;
                    Session["IdCRM"] = item.CRMC_CD_ID;

                    // Processa Anexos
                    if (Session["FileQueueCRMComercial"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCRMComercial"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueCRMComercial(file);
                            }
                        }
                        Session["FileQueueCRMComercial"] = null;
                    }

                    foreach (var itpc in (List<CRM_COMERCIAL_ITEM>)Session["ListaITPC"])
                    {
                        itpc.CRCI_QN_QUANTIDADE_REVISADA = itpc.CRCI_QN_QUANTIDADE;
                        itpc.CRCI_IN_ATIVO = 1;
                        itpc.CRMC_CD_ID = item.CRMC_CD_ID;
                        PRODUTO prod = proApp.GetItemById((Int32)itpc.PROD_CD_ID);
                        itpc.CRCI_VL_VALOR = prod.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).Count() > 0 ? prod.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).First().PRTP_VL_CUSTO : null;
                        Int32 voltaItem = baseApp.ValidateCreateItemCRM(itpc);
                    }

                    // Processa voltas
                    if ((Int32)Session["VoltaCRM"] == 3)
                    {
                        Session["VoltaCRM"] = 0;
                        Session["CRMAtendimento"] = 0;
                        return RedirectToAction("IncluirProcessoCRMComercial", "CRMComercial");
                    }
                    Session["CRMAtendimento"] = 0;
                    Session["IdVolta"] = item.CRMC_CD_ID;
                    Session["IdCompra"] = item.CRMC_CD_ID;
                    return RedirectToAction("MontarTelaCRMComercial");
                }
                catch (Exception ex)
                {
                    Session["ListaITPC"] = null;
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
            Session["FileQueueCRM"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueCRMComercial(FileQueue file)
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
                return RedirectToAction("VoltarAnexoCRMComercial");
            }

            CRM_COMERCIAL item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                return RedirectToAction("VoltarAnexoCRMComercial");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/CRM-Comercial/" + item.CRMC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CRM_COMERCIAL_ANEXO foto = new CRM_COMERCIAL_ANEXO();
            foto.CRCA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRCA_DT_ANEXO = DateTime.Today;
            foto.CRCA_IN_ATIVO = 1;
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
            foto.CRCA_IN_TIPO = tipo;
            foto.CRCA_NM_TITULO = fileName;
            foto.CRMC_CD_ID = item.CRMC_CD_ID;

            item.CRM_COMERCIAL_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAnexoCRMComercial");
        }

        [HttpPost]
        public ActionResult UploadFileCRMComercial(HttpPostedFileBase file)
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
                return RedirectToAction("VoltarAnexoCRM");
            }

            CRM_COMERCIAL item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                return RedirectToAction("VoltarAnexoCRMComercial");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/CRM-Comercial/" + item.CRMC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CRM_COMERCIAL_ANEXO foto = new CRM_COMERCIAL_ANEXO();
            foto.CRCA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRCA_DT_ANEXO = DateTime.Today;
            foto.CRCA_IN_ATIVO = 1;
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
            foto.CRCA_IN_TIPO = tipo;
            foto.CRCA_NM_TITULO = fileName;
            foto.CRMC_CD_ID = item.CRMC_CD_ID;

            item.CRM_COMERCIAL_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAnexoCRMComercial");
        }

        public ActionResult VoltarAnexoCRMComercial()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            return RedirectToAction("EditarProcessoCRMComercial", new { id = (Int32)Session["IdCRM"] });
        }

        [HttpGet]
        public ActionResult VerAnexoCRMComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial", "CRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CRM_COMERCIAL_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult CancelarProcessoCRMComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial", "CRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Motivos = new SelectList(baseApp.GetAllMotivoCancelamento(idAss).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM_COMERCIAL item = baseApp.GetItemById(id);

            // Checa ações
            Session["TemAcao"] = 0;
            if (item.CRM_COMERCIAL_ACAO.Where(p => p.CRCA_IN_ATIVO == 1).ToList().Count > 0)
            {
                Session["TemAcao"] = 1;
            }

            // Recupera total
            Decimal custo = 0;
            foreach (var i in item.CRM_COMERCIAL_ITEM)
            {
                if (i.CRCI_VL_VALOR != null)
                {
                    custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                }
            }
            Session["CustoTotal"] = custo;

            // Prepara view
            CRMComercialViewModel vm = Mapper.Map<CRM_COMERCIAL, CRMComercialViewModel>(item);
            vm.CRMC_DT_CANCELAMENTO = DateTime.Today.Date;
            vm.CRMC_IN_STATUS = 8;
            vm.VALOR_TOTAL = custo;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CancelarProcessoCRMComercial(CRMComercialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Motivos = new SelectList(baseApp.GetAllMotivoCancelamento(idAss).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");

            if (ModelState.IsValid)
            {
                Session["CustoTotal"] = 0;
                try
                {
                    // Executa a operação
                    CRM_COMERCIAL item = Mapper.Map<CRMComercialViewModel, CRM_COMERCIAL>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCancelamento(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 70;
                        return RedirectToAction("MontarTelaCRMComercial");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 71;
                        return RedirectToAction("MontarTelaCRMComercial");
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 72;
                        return RedirectToAction("MontarTelaCRMComercial");
                    }

                    // Listas
                    listaMaster = new List<CRM_COMERCIAL>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRMC_CD_ID;
                    Session["IdCRM"] = item.CRMC_CD_ID;
                    return RedirectToAction("MontarTelaCRMComercial");
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
        public ActionResult EditarProcessoCRMComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial", "CRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Monta listas
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");

            // Recupera
            CRM_COMERCIAL item = baseApp.GetItemById(id);
            Session["CRM"] = item;
            ViewBag.Incluir = (Int32)Session["IncluirCRM"];

            Decimal custo = 0;
            foreach (var i in item.CRM_COMERCIAL_ITEM)
            {
                if (i.CRCI_VL_VALOR != null)
                {
                    custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                }
            }

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
            Session["VoltaCRM1"] = 1;
            objetoAntes = item;
            Session["IdCRM"] = id;
            CRMComercialViewModel vm = Mapper.Map<CRM_COMERCIAL, CRMComercialViewModel>(item);
            vm.VALOR_TOTAL = custo;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarProcessoCRMComercial(CRMComercialViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");

            // Indicadores
            ViewBag.Incluir = (Int32)Session["IncluirCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_COMERCIAL item = Mapper.Map<CRMComercialViewModel, CRM_COMERCIAL>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, (CRM_COMERCIAL)Session["CRM"], usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("MontarTelaCRMComercial");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("MontarTelaCRMComercial");
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 62;
                        return RedirectToAction("MontarTelaCRMComercial");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 63;
                        return RedirectToAction("MontarTelaCRMComercial");
                    }

                    // Sucesso
                    listaMaster = new List<CRM_COMERCIAL>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 0;

                    if (Session["FiltroCRM"] != null)
                    {
                        FiltrarCRMComercial((CRM_COMERCIAL)Session["FiltroCRM"]);
                    }
                    return RedirectToAction("VoltarBaseCRMComercial");
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
        public ActionResult VisualizarProcessoCRMComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial", "CRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["IdCRM"] = id;
            CRM_COMERCIAL item = baseApp.GetItemById(id);

            Decimal custo = 0;
            foreach (var i in item.CRM_COMERCIAL_ITEM)
            {
                if (i.CRCI_VL_VALOR != null)
                {
                    custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                }
            }
            ViewBag.CustoTotal = custo;
            CRMComercialViewModel vm = Mapper.Map<CRM_COMERCIAL, CRMComercialViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarContatoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAnexoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");

            CRM_COMERCIAL_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            Session["Contato"] = item;
            CRMComercialContatoViewModel vm = Mapper.Map<CRM_COMERCIAL_CONTATO, CRMComercialContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContatoComercial(CRMComercialContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Checa principal
                    CRM_COMERCIAL_CONTATO cont = (CRM_COMERCIAL_CONTATO)Session["Contato"];
                    if (cont.CRCO_IN_PRINCIPAL == 0)
                    {
                        if (((CRM_COMERCIAL)Session["CRM"]).CRM_COMERCIAL_CONTATO.Where(p => p.CRCO_IN_PRINCIPAL == 1).ToList().Count > 0 & vm.CRCO_IN_PRINCIPAL == 1)
                        {
                            Session["MensCRM"] = 50;
                            return RedirectToAction("VoltarAnexoCRMComercial");
                        }
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM_COMERCIAL_CONTATO item = Mapper.Map<CRMComercialContatoViewModel, CRM_COMERCIAL_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateEditContato(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCRMComercial");
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
        public ActionResult ExcluirContatoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAnexoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_COMERCIAL_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            item.CRCO_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoCRMComercial");
        }

        [HttpGet]
        public ActionResult ReativarContatoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAnexoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_COMERCIAL_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            item.CRCO_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoCRMComercial");
        }

        [HttpGet]
        public ActionResult IncluirContatoComercial()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAnexoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");

            CRM_COMERCIAL_CONTATO item = new CRM_COMERCIAL_CONTATO();
            CRMComercialContatoViewModel vm = Mapper.Map<CRM_COMERCIAL_CONTATO, CRMComercialContatoViewModel>(item);
            vm.CRMC_CD_ID = (Int32)Session["IdCRM"];
            vm.CRCO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContatoComercial(CRMComercialContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Checa principal
                    CRM_COMERCIAL crm = (CRM_COMERCIAL)Session["CRM"];
                    if (crm.CRM_COMERCIAL_CONTATO != null)
                    {
                        if (((CRM_COMERCIAL)Session["CRM"]).CRM_COMERCIAL_CONTATO.Where(p => p.CRCO_IN_PRINCIPAL == 1).ToList().Count > 0 & vm.CRCO_IN_PRINCIPAL == 1)
                        {
                            Session["MensCRM"] = 50;
                            return RedirectToAction("VoltarAnexoCRM");
                        }
                    }

                    // Executa a operação
                    CRM_COMERCIAL_CONTATO item = Mapper.Map<CRMComercialContatoViewModel, CRM_COMERCIAL_CONTATO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCRMComercial");
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
        public ActionResult AcompanhamentoProcessoCRMComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial", "CRMComercial");
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
            Session["IdCRM"] = id;
            CRM_COMERCIAL item = baseApp.GetItemById(id);
            CRMComercialViewModel vm = Mapper.Map<CRM_COMERCIAL, CRMComercialViewModel>(item);
            List<CRM_COMERCIAL_ACAO> acoes = item.CRM_COMERCIAL_ACAO.ToList().OrderByDescending(p => p.CRCA_DT_ACAO).ToList();
            CRM_COMERCIAL_ACAO acao = acoes.Where(p => p.CRCA_IN_STATUS == 1).FirstOrDefault();
            Session["Acoes"] = acoes;
            Session["CRM"] = item;
            Session["VoltaCRM"] = 10;
            ViewBag.Acoes = acoes;
            ViewBag.Acao = acao;
            return View(vm);
        }

        public ActionResult GerarRelatorioDetalheCRMComercial()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EnviarEMailContatoComercial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 crm = (Int32)Session["IdCRM"];
            CRM_COMERCIAL item = baseApp.GetItemById(crm);
            CRM_COMERCIAL_CONTATO cont = baseApp.GetContatoById(id);
            Session["Contato"] = cont;
            ViewBag.Contato = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.CRCO_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.CRCO_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailContatoComercial(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioEMailContato(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRMComercial", new { id = idNot });
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
        public ActionResult EnviarSMSContatoComercial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 crm = (Int32)Session["IdCRM"];
            CRM_COMERCIAL item = baseApp.GetItemById(crm);
            CRM_COMERCIAL_CONTATO cont = baseApp.GetContatoById(id);
            Session["Contato"] = cont;
            ViewBag.Contato = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.CRCO_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.CRCO_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSContatoComercial(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioSMSContato(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRMComercial", new { id = idNot });
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
        public ActionResult EnviarSMSClienteComercial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 crm = (Int32)Session["IdCRM"];
            CRM_COMERCIAL item = baseApp.GetItemById(crm);
            CLIENTE cont = cliApp.GetItemById(id);
            Session["Cliente"] = cont;
            ViewBag.Cliente = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.CLIE_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.CLIE_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSClienteComercial(MensagemViewModel vm)
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
                    return RedirectToAction("AcompanhamentoProcessoCRMComercial", new { id = idNot });
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
        public ActionResult IncluirComentarioCRMComercial()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdCRM"];
            CRM_COMERCIAL item = baseApp.GetItemById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            CRM_COMERCIAL_COMENTARIO_NOVA coment = new CRM_COMERCIAL_COMENTARIO_NOVA();
            CRMComercialComentarioViewModel vm = Mapper.Map<CRM_COMERCIAL_COMENTARIO_NOVA, CRMComercialComentarioViewModel>(coment);
            vm.CRCC_DT_COMENTARIO = DateTime.Now;
            vm.CRCC_IN_ATIVO = 1;
            vm.CRMC_CD_ID = item.CRMC_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentarioCRMComercial(CRMComercialComentarioViewModel vm)
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
                    CRM_COMERCIAL_COMENTARIO_NOVA item = Mapper.Map<CRMComercialComentarioViewModel, CRM_COMERCIAL_COMENTARIO_NOVA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM_COMERCIAL not = baseApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.CRM_COMERCIAL_COMENTARIO_NOVA.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRMComercial", new { id = idNot });
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
        public ActionResult EditarAcaoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode editar ação
            CRM_COMERCIAL_ACAO item = baseApp.GetAcaoById(id);
            if (item.CRCA_IN_STATUS > 2)
            {
                Session["MensCRM"] = 43;
                return RedirectToAction("VoltarAcompanhamentoCRMComercial");
            }

            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipoAcao(idAss).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            // Monta Status
            List<SelectListItem> status = new List<SelectListItem>();
            if (item.CRCA_IN_STATUS == 1)
            {
                status.Add(new SelectListItem() { Text = "Pendente", Value = "2" });
                status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
            }
            else if (item.CRCA_IN_STATUS == 2)
            {
                status.Add(new SelectListItem() { Text = "Ativa", Value = "1" });
                status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
            }

            // Processa
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            CRMComercialAcaoViewModel vm = Mapper.Map<CRM_COMERCIAL_ACAO, CRMComercialAcaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAcaoComercial(CRMComercialAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipoAcao(idAss).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_COMERCIAL_ACAO item = Mapper.Map<CRMComercialAcaoViewModel, CRM_COMERCIAL_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEditAcao(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAcompanhamentoCRMComercial");
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
        public ActionResult ExcluirAcaoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            CRM_COMERCIAL_ACAO item = baseApp.GetAcaoById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            item.CRCA_IN_ATIVO = 0;
            item.CRCA_IN_STATUS = 4;
            Int32 volta = baseApp.ValidateEditAcao(item);
            return RedirectToAction("VoltarAcompanhamentoCRMComercial");
        }

        [HttpGet]
        public ActionResult ReativarAcaoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode reativar ação
            List<CRM_COMERCIAL_ACAO> acoes = (List<CRM_COMERCIAL_ACAO>)Session["Acoes"];
            if (acoes.Where(p => p.CRCA_IN_STATUS == 1).ToList().Count > 0)
            {
                Session["MensCRM"] = 44;
                return RedirectToAction("VoltarAcompanhamentoCRMComercial");
            }

            // Processa
            CRM_COMERCIAL_ACAO item = baseApp.GetAcaoById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            item.CRCA_IN_ATIVO = 1;
            item.CRCA_IN_STATUS = 1;
            Int32 volta = baseApp.ValidateEditAcao(item);
            return RedirectToAction("VoltarAcompanhamentoCRMComercial");
        }

        [HttpGet]
        public ActionResult EditarClienteComercial(Int32 id)
        {
            Session["VoltaCRM"] = 11;
            Session["IdCliente"] = id;
            return RedirectToAction("VoltarAnexoCliente", "Cliente");
        }

        public ActionResult VerAcaoComercial(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            CRM_COMERCIAL_ACAO item = baseApp.GetAcaoById(id);
            objetoAntes = (CRM_COMERCIAL)Session["CRM"];
            CRMComercialAcaoViewModel vm = Mapper.Map<CRM_COMERCIAL_ACAO, CRMComercialAcaoViewModel>(item);
            return View(vm);
        }

        public ActionResult VerAcoesUsuarioCRMComercial()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            List<CRM_COMERCIAL_ACAO> lista = baseApp.GetAllAcoes(idAss).Where(p => p.USUA_CD_ID2 == usuario.USUA_CD_ID).OrderByDescending(m => m.CRCA_DT_PREVISTA).ToList();
            ViewBag.Lista = lista;
            return View();
        }

        [HttpGet]
        public ActionResult IncluirAcaoComercial()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRMComercial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode inlcuir ação
            List<CRM_COMERCIAL_ACAO> acoes = (List<CRM_COMERCIAL_ACAO>)Session["Acoes"];
            if (acoes.Where(p => p.CRCA_IN_STATUS == 1).ToList().Count > 0)
            {
                Session["MensCRM"] = 42;
                return RedirectToAction("VoltarAcompanhamentoCRMComercial");
            }

            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipoAcao(idAss).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> agenda = new List<SelectListItem>();
            agenda.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            agenda.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Agenda = new SelectList(agenda, "Value", "Text");

            CRM_COMERCIAL_ACAO item = new CRM_COMERCIAL_ACAO();
            CRMComercialAcaoViewModel vm = Mapper.Map<CRM_COMERCIAL_ACAO, CRMComercialAcaoViewModel>(item);
            vm.CRMC_CD_ID = (Int32)Session["IdCRM"];
            vm.CRCA_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.CRCA_DT_ACAO = DateTime.Now;
            vm.CRCA_IN_STATUS = 1;
            vm.USUA_CD_ID2 = usuario.USUA_CD_ID;
            vm.CRIA_AGENDA = 2;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcaoComercial(CRMComercialAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipoAcao(idAss).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_COMERCIAL_ACAO item = Mapper.Map<CRMComercialAcaoViewModel, CRM_COMERCIAL_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateAcao(item, usuarioLogado);

                    // Processa agenda
                    if (vm.CRIA_AGENDA == 1)
                    {
                        AGENDA ag = new AGENDA();
                        ag.AGEN_DS_DESCRICAO = "Ação: " + vm.CRCA_DS_DESCRICAO;
                        ag.AGEN_DT_DATA = vm.CRCA_DT_PREVISTA;
                        ag.AGEN_HR_HORA = vm.CRCA_DT_PREVISTA.TimeOfDay;
                        ag.AGEN_IN_ATIVO = 1;
                        ag.AGEN_IN_STATUS = 1;
                        ag.AGEN_NM_TITULO = vm.CRCA_NM_TITULO;
                        ag.ASSI_CD_ID = idAss;
                        ag.CAAG_CD_ID = 1;
                        ag.AGEN_CD_USUARIO = vm.USUA_CD_ID2;
                        ag.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                        Int32 voltaAg = ageApp.ValidateCreate(ag, usuarioLogado);
                    }

                    // Verifica retorno
                    return RedirectToAction("VoltarAcompanhamentoCRMComercial");
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
        public Int32 ProcessaEnvioSMSContato(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CRM_COMERCIAL_CONTATO cont = (CRM_COMERCIAL_CONTATO)Session["Contato"];

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

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
                String listaDest = "55" + Regex.Replace(cont.CRCO_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSCliente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cont = (CLIENTE)Session["Cliente"];

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

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
                String listaDest = "55" + Regex.Replace(cont.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailContato(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contato
            Int32 idAss = (Int32)Session["IdAssinante"];
            CRM_COMERCIAL_CONTATO cont = (CRM_COMERCIAL_CONTATO)Session["Contato"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a)." + cont.CRCO_NM_NOME;

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = assi.ASSI_NM_NOME;

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
            mensagem.EMAIL_DESTINO = cont.CRCO_NM_EMAIL;
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailCliente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera cliente
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cont = (CLIENTE)Session["Cliente"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a)." + cont.CLIE_NM_NOME;

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = assi.ASSI_NM_NOME;

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
            mensagem.EMAIL_DESTINO = cont.CLIE_NM_EMAIL;
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
        public JsonResult GetProcessosComercial()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            listaMaster = baseApp.GetAllItens(idAss);
            var listaHash = new List<Hashtable>();
            foreach (var item in listaMaster)
            {
                var hash = new Hashtable();
                hash.Add("CRMC_IN_STATUS", item.CRMC_IN_STATUS);
                hash.Add("CRMC_CD_ID", item.CRMC_CD_ID);
                hash.Add("CRMC_NM_NOME", item.CRMC_NM_NOME);
                hash.Add("CRMC_DT_CRIACAO", item.CRMC_DT_CRIACAO.ToString("dd/MM/yyyy"));
                if (item.CRMC_DT_ENCERRAMENTO != null)
                {
                    hash.Add("CRMC_DT_ENCERRAMENTO", item.CRMC_DT_ENCERRAMENTO.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    hash.Add("CRMC_DT_ENCERRAMENTO", "-");
                }
                hash.Add("CRMC_NM_CLIENTE", item.CLIENTE.CLIE_NM_NOME);
                listaHash.Add(hash);
            }
            return Json(listaHash);
        }

        [HttpPost]
        public JsonResult EditarStatusCRMComercial(Int32 id, Int32 status, DateTime? dtEnc)
        {
            CRM_COMERCIAL crm = baseApp.GetById(id);
            crm.CRMC_IN_STATUS = status;
            crm.CRMC_DT_ENCERRAMENTO = dtEnc;
            crm.MOEN_CD_ID = 1;
            crm.CRMC_DS_JUSTIFICATIVA_ENCERRAMENTO = "Processo Encerrado";

            //CRM item = new CRM();
            //item.TARE_CD_ID = tarefa.TARE_CD_ID;
            //item.TARE_DS_DESCRICAO = tarefa.TARE_DS_DESCRICAO;
            //item.TARE_DT_CADASTRO = tarefa.TARE_DT_CADASTRO;
            //item.TARE_DT_ESTIMADA = tarefa.TARE_DT_ESTIMADA;
            //item.TARE_DT_REALIZADA = dtEnc;
            //item.TARE_IN_ATIVO = tarefa.TARE_IN_ATIVO;
            //item.TARE_IN_AVISA = tarefa.TARE_IN_AVISA;
            //item.TARE_IN_PRIORIDADE = tarefa.TARE_IN_PRIORIDADE;
            //item.TARE_IN_STATUS = tarefa.TARE_IN_STATUS;
            //item.TARE_NM_LOCAL = tarefa.TARE_NM_LOCAL;
            //item.TARE_NM_TITULO = tarefa.TARE_NM_TITULO;
            //item.TARE_TX_OBSERVACOES = tarefa.TARE_TX_OBSERVACOES;
            //item.TITR_CD_ID = tarefa.TITR_CD_ID;
            //item.USUA_CD_ID = tarefa.USUA_CD_ID;

            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateEdit(crm, crm, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    return Json(PlatMensagens_Resources.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(PlatMensagens_Resources.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }

                Session["ListaCRM"] = null;
                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Json(ex.Message);
            }
        }

        public ActionResult GerarRelatorioDetalheComercial()
        {
            // Prepara geração
            CRM_COMERCIAL aten = baseApp.GetItemById((Int32)Session["IdCRM"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "CRM-Comercial" + aten.CRMC_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            Font meuFontVerde = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
            Font meuFontAzul= FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
            Font meuFontVermelho = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.RED);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/base/favicon_SystemBR.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Processo CRM-Comercial - Detalhes", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);

            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Dados do Cliente
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados do Cliente", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.CLIENTE.CLIE_NM_NOME, meuFontVerde));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CLIENTE.CLIE_NM_ENDERECO != null)
            {
                cell = new PdfPCell(new Paragraph("Endereço: " + aten.CLIENTE.CLIE_NM_ENDERECO + " " + aten.CLIENTE.CLIE_NR_NUMERO + " " + aten.CLIENTE.CLIE_NM_COMPLEMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.CLIENTE.UF != null)
                {
                    cell = new PdfPCell(new Paragraph("          " + aten.CLIENTE.CLIE_NM_BAIRRO + " - " + aten.CLIENTE.CLIE_NM_CIDADE + " - " + aten.CLIENTE.UF.UF_SG_SIGLA + " - " + aten.CLIENTE.CLIE_NR_CEP, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("          " + aten.CLIENTE.CLIE_NM_BAIRRO + " - " + aten.CLIENTE.CLIE_NM_CIDADE + " - " + aten.CLIENTE.CLIE_NR_CEP, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Endereço: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Telefone: " + aten.CLIENTE.CLIE_NR_TELEFONE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular: " + aten.CLIENTE.CLIE_NR_CELULAR, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("E-Mail: " + aten.CLIENTE.CLIE_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Dados do Processo
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados do Processo", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.CRMC_NM_NOME, meuFontVerde));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.CRMC_IN_STATUS == 1)
            {
                cell = new PdfPCell(new Paragraph("Status: Oportunidade", meuFontAzul));
            }
            else if (aten.CRMC_IN_STATUS == 2)
            {
                cell = new PdfPCell(new Paragraph("Status: Proposta", meuFontAzul));
            }
            else if (aten.CRMC_IN_STATUS == 3)
            {
                cell = new PdfPCell(new Paragraph("Status: Em Aprovação", meuFontAzul));
            }
            else if (aten.CRMC_IN_STATUS == 4)
            {
                cell = new PdfPCell(new Paragraph("Status: Aprovado", meuFontAzul));
            }
            else if (aten.CRMC_IN_STATUS == 5)
            {
                cell = new PdfPCell(new Paragraph("Status: Reprovado", meuFontAzul));
            }
            else if (aten.CRMC_IN_STATUS == 6)
            {
                cell = new PdfPCell(new Paragraph("Status: Vendido", meuFontAzul));
            }
            else if (aten.CRMC_IN_STATUS == 7)
            {
                cell = new PdfPCell(new Paragraph("Status: Encerrado", meuFontAzul));
            }
            else if (aten.CRMC_IN_STATUS == 8)
            {
                cell = new PdfPCell(new Paragraph("Status: Cancelado", meuFontAzul));
            }
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Descrição: " + aten.CRMC_DS_DESCRICAO, meuFontVerde));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Informações: " + aten.CRMC_DS_INFORMACOES_GERAIS, meuFontVerde));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Criação: " + aten.CRMC_DT_CRIACAO.ToShortDateString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Responsável: " + aten.USUARIO.USUA_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Origem: " + aten.CRM_ORIGEM.CROR_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CRMC_IN_STATUS == 8)
            {
                cell = new PdfPCell(new Paragraph("Data Cancelamento: " + aten.CRMC_DT_CANCELAMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Motivo: " + aten.MOTIVO_CANCELAMENTO.MOCA_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Justificativa: " + aten.CRMC_DS_JUSTIFICATIVA_CANCELAMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.CRMC_IN_STATUS == 7)
            {
                cell = new PdfPCell(new Paragraph("Data Encerramento: " + aten.CRMC_DT_ENCERRAMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Motivo: " + aten.MOTIVO_ENCERRAMENTO.MOEN_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Justificativa: " + aten.CRMC_DS_JUSTIFICATIVA_ENCERRAMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Contatos
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Contatos", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CRM_COMERCIAL_CONTATO.Count > 0)
            {
                table = new PdfPTable(new float[] { 130f, 100f, 100f, 80f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cargo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CRM_COMERCIAL_CONTATO item in aten.CRM_COMERCIAL_CONTATO)
                {
                    cell = new PdfPCell(new Paragraph(item.CRCO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRCO_NM_CARGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRCO_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRCO_NR_TELEFONE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell = new PdfPCell(new Paragraph(item.CRCO_NR_CELULAR, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Dados Ações
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Ações", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CRM_COMERCIAL_ACAO.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 80f, 80f, 100f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Título", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Criação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Previsão", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Dias (Prevista)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Status", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CRM_COMERCIAL_ACAO item in aten.CRM_COMERCIAL_ACAO)
                {
                    cell = new PdfPCell(new Paragraph(item.CRCA_NM_TITULO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRCA_DT_ACAO.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.CRCA_DT_PREVISTA > DateTime.Today.Date)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRCA_DT_PREVISTA.ToShortDateString(), meuFontVerde))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRCA_DT_PREVISTA == DateTime.Today.Date)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRCA_DT_PREVISTA.ToShortDateString(), meuFontAzul))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph(item.CRCA_DT_PREVISTA.ToShortDateString(), meuFontAzul))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);

                    if ((item.CRCA_DT_PREVISTA.Date - DateTime.Today.Date).Days > 0)
                    {
                        cell = new PdfPCell(new Paragraph((item.CRCA_DT_PREVISTA.Date - DateTime.Today.Date).Days.ToString(), meuFontVerde))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph((item.CRCA_DT_PREVISTA.Date - DateTime.Today.Date).Days.ToString(), meuFontVermelho))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);

                    if (item.CRCA_IN_STATUS == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativa", meuFontVerde))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRCA_IN_STATUS == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Pendente", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRCA_IN_STATUS == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Encerrada", meuFontAzul))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRCA_IN_STATUS == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Excluída", meuFontVermelho))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoCRMComercial");
        }

        public ActionResult VerCRMExpansaoComercial()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("Voltar");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            DateTime inicio = Convert.ToDateTime("01/" + DateTime.Today.Date.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Today.Date.Year.ToString());
            DateTime hoje = DateTime.Today.Date;
            Session["Hoje"] = hoje;
            if (Session["ListaCRMCheia"] == null)
            {
                List<CRM_COMERCIAL> listaCRMCheia = baseApp.GetAllItensAdm(idAss);
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            if (Session["ListaCRM"] == null)
            {
                List<CRM_COMERCIAL> listaCRM = baseApp.GetAllItens(idAss);
                Session["ListaCRM"] = listaCRM;
            }
            // Retorna
            return View();
        }

        public JsonResult GetDadosProcessosStatusComercial()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            List<CRM_COMERCIAL> listaCRMCheia = new List<CRM_COMERCIAL>();

            if (Session["ListaCRMCheia"] == null)
            {
                listaCRMCheia = baseApp.GetAllItensAdm(idAss);
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            else
            {
                listaCRMCheia = (List<CRM_COMERCIAL>)Session["ListaCRMCheia"];
            }

            // Prepara
            List<CRMDTOViewModel> lista = new List<CRMDTOViewModel>();
            CRMDTOViewModel dto = new CRMDTOViewModel();

            // Carrega vetores
            Int32 prosp = listaCRMCheia.Where(p => p.CRMC_IN_STATUS == 1).ToList().Count;
            desc.Add("Oportunidade");
            quant.Add(prosp);
            dto.DESCRICAO = "Oportunidade";
            dto.QUANTIDADE = prosp;
            lista.Add(dto);

            Int32 cont = listaCRMCheia.Where(p => p.CRMC_IN_STATUS == 2).ToList().Count;
            desc.Add("Proposta");
            quant.Add(cont);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Proposta";
            dto.QUANTIDADE = cont;
            lista.Add(dto);

            Int32 prop = listaCRMCheia.Where(p => p.CRMC_IN_STATUS == 3).ToList().Count;
            desc.Add("Em Aprovação");
            quant.Add(prop);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Em Aprovação";
            dto.QUANTIDADE = prop;
            lista.Add(dto);

            Int32 neg = listaCRMCheia.Where(p => p.CRMC_IN_STATUS == 4).ToList().Count;
            desc.Add("Aprovado");
            quant.Add(neg);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Aprovado";
            dto.QUANTIDADE = neg;
            lista.Add(dto);

            Int32 enc = listaCRMCheia.Where(p => p.CRMC_IN_STATUS == 5).ToList().Count;
            desc.Add("Reprovado");
            quant.Add(enc);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Reprovado";
            dto.QUANTIDADE = enc;
            lista.Add(dto);

            Int32 enc1 = listaCRMCheia.Where(p => p.CRMC_IN_STATUS == 6).ToList().Count;
            desc.Add("Vendido");
            quant.Add(enc1);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Vendido";
            dto.QUANTIDADE = enc1;
            lista.Add(dto);

            Int32 enc2 = listaCRMCheia.Where(p => p.CRMC_IN_STATUS == 7).ToList().Count;
            desc.Add("Encerrado");
            quant.Add(enc2);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Encerrado";
            dto.QUANTIDADE = enc2;
            lista.Add(dto);

            Int32 enc3 = listaCRMCheia.Where(p => p.CRMC_IN_STATUS == 8).ToList().Count;
            desc.Add("Cancelado");
            quant.Add(enc3);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Cancelado";
            dto.QUANTIDADE = enc3;
            lista.Add(dto);
            Session["ListaProcessosStatus"] = lista;

            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            cor.Add("#D63131");
            cor.Add("#27A1C6");
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");

            // retorna
            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosProcessosSituacaoComercial()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            List<CRM_COMERCIAL> listaCRMCheia = new List<CRM_COMERCIAL>();

            if (Session["ListaCRMCheia"] == null)
            {
                listaCRMCheia = baseApp.GetAllItensAdm(idAss);
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            else
            {
                listaCRMCheia = (List<CRM_COMERCIAL>)Session["ListaCRMCheia"];
            }

            // Prepara
            List<CRMDTOViewModel> listaSit = new List<CRMDTOViewModel>();
            CRMDTOViewModel dto = new CRMDTOViewModel();

            // Carrega vetores
            Int32 prosp = listaCRMCheia.Where(p => p.CRMC_IN_ATIVO == 1).ToList().Count;
            desc.Add("Ativos");
            quant.Add(prosp);
            dto.DESCRICAO = "Ativos";
            dto.QUANTIDADE = prosp;
            listaSit.Add(dto);

            Int32 cont = listaCRMCheia.Where(p => p.CRMC_IN_ATIVO == 2).ToList().Count;
            desc.Add("Arquivados");
            quant.Add(cont);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Arquivados";
            dto.QUANTIDADE = cont;
            listaSit.Add(dto);
            Session["ListaProcessosSituacao"] = listaSit;

            cor.Add("#359E18");
            cor.Add("#FFAE00");

            // retorna
            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public ActionResult VerProcessosStatusExpansaoComercial()
        {
            // Prepara view
            List<CRMDTOViewModel> lista = (List<CRMDTOViewModel>)Session["ListaProcessosStatus"];
            //List<CRMDTOViewModel> listaSit = (List<CRMDTOViewModel>)Session["ListaProcessosSituacao"];
            ViewBag.Lista = lista;
            //ViewBag.ListaSit = listaSit;
            return View();
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult EnviarEMailCliente(Int32 id)
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
            CLIENTE cli = cliApp.GetItemById(id);
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            Session["Cliente"] = cli;

            // Prepara mensagem
            String header = "Prezado <b>" + cli.CLIE_NM_NOME + "</b>";
            String body = String.Empty;
            String footer = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Mota vm
            MensagemViewModel vm = new MensagemViewModel();
            vm.ASSI_CD_ID = idAss;
            vm.MENS_DT_CRIACAO = DateTime.Now;
            vm.MENS_IN_ATIVO = 1;
            vm.NOME = cli.CLIE_NM_NOME;
            vm.ID = id;
            vm.MODELO = cli.CLIE_NM_EMAIL;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.MENS_NM_CABECALHO = header;
            vm.MENS_NM_RODAPE = footer;
            vm.MENS_IN_TIPO = 1;
            vm.ID = cli.CLIE_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarEMailCliente(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
          
            if (ModelState.IsValid)
            {
                Int32 idNot = (Int32)Session["IdCRM"];
                try
                {
                    // Checa corpo da mensagem
                    if (String.IsNullOrEmpty(vm.MENS_TX_TEXTO))
                    {
                        Session["MensMensagem"] = 66;
                        return RedirectToAction("EnviarEMailCliente");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailCliente(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRMComercial", new { id = idNot });
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
        public ActionResult MontarTelaDashboardCRMComercial()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRMComercial", "CRMComercial");
                }
                if ((Int32)Session["PermCRM"] == 0)
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera listas
            List<CRM_COMERCIAL> lt = baseApp.GetAllItens(idAss);
            List<CRM_COMERCIAL> lm = lt.Where(p => p.CRMC_DT_CRIACAO.Month == DateTime.Today.Date.Month & p.CRMC_DT_CRIACAO.Year == DateTime.Today.Date.Year).ToList();
            List<CRM_COMERCIAL> la = lt.Where(p => p.CRMC_IN_ATIVO == 1).ToList();
            List<CRM_COMERCIAL> lq = lt.Where(p => p.CRMC_IN_ATIVO == 2).ToList();
            List<CRM_COMERCIAL> ls = lt.Where(p => p.CRMC_IN_STATUS == 7).ToList();
            List<CRM_COMERCIAL> lc = lt.Where(p => p.CRMC_IN_STATUS == 8).ToList();
            List<CRM_COMERCIAL_ACAO> acoes = baseApp.GetAllAcoes(idAss);
            List<CRM_COMERCIAL_ACAO> acoesPend = acoes.Where(p => p.CRCA_IN_STATUS == 1).ToList();
            List<CLIENTE> cli = cliApp.GetAllItens(idAss);

            // Estatisticas 
            ViewBag.Total = lt.Count;
            ViewBag.TotalAtivo = la.Count;
            ViewBag.TotalSucesso = ls.Count;
            ViewBag.TotalCancelado = lc.Count;
            ViewBag.Acoes = acoes.Count;
            ViewBag.AcoesPend = acoesPend.Count;
            ViewBag.Clientes = cli.Count;

            ViewBag.TotalPes = lt.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.TotalAtivoPes = la.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.TotalSucessoPes = ls.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.TotalCanceladoPes = lc.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.AcoesPes = acoes.Where(p => p.CRM_COMERCIAL.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.AcoesPendPes = acoesPend.Where(p => p.CRM_COMERCIAL.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;

            Session["ListaCRM"] = lt;
            Session["ListaCRMMes"] = lm;
            Session["ListaCRMAtivo"] = la;
            Session["ListaCRMSucesso"] = ls;
            Session["ListaCRMCanc"] = lc;
            Session["ListaCRMAcoes"] = acoes;
            Session["ListaCRMAcoesPend"] = acoesPend;

            Session["CRMAtivos"] = la.Count;
            Session["CRMArquivados"] = lq.Count;
            Session["CRMCancelados"] = lc.Count;
            Session["CRMSucessos"] = la.Count;

            Session["CRMOpor"] = lt.Where(p => p.CRMC_IN_STATUS == 1).ToList().Count;
            Session["CRMProp"] =  lt.Where(p => p.CRMC_IN_STATUS == 2).ToList().Count;
            Session["CRMEmAp"] =  lt.Where(p => p.CRMC_IN_STATUS == 3).ToList().Count;
            Session["CRMApro"] =  lt.Where(p => p.CRMC_IN_STATUS == 4).ToList().Count;
            Session["CRMRepr"] =  lt.Where(p => p.CRMC_IN_STATUS == 5).ToList().Count;
            Session["CRMVend"] = lt.Where(p => p.CRMC_IN_STATUS == 6).ToList().Count;
            Session["CRMEnc"] = lt.Where(p => p.CRMC_IN_STATUS == 7).ToList().Count;
            Session["CRMCanc"] = lt.Where(p => p.CRMC_IN_STATUS == 8).ToList().Count;

            // Resumo Mes CRM
            List<DateTime> datas = lm.Select(p => p.CRMC_DT_CRIACAO.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = lm.Where(p => p.CRMC_DT_CRIACAO.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaCRMMes = lista;
            ViewBag.ContaCRMMes = lm.Count;
            Session["ListaDatasCRM"] = datas;
            Session["ListaCRMMesResumo"] = lista;

            // Resumo Situacao CRM 
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = lt.Where(p => p.CRMC_IN_ATIVO == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1? "Ativo" : "Arquivado";
                mod.Valor = conta;
                lista1.Add(mod);
            }
            ViewBag.ListaCRMSituacao = lista1;
            Session["ListaCRMSituacao"] = lista1;

            // Resumo Status CRM 
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = lt.Where(p => p.CRMC_IN_STATUS == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1 ? "Oportunidade" : (i == 2 ? "Proposta" : (i == 3 ? "Em Aprovação" : (i == 4 ? "Aprovado" : (i == 5 ? "Reprovado" : (i == 6 ? "Vendido" : (i == 7 ? "Encerrado" : "Cancelado"))))));
                mod.Valor = conta;
                lista2.Add(mod);
            }
            ViewBag.ListaCRMStatus = lista2;
            Session["ListaCRMStatus"] = lista2;
            return View(vm);
        }

        public JsonResult GetDadosGraficoCRMSituacaoComercial()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["CRMAtivos"];
            Int32 q2 = (Int32)Session["CRMArquivados"];

            desc.Add("Ativos");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Arquivados");
            quant.Add(q2);

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCRMStatusComercial()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["CRMOpor"];
            Int32 q2 = (Int32)Session["CRMProp"];
            Int32 q3 = (Int32)Session["CRMEmAp"];
            Int32 q4 = (Int32)Session["CRMApro"];
            Int32 q5 = (Int32)Session["CRMRepr"];
            Int32 q6 = (Int32)Session["CRMVend"];
            Int32 q7 = (Int32)Session["CRMEnc"];
            Int32 q8 = (Int32)Session["CRMCanc"];

            desc.Add("Oportunidade");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Proposta");
            quant.Add(q2);
            cor.Add("#FFAE00");
            desc.Add("Em Aprovação");
            quant.Add(q3);
            cor.Add("#FF7F00");
            desc.Add("Aprovado");
            quant.Add(q4);
            cor.Add("#D63131");
            desc.Add("Reprovado");
            quant.Add(q5);
            cor.Add("#27A1C6");
            desc.Add("Vendido");
            quant.Add(q6);
            cor.Add("#FF7F00");
            desc.Add("Encerrado");
            quant.Add(q7);
            cor.Add("#D63131");
            desc.Add("Cancelado");
            quant.Add(q8);
            cor.Add("#27A1C6");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCRMComercial()
        {
            List<CRM_COMERCIAL> listaCP1 = (List<CRM_COMERCIAL>)Session["ListaCRMMes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasCRM"];
            List<CRM_COMERCIAL> listaDia = new List<CRM_COMERCIAL>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.CRMC_DT_CRIACAO.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult MostrarClientesComercial()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 40;
            return RedirectToAction("MontarTelaCliente", "Cliente");
        }

        public ActionResult IncluirClienteRapidoComercial()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 40;
            return RedirectToAction("IncluirClienteRapido", "Cliente");
        }

        [HttpPost]
        public void MontaListaItemPedidoComercial(CRM_COMERCIAL_ITEM item)
        {
            if (Session["ListaITPC"] == null)
            {
                Session["ListaITPC"] = new List<CRM_COMERCIAL_ITEM>();
            }
            List<CRM_COMERCIAL_ITEM> lit = (List<CRM_COMERCIAL_ITEM>)Session["ListaITPC"];
            lit.Add(item);
            Session["ListaITPC"] = lit;
        }

        [HttpPost]
        public void RemoveItpcTabelaComercial(CRM_COMERCIAL_ITEM item)
        {
            if (Session["ListaITPC"] != null)
            {
                List<CRM_COMERCIAL_ITEM> lit = (List<CRM_COMERCIAL_ITEM>)Session["ListaITPC"];
                lit.RemoveAll(x => x.PROD_CD_ID == item.PROD_CD_ID);
                Session["ListaITPC"] = lit;
            }
        }

        [HttpGet]
        public ActionResult IncluirProdutoComercial()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            Session["VoltaProduto"] = 4;
            return RedirectToAction("IncluirProduto", "Produto");
        }

        public void ProcessarCliente(Int32 id)
        {
            Session["FornCotacao"] = id;
            Session["EscolheuForn"] = 1;
        }

        [HttpGet]
        public ActionResult AprovarProcessoCRMComercial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            CRM_COMERCIAL item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.CRM_COMERCIAL_ITEM)
            {
                if (i.CRCI_VL_VALOR != null)
                {
                    custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                }
            }
            ViewBag.CustoTotal = custo;
            Session["IdVolta"] = id;
            CRMComercialViewModel vm = Mapper.Map<CRM_COMERCIAL, CRMComercialViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult AprovarProcessoCRMComercial(CRMComercialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                CRM_COMERCIAL item = Mapper.Map<CRMComercialViewModel, CRM_COMERCIAL>(vm);
                Decimal custo = 0;
                foreach (var i in item.CRM_COMERCIAL_ITEM)
                {
                    if (i.CRCI_VL_VALOR != null)
                    {
                        custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateAprovacao(item);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0103", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0104", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0105", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<CRM_COMERCIAL>();
                Session["ListaCRM"] = null;
                Session["IdCRM"] = item.CRMC_CD_ID;
                return RedirectToAction("MontarTelaCRMComercial");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReprovarProcessoCRMComercial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            CRM_COMERCIAL item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.CRM_COMERCIAL_ITEM)
            {
                if (i.CRCI_VL_VALOR != null)
                {
                    custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                }
            }
            ViewBag.CustoTotal = custo;
            Session["IdVolta"] = id;
            CRMComercialViewModel vm = Mapper.Map<CRM_COMERCIAL, CRMComercialViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReprovarProcessoCRMComercial(CRMComercialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                CRM_COMERCIAL item = Mapper.Map<CRMComercialViewModel, CRM_COMERCIAL>(vm);
                Decimal custo = 0;
                foreach (var i in item.CRM_COMERCIAL_ITEM)
                {
                    if (i.CRCI_VL_VALOR != null)
                    {
                        custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateReprovacao(item);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0103", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0104", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0105", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<CRM_COMERCIAL>();
                Session["ListaCRM"] = null;
                Session["IdCRM"] = item.CRMC_CD_ID;
                return RedirectToAction("MontarTelaCRMComercial");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ProcessarEncerrarProcessoCRMComercial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            CRM_COMERCIAL item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.CRM_COMERCIAL_ITEM)
            {
                if (i.CRCI_VL_VALOR != null)
                {
                    custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                }
            }
            ViewBag.CustoTotal = custo;
            CRMComercialViewModel vm = Mapper.Map<CRM_COMERCIAL, CRMComercialViewModel>(item);
            Session["VoltaCompra"] = 7;
            Session["IdVolta"] = id;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ProcessarEncerrarProcessoCRMComercial(CRMComercialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                CRM_COMERCIAL item = Mapper.Map<CRMComercialViewModel, CRM_COMERCIAL>(vm);
                Decimal custo = 0;
                foreach (var i in item.CRM_COMERCIAL_ITEM)
                {
                    if (i.CRCI_VL_VALOR != null)
                    {
                        custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateEfetuarVenda(item);
                Int32 volta1 = baseApp.ValidateEncerrar(item, usuario);

                // Verifica retorno
                CONTA_RECEBER cp = new CONTA_RECEBER();
                if (volta == 0)
                {
                    // Recupera fornecedor
                    CLIENTE forn = cliApp.GetItemById((Int32)item.CLIE_CD_ID);

                    // Calcula valor
                    Decimal valor = 0;
                    foreach (var i in item.CRM_COMERCIAL_ITEM)
                    {
                        if (i.CRCI_VL_VALOR != null)
                        {
                            custo += (Int32)i.CRCI_QN_QUANTIDADE * (Decimal)i.CRCI_VL_VALOR;
                        }
                    }

                    // Gera CR
                    cp.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    cp.CARE_DS_DESCRICAO = "Lançamento a receber referente ao processo CRM Comercial " + item.CRMC_NM_NOME + " de número " + item.CRMC_NR_NUMERO;
                    cp.CARE_DT_COMPETENCIA = DateTime.Today.Date;
                    cp.CARE_DT_LANCAMENTO = DateTime.Today.Date;
                    cp.CARE_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
                    cp.CARE_IN_ATIVO = 1;
                    cp.CARE_IN_LIQUIDADA = 0;
                    cp.CARE_IN_PAGA_PARCIAL = 0;
                    cp.CARE_IN_PARCELADA = 0;
                    cp.CARE_IN_PARCELAS = 0;
                    cp.CARE_IN_TIPO_LANCAMENTO = 1;
                    cp.CARE_NR_DOCUMENTO = item.CRMC_NR_NOTA_FISCAL;
                    cp.CARE_VL_DESCONTO = 0;
                    cp.CARE_VL_JUROS = 0;
                    cp.CARE_VL_PARCELADO = 0;
                    cp.CARE_VL_PARCIAL = 0;
                    cp.CARE_VL_TAXAS = 0;
                    cp.CARE_VL_VALOR_RECEBIDO = 0;
                    cp.CARE_VL_VALOR = valor;
                    cp.CARE_VL_SALDO = valor;
                    cp.CLIE_CD_ID = forn.CLIE_CD_ID;
                    cp.CRMC_CD_ID = item.CRMC_CD_ID;
                    cp.USUA_CD_ID = item.USUA_CD_ID;
                    cp.COBA_CD_ID = cbApp.GetContaPadrao(idAss).COBA_CD_ID;
                }

                // Sucesso
                listaMaster = new List<CRM_COMERCIAL>();
                Session["ListaCRM"] = null;
                Session["ContaReceber"] = cp;
                Session["VoltaCRM"] = 1;
                Session["IdCRM"] = item.CRMC_CD_ID;
                return RedirectToAction("IncluirCR", "ContaReceber", new { voltaCompra = 1 });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EntregarItemProcessoCRM(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            CRM_COMERCIAL_ITEM item = baseApp.GetItemCRMById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReceberItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                if (vm.ITPC_NR_QUANTIDADE_REVISADA != vm.ITPC_NR_QUANTIDADE_RECEBIDA && vm.ITPC_DS_JUSTIFICATIVA == null)
                {
                    ModelState.AddModelError("", "Para quantidade recebida diferente do previsto, necessário justificativa");
                    return View(vm);
                }

                // Executa a operação
                ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateItemRecebido(item, usuario);

                // Verifica retorno
                if (volta == 2)
                {
                    Session["PedidoRecebido"] = vm.PECO_CD_ID;
                    return RedirectToAction("MontarTelaPedidoCompra");
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                return RedirectToAction("ReceberPedidoCompra", new { id = Session["IdVolta"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }







    }
}