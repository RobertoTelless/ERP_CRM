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
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ERP_CRM_Solution.Controllers
{
    public class MensagemController : Controller
    {
        private readonly IMensagemAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IClienteAppService cliApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITemplateSMSAppService temApp;
        private readonly IGrupoAppService gruApp;
        private readonly ITemplateEMailAppService temaApp;
        private readonly IEMailAgendaAppService emApp;
        private readonly ICRMAppService crmApp;

        private String msg;
        private Exception exception;

        MENSAGENS objeto = new MENSAGENS();
        MENSAGENS objetoAntes = new MENSAGENS();
        List<MENSAGENS> listaMaster = new List<MENSAGENS>();
        String extensao;

        public MensagemController(IMensagemAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IClienteAppService cliApps, IConfiguracaoAppService confApps, ITemplateSMSAppService temApps, IGrupoAppService gruApps, ITemplateEMailAppService temaApps, IEMailAgendaAppService emApps, ICRMAppService crmApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            cliApp = cliApps;
            confApp = confApps;
            temApp = temApps;
            gruApp = gruApps;
            temaApp = temaApps;
            emApp = emApps;
            crmApp = crmApps;
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

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<Hashtable> listResult = new List<Hashtable>();
            List<CLIENTE> clientes = cliApp.GetAllItens(idAss);

            if (nome != null)
            {
                List<CLIENTE> lstCliente = clientes.Where(x => x.CLIE_NM_NOME != null && x.CLIE_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = clientes.Where(x => x.CLIE_NM_RAZAO != null).ToList<CLIENTE>();
                    lstCliente = lstCliente.Where(x => x.CLIE_NM_RAZAO.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.CLIE_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.CLIE_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.CLIE_NM_NOME + " (" + item.CLIE_NM_RAZAO + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaMensagemSMS()
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
            if ((List<MENSAGENS>)Session["ListaMensagem"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
                Session["ListaMensagem"] = listaMaster;
            }
            ViewBag.Listas = (List<MENSAGENS>)Session["ListaMensagem"];
            Session["Mensagem"] = null;
            Session["IncluirMensagem"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 51)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 40)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 60)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0064", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 61)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            objeto = new MENSAGENS();
            //if (Session["FiltroMensagem"] != null)
            //{
            //    objeto = (MENSAGENS)Session["FiltroMensagem"];
            //}
            return View(objeto);
        }

        public ActionResult RetirarFiltroMensagemSMS()
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
            Session["ListaMensagem"] = null;
            Session["FiltroMensagem"] = null;
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        public ActionResult MostrarTudoMensagemSMS()
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
            listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            Session["ListaMensagem"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        public ActionResult MostrarMesesMensagemSMS()
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
            listaMaster = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 2).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            Session["ListaMensagem"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        [HttpPost]
        public ActionResult FiltrarMensagemSMS(MENSAGENS item)
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
                List<MENSAGENS> listaObj = new List<MENSAGENS>();
                Session["FiltroMensagem"] = item;
                Int32 volta = baseApp.ExecuteFilterSMS(item.MENS_DT_ENVIO, item.MENS_IN_ATIVO.Value, item.MENS_TX_TEXTO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaMensagem"] = listaObj;
                return RedirectToAction("MontarTelaMensagemSMS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMensagem");
            }
        }

        public ActionResult VoltarBaseMensagemSMS()
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
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        public ActionResult VoltarMensagemAnexoSMS()
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
            Int32 volta = (Int32)Session["VoltaMensagem"];
            if (volta == 1)
            {
                return RedirectToAction("MontarTelaMensagemSMS");
            }
            else if (volta == 2)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            else if (volta == 3)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaMensagemSMS");
        }

        [HttpGet]
        public ActionResult ExcluirMensagemSMS(Int32 id)
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

            MENSAGENS item = baseApp.GetItemById(id);
            item.MENS_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            Session["ListaMensagem"] = null;
            return RedirectToAction("VoltarBaseMensagemSMS");
        }

        [HttpGet]
        public ActionResult ReativarMensagemSMS(Int32 id)
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

            MENSAGENS item = baseApp.GetItemById(id);
            item.MENS_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaMensagem"] = null;
            return RedirectToAction("VoltarBaseMensagemSMS");
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
        public ActionResult IncluirMensagemSMS()
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
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemSMS");
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

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO.Value == 2 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
            if ((Int32)Session["NumSMS"] <= num)
            {
                Session["MensMensagem"] = 51;
                return RedirectToAction("VoltarBaseMensagemSMS");
            }

            // Prepara listas   
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(temApp.GetAllItens(idAss).ToList().OrderBy(p => p.TSMS_NM_NOME), "TSMS_CD_ID", "TSMS_NM_NOME");

            // Prepara view
            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
            }

            Session["MensagemNovo"] = 0;
            MENSAGENS item = new MENSAGENS();
            MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.MENS_DT_CRIACAO = DateTime.Now;
            vm.MENS_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.MENS_IN_TIPO = 2;
            vm.MENS_TX_SMS = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemSMS(MensagemViewModel vm)
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

            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(temApp.GetAllItens(idAss).ToList().OrderBy(p => p.TSMS_NM_NOME), "TSMS_CD_ID", "TSMS_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Checa mensagens
                    if (String.IsNullOrEmpty(vm.MENS_TX_SMS) & vm.TEEM_CD_ID == null)
                    {
                        Session["MensMensagem"] = 3;
                        return RedirectToAction("IncluirMensagemSMS");
                    }

                    // Executa a operação
                    MENSAGENS item = Mapper.Map<MensagemViewModel, MENSAGENS>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdMensagem"] = item.MENS_CD_ID;
                    if (Session["FileQueueMensagem"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueMensagem"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueMensagem(file);
                            }
                        }
                        Session["FileQueueMensagem"] = null;
                    }

                    // Processa
                    MENSAGENS mens = baseApp.GetItemById(item.MENS_CD_ID);
                    Session["IdMensagem"] = mens.MENS_CD_ID;
                    vm.MENS_CD_ID = mens.MENS_CD_ID;
                    vm.MENSAGEM_ANEXO = mens.MENSAGEM_ANEXO;
                    Int32 retGrava = ProcessarEnvioMensagemSMS(vm, usuario);
                    if (retGrava > 0)
                    {

                    }

                    // Sucesso
                    listaMaster = new List<MENSAGENS>();
                    Session["ListaMensagem"] = null;
                    Session["MensagemNovo"] = item.MENS_CD_ID;
                    return RedirectToAction("MontarTelaMensagemSMS");
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
            List<System.Net.Mail.Attachment> att = new List<System.Net.Mail.Attachment>();
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
                att.Add(new System.Net.Mail.Attachment(file.InputStream, f.Name));
                queue.Add(f);
            }
            Session["FileQueueMensagem"] = queue;
            Session["Attachments"] = att;
        }

        [HttpPost]
        public ActionResult UploadFileQueueMensagem(FileQueue file)
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
            Int32 idNot = (Int32)Session["IdMensagem"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensMensagem"] = 10;
                return RedirectToAction("VoltarBaseMensagemSMS");
            }

            MENSAGENS item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensMensagem"] = 11;
                return RedirectToAction("VoltarBaseMensagemSMS");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            MENSAGEM_ANEXO foto = new MENSAGEM_ANEXO();
            foto.MEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.MEAN_DT_ANEXO = DateTime.Today;
            foto.MEAN_IN_ATIVO = 1;
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
            foto.MEAN_IN_TIPO = tipo;
            foto.MEAN_NM_TITULO = fileName.Length < 49 ? fileName : fileName.Substring(0, 48);
            foto.MENS_CD_ID = item.MENS_CD_ID;

            item.MENSAGEM_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarBaseMensagemSMS");
        }

        [HttpGet]
        public ActionResult VerAnexoMensagemSMS(Int32 id)
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
                    Session["MensMensagens"] = 2;
                    return RedirectToAction("MontarTelaMensagemSMS", "Mensagem");
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

            // Prepara view
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoMensagemSMS()
        {
            if ((Int32)Session["PermMens"] == 0)
            {
                Session["MensPermissao"] = 2;
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("VerMensagemSMS", new { id = (Int32)Session["IdMensagem"] });
        }

        public FileResult DownloadMensagemSMS(Int32 id)
        {
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.MEAN_AQ_ARQUIVO;
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
        public ActionResult VerMensagemSMS(Int32 id)
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
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("MontarTelaMensagem", "Mensagem");
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

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 40)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
            }
            Session["IdMensagem"] = id;
            Session["VoltaMensagem"] = 1;
            MENSAGENS item = baseApp.GetItemById(id);
            MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
            return View(vm);
        }

        [ValidateInput(false)]
        public Int32 ProcessarEnvioMensagemSMS(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cliente = null;
            GRUPO grupo = null;
            List<CLIENTE> listaCli = new List<CLIENTE>();
            Int32 escopo = 0;
            String erro = null;
            Int32 volta = 0;
            ERP_CRMEntities Db = new ERP_CRMEntities();
            MENSAGENS mens = baseApp.GetItemById(vm.MENS_CD_ID);

            // Nome
            if (vm.ID > 0)
            {                
                cliente = cliApp.GetItemById(vm.ID.Value);
                escopo = 1;
            }
            else if (vm.GRUPO > 0)
            {
                listaCli = new List<CLIENTE>();
                grupo = gruApp.GetItemById(vm.GRUPO.Value);
                foreach (GRUPO_CLIENTE item in grupo.GRUPO_CLIENTE)
                {
                    if (item.GRCL_IN_ATIVO == 1)
                    {
                        listaCli.Add(item.CLIENTE);
                    }
                }
                escopo = 2;
            }

            // Monta token
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = String.Empty;
            String link = String.Empty;
            if (vm.TEEM_CD_ID != null)
            {
                TEMPLATE_SMS temp = temApp.GetItemById(vm.TEEM_CD_ID.Value);
                texto = temp.TSMS_TX_CORPO;
                link = temp.TSMS_LK_LINK;
            }
            else
            {
                texto = vm.MENS_TX_SMS;
                link = vm.MENS_NM_LINK;
            }

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(texto);
            if (!String.IsNullOrEmpty(link))
            {
                if (!link.Contains("www."))
                {
                    link = "www." + link;
                }
                if (!link.Contains("http://"))
                {
                    link = "http://" + link;
                }
                //str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                str.AppendLine(link + " Clique aqui para maiores informações");
                texto += "  " + link;
            }
            String body = str.ToString();
            body = body.Replace("\r\n", " ");
            String smsBody = body;
                
            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            if (escopo == 1)
            {
                try
                {
                    String listaDest = "55" + Regex.Replace(cliente.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    String customId = Cryptography.GenerateRandomPassword(8);
                    String data = String.Empty;
                    String json = String.Empty;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        if (vm.MENS_DT_AGENDAMENTO != null)
                        {
                            data = vm.MENS_DT_AGENDAMENTO.Value.Year.ToString() + "-" + vm.MENS_DT_AGENDAMENTO.Value.Month.ToString() + "-" + vm.MENS_DT_AGENDAMENTO.Value.Day.ToString() + "T" + vm.MENS_DT_AGENDAMENTO.Value.ToShortTimeString() + ":00";
                            json = String.Concat("{\"scheduleTime\": \"", data ,"\",\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", body, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                        }
                        else
                        {
                            json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", body, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                        }
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

                // Grava mensagem/destino e erros
                if (erro == null)
                {
                    MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                    dest.MEDE_IN_ATIVO = 1;
                    dest.MEDE_IN_POSICAO = 1;
                    dest.MEDE_IN_STATUS = 1;
                    dest.CLIE_CD_ID = cliente.CLIE_CD_ID;
                    dest.MEDE_DS_ERRO_ENVIO = resposta;
                    dest.MENS_CD_ID = mens.MENS_CD_ID;
                    mens.MENSAGENS_DESTINOS.Add(dest);
                    mens.MENS_DT_ENVIO = DateTime.Now;
                    mens.MENS_TX_SMS = body;
                    volta = baseApp.ValidateEdit(mens, mens);
                }
                else
                {
                    mens.MENS_TX_RETORNO = erro;
                    volta = baseApp.ValidateEdit(mens, mens);
                }
                erro = null;
                return volta;
            }
            else
            {
                try
                {
                    // Monta Array de envio
                    String vetor = String.Empty;
                    foreach (CLIENTE cli in listaCli)
                    {
                        String listaDest = "55" + Regex.Replace(cli.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                        String customId = Cryptography.GenerateRandomPassword(8);
                        if (vetor == String.Empty)
                        {
                            vetor = "{\"to\": \"," + listaDest + ", \", \"text\": \"," + texto + "\", \"from\": \"ERPSys\"}";
                        }
                        else
                        {
                            vetor += ",{\"to\": \"," + listaDest + ", \", \"text\": \"," + texto + "\", \"from\": \"ERPSys\"}";
                        }
                    }

                    // Configura                    
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    String data = String.Empty;
                    String json = String.Empty;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        json = String.Concat("{\"destinations\": [", vetor ,"]}");
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

                // Grava mensagem/destino e erros
                if (erro == null)
                {
                    foreach (CLIENTE cli in listaCli)
                    {
                        MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                        dest.MEDE_IN_ATIVO = 1;
                        dest.MEDE_IN_POSICAO = 1;
                        dest.MEDE_IN_STATUS = 1;
                        dest.CLIE_CD_ID = cli.CLIE_CD_ID;
                        dest.MEDE_DS_ERRO_ENVIO = resposta;
                        dest.MENS_CD_ID = mens.MENS_CD_ID;
                        mens.MENSAGENS_DESTINOS.Add(dest);
                    }
                    mens.MENS_DT_ENVIO = DateTime.Now;
                    volta = baseApp.ValidateEdit(mens, mens);
                }
                else
                {
                    mens.MENS_TX_RETORNO = erro;
                    volta = baseApp.ValidateEdit(mens, mens);
                }
                erro = null;
                return volta;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult ConverterMensagemCRMSMS(Int32 id)
        {
            // Recupera Mensagem e contato
            MENSAGENS_DESTINOS dest = baseApp.GetDestinoById(id);
            MENSAGENS mensagem = baseApp.GetItemById(dest.MENS_CD_ID);
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            if ((Int32)Session["PermCRM"] == 1)
            {
                if (mensagem.MENS_IN_CRM == 0)
                {
                    Int32 num = crmApp.GetAllItens(idAss).Count;
                    if ((Int32)Session["NumProc"] <= num)
                    {
                        Session["MensMensagem"] = 50;
                        return RedirectToAction("VoltarAnexoMensagemSMS");
                    }
                }
                else
                {
                    Session["MensMensagem"] = 60;
                    return RedirectToAction("VoltarAnexoMensagemSMS");
                }
            }
            else
            {
                Session["MensMensagem"] = 61;
                return RedirectToAction("VoltarVoltarAnexoMensagemSMSAnexoMensagemEMail");
            }

            // Cria CRM
            CRM crm = new CRM();
            crm.ASSI_CD_ID = mensagem.ASSI_CD_ID;
            crm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            crm.CRM1_DS_DESCRICAO = "Processo criado a partir de SMS";
            crm.CRM1_DT_CRIACAO = DateTime.Today.Date;
            crm.CRM1_IN_ATIVO = 1;
            crm.CRM1_IN_STATUS = 1;
            crm.CRM1_NM_NOME = "Processo criado a partir de SMS";
            crm.TICR_CD_ID = 1;
            crm.USUA_CD_ID = usuario.USUA_CD_ID;
            crm.MENS_CD_ID = mensagem.MENS_CD_ID;
            crm.ORIG_CD_ID = 1;
            Int32 volta = crmApp.ValidateCreate(crm, usuario);

            // Atualiza destino
            dest.MEDE_IN_POSICAO = 4;
            Int32 volta1 = baseApp.ValidateEditDestino(dest);

            // Atualiza mensagem
            mensagem.MENS_IN_CRM = 1;
            Int32 volta2 = baseApp.ValidateEdit(mensagem, mensagem);

            // Retorno
            Session["MensMensagem"] = 40;
            return RedirectToAction("VoltarAnexoMensagemSMS");
        }

        [HttpGet]
        public ActionResult ConverterMensagemCRMEMail(Int32 id)
        {         
            // Recupera Mensagem e contato
            MENSAGENS_DESTINOS dest = baseApp.GetDestinoById(id);
            MENSAGENS mensagem = baseApp.GetItemById(dest.MENS_CD_ID);
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            if ((Int32)Session["PermCRM"] == 1)
            {
                if (mensagem.MENS_IN_CRM == 0)
                {
                    Int32 num = crmApp.GetAllItens(idAss).Count;
                    if ((Int32)Session["NumProc"] <= num)
                    {
                        Session["MensMensagem"] = 50;
                        return RedirectToAction("VoltarAnexoMensagemEMail");
                    }
                }
                else
                {
                    Session["MensMensagem"] = 60;
                    return RedirectToAction("VoltarAnexoMensagemEMail");
                }
            }
            else
            {
                Session["MensMensagem"] = 61;
                return RedirectToAction("VoltarAnexoMensagemEMail");
            }

            // Cria CRM
            CRM crm = new CRM();
            crm.ASSI_CD_ID = mensagem.ASSI_CD_ID;
            crm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            crm.CRM1_DS_DESCRICAO = "Processo criado a partir de E-Mail";
            crm.CRM1_DT_CRIACAO = DateTime.Today.Date;
            crm.CRM1_IN_ATIVO = 1;
            crm.CRM1_IN_STATUS = 1;
            crm.CRM1_NM_NOME = "Processo criado a partir de E-Mail";
            crm.TICR_CD_ID = 1;
            crm.USUA_CD_ID = usuario.USUA_CD_ID;
            crm.MENS_CD_ID = mensagem.MENS_CD_ID;
            crm.ORIG_CD_ID = 1;
            Int32 volta = crmApp.ValidateCreate(crm, usuario);

            // Atualiza destino
            dest.MEDE_IN_POSICAO = 4;
            Int32 volta1 = baseApp.ValidateEditDestino(dest);

            // Atualiza mensagem
            mensagem.MENS_IN_CRM = 1;
            Int32 volta2 = baseApp.ValidateEdit(mensagem, mensagem);

            // Retorno
            Session["MensMensagem"] = 40;
            return RedirectToAction("VoltarAnexoMensagemEMail");
        }

        [HttpGet]
        public ActionResult MontarTelaMensagemEMail()
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
            if ((List<MENSAGENS>)Session["ListaMensagem"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
                Session["ListaMensagem"] = listaMaster;
            }
            ViewBag.Listas = (List<MENSAGENS>)Session["ListaMensagem"];
            Session["Mensagem"] = null;
            Session["IncluirMensagem"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 51)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 40)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 60)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0064", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMensagem"] == 61)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            objeto = new MENSAGENS();
            //if (Session["FiltroMensagem"] != null)
            //{
            //    objeto = (MENSAGENS)Session["FiltroMensagem"];
            //}
            return View(objeto);
        }

        public ActionResult RetirarFiltroMensagemEMail()
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
            Session["ListaMensagem"] = null;
            Session["FiltroMensagem"] = null;
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult MostrarTudoMensagemEMail()
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
            listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            Session["ListaMensagem"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult MostrarMesesMensagemEMail()
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
            listaMaster = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 1).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
            Session["ListaMensagem"] = listaMaster;
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        [HttpPost]
        public ActionResult FiltrarMensagemEMail(MENSAGENS item)
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
                List<MENSAGENS> listaObj = new List<MENSAGENS>();
                Session["FiltroMensagem"] = item;
                Int32 volta = baseApp.ExecuteFilterEMail(item.MENS_DT_ENVIO, item.MENS_IN_ATIVO.Value, item.MENS_TX_TEXTO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaMensagem"] = listaObj;
                return RedirectToAction("MontarTelaMensagemEmail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMensagemEmail");
            }
        }

        public ActionResult VoltarBaseMensagemEMail()
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
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult VoltarAnexoMensagemEMail()
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
            Int32 volta = (Int32)Session["VoltaMensagem"];
            if (volta == 1)
            {
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            else if (volta == 2)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            else if (volta == 3)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        [HttpGet]
        public ActionResult ExcluirMensagemEMail(Int32 id)
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
                    return RedirectToAction("VoltarBaseMensagemEMail");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MENSAGENS item = baseApp.GetItemById(id);
            item.MENS_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            Session["ListaMensagem"] = null;
            return RedirectToAction("VoltarBaseMensagemEMail");
        }

        [HttpGet]
        public ActionResult ReativarMensagemEMail(Int32 id)
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
                    return RedirectToAction("VoltarBaseMensagemEMail");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MENSAGENS item = baseApp.GetItemById(id);
            item.MENS_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaMensagem"] = null;
            return RedirectToAction("VoltarBaseMensagemEMail");
        }

        public JsonResult PesquisaTemplateEMail(String temp)
        {
            // Recupera Template
            TEMPLATE_EMAIL tmp = temaApp.GetItemById(Convert.ToInt32(temp));

            // Atualiza
            var hash = new Hashtable();
            hash.Add("MENS_TX_CORPO", tmp.TEEM_TX_CORPO);
            hash.Add("MENS_NM_CABECALHO", tmp.TEEM_TX_CABECALHO);
            hash.Add("MENS_NM_RODAPE", tmp.TEEM_TX_DADOS);
            hash.Add("MENS_LK_LINK", tmp.TEEM_LK_LINK);
            hash.Add("STATUS", tmp.TEEM_IN_HTML);
            hash.Add("MODELO", tmp.TEEM_AQ_ARQUIVO);

            // Retorna
            return Json(hash);
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemEMail()
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
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("VoltarBaseMensagemEMail");
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

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO.Value == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
            if ((Int32)Session["NumEMail"] <= num)
            {
                Session["MensMensagem"] = 51;
                return RedirectToAction("VoltarBaseMensagemEMail");
            }

            // Prepara listas   
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(temaApp.GetAllItens(idAss).OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");

            //String caminho = "/TemplatesHTML/";
            //String path = Path.Combine(Server.MapPath(caminho));
            //String[] files = Directory.GetFiles(path, "*.html");
            //List<SelectListItem> mod = new List<SelectListItem>();
            //foreach (String file in files)
            //{
            //    mod.Add(new SelectListItem() { Text = System.IO.Path.GetFileNameWithoutExtension(file), Value = file });
            //}
            //ViewBag.Modelos = new SelectList(mod, "Value", "Text");

            // Prepara view

            // Mensagens
            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
            }

            Session["MensagemNovo"] = 0;
            MENSAGENS item = new MENSAGENS();
            MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.MENS_DT_CRIACAO = DateTime.Now;
            vm.MENS_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.MENS_IN_TIPO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemEMail(MensagemViewModel vm)
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

            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(idAss).OrderBy(p => p.CLIE_NM_NOME), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(temaApp.GetAllItens(idAss).OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Checa mensagens
                    if (String.IsNullOrEmpty(vm.MENS_TX_TEXTO) & vm.TEEM_CD_ID == null)
                    {
                        Session["MensMensagem"] = 3;
                        return RedirectToAction("IncluirMensagemEmail");
                    }

                    // Executa a operação
                    MENSAGENS item = Mapper.Map<MensagemViewModel, MENSAGENS>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdMensagem"] = item.MENS_CD_ID;
                    if (Session["FileQueueMensagem"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueMensagem"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueMensagem(file);
                            }
                        }
                        Session["FileQueueMensagem"] = null;
                    }

                    // Processa
                    if (item.MENS_DT_AGENDAMENTO == null)
                    {
                        MENSAGENS mens = baseApp.GetItemById(item.MENS_CD_ID);
                        Session["IdMensagem"] = mens.MENS_CD_ID;
                        vm.MENS_CD_ID = mens.MENS_CD_ID;
                        vm.MENSAGEM_ANEXO = mens.MENSAGEM_ANEXO;
                        Int32 retGrava = ProcessarEnvioMensagemEMail(vm, usuario);
                        if (retGrava > 0)
                        {

                        }
                    }
                    else
                    {
                        MENSAGENS mens = baseApp.GetItemById(item.MENS_CD_ID);
                        Session["IdMensagem"] = mens.MENS_CD_ID;
                        vm.MENS_CD_ID = mens.MENS_CD_ID;
                        vm.MENSAGEM_ANEXO = mens.MENSAGEM_ANEXO;
                        Int32 retGrava = ProcessarAgendamentoMensagemEMail(vm, usuario);
                        if (retGrava > 0)
                        {

                        }
                    }

                    // Sucesso
                    listaMaster = new List<MENSAGENS>();
                    Session["ListaMensagem"] = null;
                    Session["MensagemNovo"] = item.MENS_CD_ID;
                    return RedirectToAction("MontarTelaMensagemEMail");
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
        public ActionResult VerEMailAgendados()
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

            Session["EMailAgenda"] = emApp.GetAllItens(idAss).OrderByDescending(p => p.EMAG_DT_AGENDAMENTO).ToList();
            MENSAGENS mens = new MENSAGENS();
            ViewBag.Listas = (List<EMAIL_AGENDAMENTO>)Session["EMailAgenda"];
            Session["VoltaMensagem"] = 3;
            return View(mens);
        }

        public static async Task SendEmailAssync(Email email)
        {
            try
            {
                MailMessage mensagem = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
                mensagem.To.Add(email.EMAIL_DESTINO);
                mensagem.Subject = email.ASSUNTO;
                mensagem.IsBodyHtml = true;
                mensagem.Body = email.CORPO;
                mensagem.Priority = email.PRIORIDADE;
                mensagem.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        mensagem.Attachments.Add(attachment);
                    }
                }
                smtp.EnableSsl = email.ENABLE_SSL;
                smtp.Port = Convert.ToInt32(email.PORTA);
                smtp.Host = email.SMTP;
                smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
                smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
                await smtp.SendMailAsync(mensagem);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string SendEmail(Email email)
        {
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            
            var tasks = new List<Task>();
            var smtp = new SmtpClient();
            smtp.EnableSsl = email.ENABLE_SSL;
            smtp.Port = Convert.ToInt32(email.PORTA);
            smtp.Host = email.SMTP;
            smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
            smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
            //foreach (var emailAddress in EmailList)
            //{
            //    var message = new MailMessage("myemail@gmail.com", emailAddress);
            //    message.Subject = "hi";
            //    tasks.Add(client.SendMailAsync(message));
            //}

            MailMessage mensagem = new MailMessage();
            mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
            mensagem.To.Add(email.EMAIL_DESTINO);
            mensagem.Subject = email.ASSUNTO;
            mensagem.IsBodyHtml = true;
            mensagem.Body = email.CORPO;
            mensagem.Priority = email.PRIORIDADE;
            mensagem.IsBodyHtml = true;
            if (email.ATTACHMENT != null)
            {
                foreach (var attachment in email.ATTACHMENT)
                {
                    mensagem.Attachments.Add(attachment);
                }
            }
            tasks.Add(smtp.SendMailAsync(mensagem));
            while (tasks.Count > 0)
            {
                var idx = Task.WaitAny(tasks.ToArray());
                tasks.RemoveAt(idx);
            }
            return "done";
        }

        public static async Task Execute(Email email, String apiKey)
        {
            //var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
            var subject = email.ASSUNTO;
            var to = new EmailAddress(email.EMAIL_DESTINO);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = email.CORPO;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        [ValidateInput(false)]
        public Int32 ProcessarEnvioMensagemEMail(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cliente = null;
            GRUPO grupo = null;
            List<CLIENTE> listaCli = new List<CLIENTE>();
            Int32 escopo = 0;
            String erro = null;
            Int32 volta = 0;
            ERP_CRMEntities Db = new ERP_CRMEntities();
            MENSAGENS mens = baseApp.GetItemById(vm.MENS_CD_ID);

            // Nome
            if (vm.ID > 0)
            {                
                cliente = cliApp.GetItemById(vm.ID.Value);
                escopo = 1;
            }
            else if (vm.GRUPO > 0)
            {
                listaCli = new List<CLIENTE>();
                grupo = gruApp.GetItemById(vm.GRUPO.Value);
                foreach (GRUPO_CLIENTE item in grupo.GRUPO_CLIENTE)
                {
                    if (item.GRCL_IN_ATIVO == 1)
                    {
                        listaCli.Add(item.CLIENTE);
                    }
                }
                escopo = 2;
            }

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            if (escopo == 1)
            {
                // Template HTML
                if (vm.TEEM_CD_ID != null)
                {
                    TEMPLATE_EMAIL temp = temaApp.GetItemById(vm.TEEM_CD_ID.Value);
                    if (temp.TEEM_IN_HTML == 1)
                    {
                        // Prepara corpo do e-mail e trata link
                        String corpo = String.Empty;
                        String caminho = "/Imagens/" + idAss.ToString() + "/TemplatesHTML/" + temp.TEEM_CD_ID.ToString() + "/Arquivos/" + temp.TEEM_AQ_ARQUIVO;
                        String path = Path.Combine(Server.MapPath(caminho));
                        using (StreamReader reader = new System.IO.StreamReader(path))
                        {
                            corpo = reader.ReadToEnd();
                        }
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(corpo);
                        String body = str.ToString();
                        String emailBody = body;

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (vm.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                            {
                                String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Monta e-mail
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_DESTINO = cliente.CLIE_NM_EMAIL;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = cliente.ASSINANTE.ASSI_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.IS_HTML = true;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        // Envia mensagem
                        try
                        {
                            Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            if (ex.InnerException != null)
                            {
                                erro += ex.InnerException.Message;
                            }
                            if (ex.GetType() == typeof(SmtpFailedRecipientException))
                            {
                                var se = (SmtpFailedRecipientException)ex;
                                erro += se.FailedRecipient;
                            }
                        }

                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.CLIE_CD_ID = cliente.CLIE_CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = erro;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_TX_TEXTO = corpo;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }

                        // Envia assincrono

                        erro = null;
                        return volta;
                    }
                    else
                    {
                        // Prepara inicial
                        String body = String.Empty;
                        String header = String.Empty;
                        String footer = String.Empty;
                        String link = String.Empty;
                        if (vm.TEEM_CD_ID != null)
                        {
                            body = temp.TEEM_TX_CORPO;
                            header = temp.TEEM_TX_CABECALHO;
                            footer = temp.TEEM_TX_DADOS;
                            link = temp.TEEM_LK_LINK;                    
                        }

                        // Prepara cabeçalho
                        header = header.Replace("{Nome}", cliente.CLIE_NM_NOME);

                        // Prepara rodape
                        ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                        footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);
                    
                        // Trata corpo
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(body);

                        // Trata link
                        if (!String.IsNullOrEmpty(link))
                        {
                            if (!link.Contains("www."))
                            {
                                link = "www." + link;
                            }
                            if (!link.Contains("http://"))
                            {
                                link = "http://" + link;
                            }
                            str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                        }
                        body = str.ToString();                  
                        String emailBody = header + body + footer;

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (vm.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                            {
                                String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Monta e-mail
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_DESTINO = cliente.CLIE_NM_EMAIL;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = cliente.ASSINANTE.ASSI_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.IS_HTML = true;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        // Envia mensagem
                        try
                        {
                            Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            if (ex.InnerException != null)
                            {
                                erro += ex.InnerException.Message;
                            }
                            if (ex.GetType() == typeof(SmtpFailedRecipientException))
                            {
                                var se = (SmtpFailedRecipientException)ex;
                                erro += se.FailedRecipient;
                            }
                        }

                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            String cab = Regex.Replace(header, "<.*?>", String.Empty);
                            String cor = Regex.Replace(body, "<.*?>", String.Empty);
                            String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                            String tex = cab + " " + cor + " " + foot;
                            tex = tex.Replace("\r\n", " ");

                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.CLIE_CD_ID = cliente.CLIE_CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = erro;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_NM_CABECALHO = header;
                            mens.MENS_NM_RODAPE = footer;
                            mens.MENS_TX_TEXTO = body;
                            mens.MENS_TX_TEXTO_LIMPO = tex;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }

                        // Envia assincrono
                        erro = null;
                        return volta;
                    }
                }
                // Normal
                else
                {
                    // Prepara inicial
                    String body = String.Empty;
                    String header = String.Empty;
                    String footer = String.Empty;
                    String link = String.Empty;
                    body = vm.MENS_TX_TEXTO;
                    header = vm.MENS_NM_CABECALHO;
                    footer = vm.MENS_NM_RODAPE;
                    link = vm.MENS_NM_LINK;

                    // Prepara cabeçalho
                    header = header.Replace("{Nome}", cliente.CLIE_NM_NOME);

                    // Prepara rodape
                    ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                    footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);
                    
                    // Trata corpo
                    StringBuilder str = new StringBuilder();
                    str.AppendLine(body);

                    // Trata link
                    if (!String.IsNullOrEmpty(link))
                    {
                        if (!link.Contains("www."))
                        {
                            link = "www." + link;
                        }
                        if (!link.Contains("http://"))
                        {
                            link = "http://" + link;
                        }
                        str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                    }
                    body = str.ToString();                  
                    String emailBody = header + body + footer;

                    // Checa e monta anexos
                    List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                    if (vm.MENSAGEM_ANEXO.Count > 0)
                    {
                        foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                        {
                            String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                            System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                            listaAnexo.Add(anexo);
                        }
                    }

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = cliente.CLIE_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = cliente.ASSINANTE.ASSI_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.IS_HTML = true;
                    mensagem.NETWORK_CREDENTIAL = net;
                    mensagem.ATTACHMENT = listaAnexo;

                    // Envia mensagem
                    try
                    {
                        Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                        //Execute(mensagem).Wait();
                        //var task = SendEmailAssync(mensagem);
                        //String ret = SendEmail(mensagem);
                    }
                    catch (Exception ex)
                    {
                        erro = ex.Message;
                        if (ex.InnerException != null)
                        {
                            erro += ex.InnerException.Message;
                        }
                        if (ex.GetType() == typeof(SmtpFailedRecipientException))
                        {
                            var se = (SmtpFailedRecipientException)ex;
                            erro += se.FailedRecipient;
                        }
                    }

                    // Grava mensagem/destino e erros
                    if (erro == null)
                    {
                        String cab = Regex.Replace(header, "<.*?>", String.Empty);
                        String cor = Regex.Replace(body, "<.*?>", String.Empty);
                        String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                        String tex = cab + " " + cor + " " + foot;
                        tex = tex.Replace("\r\n", " ");

                        MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                        dest.MEDE_IN_ATIVO = 1;
                        dest.MEDE_IN_POSICAO = 1;
                        dest.MEDE_IN_STATUS = 1;
                        dest.CLIE_CD_ID = cliente.CLIE_CD_ID;
                        dest.MEDE_DS_ERRO_ENVIO = erro;
                        dest.MENS_CD_ID = mens.MENS_CD_ID;
                        mens.MENSAGENS_DESTINOS.Add(dest);
                        mens.MENS_DT_ENVIO = DateTime.Now;
                        mens.MENS_TX_TEXTO = body;
                        mens.MENS_NM_CABECALHO = header;
                        mens.MENS_NM_RODAPE = footer;
                        mens.MENS_TX_TEXTO_LIMPO = tex;
                        volta = baseApp.ValidateEdit(mens, mens);
                    }
                    else
                    {
                        mens.MENS_TX_RETORNO = erro;
                        volta = baseApp.ValidateEdit(mens, mens);
                    }

                    // Envia assincrono
                    erro = null;
                    return volta;
                }
            }
            else
            {
                // Template HTML
                if (vm.TEEM_CD_ID != null)
                {
                    TEMPLATE_EMAIL temp = temaApp.GetItemById(vm.TEEM_CD_ID.Value);
                    if (temp.TEEM_IN_HTML == 1)
                    {
                        // Prepara corpo do e-mail e trata link
                        String corpo = String.Empty;
                        String caminho = "/Imagens/" + idAss.ToString() + "/TemplatesHTML/" + temp.TEEM_CD_ID.ToString() + "/Arquivos/" + temp.TEEM_AQ_ARQUIVO;
                        String path = Path.Combine(Server.MapPath(caminho));
                        using (StreamReader reader = new System.IO.StreamReader(path))
                        {
                            corpo = reader.ReadToEnd();
                        }
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(corpo);
                        String body = str.ToString();
                        String emailBody = body;

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (vm.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                            {
                                String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Monta mensagem
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        // Monta destinos
                        MailAddressCollection col = new MailAddressCollection();
                        foreach (CLIENTE item in listaCli)
                        {
                            col.Add(item.CLIE_NM_EMAIL);
                        }

                        // Envia mensagem
                        try
                        {
                            Int32 voltaMail = CommunicationPackage.SendEmailCollection(mensagem, col);
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            if (ex.GetType() == typeof(SmtpFailedRecipientException))
                            {
                                var se = (SmtpFailedRecipientException)ex;
                                erro += se.FailedRecipient;
                            }
                        }

                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            foreach (CLIENTE item in listaCli)
                            {
                                MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                dest.MEDE_IN_ATIVO = 1;
                                dest.MEDE_IN_POSICAO = 1;
                                dest.MEDE_IN_STATUS = 1;
                                dest.CLIE_CD_ID = item.CLIE_CD_ID;
                                dest.MEDE_DS_ERRO_ENVIO = erro;
                                dest.MENS_CD_ID = mens.MENS_CD_ID;
                                mens.MENSAGENS_DESTINOS.Add(dest);
                                mens.MENS_DT_ENVIO = DateTime.Now;
                                mens.MENS_TX_TEXTO = body;
                                volta = baseApp.ValidateEdit(mens, mens);
                            }
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        erro = null;
                        return volta;
                    }
                    else
                    {
                        // Prepara inicial
                        String body = String.Empty;
                        String header = String.Empty;
                        String footer = String.Empty;
                        String link = String.Empty;
                        if (vm.TEEM_CD_ID != null)
                        {
                            body = temp.TEEM_TX_CORPO;
                            header = temp.TEEM_TX_CABECALHO;
                            footer = temp.TEEM_TX_DADOS;
                            link = temp.TEEM_LK_LINK;                    
                        }
                   
                        // Trata corpo
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(body);

                        // Trata link
                        if (!String.IsNullOrEmpty(link))
                        {
                            if (!link.Contains("www."))
                            {
                                link = "www." + link;
                            }
                            if (!link.Contains("http://"))
                            {
                                link = "http://" + link;
                            }
                            str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                        }
                        body = str.ToString();                  

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (vm.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                            {
                                String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Prepara rodape
                        ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                        footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);

                        // Monta Mensagem
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = assi.ASSI_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        // Checa cabeçalho e envia
                        if (header.Contains("{Nome}"))
                        {
                            foreach (CLIENTE item in listaCli)
                            {
                                // Prepara cabeçalho
                                header = header.Replace("{Nome}", item.CLIE_NM_NOME);
                                String emailBody = header + body + footer;

                                // Monta e-mail
                                mensagem.CORPO = emailBody;
                                mensagem.EMAIL_DESTINO = item.CLIE_NM_EMAIL;

                                // Envia mensagem
                                try
                                {
                                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);

                                }
                                catch (Exception ex)
                                {
                                    erro = ex.Message;
                                    if (ex.GetType() == typeof(SmtpFailedRecipientException))
                                    {
                                        var se = (SmtpFailedRecipientException)ex;
                                        erro += se.FailedRecipient;
                                    }
                                }
                            }
                        }
                        else
                        {
                            String emailBody = header + body + footer;
                            mensagem.CORPO = emailBody;

                            MailAddressCollection col = new MailAddressCollection();
                            foreach (CLIENTE item in listaCli)
                            {
                                col.Add(item.CLIE_NM_EMAIL);
                            }

                            // Envia mensagem
                            try
                            {
                                Int32 voltaMail = CommunicationPackage.SendEmailCollection(mensagem, col);

                            }
                            catch (Exception ex)
                            {
                                erro = ex.Message;
                                if (ex.GetType() == typeof(SmtpFailedRecipientException))
                                {
                                    var se = (SmtpFailedRecipientException)ex;
                                    erro += se.FailedRecipient;
                                }
                            }
                        }

                        // Loop de retorno
                        foreach (CLIENTE item in listaCli)
                        {
                            // Grava mensagem/destino e erros
                            if (erro == null)
                            {
                                String cab = Regex.Replace(header, "<.*?>", String.Empty);
                                String cor = Regex.Replace(body, "<.*?>", String.Empty);
                                String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                                String tex = cab + " " + cor + " " + foot;
                                tex = tex.Replace("\r\n", " ");

                                MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                dest.MEDE_IN_ATIVO = 1;
                                dest.MEDE_IN_POSICAO = 1;
                                dest.MEDE_IN_STATUS = 1;
                                dest.CLIE_CD_ID = item.CLIE_CD_ID;
                                dest.MEDE_DS_ERRO_ENVIO = erro;
                                dest.MENS_CD_ID = mens.MENS_CD_ID;
                                mens.MENSAGENS_DESTINOS.Add(dest);
                                mens.MENS_DT_ENVIO = DateTime.Now;
                                mens.MENS_TX_TEXTO = body;
                                mens.MENS_NM_CABECALHO = header;
                                mens.MENS_NM_RODAPE = footer;
                                mens.MENS_TX_TEXTO_LIMPO = tex;
                                volta = baseApp.ValidateEdit(mens, mens);
                            }
                            else
                            {
                                mens.MENS_TX_RETORNO = erro;
                                volta = baseApp.ValidateEdit(mens, mens);
                            }
                            erro = null;
                        }
                        return volta;
                    }
                }
                // Normal
                else
                {
                    // Prepara inicial
                    String body = String.Empty;
                    String header = String.Empty;
                    String footer = String.Empty;
                    String link = String.Empty;
                    body = vm.MENS_TX_TEXTO;
                    header = vm.MENS_NM_CABECALHO;
                    footer = vm.MENS_NM_RODAPE;
                    link = vm.MENS_NM_LINK;

                    // Prepara rodape
                    ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                    footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);
                    
                    // Trata corpo
                    StringBuilder str = new StringBuilder();
                    str.AppendLine(body);

                    // Trata link
                    if (!String.IsNullOrEmpty(link))
                    {
                        if (!link.Contains("www."))
                        {
                            link = "www." + link;
                        }
                        if (!link.Contains("http://"))
                        {
                            link = "http://" + link;
                        }
                        str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                    }
                    body = str.ToString();                  

                    // Checa e monta anexos
                    List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                    if (vm.MENSAGEM_ANEXO.Count > 0)
                    {
                        foreach (MENSAGEM_ANEXO item in vm.MENSAGEM_ANEXO)
                        {
                            String fn = Server.MapPath(item.MEAN_AQ_ARQUIVO);
                            System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                            listaAnexo.Add(anexo);
                        }
                    }

                    // Monta Mensagem
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA != null ? vm.MENS_NM_CAMPANHA : "Assunto Diverso";
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = assi.ASSI_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;
                    mensagem.ATTACHMENT = listaAnexo;

                    String emailBody = header + body + footer;
                    mensagem.CORPO = emailBody;

                    MailAddressCollection col = new MailAddressCollection();
                    foreach (CLIENTE item in listaCli)
                    {
                        col.Add(item.CLIE_NM_EMAIL);
                    }

                    // Envia mensagem
                    try
                    {
                        Int32 voltaMail = CommunicationPackage.SendEmailCollection(mensagem, col);

                    }
                    catch (Exception ex)
                    {
                        erro = ex.Message;
                        if (ex.GetType() == typeof(SmtpFailedRecipientException))
                        {
                            var se = (SmtpFailedRecipientException)ex;
                            erro += se.FailedRecipient;
                        }
                    }

                    // Loop de retorno
                    foreach (CLIENTE item in listaCli)
                    {
                        // Grava mensagem/destino e erros
                        if (erro == null)
                        {
                            String cab = Regex.Replace(header, "<.*?>", String.Empty);
                            String cor = Regex.Replace(body, "<.*?>", String.Empty);
                            String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                            String tex = cab + " " + cor + " " + foot;
                            tex = tex.Replace("\r\n", " ");

                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.CLIE_CD_ID = item.CLIE_CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = erro;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_TX_TEXTO = body;
                            mens.MENS_NM_CABECALHO = header;
                            mens.MENS_NM_RODAPE = footer;
                            mens.MENS_TX_TEXTO_LIMPO = tex;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        else
                        {
                            mens.MENS_TX_RETORNO = erro;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        erro = null;
                    }
                    return volta;
                }
            }
            return 0;
        }

        [ValidateInput(false)]
        public Int32 ProcessarAgendamentoMensagemEMail(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cliente = null;
            GRUPO grupo = null;
            List<CLIENTE> listaCli = new List<CLIENTE>();
            Int32 escopo = 0;
            String erro = null;
            Int32 volta = 0;
            ERP_CRMEntities Db = new ERP_CRMEntities();
            MENSAGENS mens = baseApp.GetItemById(vm.MENS_CD_ID);

            // Nome
            if (vm.ID > 0)
            {                
                cliente = cliApp.GetItemById(vm.ID.Value);
                escopo = 1;
            }
            else if (vm.GRUPO > 0)
            {
                listaCli = new List<CLIENTE>();
                grupo = gruApp.GetItemById(vm.GRUPO.Value);
                foreach (GRUPO_CLIENTE item in grupo.GRUPO_CLIENTE)
                {
                    if (item.GRCL_IN_ATIVO == 1)
                    {
                        listaCli.Add(item.CLIENTE);
                    }
                }
                escopo = 2;
            }

            // Recupera textos
            String body = String.Empty;
            String header = String.Empty;
            String footer = String.Empty;
            if (vm.TEEM_CD_ID != null)
            {
                TEMPLATE_EMAIL temp = temaApp.GetItemById(vm.TEEM_CD_ID.Value);
                body = temp.TEEM_TX_CORPO;
                header = temp.TEEM_TX_CABECALHO;
                footer = temp.TEEM_TX_DADOS;
            }
            else
            {
                body = vm.MENS_TX_TEXTO;
                header = vm.MENS_NM_CABECALHO;
                footer = vm.MENS_NM_RODAPE;
            }
            String cab = Regex.Replace(header, "<.*?>", String.Empty);
            String cor = Regex.Replace(body, "<.*?>", String.Empty);
            String foot = Regex.Replace(footer, "<.*?>", String.Empty);
            String tex = cab + " " + cor + " " + foot;
            tex = tex.Replace("\r\n", " ");

            // Regrava mensagem
            MENSAGENS mensX = baseApp.GetItemById(vm.MENS_CD_ID);
            mensX.MENS_TX_TEXTO_LIMPO = tex;
            Int32 vol1 = baseApp.ValidateEdit(mensX, mensX);

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            if (escopo == 1)
            {
                // Grava Agendamento
                EMAIL_AGENDAMENTO ag = new EMAIL_AGENDAMENTO();
                ag.ASSI_CD_ID = idAss;
                ag.CLIE_CD_ID = cliente.CLIE_CD_ID;
                ag.MENS_CD_ID = vm.MENS_CD_ID;
                ag.EMAG_DT_AGENDAMENTO = vm.MENS_DT_AGENDAMENTO;
                ag.EMAG_IN_ENVIADO = 0;
                Int32 volta1 = emApp.ValidateCreate(ag);
                return volta;
            }
            else
            {
                foreach (CLIENTE item in listaCli)
                {

                    // Grava Agendamento
                    EMAIL_AGENDAMENTO ag = new EMAIL_AGENDAMENTO();
                    ag.ASSI_CD_ID = idAss;
                    ag.CLIE_CD_ID = item.CLIE_CD_ID;
                    ag.MENS_CD_ID = vm.MENS_CD_ID;
                    ag.EMAG_DT_AGENDAMENTO = vm.MENS_DT_AGENDAMENTO;
                    ag.EMAG_IN_ENVIADO = 0;
                    Int32 volta1 = emApp.ValidateCreate(ag);
                }
                return volta;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult VerAnexoMensagem(Int32 id)
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
                    return RedirectToAction("MontarTelaMensagem", "Mensagem");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoMensagem()
        {

            return RedirectToAction("VerMensagem", new { id = (Int32)Session["IdMensagem"] });
        }

        public FileResult DownloadMensagem(Int32 id)
        {
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.MEAN_AQ_ARQUIVO;
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
        public ActionResult VerMensagemEMail(Int32 id)
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
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("MontarTelaMensagem", "Mensagem");
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

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 40)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
            }
            Session["IdMensagem"] = id;
            Session["VoltaMensagem"] = 1;
            MENSAGENS item = baseApp.GetItemById(id);
            MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardMensagens()
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
                    Session["MensMensagem"] = 2;
                    return RedirectToAction("MontarTelaMensagem", "Mensagem");
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);
            
            // Recupera listas Mensagens
            List<MENSAGENS> lt = baseApp.GetAllItens(idAss);
            List<MENSAGENS> le = lt.Where(p => p.MENS_DT_ENVIO != null).ToList();
            List<MENSAGENS> lm = le.Where(p => p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_ENVIO.Value.Year == DateTime.Today.Date.Year).ToList();
            List<MENSAGENS> agSMS = lt.Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_TIPO == 2).ToList();
            agSMS = agSMS.Where(p => p.MENS_DT_AGENDAMENTO.Value.Date > DateTime.Today.Date & p.MENS_IN_TIPO == 2).ToList();
            List<EMAIL_AGENDAMENTO> emAg = emApp.GetAllItens(idAss).Where(p => p.EMAG_IN_ENVIADO == 0 & p.EMAG_DT_AGENDAMENTO.Value.Date > DateTime.Today.Date).ToList();
            List<CLIENTE> cli = cliApp.GetAllItens(idAss);
            List<GRUPO> gru = gruApp.GetAllItens(idAss);

            // Abre destinos
            Int32 tot = le.SelectMany(p => p.MENSAGENS_DESTINOS).ToList().Count;
            Int32 totMes = lm.SelectMany(p => p.MENSAGENS_DESTINOS).ToList().Count;
            Int32 smsTot = le.Where(p => p.MENS_IN_TIPO == 2).SelectMany(p => p.MENSAGENS_DESTINOS).ToList().Count;
            Int32 smsMes = lm.Where(p => p.MENS_IN_TIPO == 2).SelectMany(p => p.MENSAGENS_DESTINOS).ToList().Count;
            Int32 emailTot = le.Where(p => p.MENS_IN_TIPO == 1).SelectMany(p => p.MENSAGENS_DESTINOS).ToList().Count;
            Int32 emailMes = lm.Where(p => p.MENS_IN_TIPO == 1).SelectMany(p => p.MENSAGENS_DESTINOS).ToList().Count;

            // Estatisticas Mensagens
            ViewBag.Total = lt.Count;
            ViewBag.TotalEnviado = tot;
            ViewBag.TotalMes = totMes;
            ViewBag.SMSEnviado = smsTot;
            ViewBag.EmailsEnviado = emailTot;
            ViewBag.SMSMes = smsMes;
            ViewBag.EmailsMes = emailMes;
            ViewBag.Falhas = lt.Where(p => p.MENS_TX_RETORNO != null).ToList().Count;
            ViewBag.FalhasMes = lm.Where(p => p.MENS_TX_RETORNO != null).ToList().Count;
            ViewBag.Clientes = cli.Count;
            ViewBag.Grupos = gru.Count;
            Session["ListaEMailTudo"] = le.Where(p => p.MENS_IN_TIPO == 1).ToList();
            Session["ListaSMSTudo"] = le.Where(p => p.MENS_IN_TIPO == 2).ToList();

            // Agendamentos Mensagens
            Session["SMSAgenda"] = agSMS;
            ViewBag.SMSAgenda = agSMS.Count;
            Session["EMailAgenda"] = emAg;
            ViewBag.EmailAgenda = emAg.Count;
            Session["VoltaMensagem"] = 0;
            Session["ListaMensagem"] = null;

            // Resumo Mes E-Mail
            List<MENSAGENS> lme = lm.Where(p => p.MENS_IN_TIPO == 1).ToList();
            List<DateTime> datas = lme.Select(p => p.MENS_DT_ENVIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = lme.Where(p => p.MENS_DT_ENVIO.Value.Date == item).SelectMany(p => p.MENSAGENS_DESTINOS).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaEMailMes = lista;
            ViewBag.ContaEMailMes = lme.Count;

            Session["ListaEMail"] = lme;
            Session["ListaDatasEMail"] = datas;

            // Resumo Mes SMS
            List<MENSAGENS> lms = lm.Where(p => p.MENS_IN_TIPO == 2).ToList();
            List<DateTime> datas1 = lms.Select(p => p.MENS_DT_ENVIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            foreach (DateTime item in datas1)
            {
                Int32 conta = lms.Where(p => p.MENS_DT_ENVIO.Value.Date == item).SelectMany(p => p.MENSAGENS_DESTINOS).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista1.Add(mod);
            }
            ViewBag.ListaSMSMes = lista1;
            ViewBag.ContaSMSMes = lms.Count;
            Session["ListaSMS"] = lms;
            Session["ListaDatasSMS"] = datas;
            return View(vm);
        }

        public ActionResult VerTotalExpansao()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase1 = baseApp.GetAllItens(idAss).Where(p => p.MENS_DT_ENVIO != null).ToList();
            List<MENSAGENS> listaBase = listaBase1.Where(p => p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Month).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_ENVIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_ENVIO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaTotal"] = listaBase;
            Session["ListaDatasTotal"] = datas;
            return View();
        }

        public JsonResult GetDadosGraficoTotal()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaTotalTodas"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasTotalTodas"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_ENVIO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoTotalTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaTotal"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasTotal"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_ENVIO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult VerTotalExpansaoTodos()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase = baseApp.GetAllItens(idAss).Where(p => p.MENS_DT_ENVIO != null).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_ENVIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_ENVIO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaTotalTodas"] = listaBase;
            Session["ListaDatasTotalTodas"] = datas;
            return View();
        }

        public ActionResult VerEMailExpansao()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase1 = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_ENVIO != null).ToList();
            List<MENSAGENS> listaBase = listaBase1.Where(p => p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Month).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_ENVIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_ENVIO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaEMail"] = listaBase;
            Session["ListaDatas"] = datas;
            return View();
        }

        public JsonResult GetDadosGraficoEmail()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaEMail"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasEMail"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                Int32 contaDia = listaCP1.Where(p => p.MENS_DT_ENVIO.Value.Date == item).SelectMany(p => p.MENSAGENS_DESTINOS).ToList().Count;
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoEmailTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaEMailTudo"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasEMailTudo"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_ENVIO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult VerEMailExpansaoTodos()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase = (List<MENSAGENS>)Session["ListaEMailTudo"];
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_ENVIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_ENVIO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaEMailTudo = lista;
            ViewBag.ContaEMailTudo = listaBase.Count;
            Session["ListaEMailTudo"] = listaBase;
            Session["ListaDatasEMailTudo"] = datas;
            return View();
        }

        public ActionResult VerSMSExpansao()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase1 = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_ENVIO != null).ToList();
            List<MENSAGENS> listaBase = listaBase1.Where(p => p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Month).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_ENVIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_ENVIO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaSMS"] = listaBase;
            Session["ListaDatasSMS"] = datas;
            return View();
        }

        public JsonResult GetDadosGraficoSMS()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaSMS"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasSMS"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_ENVIO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoSMSTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaSMSTudo"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasSMSTudo"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_ENVIO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult VerSMSExpansaoTodos()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase = (List<MENSAGENS>)Session["ListaSMSTudo"];
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_ENVIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_ENVIO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaSMSTudo = lista;
            ViewBag.ContaSMSTudo = listaBase.Count;
            Session["ListaSMSTudo"] = listaBase;
            Session["ListaDatasSMSTudo"] = datas;
            return View();
        }

        public ActionResult VerFalhasExpansao()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase1 = baseApp.GetAllItens(idAss).Where(p => p.MENS_TX_RETORNO != null).ToList();
            List<MENSAGENS> listaBase = listaBase1.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Month).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaFalha"] = listaBase;
            Session["ListaDatasFalha"] = datas;
            return View();
        }

        public JsonResult GetDadosGraficoFalhas()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaFalha"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasFalha"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoFalhasTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaFalhaTodas"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasFalhaTodas"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult VerFalhasExpansaoTodos()
        {
            // Prepara grid
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> listaBase = baseApp.GetAllItens(idAss).Where(p => p.MENS_TX_RETORNO != null).ToList();
            List<DateTime> datas = listaBase.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaBase.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.Lista = lista;
            ViewBag.Conta = listaBase.Count;
            Session["ListaFalha"] = listaBase;
            Session["ListaDatasFalha"] = datas;
            return View();
        }

        public ActionResult MostrarClientes()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 30;
            return RedirectToAction("MontarTelaCliente", "Cliente");
        }

        public ActionResult MostrarGrupos()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 40;
            return RedirectToAction("MontarTelaGrupo", "Grupo");
        }

        public ActionResult IncluirClienteRapido()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 30;
            return RedirectToAction("IncluirClienteRapido", "Cliente");
        }

    }
}