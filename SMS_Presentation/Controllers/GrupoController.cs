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

namespace ERP_CRM_Solution.Controllers
{
    public class GrupoController : Controller
    {
        private readonly IGrupoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IClienteAppService cliApp;
        private readonly IMensagemAppService menApp;

        private String msg;
        private Exception exception;
        GRUPO objeto = new GRUPO();
        GRUPO objetoAntes = new GRUPO();
        List<GRUPO> listaMaster = new List<GRUPO>();
        String extensao;

        public GrupoController(IGrupoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IClienteAppService cliApps, IMensagemAppService menApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            cliApp = cliApps;
            menApp = menApps;
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
        public ActionResult MontarTelaGrupo()
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
            if ((List<GRUPO>)Session["ListaGrupo"] == null || ((List<GRUPO>)Session["ListaGrupo"]).Count == 0)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaGrupo"] = listaMaster;
            }
            ViewBag.Listas = (List<GRUPO>)Session["ListaGrupo"];
            ViewBag.Title = "Grupos";
            Session["Grupo"] = null;
            Session["IncluirGrupo"] = 0;
            Session["ListaClienteGrupo"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensGrupo"] != null)
            {
                if ((Int32)Session["MensGrupo"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0025", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0062", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 11)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0063", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaGrupo"] = 1;
            objeto = new GRUPO();
            return View(objeto);
        }

        public ActionResult RetirarFiltroGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult MostrarTudoGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult VoltarBaseGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCliGrupo"] == 10)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            if ((Int32)Session["VoltaGrupo"] == 11)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult IncluirGrupo()
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Cats = new SelectList(menApp.GetAllTipos(idAss).OrderBy(p => p.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.UF = new SelectList(menApp.GetAllUF().OrderBy(p => p.UF_SG_SIGLA), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Sexo = new SelectList(cliApp.GetAllSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            List<SelectListItem> dias = new List<SelectListItem>();
            for (int i = 1; i < 32; i++)
            {
                dias.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.Dias = new SelectList(dias, "Value", "Text");
            List<SelectListItem> meses = new List<SelectListItem>();
            meses.Add(new SelectListItem() { Text = "Janeiro", Value = "1" });
            meses.Add(new SelectListItem() { Text = "Fevereiro", Value = "2" });
            meses.Add(new SelectListItem() { Text = "Março", Value = "3" });
            meses.Add(new SelectListItem() { Text = "Abril", Value = "4" });
            meses.Add(new SelectListItem() { Text = "Maio", Value = "5" });
            meses.Add(new SelectListItem() { Text = "Junho", Value = "6" });
            meses.Add(new SelectListItem() { Text = "Julho", Value = "7" });
            meses.Add(new SelectListItem() { Text = "Agosto", Value = "8" });
            meses.Add(new SelectListItem() { Text = "Setembro", Value = "9" });
            meses.Add(new SelectListItem() { Text = "Outubro", Value = "10" });
            meses.Add(new SelectListItem() { Text = "Novembro", Value = "11" });
            meses.Add(new SelectListItem() { Text = "Dezembro", Value = "12" });
            ViewBag.Meses = new SelectList(meses, "Value", "Text");

            // Prepara view
            Session["GrupoNovo"] = 0;
            GRUPO item = new GRUPO();
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.GRUP_DT_CADASTRO = DateTime.Today.Date;
            vm.GRUP_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirGrupo(GrupoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Cats = new SelectList(menApp.GetAllTipos(idAss).OrderBy(p => p.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.UF = new SelectList(menApp.GetAllUF().OrderBy(p => p.UF_SG_SIGLA), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Sexo = new SelectList(cliApp.GetAllSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            List<SelectListItem> dias = new List<SelectListItem>();
            for (int i = 1; i < 32; i++)
            {
                dias.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.Dias = new SelectList(dias, "Value", "Text");
            List<SelectListItem> meses = new List<SelectListItem>();
            meses.Add(new SelectListItem() { Text = "Janeiro", Value = "1" });
            meses.Add(new SelectListItem() { Text = "Fevereiro", Value = "2" });
            meses.Add(new SelectListItem() { Text = "Março", Value = "3" });
            meses.Add(new SelectListItem() { Text = "Abril", Value = "4" });
            meses.Add(new SelectListItem() { Text = "Maio", Value = "5" });
            meses.Add(new SelectListItem() { Text = "Junho", Value = "6" });
            meses.Add(new SelectListItem() { Text = "Julho", Value = "7" });
            meses.Add(new SelectListItem() { Text = "Agosto", Value = "8" });
            meses.Add(new SelectListItem() { Text = "Setembro", Value = "9" });
            meses.Add(new SelectListItem() { Text = "Outubro", Value = "10" });
            meses.Add(new SelectListItem() { Text = "Novembro", Value = "11" });
            meses.Add(new SelectListItem() { Text = "Dezembro", Value = "12" });
            ViewBag.Meses = new SelectList(meses, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Crítica
                    if (vm.SEXO == null & vm.CATEGORIA == null & vm.CIDADE == null & vm.UF == null & vm.DATA_NASC == null & vm.DIA == null & vm.MES == null & vm.ANO == null & vm.ID == null)
                    {
                        Session["MensGrupo"] = 10;
                        return RedirectToAction("MontarTelaGrupo");
                    }

                    // Executa a operação
                    GRUPO item = Mapper.Map<GrupoViewModel, GRUPO>(vm);
                    MontagemGrupo grupo = new MontagemGrupo();
                    grupo.ANO = vm.ANO;
                    grupo.ASSI_CD_ID = vm.ASSI_CD_ID;
                    grupo.CATEGORIA = vm.CATEGORIA;
                    grupo.DATA_NASC = vm.DATA_NASC;
                    grupo.DIA = vm.DIA;
                    grupo.GRUP_DT_CADASTRO = vm.GRUP_DT_CADASTRO;
                    grupo.GRUP_IN_ATIVO = vm.GRUP_IN_ATIVO;
                    grupo.USUA_CD_ID = vm.USUA_CD_ID;
                    grupo.GRUPO = vm.GRUPO;
                    grupo.GRUP_NM_NOME = vm.GRUP_NM_NOME;
                    grupo.ID = vm.ID;
                    grupo.LINK = vm.LINK;
                    grupo.MODELO = vm.MODELO;
                    grupo.NOME = vm.NOME;
                    grupo.MES = vm.MES;
                    grupo.SEXO = vm.SEXO;
                    grupo.STATUS = vm.STATUS;
                    grupo.UF = vm.UF;
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, grupo, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensGrupo"] = 3;
                        return RedirectToAction("MontarTelaGrupo");
                    }
                    if (volta == 2)
                    {
                        Session["MensGrupo"] = 11;
                        return RedirectToAction("MontarTelaGrupo");
                    }

                    // Sucesso
                    listaMaster = new List<GRUPO>();
                    Session["ListaGrupo"] = null;
                    Session["IncluirGrupo"] = 1;
                    Session["GrupoNovo"] = item.GRUP_CD_ID;
                    Session["IdGrupo"] = item.GRUP_CD_ID;
                    if ((Int32)Session["VoltaGrupo"] == 11)
                    {
                        return RedirectToAction("VoltarAnexoCliente", "Cliente");
                    }
                    if ((Int32)Session["VoltaGrupo"] == 1)
                    {
                        return RedirectToAction("VoltarBaseGrupo");
                    }
                    if ((Int32)Session["VoltaGrupo"] == 10)
                    {
                        return RedirectToAction("IncluirMensagemAutomacao", "MensagemAutomacao");
                    }
                    return RedirectToAction("VoltarBaseGrupo");
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
        public ActionResult EditarGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO item = baseApp.GetItemById(id);
            Session["Grupo"] = item;

            // Indicadores
            ViewBag.Incluir = (Int32)Session["IncluirGrupo"];

            // Mensagens
            if (Session["MensGrupo"] != null)
            {


            }

            Session["VoltaGrupo"] = 1;
            objetoAntes = item;
            Session["IdGrupo"] = id;
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarGrupo(GrupoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    GRUPO item = Mapper.Map<GrupoViewModel, GRUPO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<GRUPO>();
                    Session["ListaGrupo"] = null;
                    if ((Int32)Session["VoltaGrupo"] == 10)
                    {
                        return RedirectToAction("VoltarAnexoCliente", "Cliente");
                    }
                    if ((Int32)Session["VoltaCliGrupo"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoCliente", "Cliente");
                    }
                    return RedirectToAction("MontarTelaGrupo");
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
        public ActionResult VerGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO item = baseApp.GetItemById(id);
            Session["Grupo"] = item;

            // Indicadores
            objetoAntes = item;
            Session["IdGrupo"] = id;
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            return View(vm);
        }

        public ActionResult VoltarAnexoGrupo()
        {
            return RedirectToAction("EditarGrupo", new { id = (Int32)Session["IdGrupo"] });
        }

        [HttpGet]
        public ActionResult ExcluirGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO item = baseApp.GetItemById(id);
            objetoAntes = (GRUPO)Session["Grupo"];
            item.GRUP_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult ReativarGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO item = baseApp.GetItemById(id);
            objetoAntes = (GRUPO)Session["Grupo"];
            item.GRUP_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult IncluirContatoGrupo()
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("VoltarAnexoCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Mensagens
            if (Session["MensGrupo"] != null)
            {
                if ((Int32)Session["MensGrupo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0027", CultureInfo.CurrentCulture));
                }
            }
            Session["MensGrupo"] = 0;

            // Prepara view
            List<CLIENTE> lista = null;
            if (Session["ListaClienteGrupo"] == null)
            {
                lista = cliApp.GetAllItens(idAss);
                Session["ListaClienteGrupo"] = lista;
            }
            else
            {
                lista = (List<CLIENTE>)Session["ListaClienteGrupo"];
            }
            ViewBag.Lista = new SelectList(lista.OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            GRUPO_CLIENTE item = new GRUPO_CLIENTE();
            GrupoContatoViewModel vm = Mapper.Map<GRUPO_CLIENTE, GrupoContatoViewModel>(item);
            vm.GRCL_IN_ATIVO = 1;
            vm.GRUP_CD_ID = (Int32)Session["IdGrupo"];
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirContatoGrupo(GrupoContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    GRUPO_CLIENTE item = Mapper.Map<GrupoContatoViewModel, GRUPO_CLIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    
                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensGrupo"] = 4;
                        return RedirectToAction("IncluirContatoGrupo");
                    }

                    // Verifica retorno
                    return RedirectToAction("IncluirContatoGrupo");
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
        public ActionResult ExcluirContatoGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("VoltarAnexoGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO_CLIENTE item = baseApp.GetContatoById(id);
            objetoAntes = (GRUPO)Session["Grupo"];
            item.GRCL_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoGrupo");
        }

        [HttpGet]
        public ActionResult ReativarContatoGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("VoltarAnexoGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO_CLIENTE item = baseApp.GetContatoById(id);
            objetoAntes = (GRUPO)Session["Grupo"];
            item.GRCL_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoGrupo");
        }
    }
}