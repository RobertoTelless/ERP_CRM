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
    public class MensagemController : Controller
    {
        private readonly IMensagemAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IClienteAppService cliApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITemplateSMSAppService temApp;
        private readonly IGrupoAppService gruApp;
        //private readonly IEMailAgendaAppService emApp;
        //private readonly ICRMAppService crmApp;

        private String msg;
        private Exception exception;

        MENSAGENS objeto = new MENSAGENS();
        MENSAGENS objetoAntes = new MENSAGENS();
        List<MENSAGENS> listaMaster = new List<MENSAGENS>();
        String extensao;

        public MensagemController(IMensagemAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IClienteAppService cliApps, IConfiguracaoAppService confApps, ITemplateSMSAppService temApps, IGrupoAppService gruApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            cliApp = cliApps;
            confApp = confApps;
            temApp = temApps;
            gruApp = gruApps;
            //emApp = emApps;
            //crmApp = crmApps;
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
                listaMaster = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_ENVIO).ToList();
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
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            objeto = new MENSAGENS();
            if (Session["FiltroMensagem"] != null)
            {
                objeto = (MENSAGENS)Session["FiltroMensagem"];
            }
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
            listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_ENVIO).ToList();
            Session["ListaMensagem"] = null;
            Session["FiltroMensagem"] = listaMaster;
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
            listaMaster = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 2).OrderByDescending(m => m.MENS_DT_ENVIO).ToList();
            Session["ListaMensagem"] = null;
            Session["FiltroMensagem"] = listaMaster;
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
            Int32 num = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO.Value == 2 & p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_ENVIO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
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
            String body = temApp.GetByCode("SMSBAS", idAss).TSMS_TX_CORPO;

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
                    if (String.IsNullOrEmpty(vm.MENS_TX_SMS))
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
                    listaCli.Add(item.CLIENTE);
                }
                escopo = 2;
            }

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
                //texto += "   <a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>";
                texto += "  " + vm.MENS_NM_LINK;
            }
            String body = str.ToString();
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
                            json = String.Concat("{\"scheduleTime\": \"", data ,"\",\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"SystemBR\"}]}");
                        }
                        else
                        {
                            json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"SystemBR\"}]}");
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
                            vetor += "{\"to\": \"," + listaDest + ", \", \"text\": \"," + texto + "\", \"from\": \"SystemBR\"}";
                        }
                        else
                        {
                            vetor += ",{\"to\": \"," + listaDest + ", \", \"text\": \"," + texto + "\", \"from\": \"SystemBR\"}";
                        }
                    }
                    
                    // Configura                    
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = String.Concat("{\"destinations\": [", vetor ,"]}");
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
        public ActionResult ConverterMensagemCRM(Int32 id)
        {
            // Recupera Mensagem e contato
            MENSAGENS_DESTINOS dest = baseApp.GetDestinoById(id);
            MENSAGENS mensagem = baseApp.GetItemById(dest.MENS_CD_ID);
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Cria CRM
            CRM crm = new CRM();
            crm.ASSI_CD_ID = mensagem.ASSI_CD_ID;
            crm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            crm.CRM1_DS_DESCRICAO = "Processo criado a partir de mensagem";
            crm.CRM1_DT_CRIACAO = DateTime.Today.Date;
            crm.CRM1_IN_ATIVO = 1;
            crm.CRM1_IN_STATUS = 1;
            crm.CRM1_NM_NOME = "Processo criado a partir de mensagem";
            crm.TICR_CD_ID = 1;
            crm.USUA_CD_ID = usuario.USUA_CD_ID;
            crm.MENS_CD_ID = mensagem.MENS_CD_ID;
            crm.ORIG_CD_ID = 1;
            //Int32 volta = crmApp.ValidateCreate(crm, usuario);

            // Atualiza mensagem
            dest.MEDE_IN_POSICAO = 4;
            Int32 volta1 = baseApp.ValidateEditDestino(dest);

            // Retorno
            Session["MensMensagem"] = 40;
            return RedirectToAction("VoltarAnexoMensagemSMS");
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
                listaMaster = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_ENVIO).ToList();
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
            }

            // Abre view
            Session["VoltaMensagem"] = 1;
            objeto = new MENSAGENS();
            if (Session["FiltroMensagem"] != null)
            {
                objeto = (MENSAGENS)Session["FiltroMensagem"];
            }
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
            listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_ENVIO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_ENVIO).ToList();
            Session["ListaMensagem"] = null;
            Session["FiltroMensagem"] = listaMaster;
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
            listaMaster = baseApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 1).OrderByDescending(m => m.MENS_DT_ENVIO).ToList();
            Session["ListaMensagem"] = null;
            Session["FiltroMensagem"] = listaMaster;
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
                return RedirectToAction("MontarTelaMensagemSMS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMensagem");
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

        public ActionResult VoltarMensagemAnexoEMail()
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
            TEMPLATE_EMAIL tmp = temApp.GetItemById(Convert.ToInt32(temp));

            // Atualiza
            var hash = new Hashtable();
            hash.Add("TSMS_TX_CORPO", tmp.TSMS_TX_CORPO);
            hash.Add("TSMS_LK_LINK", tmp.TSMS_LK_LINK);

            // Retorna
            return Json(hash);
        }




    }
}