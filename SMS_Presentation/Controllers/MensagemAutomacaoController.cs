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
//using SendGrid;
//using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace ERP_CRM_Solution.Controllers
{
    public class MensagemAutomacaoController : Controller
    {
        private readonly IMensagemAutomacaoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITemplateSMSAppService temApp;
        private readonly IGrupoAppService gruApp;
        private readonly ITemplateEMailAppService temaApp;
        private readonly IPeriodicidadeAppService perApp;

        private String msg;
        private Exception exception;

        MENSAGEM_AUTOMACAO objeto = new MENSAGEM_AUTOMACAO();
        MENSAGEM_AUTOMACAO objetoAntes = new MENSAGEM_AUTOMACAO();
        List<MENSAGEM_AUTOMACAO> listaMaster = new List<MENSAGEM_AUTOMACAO>();
        String extensao;

        public MensagemAutomacaoController(IMensagemAutomacaoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, ITemplateSMSAppService temApps, IGrupoAppService gruApps, ITemplateEMailAppService temaApps, IPeriodicidadeAppService perApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            temApp = temApps;
            gruApp = gruApps;
            temaApp = temaApps;
            perApp = perApps;
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
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult IncluirGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaGrupo"] = 10;
            return RedirectToAction("IncluirGrupo", "Grupo");
        }

        [HttpGet]
        public ActionResult MontarTelaAutomacao()
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
                if ((Int32)Session["PermMens"] == 0)
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
            if ((List<MENSAGEM_AUTOMACAO>)Session["ListaMensagemAutomacao"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss).OrderByDescending(m => m.MEAU_DT_CADASTRO).ToList();
                Session["ListaMensagemAutomacao"] = listaMaster;
            }
            ViewBag.Listas = (List<MENSAGEM_AUTOMACAO>)Session["ListaMensagemAutomacao"];
            Session["MensagemAutomacao"] = null;
            Session["IncluirMensagemAutomacao"] = 0;

            // Listas
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "SMS", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "WhatsApp", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

            // Mensagens
            if (Session["MensMensagemAutomacao"] != null)
            {
                if ((Int32)Session["MensMensagemAutomacao"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagemAutomacao"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0113", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagemAutomacao"] == 40)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagemAutomacao"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagemAutomacao"] == 60)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0064", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagemAutomacao"] == 61)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMensagemAutomacao"] = 1;
            objeto = new MENSAGEM_AUTOMACAO();
            if (Session["FiltroMensagemAutomacao"] != null)
            {
                objeto = (MENSAGEM_AUTOMACAO)Session["FiltroMensagemAutomacao"];
            }
            return View(objeto);
        }

        public ActionResult RetirarFiltroMensagemAutomacao()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMensagemAutomacao"] = null;
            Session["FiltroMensagemAutomacao"] = null;
            return RedirectToAction("MontarTelaAutomacao");
        }

        public ActionResult MostrarTudoMensagemAutomacao()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss).OrderByDescending(m => m.MEAU_DT_CADASTRO).ToList();
            Session["ListaMensagemAutomacao"] = null;
            Session["FiltroMensagemAutomacao"] = listaMaster;
            return RedirectToAction("MontarTelaAutomacao");
        }

        [HttpPost]
        public ActionResult FiltrarMensagemAutomacao(MENSAGEM_AUTOMACAO item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<MENSAGEM_AUTOMACAO> listaObj = new List<MENSAGEM_AUTOMACAO>();
                Session["FiltroMensagemAutomacao"] = item;
                Int32 volta = baseApp.ExecuteFilter(item.MEAU_IN_TIPO, item.GRUP_CD_ID, item.MEAU_DS_DESCRICAO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaMensagemAutomacao"] = listaObj;
                return RedirectToAction("MontarTelaAutomacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaAutomacao");
            }
        }

        public ActionResult VoltarBaseMensagemAutomacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaAutomacao");
        }

        public ActionResult VoltarMensagemAnexoAutomacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaAutomacao");
        }

        [HttpGet]
        public ActionResult ExcluirMensagemAutomacao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensAutomacao"] = 2;
                    return RedirectToAction("VoltarBaseMensagemAutomacao");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MENSAGEM_AUTOMACAO item = baseApp.GetItemById(id);
            item.MEAU_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            Session["ListaMensagemAutomacao"] = null;
            return RedirectToAction("VoltarBaseMensagemAutomacao");
        }

        [HttpGet]
        public ActionResult ReativarMensagemAutomacao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemSMS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MENSAGEM_AUTOMACAO item = baseApp.GetItemById(id);
            item.MEAU_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaMensagemAutomacao"] = null;
            return RedirectToAction("VoltarBaseMensagemAutomacao");
        }

        public JsonResult PesquisaTemplateSMS(String temp)
        {
            // Recupera Template
            TEMPLATE_SMS tmp = temApp.GetItemById(Convert.ToInt32(temp));

            // Atualiza
            var hash = new Hashtable();
            hash.Add("TSMS_TX_CORPO", tmp.TSMS_TX_CORPO);
            hash.Add("TSMS_LK_LINK", tmp.TSMS_LK_LINK);

            // Retorna
            return Json(hash);
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemAutomacao()
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
                    Session["MensMensagemAutomacao"] = 2;
                    return RedirectToAction("VoltarBaseMensagemAutomacao");
                }
                if ((Int32)Session["PermMens"] == 0)
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

            // Prepara listas   
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "SMS", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "WhatsApp", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> tipoSelecao = new List<SelectListItem>();
            tipoSelecao.Add(new SelectListItem() { Text = "Faixa de Datas", Value = "1" });
            tipoSelecao.Add(new SelectListItem() { Text = "Datas Fixas", Value = "2" });
            ViewBag.TipoSelecao = new SelectList(tipoSelecao, "Value", "Text");
            ViewBag.TempSMS = new SelectList(temApp.GetAllItens(idAss).ToList().OrderBy(p => p.TSMS_NM_NOME), "TSMS_CD_ID", "TSMS_NM_NOME");
            ViewBag.TempEMail = new SelectList(temaApp.GetAllItens(idAss).ToList().OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");
            List<SelectListItem> dia = new List<SelectListItem>();
            dia.Add(new SelectListItem() { Text = "2a Feira", Value = "1" });
            dia.Add(new SelectListItem() { Text = "3a Feira", Value = "2" });
            dia.Add(new SelectListItem() { Text = "4a Feira", Value = "3" });
            dia.Add(new SelectListItem() { Text = "5a Feira", Value = "4" });
            dia.Add(new SelectListItem() { Text = "6a Feira", Value = "5" });
            dia.Add(new SelectListItem() { Text = "Sábado", Value = "6" });
            dia.Add(new SelectListItem() { Text = "Domingo", Value = "7" });
            ViewBag.Dias = new SelectList(dia, "Value", "Text");
            ViewBag.Periodicidades = new SelectList(perApp.GetAllItens(idAss).OrderBy(p => p.PERI_NM_NOME), "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> aniv = new List<SelectListItem>();
            aniv.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aniv.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Aniversario = new SelectList(aniv, "Value", "Text");
            List<SelectListItem> tipoSel = new List<SelectListItem>();
            tipoSel.Add(new SelectListItem() { Text = "Peridicidade", Value = "1" });
            tipoSel.Add(new SelectListItem() { Text = "Dia da Semana", Value = "2" });
            tipoSel.Add(new SelectListItem() { Text = "Data Fixa", Value = "3" });
            ViewBag.TipoSel = new SelectList(tipoSel, "Value", "Text");

            // Prepara view
            if (Session["MensMensagemAutomacao"] != null)
            {
                if ((Int32)Session["MensMensagemAutomacao"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
            }

            Session["MensagemNovo"] = 0;
            MENSAGEM_AUTOMACAO item = new MENSAGEM_AUTOMACAO();
            MensagemAutomacaoViewModel vm = Mapper.Map<MENSAGEM_AUTOMACAO, MensagemAutomacaoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.MEAU_DT_CADASTRO = DateTime.Now;
            vm.MEAU_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemAutomacao(MensagemAutomacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "SMS", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "WhatsApp", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> tipoSelecao = new List<SelectListItem>();
            tipoSelecao.Add(new SelectListItem() { Text = "Faixa de Datas", Value = "1" });
            tipoSelecao.Add(new SelectListItem() { Text = "Datas Fixas", Value = "2" });
            ViewBag.TipoSelecao = new SelectList(tipoSelecao, "Value", "Text");
            ViewBag.TempSMS = new SelectList(temApp.GetAllItens(idAss).ToList().OrderBy(p => p.TSMS_NM_NOME), "TSMS_CD_ID", "TSMS_NM_NOME");
            ViewBag.TempEMail = new SelectList(temaApp.GetAllItens(idAss).ToList().OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");
            List<SelectListItem> dia = new List<SelectListItem>();
            dia.Add(new SelectListItem() { Text = "2a Feira", Value = "1" });
            dia.Add(new SelectListItem() { Text = "3a Feira", Value = "2" });
            dia.Add(new SelectListItem() { Text = "4a Feira", Value = "3" });
            dia.Add(new SelectListItem() { Text = "5a Feira", Value = "4" });
            dia.Add(new SelectListItem() { Text = "6a Feira", Value = "5" });
            dia.Add(new SelectListItem() { Text = "Sábado", Value = "6" });
            dia.Add(new SelectListItem() { Text = "Domingo", Value = "7" });
            ViewBag.Dias = new SelectList(dia, "Value", "Text");
            ViewBag.Periodicidades = new SelectList(perApp.GetAllItens(idAss).OrderBy(p => p.PERI_NM_NOME), "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> aniv = new List<SelectListItem>();
            aniv.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aniv.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Aniversario = new SelectList(aniv, "Value", "Text");
            List<SelectListItem> tipoSel = new List<SelectListItem>();
            tipoSel.Add(new SelectListItem() { Text = "Peridicidade", Value = "1" });
            tipoSel.Add(new SelectListItem() { Text = "Dia da Semana", Value = "2" });
            tipoSel.Add(new SelectListItem() { Text = "Data Fixa", Value = "3" });
            ViewBag.TipoSel = new SelectList(tipoSel, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    MENSAGEM_AUTOMACAO item = Mapper.Map<MensagemAutomacaoViewModel, MENSAGEM_AUTOMACAO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMensagemAutomacao"] = 3;
                        return RedirectToAction("MontarTelaAutomacao");
                    }
                    Session["IdMensagem"] = item.MEAU_CD_ID;

                    // Sucesso
                    listaMaster = new List<MENSAGEM_AUTOMACAO>();
                    Session["ListaMensagemAutomacao"] = null;
                    Session["MensagemNovoAutomacao"] = item.MEAU_CD_ID;
                    return RedirectToAction("MontarTelaAutomacao");
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

        public ActionResult VoltarAnexoMensagemAutomacao()
        {
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("VerMensagemAutomacao", new { id = (Int32)Session["IdMensagem"] });
        }

        [HttpGet]
        public ActionResult VerMensagemAutomacao(Int32 id)
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
                    Session["MensMensagemAutomacao"] = 2;
                    return RedirectToAction("MontarTelaAutomacao");
                }
                if ((Int32)Session["PermMens"] == 0)
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

            if (Session["MensMensagemAutomacao"] != null)
            {
                if ((Int32)Session["MensMensagemAutomacao"] == 40)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
            }
            Session["IdMensagem"] = id;
            Session["VoltaMensagemAutomacao"] = 1;
            MENSAGEM_AUTOMACAO item = baseApp.GetItemById(id);
            MensagemAutomacaoViewModel vm = Mapper.Map<MENSAGEM_AUTOMACAO, MensagemAutomacaoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult EditarMensagemAutomacao(Int32 id)
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
                    Session["MensMensagemAutomacao"] = 2;
                    return RedirectToAction("VoltarBaseMensagemAutomacao");
                }
                if ((Int32)Session["PermMens"] == 0)
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

            // Prepara listas   
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            if (Session["MensMensagemAutomacao"] != null)
            {
                if ((Int32)Session["MensMensagemAutomacao"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
            }

            Session["MensagemNovo"] = 0;
            MENSAGEM_AUTOMACAO item = baseApp.GetItemById(id);
            MensagemAutomacaoViewModel vm = Mapper.Map<MENSAGEM_AUTOMACAO, MensagemAutomacaoViewModel>(item);
            vm.Tipo = vm.MEAU_IN_TIPO == 1 ? "E-Mail" : "SMS";
            vm.DataDisparo = vm.MEAU_DT_DATA_FIXA.Value.ToShortDateString().Substring(0, 5);
            vm.Aniversario = vm.MEAU_IN_ANIVERSARIO == 1 ? "Sim" : "Não";
            if (vm.MEAU_IN_DIA_SEMANA == 1)
            {
                vm.DiaSemana = "2a Feira";
            }
            else if (vm.MEAU_IN_DIA_SEMANA == 2)
            {
                vm.DiaSemana = "3a Feira";
            }
            else if (vm.MEAU_IN_DIA_SEMANA == 3)
            {
                vm.DiaSemana = "4a Feira";
            }
            else if (vm.MEAU_IN_DIA_SEMANA == 4)
            {
                vm.DiaSemana = "5a Feira";
            }
            else if (vm.MEAU_IN_DIA_SEMANA == 5)
            {
                vm.DiaSemana = "6a Feira";
            }
            else if (vm.MEAU_IN_DIA_SEMANA == 6)
            {
                vm.DiaSemana = "Sábado";
            }
            else if (vm.MEAU_IN_DIA_SEMANA == 7)
            {
                vm.DiaSemana = "Domingo";
            }
            if (vm.MEAU_IN_TIPO_ENVIO == 1)
            {
                vm.TipoSel = "Periodicidade";
            }
            else if (vm.MEAU_IN_TIPO_ENVIO == 2)
            {
                vm.TipoSel = "Dia da Semana";
            }
            else if (vm.MEAU_IN_TIPO_ENVIO == 3)
            {
                vm.TipoSel = "Data Fixa";
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarMensagemAutomacao(MensagemAutomacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    MENSAGEM_AUTOMACAO item = Mapper.Map<MensagemAutomacaoViewModel, MENSAGEM_AUTOMACAO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<MENSAGEM_AUTOMACAO>();
                    Session["ListaMensagemAutomacao"] = null;
                    Session["MensagemNovoAutomacao"] = item.MEAU_CD_ID;
                    return RedirectToAction("MontarTelaAutomacao");
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






    }
}