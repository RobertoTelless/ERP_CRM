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

namespace SMS_Presentation.Controllers
{
    public class TemplateEmailController : Controller
    {
        private readonly ITemplateEMailAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        TEMPLATE_EMAIL objeto = new TEMPLATE_EMAIL();
        TEMPLATE_EMAIL objetoAntes = new TEMPLATE_EMAIL();
        List<TEMPLATE_EMAIL> listaMaster = new List<TEMPLATE_EMAIL>();
        String extensao;

        public TemplateEmailController(ITemplateEMailAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
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

        [HttpGet]
        public ActionResult MontarTelaTemplateEMail()
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
            if ((List<TEMPLATE_SMS>)Session["ListaTemplateEMail"] == null || ((List<TEMPLATE_SMS>)Session["ListaTemplateEMail"]).Count == 0)
            {
                listaMaster = baseApp.GetAllItens(idAss).ToList();
                Session["ListaTemplateEMail"] = listaMaster;
            }
            ViewBag.Listas = (List<TEMPLATE_SMS>)Session["ListaTemplateEMail"];
            ViewBag.Title = "Template";
            Session["TemplateEMail"] = null;
            Session["IncluirTemplateEMail"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensTemplateEMail"] != null)
            {
                if ((Int32)Session["MensTemplateEMail"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTemplateEMail"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0032", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTemplateEMail"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0060", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTemplateEMail"] == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0061", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTemplateEMail"] = 1;
            objeto = new TEMPLATE_EMAIL();
            return View(objeto);
        }

        public ActionResult RetirarFiltroTemplateEMail()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaTemplateEMail"] = null;
            return RedirectToAction("MontarTelaTemplateEMail");
        }

        public ActionResult MostrarTudoTemplateEMail()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
            Session["ListaTemplateEMail"] = null;
            return RedirectToAction("MontarTelaTemplateEMail");
        }

        public ActionResult VoltarBaseTemplateEMail()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTemplateEMail");
        }

        [HttpPost]
        public ActionResult FiltrarTemplateEMail(TEMPLATE_EMAIL item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<TEMPLATE_EMAIL> listaObj = new List<TEMPLATE_EMAIL>();
                Session["FiltroTemplateEMail"] = item;
                Int32 volta = baseApp.ExecuteFilter(item.TEEM_SG_SIGLA, item.TEEM_NM_NOME, item.TEEM_TX_CORPO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensTemplateEMail"] = 1;
                    return RedirectToAction("MontarTelaTemplateEMail");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaTemplateEMail"] = listaObj;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
        }

        [HttpGet]
        public ActionResult IncluirTemplateEMail()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "OPR" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensTemplate"] = 2;
                    return RedirectToAction("MontarTelaTemplateEMail", "TemplateEMail");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            
            // Prepara view
            TEMPLATE_EMAIL item = new TEMPLATE_EMAIL();
            TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.TEEM_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTemplateEMail(TemplateEMailViewModel vm)
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
                    // Preparação
                    if (vm.file != null)
                    {
                        vm.TEEM_AQ_ARQUIVO = vm.file;
                    }
                    TEMPLATE_EMAIL item = Mapper.Map<TemplateEMailViewModel, TEMPLATE_EMAIL>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Critica
                    if (item.TEEM_AQ_ARQUIVO == null & item.TEEM_TX_CORPO == null)
                    {
                        Session["MensTemplateEMail"] = 10;
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                    if (item.TEEM_AQ_ARQUIVO == null & (item.TEEM_TX_CORPO != null & (item.TEEM_TX_CABECALHO == null | item.TEEM_TX_DADOS == null)))
                    {
                        Session["MensTemplateEMail"] = 11;
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }

                    // Acertos
                    if (item.TEEM_AQ_ARQUIVO != null)
                    {
                        item.TEEM_TX_CABECALHO = null;
                        item.TEEM_TX_CORPO = null;
                        item.TEEM_TX_DADOS = null;
                    }

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTemplateSMS"] = 3;
                        return RedirectToAction("MontarTelaTemplateSMS");
                    }

                    // Cria Pasta e copia arquivo HTML
                    if (item.TEEM_AQ_ARQUIVO != null)
                    {
                        // Cria pasta
                        String caminho = "/Imagens/" + idAss.ToString() + "/TemplatesHTML/" + item.TEEM_CD_ID.ToString() + "/Arquivos/";
                        Directory.CreateDirectory(Server.MapPath(caminho));

                        // Copia arquivo
                        String filePath = item.TEEM_AQ_ARQUIVO;
                        String fileName = Path.GetFileName(filePath);
                        String path = Path.Combine(Server.MapPath(caminho), fileName);
                        System.IO.File.Copy(filePath, path);

                        // Atualiza registro
                        item.TEEM_AQ_ARQUIVO = path;
                        volta = baseApp.ValidateEdit(item, item, usuario);
                    }

                    // Sucesso
                    listaMaster = new List<TEMPLATE_EMAIL>();
                    Session["ListaTemplateEMail"] = null;
                    Session["IdTemplateEMail"] = item.TEEM_CD_ID;
                    return RedirectToAction("MontarTelaTemplateEMail");
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
        public ActionResult EditarTemplateEMail(Int32 id)
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
                    Session["MensTemplateEMail"] = 2;
                    return RedirectToAction("MontarTelaTemplateEMail", "TemplateEMail");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TEMPLATE_EMAIL item = baseApp.GetItemById(id);
            Session["TemplateEMail"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensTemplateEMail"] != null)
            {


            }

            Session["VoltaTemplateEMail"] = 1;
            objetoAntes = item;
            Session["IdTemplate"] = id;
            TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarTemplateEMail(TemplateEMailViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TEMPLATE_EMAIL item = Mapper.Map<TemplateEMailViewModel, TEMPLATE_EMAIL>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Sucesso
                    listaMaster = new List<TEMPLATE_EMAIL>();
                    Session["ListaTemplateEMail"] = null;
                    return RedirectToAction("MontarTelaTemplateEMail");
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

        public ActionResult VoltarAnexoTemplateEMail()
        {

            return RedirectToAction("EditarTemplateEMail", new { id = (Int32)Session["IdTemplateEMail"] });
        }

        [HttpGet]
        public ActionResult ExcluirTemplateEMail(Int32 id)
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
                    Session["MensTemplateEMail"] = 2;
                    return RedirectToAction("MontarTelaTemplateEMail");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TEMPLATE_EMAIL item = baseApp.GetItemById(id);
            objetoAntes = (TEMPLATE_EMAIL)Session["TemplateEMail"];
            item.TEEM_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTemplateEMail"] = 4;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            Session["ListaTemplateEMail"] = null;
            return RedirectToAction("MontarTelaTemplateEMail");
        }

        [HttpGet]
        public ActionResult ReativarTemplateEMail(Int32 id)
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
                    Session["MensTemplateEMail"] = 2;
                    return RedirectToAction("MontarTelaTemplateEMail");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TEMPLATE_EMAIL item = baseApp.GetItemById(id);
            objetoAntes = (TEMPLATE_EMAIL)Session["TemplateEMail"];
            item.TEEM_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaTemplateEMail"] = null;
            return RedirectToAction("MontarTelaTemplateEMail");
        }

        [HttpGet]
        public ActionResult VerTemplateEMail(Int32 id)
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
                    Session["MensTemplateEMail"] = 2;
                    return RedirectToAction("MontarTelaTemplateEMail", "TemplateEMail");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            Session["IdTemplateEMail"] = id;
            TEMPLATE_EMAIL item = baseApp.GetItemById(id);
            TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
            return View(vm);
        }
    }
}